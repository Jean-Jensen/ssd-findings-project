using System;
using System.Collections.Generic;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres.Scaffolding;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Finding> Findings { get; set; }

    public virtual DbSet<Userinfo> Userinfos { get; set; }

    public virtual DbSet<Userlink> Userlinks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("severity", new[] { "Critical", "High", "Medium", "Low" })
            .HasPostgresEnum("status", new[] { "New", "Triaged", "In-Progress", "Remediated", "Accepted Risk" });

        modelBuilder.Entity<Finding>(entity =>
        {
            entity.HasKey(e => e.Findingid).HasName("findings_pkey");

            entity.ToTable("findings");

            entity.Property(e => e.Findingid).HasColumnName("findingid");
            entity.Property(e => e.Creatorid).HasColumnName("creatorid");
            entity.Property(e => e.Details).HasColumnName("details");
            entity.Property(e => e.Discoverydate).HasColumnName("discoverydate");
        });

        modelBuilder.Entity<Userinfo>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("userinfo");

            entity.Property(e => e.Additionalclaims)
                .HasColumnType("jsonb")
                .HasColumnName("additionalclaims");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Isauthenticated).HasColumnName("isauthenticated");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<Userlink>(entity =>
        {
            entity.HasKey(e => e.Userid).HasName("userlink_pkey");

            entity.ToTable("userlink");

            entity.Property(e => e.Userid).HasColumnName("userid");
            entity.Property(e => e.Email).HasColumnName("email");

            entity.HasMany(d => d.Findings).WithMany(p => p.Assigneds)
                .UsingEntity<Dictionary<string, object>>(
                    "Userfinding",
                    r => r.HasOne<Finding>().WithMany()
                        .HasForeignKey("Findingid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("findings_userfindings_fk"),
                    l => l.HasOne<Userlink>().WithMany()
                        .HasForeignKey("Assignedid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("user_userfindings_fk"),
                    j =>
                    {
                        j.HasKey("Assignedid", "Findingid").HasName("userfindings_pkey");
                        j.ToTable("userfindings");
                        j.IndexerProperty<string>("Assignedid").HasColumnName("assignedid");
                        j.IndexerProperty<string>("Findingid").HasColumnName("findingid");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
