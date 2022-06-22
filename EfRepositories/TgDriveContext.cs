using EfRepositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace EfRepositories;

public class TgDriveContext : DbContext
{
    public TgDriveContext(DbContextOptions<TgDriveContext> options)
        : base(options)
    {
    }

    public DbSet<FileEntity> Files { get; set; }
    public DbSet<DirectoryEntity> Directories { get; set; }
    public DbSet<DirectoryAccess> DirectoriesAccesses { get; set; }
    public DbSet<UserInfoEntity> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<DirectoryAccess>()
            .HasOne(x => x.Directory)
            .WithMany(x => x.Accesses);
        modelBuilder
            .Entity<DirectoryEntity>()
            .HasOne(x => x.Parent)
            .WithMany(x => x.Children)
            .HasForeignKey(x => x.ParentId);
        modelBuilder
            .Entity<DirectoryEntity>()
            .HasMany(x => x.Files)
            .WithOne(x => x.Directory)
            .HasForeignKey(x => x.DirectoryId);
        modelBuilder
            .Entity<DirectoryEntity>()
            .HasMany(x => x.Accesses)
            .WithOne(x => x.Directory)
            .HasForeignKey(x => x.DirectoryId);
    }
}
