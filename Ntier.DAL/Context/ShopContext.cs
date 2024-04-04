using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Ntier.DAL.Entities;

namespace Ntier.DAL.Context
{
    public partial class ShopContext : DbContext
    {
        public ShopContext()
        {
        }

        public ShopContext(DbContextOptions<ShopContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<ProductImage> ProductImages { get; set; } = null!;
        public virtual DbSet<ProductSize> ProductSizes { get; set; } = null!;
        public virtual DbSet<ProductSizeDetail> ProductSizeDetails { get; set; } = null!;
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("CATEGORY");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Name)
                    .HasMaxLength(20)
                    .HasColumnName("NAME");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("ORDERS");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Address)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("ADDRESS");

                entity.Property(e => e.CreateAt)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("CREATE_AT");

                entity.Property(e => e.UserId)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("USER_ID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__ORDERS__USER_ID__412EB0B6");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ORDER_DETAIL");

                entity.Property(e => e.OrderId).HasColumnName("ORDER_ID");

                entity.Property(e => e.ProductId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("PRODUCT_ID");

                entity.Property(e => e.Quantity).HasColumnName("QUANTITY");

                entity.HasOne(d => d.Order)
                    .WithMany()
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK__ORDER_DET__ORDER__32AB8735");

                entity.HasOne(d => d.Product)
                    .WithMany()
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__ORDER_DET__PRODU__3587F3E0");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("PRODUCT");

                entity.Property(e => e.Id)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("ID");

                entity.Property(e => e.CategoryId).HasColumnName("CATEGORY_ID");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATE_AT");

                entity.Property(e => e.Description)
                    .HasColumnType("text")
                    .HasColumnName("DESCRIPTION");

                entity.Property(e => e.MinPrice).HasColumnName("MIN_PRICE");

                entity.Property(e => e.Name)
                    .HasMaxLength(40)
                    .HasColumnName("NAME");

                entity.Property(e => e.Sale).HasColumnName("SALE");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK__PRODUCT__CATEGOR__2CF2ADDF");
            });

            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.HasKey(e => e.Url)
                    .HasName("PK__PRODUCT___C5B100083B40F9CD");

                entity.ToTable("PRODUCT_IMAGE");

                entity.Property(e => e.Url)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("URL");

                entity.Property(e => e.Index).HasColumnName("INDEX");

                entity.Property(e => e.ProductId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("PRODUCT_ID");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductImages)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__PRODUCT_I__PRODU__395884C4");
            });

            modelBuilder.Entity<ProductSize>(entity =>
            {
                entity.ToTable("PRODUCT_SIZE");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Size).HasColumnName("SIZE");
            });

            modelBuilder.Entity<ProductSizeDetail>(entity =>
            {
                entity.HasKey(e => new { e.ProductId, e.SizeId })
                    .HasName("PK__PRODUCT___9420F6C487A5BC55");

                entity.ToTable("PRODUCT_SIZE_DETAIL");

                entity.Property(e => e.ProductId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("PRODUCT_ID");

                entity.Property(e => e.SizeId).HasColumnName("SIZE_ID");

                entity.Property(e => e.Price).HasColumnName("PRICE");

                entity.Property(e => e.Quantity).HasColumnName("QUANTITY");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductSizeDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PRODUCT_S__PRODU__3D2915A8");

                entity.HasOne(d => d.Size)
                    .WithMany(p => p.ProductSizeDetails)
                    .HasForeignKey(d => d.SizeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PRODUCT_S__SIZE___3E1D39E1");
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.ToTable("REFRESH_TOKEN");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ExpireAt)
                    .HasColumnType("datetime")
                    .HasColumnName("EXPIRE_AT");

                entity.Property(e => e.RefreshTk)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("REFRESH_TK");

                entity.Property(e => e.Userid)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("USERID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.RefreshTokens)
                    .HasForeignKey(d => d.Userid)
                    .HasConstraintName("FK__REFRESH_T__USERI__06CD04F7");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("USER");

                entity.Property(e => e.Id)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("ID");

                entity.Property(e => e.Email)
                    .HasMaxLength(40)
                    .IsUnicode(false)
                    .HasColumnName("EMAIL");

                entity.Property(e => e.Name)
                    .HasMaxLength(30)
                    .HasColumnName("NAME");

                entity.Property(e => e.Password)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("PASSWORD");

                entity.Property(e => e.Role)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("ROLE")
                    .HasDefaultValueSql("('Member')");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
