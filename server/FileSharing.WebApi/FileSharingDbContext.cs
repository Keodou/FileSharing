using FileSharing.WebApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileSharing.WebApi;

public class FileSharingDbContext(DbContextOptions<FileSharingDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<FileModel> Files { get; set; }
}