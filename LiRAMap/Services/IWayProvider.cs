using LiRAMap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiRAMap.Services
{
	public interface IWayProvider
	{
		public Task<Tuple<IEnumerable<Way>, IEnumerable<Node>>> GetWaysInBounds(double left, double bottom, double right, double top, IEnumerable<double[]> known);
	}
}
