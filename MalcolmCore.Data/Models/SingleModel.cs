using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MalcolmCore.Data.Models
{
    public class SingleModel
    {
        [Key]
        public string Id { get; set; }
        [ForeignKey("TargetId")]
        public string TargetId { get; set; }
        public SingleTargetModel SingleTarget { get; set; }
    }

    public class SingleModelConfig : IEntityTypeConfiguration<SingleModel>
    {
        public void Configure(EntityTypeBuilder<SingleModel> builder)
        {
            builder.ToTable("singles");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedOnAdd();
            builder.HasOne(t => t.SingleTarget).WithOne(r => r.Single).HasForeignKey<SingleModel>(l=>l.TargetId); 
        }
    }
}
