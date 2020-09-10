using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LiRAMap.Models
{
	public class ConditionCoverage
	{
		public int Id { get; set; }
		public ulong Way { get; set; }
		public int StartMeters { get; set; }
		public int? EndMeters { get; set; }

		public int ConditionId { get; set; }
		//public Condition Condition { get; set; }
	}
}
