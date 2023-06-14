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

    public virtual DbSet<Staff> Staffs { get; set; }

    public virtual DbSet<StaffsMultiExtentionnumber> StaffsMultiExtentionnumbers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;port=3306;database=db_linebot;user=root;password=NTUS@1234", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.11.2-mariadb"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("latin1_swedish_ci")
            .HasCharSet("latin1");

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("departments", tb => tb.HasComment("部門表"));

            entity.Property(e => e.Id)
                .HasColumnType("int(3) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.DepartmentsName)
                .HasMaxLength(25)
                .HasDefaultValueSql("'0'")
                .HasComment("部門名稱")
                .HasColumnName("departments_name");
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("staffs", tb => tb.HasComment("職員表"));

            entity.HasIndex(e => e.StaffsDepartment, "FK_staffs_departments_department_id");

            entity.Property(e => e.Id)
                .HasColumnType("int(3) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.StaffsDepartment)
                .HasComment("所屬部門")
                .HasColumnType("int(3) unsigned")
                .HasColumnName("staffs_department");
            entity.Property(e => e.StaffsExtentionnumber)
                .HasMaxLength(4)
                .HasDefaultValueSql("'0'")
                .IsFixedLength()
                .HasComment("分機號碼")
                .HasColumnName("staffs_extentionnumber");
            entity.Property(e => e.StaffsName)
                .HasMaxLength(25)
                .HasDefaultValueSql("'0'")
                .HasComment("職員名稱")
                .HasColumnName("staffs_name");

            entity.HasOne(d => d.StaffsDepartmentNavigation).WithMany(p => p.Staff)
                .HasForeignKey(d => d.StaffsDepartment)
                .HasConstraintName("FK_staffs_departments_department_id");
        });

        modelBuilder.Entity<StaffsMultiExtentionnumber>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("staffs_multi_extentionnumbers", tb => tb.HasComment("職員有多個分機號碼"));

            entity.HasIndex(e => e.StaffsDepartment, "FK_staffs_multi_extentionnumber_departments_department_id");

            entity.Property(e => e.Id)
                .HasColumnType("int(3) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.StaffsDepartment)
                .HasComment("所屬單位")
                .HasColumnType("int(3) unsigned")
                .HasColumnName("staffs_department");
            entity.Property(e => e.StaffsExtentionnumber1)
                .HasMaxLength(4)
                .HasDefaultValueSql("'0'")
                .IsFixedLength()
                .HasComment("分機號碼1")
                .HasColumnName("staffs_extentionnumber_1");
            entity.Property(e => e.StaffsExtentionnumber2)
                .HasMaxLength(4)
                .HasDefaultValueSql("'0'")
                .IsFixedLength()
                .HasComment("分機號碼2")
                .HasColumnName("staffs_extentionnumber_2");
            entity.Property(e => e.StaffsExtentionnumber3)
                .HasMaxLength(4)
                .HasDefaultValueSql("'0'")
                .IsFixedLength()
                .HasComment("分機號碼3")
                .HasColumnName("staffs_extentionnumber_3");
            entity.Property(e => e.StaffsName)
                .HasMaxLength(25)
                .HasDefaultValueSql("'0'")
                .HasComment("職員名稱")
                .HasColumnName("staffs_name");

            entity.HasOne(d => d.StaffsDepartmentNavigation).WithMany(p => p.StaffsMultiExtentionnumbers)
                .HasForeignKey(d => d.StaffsDepartment)
                .HasConstraintName("FK_staffs_multi_extentionnumber_departments_department_id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
