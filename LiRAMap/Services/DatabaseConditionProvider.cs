using LiRAMap.Data;
using LiRAMap.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiRAMap.Services
{
	public class DatabaseConditionProvider : IConditionProvider
	{
		private readonly ConditionDbContext db;
		public DatabaseConditionProvider(ConditionDbContext database)
		{
			db = database;
		}

		public async Task<ICollection<Condition>> GetConditions(Way way)
		{
			return await db.ConditionCoverages.Where(c => c.Way == way.Id).SelectMany(c => db.Conditions).ToListAsync();
		}

		public async Task<ICollection<Condition>> GetConditions(IEnumerable<Way> ways)
		{
			HashSet<ulong> wayids = ways.Select(w => w.Id).ToHashSet();
			return await db.Conditions.Where(c => c.Coverage.Any(cov => wayids.Contains(cov.Way))).Include(c => c.Coverage).ToListAsync();
			//return await db.ConditionCoverages.Where(c => wayids.Contains(c.Way)).Select(c => db.Conditions).Include(c => c.Coverage).ToListAsync();
		}
	}
}
