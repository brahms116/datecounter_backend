using System;
using Microsoft.AspNetCore.Mvc;

namespace datecounter.Controllers{

    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase{
        
        [Route("login")]
        public ActionResult<string> Login(){
            return Ok("hello");
        }
        

    }
}