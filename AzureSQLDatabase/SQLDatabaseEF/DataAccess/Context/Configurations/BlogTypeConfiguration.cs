using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Context.Configurations
{
    internal class BlogTypeConfiguration : IEntityTypeConfiguration<Blog>
    {
        public void Configure(EntityTypeBuilder<Blog> builder)
        {
            builder.ToTable("dbo", "Blogs");

            builder.HasKey(c => c.BlogId);
            builder.Property(b => b.Url).IsRequired();
            builder.Property(b => b.Description);

        }
    }
}
