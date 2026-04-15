using Microsoft.AspNetCore.Mvc;
using Proyecto.Application.Services;
using Proyecto.Domain.Entities;
using Proyecto.DTOs;

namespace Proyecto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        readonly AuthService _authService;
        public AccountController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserDto registerUserDto)
        {
            Usuario newUser = new Usuario()
            {
                FirstName = registerUserDto.FirstName,
                LastName = registerUserDto.LastName,
                Tel = registerUserDto.Phone,
                Email = registerUserDto.Email,
                Password = registerUserDto.Password
            };
            var result = await _authService.RegisterUser(newUser);

            if (result != null)
            {
                return Ok("Registro exitoso");
            }
            return BadRequest("Error al registrar el usuario");
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var result = await _authService.Login(loginDto.Email, loginDto.Password, loginDto.RememberMe);
            return Ok(new
            {
                Token = result
            });
        }
    }
}
