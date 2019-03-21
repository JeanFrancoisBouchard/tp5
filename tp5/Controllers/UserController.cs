using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using tp5.Services;
using tp5.Models;

namespace tp5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("create")]
        public ActionResult create([FromBody]User user)
        {
            if(_userService.create(user))
            {
                return StatusCode(200);
            }
            else
            {
                return StatusCode(409);
            }
        }

        [HttpPost("login")]
        public ActionResult<string> login([FromBody]User user)
        {
            if(_userService.login(user))
            {
                string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                HttpContext.Session.SetString("token", token);

                return token;
            }
            else
            {
                return StatusCode(401);
            }
        }

        [HttpPost("logout")]
        public ActionResult logout()
        {
            HttpContext.Session.Clear();

            return StatusCode(200);
        }

        [HttpGet("secret")]
        public ActionResult<string> secret([FromBody]dynamic token)
        {
            if (checkToken(Convert.ToString(token["token"])))
            {
                return "check son pouce, y curve";
            }
            else
            {
                return StatusCode(403);
            }
        }

        private bool checkToken(string token)
        {
            return token != null && token == HttpContext.Session.GetString("token");
        }
    }
}