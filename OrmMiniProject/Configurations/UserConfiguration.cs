﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using OrmMiniProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmMiniProject.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.FullName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(u => u.Email)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(u => u.Password)
                   .IsRequired();

            builder.Property(u => u.Address)
                   .HasMaxLength(200);

            builder.HasCheckConstraint("CK_User_Email", "LEN(Email) > 0");
        }
    }
}
