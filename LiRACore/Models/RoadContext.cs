using LiRACore.Models.AutoPi.QuickLiRACore.Models.RawData.Trip;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiRACore.Models
{
   public class RoadContext : DbContext
    {
        public RoadContext(DbContextOptions<RoadContext> options) : base(options)
        {

        }

        public DbSet<Road> Roads { get; set; }

         

    }
}
