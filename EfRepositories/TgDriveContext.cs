using EfRepositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace EfRepositories;

public class TgDriveContext : DbContext
{
    public TgDriveContext(DbContextOptions<TgDriveContext> options)
        : base(options)
    {
    }

    public DbSet<FileEntity> Files => Set<FileEntity>();
    public DbSet<DirectoryEntity> Directories => Set<DirectoryEntity>();
    public DbSet<DirectoryAccess> DirectoriesAccesses => Set<DirectoryAccess>();
    public DbSet<UserInfoEntity> Users => Set<UserInfoEntity>();

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
        modelBuilder
            .Entity<DirectoryAccess>()
            .HasKey(da => new {da.UserId, da.DirectoryId});
    }
}
