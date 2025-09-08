using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NETAPI.DTOs;
using NETAPI.Models;
using NETAPI.Repository;
using NETAPI.Services;
using System.Security.Claims;
using NETAPI.Strategies;
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthRepository _repository;
    private readonly JwtService _jwtService;
    private readonly AuthStrategy _authStrategyFactory;
    public AuthController(IAuthRepository repository, JwtService jwtService, AuthStrategy authStrategy)
    {
        _repository = repository;
        _jwtService = jwtService;
        _authStrategyFactory = authStrategy;
    }

    [HttpPost("signup")]
    public async Task<IActionResult> SignUp(SignUpDTO dto)
    {
        try
        {
            var existingUser = await _repository.GetUserByEmailAsync(dto.Email);
            if (existingUser != null)
                return BadRequest("Email is already registered.");

            if (string.IsNullOrWhiteSpace(dto.Email) || !CheckValidEmail(dto.Email))
                return BadRequest("Invalid email format.");

            if (string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Password must not be empty.");

            UserType desiredType = UserType.User;
            if (!string.IsNullOrWhiteSpace(dto.UserType)
                && Enum.TryParse<UserType>(dto.UserType, true, out var parsed))
            {
                desiredType = parsed;
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                Email = dto.Email,
                Hash = passwordHash,
                Type = desiredType,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _repository.AddUserAsync(user);
            await _repository.SaveChangesAsync();

            return Created("", new { id = user.Id, email = user.Email, userType = user.Type.ToString() });
        }
        catch (Exception)
        {
            return StatusCode(500, "An internal error occurred.");
        }
    }


    [HttpPost("signin")]
    public async Task<IActionResult> SignIn(SignInDTO dto)
    {
        var user = await _repository.GetUserByEmailAsync(dto.Email);
        if (user == null)
            return BadRequest("Invalid email or password.");

        var strategy = _authStrategyFactory.GetStrategy(user.Type);
        var result = await strategy.Authenticate(user, dto.Password, dto.RememberMe);

        return Ok(result);
    }

    [HttpPost("signout")]
    public async Task<IActionResult> SignOut(RefreshTokenDTO dto)
    {
        try
        {
            // Check refresh token's existence
            var refreshToken = await _repository.GetRefreshTokenAsync(dto.RefreshToken);

            await _repository.RemoveAllRefreshTokensAsync(refreshToken);
            await _repository.SaveChangesAsync();

            // Signing out successfully
            return NoContent(); 
        }
        catch (Exception)
        {
            return StatusCode(500, "An internal error occurred."); 
        }
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(RefreshTokenDTO dto)
    {
        try
        {
            // Check refresh token's existence 
            var oldToken = await _repository.GetRefreshTokenAsync(dto.RefreshToken);
            if (oldToken == null)
                return NotFound("Refresh token not found.");

            // Get user information
            var user = await _repository.GetUserByIdAsync(oldToken.UserId);
            if (user == null)
                return StatusCode(500, "User not found."); 

            // Remove previous refresh token
            await _repository.RemoveRefreshTokenAsync(oldToken);

            // Create new token
            var newRefreshToken = _jwtService.GenerateRefreshToken();
            var accessToken = _jwtService.GenerateAccessToken(user);

            var newToken = new Token
            {
                UserId = user.Id,
                RefreshToken = newRefreshToken,
                ExpiresIn = DateTime.UtcNow.AddDays(30).ToString("o"),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _repository.AddRefreshTokenAsync(newToken);
            await _repository.SaveChangesAsync();

            // Create new tokens successfully
            return Ok(new
            {
                token = accessToken,
                refreshToken = newRefreshToken
            }); 
        }
        catch (Exception)
        {
            return StatusCode(500, "An internal error occurred."); 
        }
    }
    private bool CheckValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}