using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MalcolmCore.Data.Models
{
    public class OneToManyMany
    {
        [Key]
        public string Id { get; set; }
        [ForeignKey("OneId")]
        public string OneId { get; set; }
        public OneToManySingle One { get; set; }
    }

    public class OneToManyManyConfig : IEntityTypeConfiguration<OneToManyMany>
    {
        public void Configure(EntityTypeBuilder<OneToManyMany> builder)
        {
            builder.ToTable("oneToManyManies");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedOnAdd();
            builder.HasOne(o => o.One).WithMany(o => o.oneToManies).HasForeignKey(o => o.OneId);

        }
    }
}
