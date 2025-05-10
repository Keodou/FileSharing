using FileSharing.WebApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileSharing.WebApi.Infrastructure.Persistence;

public class FileSharingDbContext(DbContextOptions<FileSharingDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<FileModel> Files { get; set; }
    public DbSet<SharedFile> SharedFiles { get; set; } 

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<FileModel>()
            .HasOne(f => f.Owner) // file has one owner (read literally)
            .WithMany(u => u.Files) // user has many files
            .HasForeignKey(f => f.OwnerId);

        modelBuilder.Entity<SharedFile>()
            .HasOne(fs => fs.File)
            .WithMany(f => f.Shares)
            .HasForeignKey(fs => fs.FileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}