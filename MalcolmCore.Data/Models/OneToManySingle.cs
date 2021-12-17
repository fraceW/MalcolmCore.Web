using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MalcolmCore.Data.Models
{
    public class OneToManySingle
    {
        [Key]
        public string Id { get; set; }
        public List<OneToManyMany> oneToManies { get; set; }
    }
    public class OneToManySingleConfig : IEntityTypeConfiguration<OneToManySingle>
    {
        public void Configure(EntityTypeBuilder<OneToManySingle> builder)
        {
            builder.ToTable("oneToManySingles");
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id).ValueGeneratedOnAdd();
            builder.HasMany(t => t.oneToManies).WithOne(p => p.One).HasForeignKey(t=>t.OneId);
        }
    }
}
