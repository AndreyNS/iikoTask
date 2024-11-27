using iikoTask.Interfaces;
using iikoTask.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iikoTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _service;

        public UserController(ILogger<UserController> logger, IUserService service)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("{internalId?}")]
        public async Task<IActionResult> GetUser(long? internalId)
        {
            string info = string.Empty;
            if (internalId == null) 
            {
                info = "ID null";
                _logger.LogWarning($"[{nameof(GetUser)}] {info}");
                return BadRequest(info);
            }

            long id = internalId.Value;

            try
            {
                var user = await _service.Get(id);
                if (user == null)
                {
                    info = $"Пользователь c ID '{id}' не найден";
                    _logger.LogWarning($"[{nameof(GetUser)}] {info}");
                    return NotFound(info);
                }

                info = $"Пользователь c ID '{id}' найден";
                _logger.LogInformation($"[{nameof(GetUser)}] {info}");
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Внутренняя ошибка");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }      
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] User user)
        {
            string info = string.Empty;
            if (user == null)
            {
                info = "Тело пусто или некорректно";
                _logger.LogWarning($"[{nameof(UpdateUser)}] {info}");
                return BadRequest(info);
            }

            try
            {
                if (!(await _service.Check(user.ClientId)))
                {
                    info = $"Пользователь c ID '{user.ClientId}' не найден";
                    _logger.LogWarning($"[{nameof(UpdateUser)}] {info}");
                    return NotFound(info);
                }
                await _service.Update(user);

                info = $"Пользователь c ID '{user.ClientId}' успешно обновлен";
                _logger.LogInformation($"[{nameof(UpdateUser)}] {info}");
                return Ok(info);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[{nameof(UpdateUser)}] Внутренняя ошибка");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("{internalId?}")]
        public async Task<IActionResult> DeleteUser(long? internalId)
        {
            string info = string.Empty;
            if (internalId == null)
            {
                info = "ID null";

                _logger.LogWarning($"[{nameof(DeleteUser)}] {info}");
                return BadRequest(info);
            }

            long id = internalId.Value;
            try
            {
                if (!(await _service.Check(id)))
                {
                    info = $"Пользователь c ID '{id}' не найден";
                    _logger.LogWarning($"[{nameof(DeleteUser)}] {info}");
                    return NotFound(info);
                }
                await _service.Remove(id);

                info = $"Пользователь c ID '{id}' успешно удален";
                _logger.LogInformation($"[{nameof(DeleteUser)}] {info}");
                return Ok(info);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[{nameof(DeleteUser)}] Внутренняя ошибка");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [HttpPost]
        public async Task<IActionResult> SetBatch([FromBody] IEnumerable<User> userList)
        {
            string info = string.Empty;
            info = userList switch
            {
                var list when list == null => "Тело пустое либо неверный формат",
                var list when list.Count() == 0 => "В массиве отсутствуют элементы",
                var list when list.Count() < 10 => "Элементов в массиве менее 10",
                _ => string.Empty
            };

            if (!string.IsNullOrEmpty(info))
            {
                _logger.LogWarning($"[{nameof(SetBatch)}] {info}");
                return BadRequest(info);
            }

            try
            {
                var nonAddUsers = await _service.SetUsers(userList);
                _logger.LogInformation($"[{nameof(DeleteUser)}] Не удалось добавить пользователей: {nonAddUsers.Count()}");
                return Ok(new { title = nonAddUsers.Count(),  list = nonAddUsers });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[{nameof(SetBatch)}] Внутренняя ошибка");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
