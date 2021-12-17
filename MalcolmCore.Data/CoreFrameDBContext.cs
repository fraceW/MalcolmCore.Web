using MalcolmCore.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MalcolmCore.Data
{
    public class CoreFrameDBContext: DbContext
    {
        public CoreFrameDBContext(DbContextOptions<CoreFrameDBContext> options) : base(options) 
        {

        }

        public DbSet<useinfo> useinfo { get; set; }
        public DbSet<useDetail> useDetails { get; set; }
        public DbSet<SingleModel> singles { get; set; }
        public DbSet<SingleTargetModel> singleTargets { get; set; }
        public DbSet<OneToManySingle> oneToManySingles { get; set; }
        public DbSet<OneToManyMany> oneToManyManies { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            base.OnModelCreating(modelBuilder);
            ///一对一
            modelBuilder.Entity<SingleModel>()
                        .HasOne(a => a.SingleTarget)
                        .WithOne(a=>a.Single)
                        .HasForeignKey<SingleModel>(c => c.TargetId);
            ///一对多
            //modelBuilder.Entity<OneToManySingle>()
            //    .HasMany(t => t.oneToManies)
            //    .WithOne(t => t.One)
            //    .HasForeignKey(p => p.OneId);
            //modelBuilder.Entity<OneToManyMany>()
            //    .HasOne(t => t.One)
            //    .WithMany(x => x.oneToManies)
            //    .HasForeignKey(t => t.OneId).IsRequired();

            modelBuilder.ApplyConfiguration(new OneToManySingleConfig());
            modelBuilder.ApplyConfiguration(new OneToManyManyConfig());


        }
    }
}
