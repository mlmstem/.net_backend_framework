using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Dtos;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.ObjectPool;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    //api/account
    public class AccountController : ControllerBase
    {
      private readonly UserManager<AppUser> _userManager;
      private readonly RoleManager <IdentityRole> _roleManager;


      private readonly IConfiguration _configuration;

      public AccountController(UserManager<AppUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;

            
        }


        //api/account/register
      [AllowAnonymous]
      [HttpPost("register")]

      public async Task<ActionResult <string>> Register (RegisterDto registerDto)
      {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);

        }

        var user = new AppUser{
            Email = registerDto.Email,
            FullName = registerDto.FullName,
            UserName = registerDto.Email,
            PhoneNumber = registerDto.PhoneNumber,
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);
        if (! result.Succeeded)
        {
            return BadRequest(result.Errors);

        }
        if(registerDto.Roles is null){
                    await _userManager.AddToRoleAsync(user,"User");
            }else{
                foreach(var role in registerDto.Roles)
                {
                    await _userManager.AddToRoleAsync(user,role);
                }
            }

    return Ok(new AuthResponseDto{
        IsSuccess = true,
        Message = "Account Created Successfully!"
    });

      }
      


      [AllowAnonymous]
      [HttpPost("login")]

      public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto){
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);

        }
        var user = await _userManager.FindByEmailAsync(loginDto.Email);

        if (user is null){
            return Unauthorized(new AuthResponseDto{
                IsSuccess = false,
                Message = "User not found with this email", 

            });
        }

        var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);

        if(!result){
            return Unauthorized(new AuthResponseDto{
                IsSuccess = false,
                Message = "Invalid Password"
            });
        }

        var token = await GenerateToken(user);

        return Ok(new AuthResponseDto{
            Token = token,
            IsSuccess = true,
            Message = "Login Success."
        });
      }

      private async Task<string> GenerateToken(AppUser user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration.GetSection("JWTSetting").GetSection("securityKey").Value!);

        var roles = await _userManager.GetRolesAsync(user); // Await async call

        List<Claim> claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new Claim(JwtRegisteredClaimNames.Name, user.FullName ?? ""),
            new Claim(JwtRegisteredClaimNames.NameId, user.Id ?? ""),
            new Claim(JwtRegisteredClaimNames.Aud, _configuration.GetSection("JWTSetting").GetSection("validAudience").Value!),
            new Claim(JwtRegisteredClaimNames.Iss, _configuration.GetSection("JWTSetting").GetSection("validIssuer").Value!)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(1),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256
            )
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }





      //api/account/detail
      
      [Authorize]
      [HttpGet("detail")]

      public async Task<ActionResult<UserDetailDto>> GetUserDetail(){

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(currentUserId!);
        if (user is null){
            return NotFound(new AuthResponseDto{
                IsSuccess = false,
                Message = "User not found",
            });

        }
        return Ok(new UserDetailDto{
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
             Roles = [..await _userManager.GetRolesAsync(user)],
            PhoneNumber = user.PhoneNumber,
            PhoneNumberConfirmed = user.PhoneNumberConfirmed,
            AccessFailedCount = user.AccessFailedCount,

        });

      }


        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDetailDto>>> GetUsers()
            {
            var users = await _userManager.Users.ToListAsync();
            var userDetails = new List<UserDetailDto>();

            foreach (var user in users)
                {
                var roles = await _userManager.GetRolesAsync(user);
                userDetails.Add(new UserDetailDto
                {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Roles = roles.ToArray()
                });
                    }

                return Ok(userDetails);
                }



    [Authorize]
    [HttpPost("update")]
    public async Task<ActionResult> UpdateUser([FromBody] UpdateUserDetailDto updateUserDto)
        {
        var user = await _userManager.FindByIdAsync(updateUserDto.UserId);
            
        if (user == null)
        {
            return NotFound(new AuthResponseDto
            {
                IsSuccess = false,
                Message = "User not found"
            });
            }

        
        Console.WriteLine($"Updating User: {updateUserDto.UserId}, Email: {updateUserDto.Email}, FullName: {updateUserDto.FullName}, PhoneNumber: {updateUserDto.PhoneNumber}");

            // Update fields if they are provided
        if (!string.IsNullOrEmpty(updateUserDto.Email))
        {
            user.Email = updateUserDto.Email;
            user.UserName = updateUserDto.Email;
        }

        if (!string.IsNullOrEmpty(updateUserDto.FullName))
            {
            user.FullName = updateUserDto.FullName;
            }

        if (!string.IsNullOrEmpty(updateUserDto.PhoneNumber))
            {
            user.PhoneNumber = updateUserDto.PhoneNumber;
            }

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            {
            return BadRequest(updateResult.Errors);
            }

        if (updateUserDto.Roles != null && updateUserDto.Roles.Any())
            {
            var validRoles = updateUserDto.Roles.Where(role => !string.IsNullOrEmpty(role)).ToList();
            var currentRoles = await _userManager.GetRolesAsync(user);
            var rolesToRemove = currentRoles.Except(validRoles).ToList();
            var rolesToAdd = validRoles.Except(currentRoles).ToList();

                if (rolesToRemove.Any())
                {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                if (!removeResult.Succeeded)
                {
                    return BadRequest(removeResult.Errors);
                    }
                }

                if (rolesToAdd.Any())
                {
                var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
                if (!addResult.Succeeded)
                    {
                    return BadRequest(addResult.Errors);
                    }
                }
            }

            return Ok(new AuthResponseDto
            {
                IsSuccess = true,
                Message = "User details updated successfully"
            });
        }

    }
}