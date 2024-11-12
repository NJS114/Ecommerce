using  Ecommerce.Services.DAO.Models;

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

        public ProductBuilder SetId(int id)
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

        public ProductBuilder SetPrice(float price)
        {
            _product.Price = price;
            return this;
        }

        public ProductBuilder SetCategoryId(int categoryId)
        {
            _product.CategoryId = categoryId;
            return this;
        }

      
        public Product Build()
        {
            return _product;
        }
    }
}