using System;
using System.Collections.Generic;
using System.Text;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Contexts.Configurations
{
    internal class PersonTypeConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.ToTable("Person", "Person");

            builder.HasKey(p => p.BusinessEntityID);

            builder.Property(p => p.PersonType).IsRequired().HasMaxLength(2).IsFixedLength();
            builder.Property(p => p.NameStyle).IsRequired();
            builder.Property(p => p.Title).HasMaxLength(8);
            builder.Property(p => p.FirstName).IsRequired().HasMaxLength(50);
            builder.Property(p => p.MiddleName).HasMaxLength(50);
            builder.Property(p => p.LastName).IsRequired().HasMaxLength(50);
        }
    }
}
