using  Ecommerce.Services.DAO.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace  Ecommerce.Services.DAO.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id); 
            builder.Property(p => p.Name)
                .IsRequired() 
                .HasMaxLength(100); 
            builder.Property(p => p.Description)
                .HasMaxLength(500);  
            builder.Property(p => p.Price)
                .HasColumnType("float(18,2)");
            builder.Property(p => p.Stock)
               .HasColumnType("float(18,2)");
        }
    }
}
