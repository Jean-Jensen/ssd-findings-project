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

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<Finding> Findings { get; set; }

    public virtual DbSet<UserLink> UserLinks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.AuditId).HasName("audit_log_pkey");

            entity.ToTable("audit_log");

            entity.HasIndex(e => new { e.FindingId, e.OccurredAt }, "ix_audit_finding_time");

            entity.Property(e => e.AuditId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("audit_id");
            entity.Property(e => e.Action).HasColumnName("action");
            entity.Property(e => e.ActorEmail).HasColumnName("actor_email");
            entity.Property(e => e.ActorId).HasColumnName("actor_id");
            entity.Property(e => e.FieldName).HasColumnName("field_name");
            entity.Property(e => e.FindingId).HasColumnName("finding_id");
            entity.Property(e => e.NewValue).HasColumnName("new_value");
            entity.Property(e => e.OccurredAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("occurred_at");
            entity.Property(e => e.OldValue).HasColumnName("old_value");

            entity.HasOne(d => d.Finding).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.FindingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("audit_log_finding_id_fkey");
        });

        modelBuilder.Entity<Finding>(entity =>
        {
            entity.HasKey(e => e.FindingId).HasName("findings_pkey");

            entity.ToTable("findings");

            entity.HasIndex(e => e.AffectedAsset, "ix_findings_asset");

            entity.HasIndex(e => e.DiscoveryDate, "ix_findings_discovery");

            entity.HasIndex(e => e.DueDate, "ix_findings_due_open").HasFilter("(status = ANY (ARRAY['New'::text, 'Triaged'::text, 'In Progress'::text]))");

            entity.HasIndex(e => new { e.Status, e.Severity }, "ix_findings_status_severity");

            entity.Property(e => e.FindingId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("finding_id");
            entity.Property(e => e.AffectedAsset).HasColumnName("affected_asset");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatorId).HasColumnName("creator_id");
            entity.Property(e => e.Details).HasColumnName("details");
            entity.Property(e => e.DiscoveryDate).HasColumnName("discovery_date");
            entity.Property(e => e.DueDate).HasColumnName("due_date");
            entity.Property(e => e.ReporterEmail).HasColumnName("reporter_email");
            entity.Property(e => e.Severity).HasColumnName("severity");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'New'::text")
                .HasColumnName("status");
            entity.Property(e => e.Title).HasColumnName("title");

            entity.HasOne(d => d.Creator).WithMany(p => p.Findings)
                .HasForeignKey(d => d.CreatorId)
                .HasConstraintName("findings_creator_id_fkey");

            entity.HasMany(d => d.Users).WithMany(p => p.FindingsNavigation)
                .UsingEntity<Dictionary<string, object>>(
                    "UserFinding",
                    r => r.HasOne<UserLink>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("user_findings_user_id_fkey"),
                    l => l.HasOne<Finding>().WithMany()
                        .HasForeignKey("FindingId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("user_findings_finding_id_fkey"),
                    j =>
                    {
                        j.HasKey("FindingId", "UserId").HasName("user_findings_pkey");
                        j.ToTable("user_findings");
                        j.HasIndex(new[] { "UserId" }, "ix_user_findings_user");
                        j.IndexerProperty<Guid>("FindingId").HasColumnName("finding_id");
                        j.IndexerProperty<Guid>("UserId").HasColumnName("user_id");
                    });
        });

        modelBuilder.Entity<UserLink>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("user_link_pkey");

            entity.ToTable("user_link");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("user_id");
            entity.Property(e => e.DisplayName).HasColumnName("display_name");
            entity.Property(e => e.Email).HasColumnName("email");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
