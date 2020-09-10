using LiRACore.Models.RawData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiRACore.Migrations
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<LiRACore.Models.RawData.DeviceContext>
    {
        public LiRACore.Models.RawData.DeviceContext CreateDbContext(string[] args)
        {

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(@"C:\Users\shmp\LiRA\Source\Backend\PullingRawDataService")
                .AddJsonFile("appsettings.json")
                .Build();
            var builder = new DbContextOptionsBuilder<LiRACore.Models.RawData.DeviceContext>();
            var connectionString = configuration.GetConnectionString("WriteConnectionStrings:SaveRawDataConnection");
            builder.UseNpgsql("User ID =postgres;Password=SuperPass;Server=localhost;Port=5432;Database=DRD_RAW_DBStrcuture;Integrated Security=true;Pooling=true;");
            return new LiRACore.Models.RawData.DeviceContext(builder.Options);

       
    }
    }
}
