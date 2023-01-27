using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Poke.Api.Database;
using Poke.Api.ViewModels;

namespace Poke.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly PokemonDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public AuthenticationController(
        UserManager<IdentityUser> userManager, 
        PokemonDbContext dbContext, 
        IConfiguration configuration)
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _configuration = configuration;
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterVM registerVM)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Fill required fields");
        }

        var userExists = await _userManager.FindByEmailAsync(registerVM.EmailAddress);
        if (userExists != null)
        {
            return BadRequest("An account with that email address already exists");
        }

        IdentityUser newUser = new IdentityUser()
        {
            Email = registerVM.EmailAddress,
            UserName = registerVM.Username,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        var result = await _userManager.CreateAsync(newUser, registerVM.Password);
        if (result.Succeeded)
        {
            return Ok("User created");
        }

        return BadRequest("An unexpected error has occured");
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginVM loginVm)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Pleas provide all required parameters");
        }

        var userExists = await _userManager.FindByEmailAsync(loginVm.EmailAddress);
        if (userExists != null && await _userManager.CheckPasswordAsync(userExists, loginVm.Password))
        { 
            //Generate auth token
            var tokenValue = await GenerateJWTTokenAsync(userExists);
            return Ok(tokenValue);
        }

        return Unauthorized();
    }

    private async Task<AuthResultVM> GenerateJWTTokenAsync(IdentityUser userExists)
    {
        //Claims encrypted into token
        var authClaims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name, userExists.UserName),
            new Claim(ClaimTypes.NameIdentifier, userExists.Id),
            new Claim(JwtRegisteredClaimNames.Email, userExists.Email),
            new Claim(JwtRegisteredClaimNames.Sub, userExists.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var authSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]));
        var token = new JwtSecurityToken(
            issuer:_configuration["JWT:Issuer"],
            audience:_configuration["JWT:Audience"],
            expires: DateTime.UtcNow.AddMinutes(5),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

        var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

        var response = new AuthResultVM()
        {
            Token = jwtToken,
            ExpiresAt = token.ValidTo
        };

        return response;
    }
}