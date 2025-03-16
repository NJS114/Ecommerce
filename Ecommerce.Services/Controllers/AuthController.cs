using Ecommerce.Services.DAO.Connexion;
using Ecommerce.Services.DAO.DTOs;
using Ecommerce.Services.DAO.Interfaces.IRepository;
using Ecommerce.Services.DAO.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Ecommerce.Services.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IJwtService _jwtService;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;


        public AuthController(IConfiguration configuration, IJwtService jwtService, IUserRepository userRepository, IEmailService emailService)
        {
            _configuration = configuration;
            _jwtService = jwtService;
            _userRepository = userRepository;
            _emailService = emailService;

        }
        #region Register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            if (registerDTO == null)
            {
                return BadRequest("Données manquantes.");
            }

            if (string.IsNullOrEmpty(registerDTO.Email) || string.IsNullOrEmpty(registerDTO.Password))
            {
                return BadRequest("L'email et le mot de passe sont requis.");
            }
            if (!IsValidPassword(registerDTO.Password))
            {
                return BadRequest("Le mot de passe doit contenir au moins 8 caractères, une lettre majuscule, une lettre minuscule, un chiffre et un caractère spécial.");
            }

            // Enregistrement de l'utilisateur avec les données envoyées
            await _userRepository.RegisterFromDTOAsync(registerDTO);

            var token = _jwtService.GenerateToken(registerDTO.Id);
            var configuration = Environment.GetEnvironmentVariable("NEXT_CONNECTION_STRING");
            var confirmationUrl = $"{configuration}/confirmation/{token}";
            var confirmationEmailBody = $"Bienvenue sur notre plateforme. Cliquez sur ce lien pour confirmer votre inscription : {confirmationUrl}";
            await _emailService.SendEmailAsync(registerDTO.Email, "Confirmation d'inscription", confirmationEmailBody);

            return Ok(new { Message = "Utilisateur créé avec succès.", Token = token });
        }
        #endregion

        #region Reset Password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("L'email est requis.");
            }

            var user = await _userRepository.GetUserWithDetailsAsync(email);
            if (user == null)
            {
                return NotFound("Aucun utilisateur trouvé avec cet email.");
            }

            var resetToken = _jwtService.GenerateToken(user.Id.ToString());

            var resetPasswordUrl = $"{_configuration["AppSettings:BaseUrl"]}/reset-password/{resetToken}";
            var resetPasswordBody = $"Cliquez sur ce lien pour réinitialiser votre mot de passe : {resetPasswordUrl}";
            await _emailService.SendEmailAsync(user.Email, "Réinitialisation de mot de passe", resetPasswordBody);

            return Ok(new { Message = "Un email de réinitialisation a été envoyé." });
        }
        #endregion

        #region Login
        public class LoginDTO
        {
            [Required(ErrorMessage = "L'email est obligatoire.")]
            [EmailAddress(ErrorMessage = "L'email n'est pas valide.")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Le mot de passe est obligatoire.")]
            [MinLength(6, ErrorMessage = "Le mot de passe doit contenir au moins 6 caractères.")]
            public string Password { get; set; } = string.Empty;
        }
        [AllowAnonymous]

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO logindto)
        {
            

            if (string.IsNullOrEmpty(logindto.Email) || string.IsNullOrEmpty(logindto.Password))
            {
                return BadRequest("L'email et le mot de passe sont obligatoires.");
            }

            var user = await _userRepository.GetUserWithDetailsAsync(logindto.Email);

            if (user == null || !VerifyPassword(logindto.Password, user.Password))
            {
                return Unauthorized(new { Message = "Identifiant ou mot de passe incorrect." });
            }

            var token = _jwtService.GenerateToken(user.Id.ToString());

            HttpContext.Response.Cookies.Append("AuthToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = _configuration.GetValue<bool>("AppSettings:UseHttps"),
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(30)
            });

            return Ok(new
            {
                Message = "Connexion réussie",
                Token = token,
                User = new
                {
                    user.Id,
                    user.Email,
                    user.FirstName
                }
            });
        }
        #endregion


        private bool VerifyPassword(string password, string passwordHash)
        {
            if (string.IsNullOrEmpty(passwordHash))
            {
                throw new ArgumentException("Le mot de passe haché ne peut pas être nul ou vide.", nameof(passwordHash));
            }
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
        private bool IsValidPassword(string password)
        {
            var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
            return regex.IsMatch(password);
        }

    }
}
