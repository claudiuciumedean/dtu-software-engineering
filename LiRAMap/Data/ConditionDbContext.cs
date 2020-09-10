using LiRAMap.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiRAMap.Data
{
	public class ConditionDbContext : DbContext
	{
		protected override void OnConfiguring(DbContextOptionsBuilder opt)
		{
			opt.UseNpgsql("Host=localhost;Database=LiRAMap;Username=liramap;Password=updateme");
			opt.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
		}

		protected override void OnModelCreating(ModelBuilder mdl)
		{
			mdl.Entity<ConditionCoverage>().HasIndex(e => e.Way);

			//Test Condition Data
			mdl.Entity<Condition>().HasData(
				new Condition { Id = 1, ConditionType = 3, Value = 35, Timestamp = DateTime.Now },
				new Condition { Id = 2, ConditionType = 9, Value = 95, Timestamp = DateTime.UtcNow },
				new Condition { Id = 3, ConditionType = 10, Value = 53, Timestamp = DateTime.Now },
				new Condition { Id = 4, ConditionType = 0, Value = 100, Timestamp = DateTime.UtcNow }
			);

			mdl.Entity<ConditionCoverage>().HasData(
				new ConditionCoverage { Id = 1, ConditionId = 1, Way = 27294498ul, StartMeters = 0, EndMeters = 150 },
				new ConditionCoverage { Id = 2, ConditionId = 1, Way = 26264463ul, StartMeters = 0, EndMeters = 250 },
				new ConditionCoverage { Id = 3, ConditionId = 1, Way = 106151483ul, StartMeters = 575, EndMeters = 1000 },
				new ConditionCoverage { Id = 4, ConditionId = 2, Way = 131196793ul, StartMeters = 52, EndMeters = 78 },
				new ConditionCoverage { Id = 5, ConditionId = 3, Way = 131196793ul, StartMeters = 60 },
				new ConditionCoverage { Id = 6, ConditionId = 4, Way = 25657310ul, StartMeters = 10 }
			);
		}

		public DbSet<Condition> Conditions { get; set; }
		public DbSet<ConditionCoverage> ConditionCoverages { get; set; }
	}
}
