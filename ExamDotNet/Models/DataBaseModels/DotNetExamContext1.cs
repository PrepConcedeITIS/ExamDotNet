using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ExamDotNet
{
    public partial class DotNetExamContext : DbContext
    {
        public DotNetExamContext()
        {
        }

        public DotNetExamContext(DbContextOptions<DotNetExamContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Domain> Domain { get; set; }
        public virtual DbSet<UrlContent> UrlContent { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=dotnetexam;Username=postgres;Password=stormwind");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Domain>(entity =>
            {
                entity.ToTable("domain");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("character varying");
            });

            modelBuilder.Entity<UrlContent>(entity =>
            {
                entity.ToTable("url_content");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasColumnName("content")
                    .HasMaxLength(1000);

                entity.Property(e => e.DomainId).HasColumnName("domain_id");

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasColumnName("url")
                    .HasColumnType("character varying");

                entity.HasOne(d => d.Domain)
                    .WithMany(p => p.UrlContent)
                    .HasForeignKey(d => d.DomainId)
                    .HasConstraintName("url_content_domain_id_fkey");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
