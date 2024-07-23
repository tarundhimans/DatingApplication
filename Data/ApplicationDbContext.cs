using DatingApplication.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DatingApplication.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ApplicationUser>ApplicationUsers { get; set; }
        public DbSet<Profile> Profiles {  get; set; }
        public DbSet<Message>Messages { get; set; }
        public DbSet<Match> Matches { get; set; }
        
    }
}
