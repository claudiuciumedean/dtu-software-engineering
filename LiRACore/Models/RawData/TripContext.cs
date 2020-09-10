using LiRACore.Models.AutoPi.QuickLiRACore.Models.RawData.Trip;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiRACore.Models.AutoPi
{
   public class TripContext : DbContext
    {
        public TripContext(DbContextOptions<TripContext> options) : base(options)
        {

        }

        public DbSet<Trip> Trips { get; set; }

         

    }
}
