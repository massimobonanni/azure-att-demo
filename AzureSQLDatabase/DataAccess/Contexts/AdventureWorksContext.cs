using System;
using System.Collections.Generic;
using System.Text;
using DataAccess.Contexts.Configurations;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Contexts
{
    public class AdventureWorksContext : DbContext
    {

        public AdventureWorksContext(DbContextOptions<AdventureWorksContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration<Customer>(new CustomerTypeConfiguration());
        }
    }
}
