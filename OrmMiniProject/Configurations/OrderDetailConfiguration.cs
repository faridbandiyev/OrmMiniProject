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
    public class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
    {
        public void Configure(EntityTypeBuilder<OrderDetail> builder)
        {
            builder.ToTable("OrderDetails");

            builder.HasKey(od => od.Id);

            builder.Property(od => od.Quantity)
                   .IsRequired();

            builder.Property(od => od.PricePerItem)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.HasCheckConstraint("CK_OrderDetail_Quantity", "Quantity >= 0");
            builder.HasCheckConstraint("CK_OrderDetail_PricePerItem", "PricePerItem >= 0");
        }
    }
}
