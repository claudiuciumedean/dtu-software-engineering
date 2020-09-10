using LiRAMap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LiRAMap.Services
{
	public class OverpassWayProvider : IWayProvider
	{
		private readonly HttpClient client;
		public OverpassWayProvider()
		{
			client = new HttpClient();
		}

		private const string APIUrl = "https://lz4.overpass-api.de/api/interpreter";
		//"?data=(way[highway][name]({0},{1},{2},{3});node(w););out qt;"
		private const string APIQueryWays = "way[highway][name]({1},{0},{3},{2});"; //Overpass is queried by (lat,lon) which is Y/X, so we reverse 0-1 and 2-3

		[XmlRoot("osm")]
		public class OverpassAPIResponse
		{
			[XmlElement("node")]
			public List<OverpassNode> Nodes;

			[XmlElement("way")]
			public List<OverpassWay> Ways;
		}
		public class OverpassWay
		{
			[XmlAttribute("id")]
			public ulong id;

			[XmlElement("nd")]
			public List<OverpassRef> Nodes;

			[XmlElement("tag")]
			public List<OverpassTag> Tags;
		}
		public class OverpassNode
		{
			[XmlAttribute("id")]
			public ulong id;

			[XmlAttribute("lat")]
			public double Lat;

			[XmlAttribute("lon")]
			public double Lon;
		}
		public class OverpassTag
		{
			[XmlAttribute("k")]
			public string Key;
			[XmlAttribute("v")]
			public string Value;
		}
		public class OverpassRef
		{
			[XmlAttribute("ref")]
			public ulong Ref;
		}

		public async Task<Tuple<IEnumerable<Way>, IEnumerable<Node>>> GetWaysInBounds(double left, double bottom, double right, double top, IEnumerable<double[]> known)
		{
			string req = String.Format(APIQueryWays, left, bottom, right, top);
			if (known != null && known.Count() > 0)
			{
				if (known.Count() == 1)
				{
					double[] coords = known.First();
					req = "(" + req + "-" + String.Format(APIQueryWays, coords[0], coords[1], coords[2], coords[3]) + ");";
				}
				else
				{
					req = "(" + req + "-(";
					foreach (double[] coords in known)
					{
						req += String.Format(APIQueryWays, coords[0], coords[1], coords[2], coords[3]);
					}
					req += "););";
				}
			}

			string final = APIUrl + "?data=(" + req + "node(w);); out qt;";
			HttpResponseMessage response = await client.GetAsync(APIUrl + "?data=(" + req + "node(w);); out qt;");
			if (!response.IsSuccessStatusCode) return null;

			string res = await response.Content.ReadAsStringAsync();

			XmlSerializer xml = new XmlSerializer(typeof(OverpassAPIResponse));
			OverpassAPIResponse resp = (OverpassAPIResponse)xml.Deserialize(await response.Content.ReadAsStreamAsync());

			return Tuple.Create(
				resp.Ways.Select(w => new Way { Id = w.id, Name = w.Tags.First(t => t.Key == "name").Value, Nodes = w.Nodes.Select(n => n.Ref)}),
				resp.Nodes.Select(n => new Node { Id = n.id, Coords = new double[] { n.Lat, n.Lon } })
			);
		}
	}
}
