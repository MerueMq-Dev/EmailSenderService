using EmailSenderService.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace EmailSenderService
{
    public class EmailAppDbContext:DbContext
    {
        public EmailAppDbContext(DbContextOptions<EmailAppDbContext> options) : base(options)
        {
                
        }

        public DbSet<EmailEntity> Emails { get; set; }

        public DbSet<EmailAdressEntity>  emailAdresses { get; set; }


        /// <summary>
        /// Create a one-to-many relationship
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EmailEntity>()
                        .HasMany(e =>e.Recipients)
                        .WithOne(r => r.EmailEntity)
                        .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
