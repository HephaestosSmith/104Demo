using Demo.Interface;
using Demo.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService service, ILogger<UserController> logger)
        {
            _service = service;
            _logger = logger;
        }


        // POST: api/user
        // 註冊使用者：允許匿名
        [HttpPost]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "註冊使用者", Description = "註冊使用者")]
        public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _service.CreateAsync(dto, cancellationToken);
            if (created == null) return Conflict(new { message = "Account already exists." });

            return CreatedAtRoute("GetUser", new { id = created.Id }, created);
        }

        // POST: api/user/login
        // 使用者登入，允許匿名
        [HttpPost("login")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "使用者登入", Description = "取得JWT Token")]
        public async Task<ActionResult<TokenResponseDto>> Login([FromBody] LoginRequestDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _service.AuthenticateAsync(dto, cancellationToken);
            if (result == null)
                return Unauthorized(new { message = "Invalid username or password." });

            return Ok(result);
        }

        // GET: api/user
        [HttpGet]
        [SwaggerOperation(Summary = "取得所有使用者資料清單", Description = "取得所有使用者資料清單")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll(CancellationToken cancellationToken)
        {
            var users = await _service.GetAllAsync(cancellationToken);
            return Ok(users);
        }

        // GET: api/user/5
        [HttpGet("{id}", Name = "GetUser")]
        [SwaggerOperation(Summary = "取得使用者資料", Description = "取得使用者資料")]
        public async Task<ActionResult<UserDto>> GetById(int id, CancellationToken cancellationToken)
        {
            var user = await _service.GetByIdAsync(id, cancellationToken);
            if (user == null) return NotFound();
            return Ok(user);
        }

        // PUT: api/user/5
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "變更使用者資料", Description = "變更使用者資料")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var ok = await _service.UpdateAsync(id, dto, cancellationToken);
            if (!ok) return NotFound();

            return NoContent();
        }

        // DELETE: api/user/5
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "刪除使用者資料", Description = "刪除使用者資料")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var ok = await _service.DeleteAsync(id, cancellationToken);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}