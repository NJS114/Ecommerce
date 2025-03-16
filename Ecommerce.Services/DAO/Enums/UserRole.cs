namespace Ecommerce.Services.DAO.Enums
{
    public enum UserRole
    {
        Buyer,      
        Seller,     
        Both         
    }
    public enum UserStatus
    {
        Logged,  // L'utilisateur est connecté
        Pending, // L'utilisateur attend la validation
        Suspended, // L'utilisateur est suspendu
        Active, // L'utilisateur est actif
        Inactive // L'utilisateur est inactif
    }
}
