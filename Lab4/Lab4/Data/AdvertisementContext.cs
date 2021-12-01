using Assignment2.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment2.Data
{
    public class AdvertisementContext : DbContext
    {
        public AdvertisementContext(DbContextOptions<AdvertisementContext> options)
     : base(options)
        { }

        public DbSet<Advertisement> Advertisements { get; set; }

        public DbSet<CommunityAdvertisement> CommunityAdvertisements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Advertisement>().ToTable("Advertisement");
            modelBuilder.Entity<CommunityAdvertisement>().HasKey(c => new { c.AdvertismentID, c.CommunityId });

        }

    }
}
