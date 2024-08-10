
using Bunnisses.Security.Implements;
using Bunnisses.Security.Interface;
using Data.DTO;
using Entity.Model.Security;
using Microsoft.AspNetCore.Mvc;
using WebC.Controllers.Interfaces;

namespace WebC.Controllers.Implements
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase, IUserController
    {
        private readonly IUserBusiness _UserBusiness;

        public UserController(IUserBusiness UserBusiness)
        {
            _UserBusiness = UserBusiness;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        {
            var result = await _UserBusiness.GetAll();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetById(int id)
        {
            var result = await _UserBusiness.GetById(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet("select")]
        public async Task<ActionResult<IEnumerable<DataSelectDto>>> GetAllSelect()
        {
            var result = await _UserBusiness.GetAllSelect();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> Save([FromBody] UserDto entity)
        {
            if (entity == null)
            {
                return BadRequest("Entity is null");
            }

            // Validar solo lo necesario
            if (string.IsNullOrWhiteSpace(entity.Nombre_usuario) || string.IsNullOrWhiteSpace(entity.Contraseña))
            {
                return BadRequest("El nombre de usuario y la contraseña son obligatorios.");
            }

            var result = await _UserBusiness.Save(entity);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserDto entity)
        {
            if (entity == null || id != entity.Id)
            {
                return BadRequest();
            }
            await _UserBusiness.Update(id, entity);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _UserBusiness.Delete(id);
            return NoContent();
        }

        [HttpGet("Nombre/{username}")]
        public async Task<ActionResult<UserDto>> GetByUsername(User user, int Id)
        {
            var result = await _UserBusiness.GetByUsername(user, Id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet("Contraseña/{password}")]
        public async Task<ActionResult<UserDto>> GetByPassword(User user, int Id)
        {
            var result = await _UserBusiness.GetByPassword(user, Id);
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserDto loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _UserBusiness.Login(loginRequest.Nombre_usuario, loginRequest.Contraseña);


                var views = await _UserBusiness.GetViewsForUser(user.Id);

                return Ok(new { User = user, Views = views });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("views/{userId}")]
        public async Task<ActionResult<IEnumerable<View>>> GetViewsForUser(int userId)
        {
            try
            {
                var views = await _UserBusiness.GetViewsForUser(userId);
                return Ok(views);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
