using GestionAppels.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace GestionAppels.Server.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Fiche> Fiches { get; set; } = null!;
    public DbSet<TypeDemande> TypeDemandes { get; set; } = null!;
    public DbSet<SousTypeDemande> SousTypeDemandes { get; set; } = null!;
    public DbSet<Adherent> Adherents { get; set; } = null!; 
    public DbSet<SyncState> SyncStates { get; set; } = null!; 
    public DbSet<Division> Divisions { get; set; } = null!;
    public DbSet<Service> Services { get; set; } = null!;
    public DbSet<FicheServiceHistory> FicheServiceHistories { get; set; } = null!;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Adherent>(entity =>
        {

            entity.HasIndex(a => a.Affiliation)
                  .HasDatabaseName("IX_Adherent_Affiliation")
                  .IsUnique();

        });

        modelBuilder.Entity<SyncState>(entity =>
        {
            entity.HasKey(s => s.SyncProcessName); 
        });

        modelBuilder.Entity<Adherent>()
            .HasMany(a => a.Fiches) 
            .WithOne(f => f.Adherent)
            .HasForeignKey(f => f.Affiliation) 
            .HasPrincipalKey(a => a.Affiliation); 

        // Configure User-Service relationship
        //modelBuilder.Entity<User>()
        //    .HasOne(u => u.Service)
        //    .WithMany()
        //    .HasForeignKey(u => u.ServiceId)
        //    .OnDelete(DeleteBehavior.Restrict);

        // Configure Service-Division relationship (already in model, but explicit config if needed)
        modelBuilder.Entity<Service>()
            .HasOne(s => s.Division)
            .WithMany(d => d.Services)
            .HasForeignKey(s => s.DivisionID)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Fiche-FicheServiceHistory relationship
        modelBuilder.Entity<FicheServiceHistory>()
            .HasOne(fsh => fsh.Fiche)
            .WithMany(f => f.ServiceHistory)
            .HasForeignKey(fsh => fsh.FicheId);

        modelBuilder.Entity<FicheServiceHistory>()
            .HasOne(fsh => fsh.Service)
            .WithMany()
            .HasForeignKey(fsh => fsh.ServiceId);
    }
}
