using OrmMiniProject.DTOs.Product;
using OrmMiniProject.Exceptions;
using OrmMiniProject.Models;
using OrmMiniProject.Repositories.Generic;
using OrmMiniProject.Repositories.Interfaces;
using OrmMiniProject.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmMiniProject.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task AddProductAsync(CreateProductDTO newProduct)
        {
            if (string.IsNullOrEmpty(newProduct.Name))
            {
                throw new InvalidProductException("Product name can't be empty!");
            }
            if (newProduct.Price <= 0)
            {
                throw new InvalidProductException("Product price must be greater than zero!");
            }
            if (newProduct.Stock < 0)
            {
                throw new InvalidProductException("Product stock can't be negative!");
            }

            var product = new Product
            {
                Name = newProduct.Name,
                Price = newProduct.Price,
                Stock = newProduct.Stock,
                Description = newProduct.Description
            };

            await _repository.CreateAsync(product);
            await _repository.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(UpdateProductDTO updatedProduct)
        {
            var product = await _repository.GetSingleAsync(p => p.Id == updatedProduct.Id);
            if (product == null)
            {
                throw new NotFoundException("Product not found!");
            }

            if (!string.IsNullOrEmpty(updatedProduct.Name))
            {
                product.Name = updatedProduct.Name;
            }
            if (updatedProduct.Price.HasValue && updatedProduct.Price.Value > 0)
            {
                product.Price = updatedProduct.Price.Value;
            }
            if (updatedProduct.Stock.HasValue && updatedProduct.Stock.Value >= 0)
            {
                product.Stock = updatedProduct.Stock.Value;
            }
            else if (updatedProduct.Stock.HasValue)
            {
                throw new InvalidProductException("Product stock can't be negative!");
            }

            product.Description = updatedProduct.Description;
            product.UpdatedDate = DateTime.UtcNow;

            _repository.Update(product);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _repository.GetSingleAsync(p => p.Id == id);
            if (product == null)
            {
                throw new NotFoundException("Product not found!");
            }

            _repository.Delete(product);
            await _repository.SaveChangesAsync();
        }

        public async Task<List<ProductDTO>> GetProductsAsync()
        {
            var products = await _repository.GetAllAsync();
            return products.Select(p => new ProductDTO
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Stock = p.Stock,
                Description = p.Description,
                CreatedDate = p.CreatedDate,
                UpdatedDate = p.UpdatedDate
            }).ToList();
        }

        public async Task<List<ProductDTO>> SearchProductsAsync(string searchQuery)
        {
            var products = await _repository.GetAllAsync();
            var filteredProducts = products
                .Where(p => p.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                .Select(p => new ProductDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Stock = p.Stock,
                    Description = p.Description,
                    CreatedDate = p.CreatedDate,
                    UpdatedDate = p.UpdatedDate
                }).ToList();

            return filteredProducts;
        }

        public async Task<ProductDTO> GetProductByIdAsync(int id)
        {
            var product = await _repository.GetSingleAsync(p => p.Id == id);
            if (product == null)
            {
                throw new NotFoundException("Product not found!");
            }

            return new ProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Stock = product.Stock,
                Description = product.Description,
                CreatedDate = product.CreatedDate,
                UpdatedDate = product.UpdatedDate
            };
        }
    }
}