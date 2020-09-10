import L, { Polyline, Layer, Circle } from 'leaflet';
import { LiRA } from "./App";
import { Types, ConditionType } from "./ConditionTypes";

//CLASS:
//	Way
//	Represents a way. This is a single road entity. For OSM-based systems, this correlates to the OSM ways.
//	It could however be extended to support Multiline however, which would allow whole roads.
export class Way {
	name: string;
	//index: number;
	id: number;

	nodes: number[];
	//conditions: Condition[]
	conditions: { [id: number]: Condition }

	//Constructor taking the API response data of a road.
	//Modify this if the API is enriched with additional data.
	constructor(data: any) {
		this.id = data.id;
		this.name = data.name;
		this.nodes = data.nodes;
		this.conditions = [];
	}

	addCondition(c: Condition) {
		//this.conditions.push(c);
		this.conditions[c.id] = c;
	}

	//draw(): Layer
	//Creates the Leaflet layer that represents the drawing of this Way. This is by default a Polyline following the path of the way.
	draw(lineToRedraw?: Polyline): Polyline | undefined {
		var path = this.calculatePath();
		if (path == null) return;

		if (lineToRedraw == null) {
			return L.polyline(path, { color: this.calculateColor() });
		} else {
			lineToRedraw.setLatLngs(path);
			lineToRedraw.redraw();
			return lineToRedraw;
		}
	}

	//calculatePath(): number[][]
	//Returns the ordered path of the line representing this way.
	//The structure is:
	//		Path: List Coordinates
	//			Coordinate: [latitude, longitude]
	calculatePath(): [number, number][] | undefined {
		var path = [];
		for (var nid of this.nodes) {
			path.push(LiRA.getNode(nid));
		}

		if (path.length > 0) return path;
	}

	//calculateColor(): string
	//Returns the HTML color string for this Way. This is the default color under all conditions drawn on top.
	calculateColor(): string {
		return "blue";
	}

	//getIdentityString(): string
	//Returns the string that represents this Way's identity (used by the UI)
	getIdentityString(): string {
		return this.name;
	}
}

//CLASS:
//	Condition
//	Represents a measured condition. These apply to any number of Ways, and any Way may contain any number of Conditions too.
//	They are represented by a type and a severity.
//
//	Static:
//		Contains static list of different Types, as well as translation of numeric severity to string display.
//		These are imported from LiRAMapConditionTypes.
export class Condition {

	//Static: Condition Type translators
	static ConditionTypes = Types;
	static getConditionType(type: number): ConditionType {
		return Condition.ConditionTypes[type];
	}
	static getConditionValueScale(type: ConditionType, value: number): number {
		//TODO: Implement condition scaling support for functions and number pairs when implemented in ConditionTypes
		return Math.min(Math.max(value / type.ValueScale, 0), 1);
	}

	id: number;
	type: ConditionType;
	typeId: number;
	value: number;
	timestamp: Date;
	ways: { [w: number]: [number, number] }

	constructor(id: number, type: number = 0, value: number = 0, timestamp: string, ways: { [w: number]: [number, number] } = {}) {
		this.id = id;
		this.type = Condition.ConditionTypes[type];
		this.typeId = type;
		this.value = value;
		this.ways = ways;
		this.timestamp = new Date(Date.parse(timestamp));
	}

	addWay(way: Way, startMeters: number, endMeters: number) {
		this.ways[way.id] = [startMeters, endMeters];
	}

	removeWay(way: Way) {
		delete this.ways[way.id];
	}

	draw(toRedraw?: Layer): Layer | undefined {
		var path = this.calculatePath();
		if (path == null) return;

		if (toRedraw == null) {
			return L.polyline(path, { color: this.calculateColor() });
		} else {
			var line = toRedraw as Polyline;
			line.setLatLngs(path);
			line.redraw();
			return toRedraw;
		}
	}

	//calculatePath(): void
	//Returns a 3-dimensional array of the following structure:
	//		Result: List of Lines
	//			Line: List of coordinates
	//				Coordinate: [latitude, longitude]
	//This represents the multi-line path this condition covers. It is calculated on top of its associated ways. Each separate line corresponds to each separate way covered.
	calculatePath(): [number, number][][] | [number, number][] | undefined { //This is a multi-line, each separate line on each way
		var list: [number, number][][] = [];
		for (var wid in this.ways) {
			var way: Way = LiRA.getWay(wid);
			if (way != null) {
				var cover = this.ways[wid];
				var wnodes: [number,number][] = [];
				var dist = 0;
				var n = LiRA.getNode(way.nodes[0]);
				var l_prev = L.latLng(n);
				var started = false;
				var ended = false;

				if (cover[0] <= 0) {
					wnodes.push(LiRA.getNode(way.nodes[0]));
					started = true;
				}

				for (var i = 1; i < way.nodes.length; i++) {
					n = LiRA.getNode(way.nodes[i]);
					var l_next = L.latLng(n);
					var d = l_prev.distanceTo(l_next);
					var total_d = dist + d

					if (!started && total_d >= cover[0]) {
						var scale = (total_d - cover[0]) / d
						wnodes.push([l_next.lat + (l_prev.lat - l_next.lat) * scale, l_next.lng + (l_prev.lng - l_next.lng) * scale]);
						started = true;
					}

					if (total_d >= cover[1]) {
						var scale = (total_d - cover[1]) / d
						wnodes.push([l_next.lat + (l_prev.lat - l_next.lat) * scale, l_next.lng + (l_prev.lng - l_next.lng) * scale]);
						ended = true;
						break;
					}

					dist = total_d;
					l_prev = l_next;
					if (started) wnodes.push(n);
				}

				if (started && !ended) {
					wnodes.push(n);
				}

				if (wnodes.length > 0) list.push(wnodes);
			}
		}

		//We only return if there is a node. If not, the path is undefined (it cannot be drawn) and we wait until an update to the necessary ways
		if (list.length > 0) return list;
	}

	//calculateColor(): string
	//Returns the HTML color string for this condition. This scales with the severity and the type.
	calculateColor(): string {
		const maxhue = 64;
		return "hsl(" + (maxhue - maxhue * this.getConditionValueScale()) + ", 100%, 50%)";
	}

	//getIdentityString(): string
	//Returns the string that represents this Conditions's identity (used by the UI)
	//TODO: Update this when types are implemented
	getIdentityString(): string {
		return "Condition: " + this.id;
	}

	//Shortcuts for Condition instances to access Types
	getConditionTypeName(): string {
		return this.type.Name;
	}
	translateConditionValue(): string {
		return this.type.TranslateValue(this.value);
	}
	getConditionTypeValueDescriptor(): string {
		return this.type.ValueDescriptor;
	}
	getConditionTypeDescription(): string {
		return this.type.Description;
	}
	getConditionValueScale(): number {
		console.log(this.type, this.value);
		return Condition.getConditionValueScale(this.type, this.value);
	}
}

//CLASS:
//	PointCondition: Extends Condition
//	Represents a point condition. This is an override on the calculatePath() method that returns a single point.
//	It could be changed to return a Marker instead.
export class PointCondition extends Condition {
	//Override of CalculatePath
	//We only need to calculate one position. This position is the single point position.
	calculatePath(): [number, number][][] | [number, number][] | undefined {
		var way: Way | null = null;
		for (var wid in this.ways) {
			way = LiRA.getWay(wid);
			break;
		}

		if (way != null) {
			var cover = this.ways[way.id];
			var dist = 0;
			var n = LiRA.getNode(way.nodes[0]);
			var l_prev = L.latLng(n);

			if (cover[0] <= 0) {
				return [n,n];
			}

			for (var i = 1; i < way.nodes.length; i++) {
				n = LiRA.getNode(way.nodes[i]);
				var l_next = L.latLng(n);
				var d = l_prev.distanceTo(l_next);
				var total_d = dist + d

				if (total_d >= cover[0]) {
					var scale = (total_d - cover[0]) / d
					var pos: [number, number] = [l_next.lat + (l_prev.lat - l_next.lat) * scale, l_next.lng + (l_prev.lng - l_next.lng) * scale];
					return [pos, pos];
				}

				dist = total_d;
				l_prev = l_next;
			}

			if (n) return [n,n];
		}
	}

	draw(toRedraw?: Layer): Layer | undefined {
		var path = this.calculatePath();
		if (path == null) return;
		var coords: number[] = path.flat();

		if (toRedraw == null) {
			return L.circle([coords[0], coords[1]], { color: this.calculateColor(), radius: 3 });
		} else {
			var point = toRedraw as Circle;
			point.setLatLng([coords[0], coords[1]]);
			point.redraw();
			return toRedraw;
		}
	}
}