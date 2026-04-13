using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using EcomApi.Services;
using EcomApi.DTOs;
using EcomApi.Data;

namespace EcomApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly ApplicationDbContext _context;

    public AuthController(AuthService authService, ApplicationDbContext context)
    {
        _authService = authService;
        _context = context;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto)
    {
        try 
        {
            var (response, error) = await _authService.Register(registerDto);
            if (response == null)
            {
                return BadRequest(new { message = error ?? "Une erreur est survenue lors de l'inscription" });
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Une erreur interne est survenue lors de l'inscription" });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var response = await _authService.Login(loginDto);
            if (response == null)
            {
                return Unauthorized(new { message = "Email ou mot de passe incorrect" });
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Une erreur interne est survenue lors de la connexion" });
        }
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<ActionResult<AuthResponseDto>> GetProfile()
    {
        try
        {
            var userEmail = User.Identity?.Name;
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null)
                return NotFound();

            return Ok(new AuthResponseDto
            {
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                IsAdmin = user.IsAdmin,
                CreatedAt = user.CreatedAt
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Une erreur est survenue lors de la récupération du profil" });
        }
    }

    [Authorize]
    [HttpPut("profile")]
    public async Task<ActionResult<AuthResponseDto>> UpdateProfile([FromBody] RegisterDto updateDto)
    {
        try
        {
            var userEmail = User.Identity?.Name;
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null)
                return NotFound();

            // Mise à jour des informations de l'utilisateur
            user.FirstName = updateDto.FirstName ?? user.FirstName;
            user.LastName = updateDto.LastName ?? user.LastName;
            user.Address = updateDto.Address ?? user.Address;
            user.PhoneNumber = updateDto.PhoneNumber ?? user.PhoneNumber;

            await _context.SaveChangesAsync();

            // Retourner les informations mises à jour
            return Ok(new AuthResponseDto
            {
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                IsAdmin = user.IsAdmin,
                CreatedAt = user.CreatedAt
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Une erreur est survenue lors de la mise à jour du profil" });
        }
    }
}