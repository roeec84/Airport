﻿// <auto-generated />
using System;
using DAL.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DAL.Migrations
{
    [DbContext(typeof(AirportDbContext))]
    partial class AirportDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("Common.Models.Airplane", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<bool>("IsAvailable")
                        .HasColumnType("bit");

                    b.Property<bool>("ReadyForNextStation")
                        .HasColumnType("bit");

                    b.Property<double>("TimeToStay")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("Airplanes");
                });

            modelBuilder.Entity("Common.Models.Flight", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<DateTime?>("ActualFlightTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("AirplaneId")
                        .HasColumnType("int");

                    b.Property<bool>("Delayed")
                        .HasColumnType("bit");

                    b.Property<string>("FlightNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("FlightTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("FlightType")
                        .HasColumnType("int");

                    b.Property<bool>("IsOver")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("AirplaneId");

                    b.ToTable("Flights");
                });

            modelBuilder.Entity("Common.Models.History", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<DateTime?>("EnterToStation")
                        .HasColumnType("datetime2");

                    b.Property<int>("FlightId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("LeftStation")
                        .HasColumnType("datetime2");

                    b.Property<int>("StationId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("FlightId");

                    b.HasIndex("StationId");

                    b.ToTable("Histories");
                });

            modelBuilder.Entity("Common.Models.Station", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int>("AirplaneId")
                        .HasColumnType("int");

                    b.Property<bool>("IsAvailable")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("Stations");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            AirplaneId = 0,
                            IsAvailable = true
                        },
                        new
                        {
                            Id = 2,
                            AirplaneId = 0,
                            IsAvailable = true
                        },
                        new
                        {
                            Id = 3,
                            AirplaneId = 0,
                            IsAvailable = true
                        },
                        new
                        {
                            Id = 4,
                            AirplaneId = 0,
                            IsAvailable = true
                        },
                        new
                        {
                            Id = 5,
                            AirplaneId = 0,
                            IsAvailable = true
                        },
                        new
                        {
                            Id = 6,
                            AirplaneId = 0,
                            IsAvailable = true
                        },
                        new
                        {
                            Id = 7,
                            AirplaneId = 0,
                            IsAvailable = true
                        },
                        new
                        {
                            Id = 8,
                            AirplaneId = 0,
                            IsAvailable = true
                        });
                });

            modelBuilder.Entity("Common.Models.Flight", b =>
                {
                    b.HasOne("Common.Models.Airplane", "Airplane")
                        .WithMany()
                        .HasForeignKey("AirplaneId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Airplane");
                });

            modelBuilder.Entity("Common.Models.History", b =>
                {
                    b.HasOne("Common.Models.Flight", "Flight")
                        .WithMany()
                        .HasForeignKey("FlightId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Common.Models.Station", "Station")
                        .WithMany()
                        .HasForeignKey("StationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Flight");

                    b.Navigation("Station");
                });
#pragma warning restore 612, 618
        }
    }
}
