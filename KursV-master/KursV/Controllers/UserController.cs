using Microsoft.AspNetCore.Mvc;
using KursV.Models;

namespace KursV.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        // Простое хранилище пользователей
        private static readonly List<UserInfo> _users = new List<UserInfo>();

        /// <summary>
        /// POST: api/User/info
        /// Сохранить пользователя
        /// </summary>
        [HttpPost("info")]
        public IActionResult SaveUserInfo([FromBody] UserInfo userInfo)
        {
            try
            {
                if (string.IsNullOrEmpty(userInfo.Name))
                    return BadRequest("Имя обязательно");

                if (string.IsNullOrEmpty(userInfo.Email))
                    return BadRequest("Email обязателен");

                // Просто добавляем пользователя в список
                _users.Add(userInfo);

                var response = new
                {
                    message = "Пользователь успешно сохранен",
                    userName = userInfo.Name,
                    userEmail = userInfo.Email,
                    totalUsers = _users.Count,
                    timestamp = DateTime.Now
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        /// <summary>
        /// GET: api/User/info
        /// Получить всех пользователей
        /// </summary>
        [HttpGet("info")]
        public IActionResult GetAllUsers()
        {
            try
            {
                return Ok(_users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

    }
}