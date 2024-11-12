namespace  Ecommerce.Services.DAO.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }    
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public float  Price { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
