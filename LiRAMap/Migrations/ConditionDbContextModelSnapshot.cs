﻿// <auto-generated />
using System;
using LiRAMap.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace LiRAMap.Migrations
{
    [DbContext(typeof(ConditionDbContext))]
    partial class ConditionDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("LiRAMap.Models.Condition", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("ConditionType")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("timestamp without time zone");

                    b.Property<float>("Value")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.ToTable("Conditions");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            ConditionType = 3,
                            Timestamp = new DateTime(2020, 5, 11, 21, 32, 12, 825, DateTimeKind.Local).AddTicks(6739),
                            Value = 35f
                        },
                        new
                        {
                            Id = 2,
                            ConditionType = 9,
                            Timestamp = new DateTime(2020, 5, 11, 19, 32, 12, 827, DateTimeKind.Utc).AddTicks(6663),
                            Value = 95f
                        },
                        new
                        {
                            Id = 3,
                            ConditionType = 10,
                            Timestamp = new DateTime(2020, 5, 11, 21, 32, 12, 827, DateTimeKind.Local).AddTicks(6674),
                            Value = 53f
                        },
                        new
                        {
                            Id = 4,
                            ConditionType = 0,
                            Timestamp = new DateTime(2020, 5, 11, 19, 32, 12, 827, DateTimeKind.Utc).AddTicks(6697),
                            Value = 100f
                        });
                });

            modelBuilder.Entity("LiRAMap.Models.ConditionCoverage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("ConditionId")
                        .HasColumnType("integer");

                    b.Property<int?>("EndMeters")
                        .HasColumnType("integer");

                    b.Property<int>("StartMeters")
                        .HasColumnType("integer");

                    b.Property<decimal>("Way")
                        .HasColumnType("numeric(20,0)");

                    b.HasKey("Id");

                    b.HasIndex("ConditionId");

                    b.HasIndex("Way");

                    b.ToTable("ConditionCoverages");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            ConditionId = 1,
                            EndMeters = 150,
                            StartMeters = 0,
                            Way = 27294498m
                        },
                        new
                        {
                            Id = 2,
                            ConditionId = 1,
                            EndMeters = 250,
                            StartMeters = 0,
                            Way = 26264463m
                        },
                        new
                        {
                            Id = 3,
                            ConditionId = 1,
                            EndMeters = 1000,
                            StartMeters = 575,
                            Way = 106151483m
                        },
                        new
                        {
                            Id = 4,
                            ConditionId = 2,
                            EndMeters = 78,
                            StartMeters = 52,
                            Way = 131196793m
                        },
                        new
                        {
                            Id = 5,
                            ConditionId = 3,
                            StartMeters = 60,
                            Way = 131196793m
                        },
                        new
                        {
                            Id = 6,
                            ConditionId = 4,
                            StartMeters = 10,
                            Way = 25657310m
                        });
                });

            modelBuilder.Entity("LiRAMap.Models.ConditionCoverage", b =>
                {
                    b.HasOne("LiRAMap.Models.Condition", null)
                        .WithMany("Coverage")
                        .HasForeignKey("ConditionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
