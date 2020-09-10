using LiRACore.Models.RawData;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiRACore.Models.RawData
{
   public class DeviceContext : DbContext 
    {
        public DeviceContext(DbContextOptions<DeviceContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //    public class Blog
            //{
            //    public int BlogId { get; set; }
            //    public string Url { get; set; }

            //    public List<Post> Posts { get; set; }
            //}

            //public class Post
            //{
            //    public int PostId { get; set; }
            //    public string Title { get; set; }
            //    public string Content { get; set; }

            //    public Blog Blog { get; set; }
            //}

            //modelBuilder.Entity<Post>()
            //            .HasOne(p => p.Blog)
            //            .WithMany(b => b.Posts);
            //            .HasForeignKey(p => p.BlogForeignKey);

            modelBuilder.Entity<SourceType>()
            .HasKey(d => d.SourceTypeId);
            modelBuilder.Entity<SourceType>()
                   .HasMany(d => d.Devices)
                   .WithOne(t => t.SourceType)
                   .HasForeignKey(t => t.FK_SourceType)
                   .IsRequired();


            modelBuilder.Entity<Device>()
                        .HasKey(d => d.DeviceId);
            modelBuilder.Entity<Device>()
                   .HasMany(d => d.Trips)
                   .WithOne(t => t.Device)
                   .HasForeignKey(t => t.FK_Device)
                   .IsRequired();
          
            modelBuilder.Entity<Trip>()
                        .HasKey(t => t.TripId);
            modelBuilder.Entity<Trip>()
                   .HasMany(t => t.Measurements)
                   .WithOne(m => m.Trip)
                   .HasForeignKey(t => t.FK_Trip);
                   //.IsRequired(); //removed because of having options for saving eigher in DRDMeasurement Table or Measurement table

           //For DRD data
            //modelBuilder.Entity<Trip>()
            //.HasKey(t => t.TripId);
            modelBuilder.Entity<Trip>()
                   .HasMany(t => t.DRDMeasurements)
                   .WithOne(m => m.Trip)
                   .HasForeignKey(t => t.FK_Trip);
                   //.IsRequired();  //removed because of having options for saving eigher in DRDMeasurement Table or Measurement table


            modelBuilder.Entity<Measurement>()
                .HasKey(m => m.MeasurementId);
            modelBuilder.Entity<Measurement>()
              .Property(m => m.MeasurementId)
              .ValueGeneratedOnAdd();

            // For DRD data
            modelBuilder.Entity<DRDMeasurement>()
           .HasKey(m => m.DRDMeasurementId);
            modelBuilder.Entity<DRDMeasurement>()
              .Property(m => m.DRDMeasurementId)
              .ValueGeneratedOnAdd();


            modelBuilder.Entity<MeasurementType>()
                   .HasKey(mt => mt.MeasurementTypeId);
            modelBuilder.Entity<MeasurementType>()
                   .HasMany(mt => mt.Measurements)
                   .WithOne(m => m.Measurement_Type)
                   .HasForeignKey(m => m.FK_MeasurementType);
            //.IsRequired();
            // For DRD data
            modelBuilder.Entity<MeasurementType>()
                   .HasMany(mt => mt.DRDMeasurements)
                   .WithOne(m => m.Measurement_Type)
                   .HasForeignKey(m => m.FK_MeasurementType);
                   //.IsRequired();


            // User ownsmany for road in future
            // modelBuilder.Entity<Order>.OwnsOne(e => e.Details).WithOwner(e => e.Order);
            //  modelBuilder.Entity<Measurement>().OwnsOne(e => e.RoadReference).WithOwner(e => e.Measurement);
            modelBuilder.Entity<Measurement>().OwnsOne(e => e.MapReferences);
            modelBuilder.Entity<Measurement>().OwnsOne(m => m.MapReferences, rr =>
            {
                rr.WithOwner()
                    .HasForeignKey(m => m.FK_MeasurementId);
                    //.HasConstraintName("FK_MeasurementId");

                rr.ToTable("MapReferences");
                rr.HasKey(m => m.MapReferenceId);

                rr.Property(e => e.MapReferenceId)
                      .ValueGeneratedOnAdd();

                //if road reference has relation with others, should be in placed here
                rr.HasOne(m => m.Section).WithMany()
                 .HasForeignKey(s => s.FK_OSMWayPointId);

                ////if road reference has relation with others, should be in placed here
                //rr.HasOne(m => m.Section).WithMany().HasForeignKey(s => s.FK_OSMWayPointId);

                //rr.HasIndex

                // if mapreference has relation with others, should be in placed here
                //rr.HasOne(m => m.Measurement) ;

                //eb.HasKey(e => e.AlternateId);
                //eb.HasIndex(e => e.Id);

                //eb.HasOne(e => e.Customer).WithOne();

                //eb.HasData(
                //    new OrderDetails
                //    {
                //        AlternateId = 1,
                //        Id = -1
                //    });
            });


            //DRD
            modelBuilder.Entity<DRDMeasurement>().OwnsOne(e => e.DRDMapReferences);
            modelBuilder.Entity<DRDMeasurement>().OwnsOne(m => m.DRDMapReferences, rr =>
            {
                rr.WithOwner()
                    .HasForeignKey(m => m.FK_DRDMeasurementId);
                //.HasConstraintName("FK_MeasurementId");

                rr.ToTable("DRDMapReferences");
                rr.HasKey(m => m.DRDMapReferenceId);

                rr.Property(e => e.DRDMapReferenceId)
                      .ValueGeneratedOnAdd();


                //if road reference has relation with others, should be in placed here
                rr.HasOne(m => m.Section).WithMany().HasForeignKey(s => s.FK_OSMWayPointId);


                //rr.HasIndex

                // if road reference has relation with others, should be in placed here
                //rr.HasOne(m => m.Measurement) ;

                //eb.HasKey(e => e.AlternateId);
                //eb.HasIndex(e => e.Id);

                //eb.HasOne(e => e.Customer).WithOne();

                //eb.HasData(
                //    new OrderDetails
                //    {
                //        AlternateId = 1,
                //        Id = -1
                //    });
            });


            modelBuilder.Entity<Road>()
                   .HasKey(r => r.Road_Id);
                        modelBuilder.Entity<Road>()
                               .HasMany(r => r.Sections)
                               .WithOne(s => s.Road)
                               .HasForeignKey(s => s.FK_Road)
                               .IsRequired();

            modelBuilder.Entity<Section>()
                    .HasKey(s => s.OSMWayPointId);
                    modelBuilder.Entity<Section>()
                         .HasMany(s => s.Nodes)
                         .WithOne(n => n.Section)
                         .HasForeignKey(s => s.FK_Section)
                         .IsRequired();

               // modelBuilder.Entity<Section>()
               //    .HasMany(s => s.MapReferences)
               //    .WithOne(n => n.Section)
               //    .HasForeignKey(s => s.FK_OSMWayPointId);
               //           // .IsRequired();
               // //DRD
               //modelBuilder.Entity<Section>()
               //     .HasMany(s => s.DRDMapReferences)
               //     .WithOne(n => n.Section)
               //     .HasForeignKey(s => s.FK_OSMWayPointId);
               //    // .IsRequired();



            modelBuilder.Entity<Node>()
                    .HasKey(s => s.NodeId);

        }


        public DbSet<SourceType> SourceTypes { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Measurement> Measurements { get; set; }

        public DbSet<DRDMeasurement> DRDMeasurements { get; set; }
        public DbSet<MeasurementType> MeasurementTypes { get; set; }
        public DbSet<MapReference> MapReferences { get; set; }

        public DbSet<DRDMapReference> DRDMapReferences { get; set; }
        public DbSet<Road> Roads { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Node> Nodes { get; set; }

    }
}
