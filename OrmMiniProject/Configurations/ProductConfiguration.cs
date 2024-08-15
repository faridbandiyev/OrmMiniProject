using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using OrmMiniProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmMiniProject.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(p => p.Price)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(p => p.Stock)
                   .IsRequired();

            builder.Property(p => p.Description)
                   .HasMaxLength(500);

            builder.Property(p => p.CreatedDate)
                   .IsRequired();

            builder.Property(p => p.UpdatedDate)
                   .IsRequired(false);

            builder.HasCheckConstraint("CK_Product_Price", "Price >= 0");
            builder.HasCheckConstraint("CK_Product_Stock", "Stock >= 0");
        }
    }
}
