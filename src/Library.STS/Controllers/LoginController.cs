using Library.STS.Domain.DTO;
using Library.STS.Repository;
using Library.STS.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.STS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        // POST api/login/authenticate
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public ActionResult<TokenInfoDto> Authenticate([FromBody] UserDto userDto)
        {
            if (userDto == null || string.IsNullOrEmpty(userDto.Username) || string.IsNullOrEmpty(userDto.Password))
                return BadRequest("The username and password cannot be null or empty"); 
            
            // Recupera o usuário
            var user = UserRepository.Get(userDto.Username, userDto.Password);

            // Verifica se o usuário existe
            if (user == null)
                return Unauthorized(new { message = "The user or password are invalid." });

            // Gerar o Token
            var tokenInfo = TokenService.GenerateToken(user);

            return Ok(tokenInfo);
        }

        // POST api/login/validate
        [AllowAnonymous]
        [HttpPost("validate")]
        public ActionResult<TokenValidateDto> Validate([FromBody] TokenValueDto obj)
        {
            if (string.IsNullOrEmpty(obj.Token))
                return BadRequest();

            // Validar o Token
            TokenValidateDto tokenValidate = TokenService.ValidateToken(obj.Token);

            if (!tokenValidate.IsValidToken)
                return Unauthorized(tokenValidate);

            return Ok(tokenValidate);        
        }
    }
    
}
