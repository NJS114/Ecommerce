using Ecommerce.Services.DAO.Models;

namespace Ecommerce.Services.DAO.DTOs
{
    public class AppointmentDTO
    {
        public string Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public bool IsConfirmed { get; set; }
        public int CustomerId { get; set; }
        public UserSummary User { get; set; }
    }

    public class UserSummary
    {
        public string UserId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
    }
}
