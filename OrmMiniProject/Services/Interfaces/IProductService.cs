using OrmMiniProject.DTOs.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmMiniProject.Services.Interfaces
{
    public interface IProductService
    {
        Task AddProductAsync(CreateProductDTO newProduct);
        Task UpdateProductAsync(UpdateProductDTO updatedProduct);
        Task DeleteProductAsync(int id);
        Task<List<ProductDTO>> GetProductsAsync();
        Task<List<ProductDTO>> SearchProductsAsync(string searchQuery);
        Task<ProductDTO> GetProductByIdAsync(int id);
    }
}
