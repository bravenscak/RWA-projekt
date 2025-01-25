using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MiniOglasnikZaBesplatneStvariLibrary.Models;

public partial class AdvertisementRwaContext : DbContext
{
    public AdvertisementRwaContext()
    {
    }

    public AdvertisementRwaContext(DbContextOptions<AdvertisementRwaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<ItemTag> ItemTags { get; set; }

    public virtual DbSet<ItemType> ItemTypes { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<UserDetail> UserDetails { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("server=.;Database=AdvertisementRWA;User=sa;Password=SQL;TrustServerCertificate=True;MultipleActiveResultSets=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.Iditem).HasName("PK__Item__C9778A10CFEBE6DE");

            entity.ToTable("Item");

            entity.Property(e => e.Iditem).HasColumnName("IDItem");
            entity.Property(e => e.Name).HasMaxLength(60);
            entity.Property(e => e.TypeId).HasColumnName("TypeID");

            entity.HasOne(d => d.Type).WithMany(p => p.Items)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Item_Type");
        });

        modelBuilder.Entity<ItemTag>(entity =>
        {
            entity.HasKey(e => e.IditemTag).HasName("PK__ItemTag__D26944C8AF5AC8B6");

            entity.ToTable("ItemTag");

            entity.Property(e => e.IditemTag).HasColumnName("IDItemTag");
            entity.Property(e => e.ItemId).HasColumnName("ItemID");
            entity.Property(e => e.TagId).HasColumnName("TagID");

            entity.HasOne(d => d.Item).WithMany(p => p.ItemTags)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ItemTag_Item");

            entity.HasOne(d => d.Tag).WithMany(p => p.ItemTags)
                .HasForeignKey(d => d.TagId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ItemTag_Tag");
        });

        modelBuilder.Entity<ItemType>(entity =>
        {
            entity.HasKey(e => e.IditemType).HasName("PK__ItemType__17AF366BCA071407");

            entity.ToTable("ItemType");

            entity.Property(e => e.IditemType).HasColumnName("IDItemType");
            entity.Property(e => e.Name).HasMaxLength(60);
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__Log__5E548648D945ABDA");

            entity.ToTable("Log");

            entity.Property(e => e.Level).HasMaxLength(100);
            entity.Property(e => e.Message).HasMaxLength(100);
            entity.Property(e => e.Timestamp).HasColumnType("datetime");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.Idreservation).HasName("PK__Reservat__53DF2D8DE3ADC324");

            entity.ToTable("Reservation");

            entity.Property(e => e.Idreservation).HasColumnName("IDReservation");
            entity.Property(e => e.ItemId).HasColumnName("ItemID");
            entity.Property(e => e.ReservationDate).HasColumnType("date");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UserDetailId).HasColumnName("UserDetailID");

            entity.HasOne(d => d.Item).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Reservation_Item");

            entity.HasOne(d => d.UserDetail).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.UserDetailId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Reservation_UserDetail");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Idtag).HasName("PK__Tag__A7023751E69D41D5");

            entity.ToTable("Tag");

            entity.Property(e => e.Idtag).HasColumnName("IDTag");
            entity.Property(e => e.Name).HasMaxLength(60);
        });

        modelBuilder.Entity<UserDetail>(entity =>
        {
            entity.HasKey(e => e.IdUserDetails).HasName("PK__UserDeta__D02C71242DE62578");

            entity.ToTable("UserDetail");

            entity.Property(e => e.IdUserDetails).HasColumnName("IDUserDetails");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(256);
            entity.Property(e => e.PasswordSalt).HasMaxLength(256);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.UserRoleId).HasColumnName("UserRoleID");
            entity.Property(e => e.Username).HasMaxLength(60);

            entity.HasOne(d => d.UserRole).WithMany(p => p.UserDetails)
                .HasForeignKey(d => d.UserRoleId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_UserDetail_UserRole");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.IduserRole).HasName("PK__UserRole__5A7AF7813EFCF476");

            entity.ToTable("UserRole");

            entity.Property(e => e.IduserRole).HasColumnName("IDUserRole");
            entity.Property(e => e.Name).HasMaxLength(60);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
