using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LiRAMap.Models
{
	public class Node
	{
		public ulong Id { get; set; }
		public double[] Coords { get; set; }
	}
}
