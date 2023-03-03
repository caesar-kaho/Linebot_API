using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LineBotMessage.Models;

public partial class LinebotAPIContext : DbContext
{
    public LinebotAPIContext()
    {
    }

    public LinebotAPIContext(DbContextOptions<LinebotAPIContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<ExtentionNumber> ExtentionNumbers { get; set; }

    public virtual DbSet<Staff> Staffs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;port=3306;database=linebot_ntus;user=root;password=NTUS@1234", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.11.2-mariadb"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("latin1_swedish_ci")
            .HasCharSet("latin1");

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("departments", tb => tb.HasComment("部門表"))
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.Property(e => e.Id)
                .HasColumnType("int(3)")
                .HasColumnName("id");
            entity.Property(e => e.DepartmentName)
                .HasMaxLength(50)
                .HasDefaultValueSql("''")
                .HasComment("部門名稱")
                .HasColumnName("departmentName");
        });

        modelBuilder.Entity<ExtentionNumber>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("extention_numbers", tb => tb.HasComment("分機表\r\n"))
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.HasIndex(e => e.StaffsId, "FK_extention_numbers_staffs");

            entity.Property(e => e.Id)
                .HasColumnType("int(3)")
                .HasColumnName("id");
            entity.Property(e => e.ExtentionNumbers)
                .HasMaxLength(10)
                .HasDefaultValueSql("'0'")
                .HasColumnName("extention_numbers");
            entity.Property(e => e.StaffsId)
                .HasColumnType("int(11)")
                .HasColumnName("staffs_id");

            entity.HasOne(d => d.Staffs).WithMany(p => p.ExtentionNumbers)
                .HasForeignKey(d => d.StaffsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_extention_numbers_staffs");
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("staffs", tb => tb.HasComment("職員表"))
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.HasIndex(e => e.DepartmentId, "department_id");

            entity.Property(e => e.Id)
                .HasColumnType("int(3)")
                .HasColumnName("id");
            entity.Property(e => e.DepartmentId)
                .HasColumnType("int(11)")
                .HasColumnName("department_id");
            entity.Property(e => e.Name)
                .HasMaxLength(25)
                .HasDefaultValueSql("'0'")
                .HasColumnName("name");

            entity.HasOne(d => d.Department).WithMany(p => p.Staff)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("department_id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
