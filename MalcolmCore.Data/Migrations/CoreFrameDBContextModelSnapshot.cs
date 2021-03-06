// <auto-generated />
using System;
using MalcolmCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MalcolmCore.Data.Migrations
{
    [DbContext(typeof(CoreFrameDBContext))]
    partial class CoreFrameDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.10");

            modelBuilder.Entity("MalcolmCore.Data.Models.OneToManyMany", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("OneId")
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("OneId");

                    b.ToTable("oneToManyManies");
                });

            modelBuilder.Entity("MalcolmCore.Data.Models.OneToManySingle", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.ToTable("oneToManySingles");
                });

            modelBuilder.Entity("MalcolmCore.Data.Models.SingleModel", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("TargetId")
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("TargetId")
                        .IsUnique();

                    b.ToTable("singles");
                });

            modelBuilder.Entity("MalcolmCore.Data.Models.SingleTargetModel", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.ToTable("singleTargets");
                });

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

            modelBuilder.Entity("MalcolmCore.Data.Models.OneToManyMany", b =>
                {
                    b.HasOne("MalcolmCore.Data.Models.OneToManySingle", "One")
                        .WithMany("oneToManies")
                        .HasForeignKey("OneId");

                    b.Navigation("One");
                });

            modelBuilder.Entity("MalcolmCore.Data.Models.SingleModel", b =>
                {
                    b.HasOne("MalcolmCore.Data.Models.SingleTargetModel", "SingleTarget")
                        .WithOne("Single")
                        .HasForeignKey("MalcolmCore.Data.Models.SingleModel", "TargetId");

                    b.Navigation("SingleTarget");
                });

            modelBuilder.Entity("MalcolmCore.Data.Models.OneToManySingle", b =>
                {
                    b.Navigation("oneToManies");
                });

            modelBuilder.Entity("MalcolmCore.Data.Models.SingleTargetModel", b =>
                {
                    b.Navigation("Single");
                });
#pragma warning restore 612, 618
        }
    }
}
