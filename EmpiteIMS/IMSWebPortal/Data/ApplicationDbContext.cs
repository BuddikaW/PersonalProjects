using IMSWebPortal.Data.Models.Email;
using IMSWebPortal.Data.Models.Identity;
using IMSWebPortal.Data.Models.Inventory;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMSWebPortal.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, AppRole, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<EmailDetail> EmailDetails { get; set; }

        public DbSet<ItemDetail> ItemDetails { get; set; }
    }
}
