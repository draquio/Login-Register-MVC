using LoginProject.Models;
using Microsoft.EntityFrameworkCore;

namespace LoginProject.Data
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>(tb =>
            {
                tb.HasKey(col => col.Id);
                tb.Property(col => col.Id)
                    .UseIdentityColumn()
                    .ValueGeneratedOnAdd();

                tb.Property(col => col.Username).HasMaxLength(50);
                tb.Property(col => col.Email).HasMaxLength(70);
                tb.Property(col => col.Password).HasMaxLength(50);
            });
            modelBuilder.Entity<User>().ToTable("User");
        }
    }
}
