using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MalcolmCore.Data.Models
{
    public class SingleTargetModel
    {
        [Key]
        public string Id { get; set; }
        public SingleModel Single { get; set; }
    }

    public class SingleTargeModelConfig : IEntityTypeConfiguration<SingleTargetModel>
    {
        public void Configure(EntityTypeBuilder<SingleTargetModel> builder)
        {
            builder.ToTable("singleTargets");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedOnAdd();
        }
    }
}
