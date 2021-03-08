using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PrsServer.Models;

namespace PrsServer.Data
{
    public class PrsDbContext : DbContext
    {
        public PrsDbContext (DbContextOptions<PrsDbContext> options)
            : base(options) { }

        public DbSet <User> Users { get; set; }
        public DbSet <Vendor> Vendors { get; set; }
        public DbSet <Product> Products { get; set; }
        public DbSet<Request> Requests { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Vendor>(e => { e.HasIndex(u => u.Code).IsUnique(true); });
            builder.Entity<User>(e => { e.HasIndex(u => u.Username).IsUnique(true); });
            builder.Entity<Product>(e => { e.HasIndex(u => u.PartNbr).IsUnique(true); });
        }
    }
}
