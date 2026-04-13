using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using EcomApi.Data;
using EcomApi.DTOs;
using EcomApi.Models;

namespace EcomApi.Services;

public class AuthService
{
    private readonly ApplicationDbContext _context;
    private readonly JwtService _jwtService;

    public AuthService(ApplicationDbContext context, JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<AuthResponseDto?> Login(LoginDto loginDto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

        if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash))
            return null;

        var token = _jwtService.GenerateToken(user);
        return new AuthResponseDto
        {
            Token = token,
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Address = user.Address,
            PhoneNumber = user.PhoneNumber,
            IsAdmin = user.IsAdmin,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<(AuthResponseDto? Response, string? Error)> Register(RegisterDto registerDto)
    {
        // Validation des champs requis
        if (string.IsNullOrWhiteSpace(registerDto.Username))
            return (null, "Le nom d'utilisateur est requis");
            
        if (string.IsNullOrWhiteSpace(registerDto.Email))
            return (null, "L'email est requis");
            
        if (string.IsNullOrWhiteSpace(registerDto.Password))
            return (null, "Le mot de passe est requis");
            
        if (string.IsNullOrWhiteSpace(registerDto.FirstName))
            return (null, "Le prénom est requis");
            
        if (string.IsNullOrWhiteSpace(registerDto.LastName))
            return (null, "Le nom est requis");
            
        if (string.IsNullOrWhiteSpace(registerDto.Address))
            return (null, "L'adresse est requise");
            
        if (string.IsNullOrWhiteSpace(registerDto.PhoneNumber))
            return (null, "Le numéro de téléphone est requis");

        // Vérification du format de l'email
        if (!System.Text.RegularExpressions.Regex.IsMatch(registerDto.Email, 
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            return (null, "Format d'email invalide");

        // Vérification du format du numéro de téléphone (exemple pour format français)
        if (!System.Text.RegularExpressions.Regex.IsMatch(registerDto.PhoneNumber, 
            @"^(\+33|0)[1-9](\d{8}|\s\d{2}\s\d{2}\s\d{2}\s\d{2})$"))
            return (null, "Format de numéro de téléphone invalide");

        // Vérification si l'email existe déjà
        if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
            return (null, "Cet email est déjà utilisé");

        // Vérification si le nom d'utilisateur existe déjà
        if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username))
            return (null, "Ce nom d'utilisateur est déjà utilisé");

        // Vérifier si c'est un email admin
        var isAdmin = IsAdminEmail(registerDto.Email);

        var user = new User
        {
            Username = registerDto.Username,
            Email = registerDto.Email,
            PasswordHash = HashPassword(registerDto.Password),
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            Address = registerDto.Address,
            PhoneNumber = registerDto.PhoneNumber,
            IsAdmin = isAdmin,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = _jwtService.GenerateToken(user);
        return (new AuthResponseDto
        {
            Token = token,
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Address = user.Address,
            PhoneNumber = user.PhoneNumber,
            IsAdmin = user.IsAdmin,
            CreatedAt = user.CreatedAt
        }, null);
    }

    private bool IsAdminEmail(string email)
    {
        return email.EndsWith("@admin.com", StringComparison.OrdinalIgnoreCase);
    }

    private string HashPassword(string password)
    {
        byte[] salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
        byte[] hash = pbkdf2.GetBytes(32);

        byte[] hashBytes = new byte[48];
        Array.Copy(salt, 0, hashBytes, 0, 16);
        Array.Copy(hash, 0, hashBytes, 16, 32);

        return Convert.ToBase64String(hashBytes);
    }

    private bool VerifyPassword(string password, string hashedPassword)
    {
        try
        {
            byte[] hashBytes = Convert.FromBase64String(hashedPassword);
            
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(32);
            
            for (int i = 0; i < 32; i++)
            {
                if (hashBytes[i + 16] != hash[i])
                    return false;
            }
            
            return true;
        }
        catch
        {
            return false;
        }
    }
}