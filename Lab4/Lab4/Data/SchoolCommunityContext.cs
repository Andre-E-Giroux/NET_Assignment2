﻿using Assignment2.Models;
using Lab4.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab4.Data
{
    public class SchoolCommunityContext : DbContext
    {
        public SchoolCommunityContext(DbContextOptions<SchoolCommunityContext> options)
        : base(options)
        {
        }


        public DbSet<Community> Communities { get; set; }

        public DbSet<Student> Students { get; set; }

        public DbSet<CommunityMembership> CommunityMemberships { get; set; }

        public DbSet<Advertisement> Advertisements { get; set; }

        public DbSet<CommunityAdvertisement> CommunityAdvertisements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {

            modelBuilder.Entity<Student>().ToTable("Student");
            modelBuilder.Entity<Community>().ToTable("Community");
            modelBuilder.Entity<Advertisement>().ToTable("Advertisement");
            modelBuilder.Entity<CommunityMembership>().HasKey(c => new { c.StudentId, c.CommunityId });
            modelBuilder.Entity<CommunityAdvertisement>().HasKey(c => new { c.AdvertismentID, c.CommunityId });

        }





    }
}
