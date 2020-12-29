using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using datecounter.Models;
namespace datecounter.Controllers{

    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase{
        

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<UserRegisterRepsonse>> Register(UserRegisterInput _input){

            UserRegisterRepsonse response = new UserRegisterRepsonse();
            response.token = "adfjkjkajf";
            return response;
        }
        

    }
}