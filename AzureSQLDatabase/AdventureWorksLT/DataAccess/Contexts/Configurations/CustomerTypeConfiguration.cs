using System;
using System.Collections.Generic;
using System.Text;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Contexts.Configurations
{
    internal class CustomerTypeConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customer", "SalesLT");

            builder.HasKey(p => p.CustomerId);

            builder.Property(p => p.NameStyle).IsRequired();
            builder.Property(p => p.Title).HasMaxLength(8);
            builder.Property(p => p.FirstName).IsRequired().HasMaxLength(50);
            builder.Property(p => p.MiddleName).HasMaxLength(50);
            builder.Property(p => p.LastName).IsRequired().HasMaxLength(50);
        }
    }
}
