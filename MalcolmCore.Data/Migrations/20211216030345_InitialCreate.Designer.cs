﻿// <auto-generated />
using System;
using MalcolmCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MalcolmCore.Data.Migrations
{
    [DbContext(typeof(CoreFrameDBContext))]
    [Migration("20211216030345_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.10");

            modelBuilder.Entity("MalcolmCore.Data.useDetail", b =>
                {
                    b.Property<string>("id")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("useDetails")
                        .HasColumnType("longtext");

                    b.HasKey("id");

                    b.ToTable("useDetails");
                });

            modelBuilder.Entity("MalcolmCore.Data.useinfo", b =>
                {
                    b.Property<string>("id")
                        .HasColumnType("varchar(255)");

                    b.Property<DateTime>("creatdate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("pwd")
                        .HasColumnType("longtext");

                    b.Property<string>("usename")
                        .HasColumnType("longtext");

                    b.Property<string>("useremark")
                        .HasColumnType("longtext");

                    b.HasKey("id");

                    b.ToTable("useinfo");
                });
#pragma warning restore 612, 618
        }
    }
}
