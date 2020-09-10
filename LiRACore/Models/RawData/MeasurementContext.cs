using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiRACore.Models.AutoPi
{
   public class MeasurementContext : DbContext
    {
        public MeasurementContext(DbContextOptions<MeasurementContext> options) : base(options)
        {

        }

        public DbSet<Measurement> Measurements { get; set; }

         

    }
}
