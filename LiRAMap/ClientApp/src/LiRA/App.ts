import L, { LatLngBounds, Evented } from 'leaflet';
import RBush, { BBox } from 'rbush';
import { Way, Condition, PointCondition } from './Classes';

class SearchBounds implements BBox {
	minX: number;
	minY: number;
	maxX: number;
	maxY: number;

	constructor(bounds: LatLngBounds) {
		this.minX = bounds.getWest();
		this.minY = bounds.getSouth();
		this.maxX = bounds.getEast();
		this.maxY = bounds.getNorth();
	}

	contains(otherBounds: SearchBounds): boolean {
		return this.minX <= otherBounds.minX && this.minY <= otherBounds.minY && this.maxX >= otherBounds.maxX && this.maxY >= otherBounds.maxY;
	}

	toBBoxString(): string {
		return this.minX + "," + this.minY + "," + this.maxX + "," + this.maxY;
	}


	//TODO: These functions can be implemented to optimize querying by cutting down the bounds in which we search depending on other already-searched bounds.

	/*cut(byBounds: SearchBounds): SearchBounds[] {

	}

	canMerge(otherBounds: SearchBounds): boolean {
		return (this.minX == otherBounds.minX && this.maxX == otherBounds.maxX && (this.minY <= otherBounds.minY != this.maxY >= otherBounds.maxY)) || (this.minY == otherBounds.minY && this.maxY == otherBounds.maxY && (this.minX <= otherBounds.minX != this.maxX >= otherBounds.maxX))
	}

	tryMerge(otherBounds: SearchBounds): SearchBounds {
		if (this.minX == otherBounds.minX && this.maxX == otherBounds.maxX) {
			
		}
	}*/
}

class LiRA extends Evented {
	Ways: { [wid: string]: Way };
	Nodes: { [nid: string]: [number, number] }
	Conditions: { [cid: string]: Condition };

	private Searches: RBush<SearchBounds>;

	constructor() {
		super();
		this.Ways = {};
		this.Nodes = {};
		this.Conditions = {};
		this.Searches = new RBush(); //We use this to index where we've already searched so we can avoid searching repeatedly
	}

	//////////////// Getters \\\\\\\\\\\\\\\\\
	getWay(id: number | string): Way {
		return this.Ways[id];
	}

	getNode(id: number | string): [number, number] {
		return this.Nodes[id];
	}

	getCondition(id: number | string): Condition {
		return this.Conditions[id];
	}

	//////////////// API Functions \\\\\\\\\\\\\\\\\

	//fetchRoads(bounds: Bounds)
	//Calls upon the server API to fetch all roads within the specified bounds.
	//Automatically filters out the areas already searched inside this box and adds this box as already searched.
	//Silently does nothing if the entire bounds are covered by other searches.

	private apicount: number = 0;
	async fetchMapData(bounds: LatLngBounds) {
		//Filter out if we are already contained within any other box
		var search = new SearchBounds(bounds);
		var others: SearchBounds[] = this.Searches.search(search);
		for (var s of others) {
			if (s.contains(search)) return; //We are completely covered by one of the boxes. Do absolutely nothing.
			if (search.contains(s)) this.Searches.remove(s);
		}
		this.Searches.insert(search);

		var body = new FormData();
		body.append("AlreadySearched", others.map(r => r.toBBoxString()).join(";"));

		//Mark that we started an API request
		if (this.apicount == 0) this.fire("apistart");
		this.apicount++;

		var resp = await fetch("api/Map?bounds=" + bounds.toBBoxString(), {
			method: 'post',
			body: body
		});

		if (resp.ok) {
			var json = await resp.json();
			var found: any = { ways: [], conditions: [], nodes: [] };

			console.log(json, found);

			if (json.ways) {
				for (var w of json.ways) {
					if (this.Ways[w.id] == null) {
						this.Ways[w.id] = new Way(w);

						found.ways.push(this.Ways[w.id]);
					}
				}
			}

			if (json.nodes) {
				for (var id in json.nodes) {
					this.Nodes[id] = json.nodes[id];
					found.nodes.push(this.Nodes[id]);
				}
			};

			if (json.conditions) {
				for (var c of json.conditions) {
					var con = this.Conditions[c.id];
					if (con == null) {
						var coveredways = Object.keys(c.coverage);
						if (coveredways.length == 1 && c.coverage[coveredways[0]].endMeters == null) { //A Point Condition is defined as covering exactly 1 way, and only having StartMeters defined.
							con = new PointCondition(c.id, c.conditionType, c.value, c.timestamp);
						} else {
							con = new Condition(c.id, c.conditionType, c.value, c.timestamp);
						}

						this.Conditions[c.id] = con;

						for (var cover of c.coverage) {
							con.ways[cover.way] = [cover.startMeters, cover.endMeters];

							var way = this.getWay(cover.way);
							if (way) way.addCondition(con);
						}
						found.conditions.push(this.Conditions[c.id]);
					}
				}
			}

			this.fire("mapdataresponse", found);
		}

		this.apicount--;
		if (this.apicount == 0) this.fire("apiend");
	}
}

var app = new LiRA();
export { app as LiRA };