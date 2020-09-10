using LiRAMap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiRAMap.Services
{
	public interface IConditionProvider
	{
		public Task<ICollection<Condition>> GetConditions(Way way);
		public Task<ICollection<Condition>> GetConditions(IEnumerable<Way> ways);
	}
}
