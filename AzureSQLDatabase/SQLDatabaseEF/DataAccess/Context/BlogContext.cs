using DataAccess.Context.Configurations;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Context
{
    public class BlogContext : DbContext
    {
        public BlogContext():base()
        {

        }

        public BlogContext(DbContextOptions<BlogContext> options) : base(options)
        {
        }

        public DbSet<Blog> Blogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>().HasKey(c => c.BlogId);
            modelBuilder.Entity<Blog>().Property(b => b.Url).IsRequired();
            modelBuilder.Entity<Blog>().Property(b => b.Description);

            //modelBuilder.ApplyConfiguration<Blog>(new BlogTypeConfiguration());
        }

    }

}
