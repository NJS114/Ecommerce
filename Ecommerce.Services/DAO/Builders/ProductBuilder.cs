using Ecommerce.Services.DAO.Enums;
using Ecommerce.Services.DAO.Models;

namespace Ecommerce.Services.DAO.Builders
{
    public class ProductBuilder
    {
        public readonly Product _product;

        public ProductBuilder()
        {
            _product = new Product();
        }

        public ProductBuilder(Product product)
        {
            _product = product;
        }

        public ProductBuilder SetId(string id)
        {
            _product.Id = id;
            return this;
        }

        public ProductBuilder SetName(string name)
        {
            _product.Name = name;
            return this;
        }

        public ProductBuilder SetDescription(string description)
        {
            _product.Description = description;
            return this;
        }

        public ProductBuilder SetPrice(decimal price)
        {
            _product.Price = price;
            return this;
        }

        public ProductBuilder SetCategory(Category category)
        {
            _product.Category = category; 
            return this;
        }
        public ProductBuilder SetStock(int stock)
        {
            _product.Stock = stock;
            return this;
        }
        public Product Build()
        {
            return _product;
        }
    }
}
