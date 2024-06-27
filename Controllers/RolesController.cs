using API.Dtos;
using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly UserManager<AppUser> _userManager;

        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {

            _roleManager = roleManager;
            _userManager = userManager;
            
        }

        [HttpPost]

        public async Task <IActionResult> CreateRole([FromBody] CreateRoleDto createRoleDto)
        {
            if (string.IsNullOrEmpty(createRoleDto.RoleName)){
                return BadRequest("Role name is required");
            }

            var roleExist = await _roleManager.RoleExistsAsync(createRoleDto.RoleName);

            if(roleExist){
                return BadRequest("Role already exist");
            }

            var roleResult = await _roleManager.CreateAsync(new IdentityRole(createRoleDto.RoleName));

            if(roleResult.Succeeded){
                return Ok( new {message = "Role Created Successfully"});
            }

            return BadRequest("Role creation Failed");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleResponseDto>>> GetRoles()
        {
            

            // list of roles with total users in each role 

            var roles = await _roleManager.Roles.Select(r=>new RoleResponseDto{
                Id = r.Id,
                Name = r.Name,
                TotalUsers = _userManager.GetUsersInRoleAsync(r.Name!).Result.Count
            }).ToListAsync();

            return Ok(roles);
        }

        [HttpDelete("{id}")]

        public async Task <IActionResult> DeleteRole (string id)
        {
            var role =  await _roleManager.FindByIdAsync(id);
            if(role is null)
            {
                return NotFound("Role not found.");
            }

            var result = await _roleManager.DeleteAsync(role);

            if(result.Succeeded)
            {
                return Ok(new {message = "Role deleted successfully. "});
            }

            return BadRequest("Role deletion failed");
        }


    }
}