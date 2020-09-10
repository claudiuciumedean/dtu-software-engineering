using System;
using System.Collections.Generic;
using System.Text;
using LiRACore.Models.RawData;
using LiRAWebApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LiRAWebApi.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<TripNotification> TodoItems { get; set; }
    }
}
