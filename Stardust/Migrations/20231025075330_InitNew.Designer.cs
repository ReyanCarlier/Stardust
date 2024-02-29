﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Stardust.Database.Context;

#nullable disable

namespace Stardust.Migrations
{
    [DbContext(typeof(StardustContext))]
    [Migration("20231025075330_InitNew")]
    partial class InitNew
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.11");

            modelBuilder.Entity("Stardust.Database.Data.GlobalScan", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("DevicesIds")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("Ended_At")
                        .HasColumnType("TEXT");

                    b.Property<string>("Initiated_By")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("Launched_At")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("GlobalScans");
                });

            modelBuilder.Entity("Stardust.Database.Data.Machine", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Hostname")
                        .HasColumnType("TEXT");

                    b.Property<string>("LastIp")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("LastScan")
                        .HasColumnType("TEXT");

                    b.Property<string>("MacAddress")
                        .HasColumnType("TEXT");

                    b.Property<bool?>("Protected")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Vendor")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Machines");
                });

            modelBuilder.Entity("Stardust.Database.Data.Report", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ClosedPorts")
                        .HasColumnType("TEXT");

                    b.Property<string>("Content")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("FilteredPorts")
                        .HasColumnType("TEXT");

                    b.Property<int?>("MachineId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("OpenPorts")
                        .HasColumnType("TEXT");

                    b.Property<int?>("Type")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UserId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Reports");
                });

            modelBuilder.Entity("Stardust.Database.Data.Script", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Category")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ConcernedPorts")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Default")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("ScriptContent")
                        .HasColumnType("TEXT");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Scripts");
                });

            modelBuilder.Entity("Stardust.Database.Data.StardustUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Avatar")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("Created_at")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .HasColumnType("TEXT");

                    b.Property<string>("GraphUserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("JiraMail")
                        .HasColumnType("TEXT");

                    b.Property<string>("JiraToken")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("Role")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("StardustUsers");
                });

            modelBuilder.Entity("Stardust.Database.Data.TaskTodo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("AssignedToId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("AssignedToName")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CompletedDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<int?>("DeviceId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("DueDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("Emergency")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("LastUpdatedDate")
                        .HasColumnType("TEXT");

                    b.Property<int?>("ReportId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("TasksTodo");
                });
#pragma warning restore 612, 618
        }
    }
}
