using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LiRAMap.Models
{
	public class Condition
	{
		public int Id { get; set; }
		public int ConditionType { get; set; }
		public float Value { get; set; }

		public DateTime Timestamp { get; set; }
		
		/*[Required]
		public ICollection<Tuple<ulong, int, int>> Ways { get; set; }
		//Tuple: <Way ID, from meters, to meters>

		//A custom converter so we can store this as a formatted string in the database
		public static readonly ValueConverter<ICollection<Tuple<ulong, int, int>>, string> WaysConverter = new ValueConverter<ICollection<Tuple<ulong, int, int>>, string>(
			v => v.Select(t => t.ToString()).Aggregate((cur, next) => cur + ";" + next),
			v => StringToWays(v)
		);

		private static ICollection<Tuple<ulong, int, int>> StringToWays(string str)
		{
			ICollection<Tuple<ulong, int, int>> Ways = new List<Tuple<ulong, int, int>>();
			IEnumerable<string> tuples = str.Split(";");

			foreach (string t in tuples)
			{
				string[] nums = str[1..^1].Split(", ");
				ulong.TryParse(nums[0], out ulong wid);
				int.TryParse(nums[0], out int fm);
				int.TryParse(nums[0], out int tm);

				Ways.Add(Tuple.Create(wid, fm, tm));
			}
			return Ways;
		}*/

		[Required]
		public ICollection<ConditionCoverage> Coverage { get; set; }
	}
}
