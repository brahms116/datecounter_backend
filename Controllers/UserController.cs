using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using datecounter.Models;
using datecounter.Services;
namespace datecounter.Controllers{

    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase{
        private readonly IUserService userService;
        private readonly IJwtService jwtService;

        public UserController(IUserService _userService, IJwtService _jwtService)
        {
            userService = _userService;
            jwtService = _jwtService;
        }

        [HttpGet]
        [Route("demo")]
        public ActionResult<string> demoEndpoint(){
            return Ok("Demo has worked");
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<UserLoginResponse>> Register(UserRegisterInput _input){
            try{
                int result = await userService.registerUser(_input);
                string token = await userService.login(new UserLoginInput(){email=_input.email,password=_input.password});
                UserLoginResponse response = new UserLoginResponse();
                response.token = token;
                return Ok(response);
            }catch(UnauthorizedAccessException ex){
                return Unauthorized(ex.Message);
            }catch(Exception ex){
                return BadRequest(ex.Message);
            }            
            
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<UserLoginResponse>> Login(UserLoginInput _input){
            try{
                string token = await userService.login(_input);
                var res = new UserLoginResponse();
                res.token = token;
                return Ok(res);
            }catch(UnauthorizedAccessException ex){
                return Unauthorized(ex.Message);
            }catch(Exception ex){
                return BadRequest(ex.Message);
            }
            
        }


        // [HttpGet]
        // [Route("userinfo")]
        // [Authorize]
        // public async Task<ActionResult<User>> GetInfo(){
        //     string bearerToken = Request.Headers["Authorization"].ToString().Split(" ").Last();
        //     string id = jwtService.getUserId(bearerToken);
        //     return Ok();
        // }
    }




}