using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using project.Models;

namespace project.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<items> items { get; set; }
        public DbSet<imge> imges { get; set; }
        public DbSet<RefreshToken> refreshTokens { get; set; }
        public DbSet<Invoice> invoices { get; set; }
    }
}
