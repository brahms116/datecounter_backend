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
        public async Task<ActionResult<UserRegisterResponse>> Register(UserRegisterInput _input){
            try{
                User insertedUser = await userService.registerUser(_input);
                string token = await userService.login(new UserLoginInput{email=_input.email,password=_input.password});
                UserRegisterResponse response = new UserRegisterResponse{isSuccess = true, payload = new UserRegisterResponseData{token=token,user=insertedUser}};
                return Ok(response);
            }catch(UnauthorizedAccessException ex){
                return Unauthorized(new UserRegisterResponse{error = new ApiError{msg=ex.Message}});
            }catch(Exception ex){
                return BadRequest(new UserRegisterResponse{error = new ApiError{msg=ex.Message}});
            }            
            
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<UserLoginResponse>> Login(UserLoginInput _input){
            try{
                string token = await userService.login(_input);
                return Ok(new UserLoginResponse{isSuccess=true,payload=new UserLoginResponseData{token=token}});
            }catch(UnauthorizedAccessException ex){               
                return Unauthorized(new UserLoginResponse{error = new ApiError{msg=ex.Message}});
            }catch(Exception ex){
                return BadRequest(new UserLoginResponse{error = new ApiError{msg=ex.Message}});
            }
            
        }


        [HttpGet]
        [Route("checktoken")]
        [Authorize]
        public IActionResult GetInfo(){
            string bearerToken = Request.Headers["Authorization"].ToString().Split(" ").Last();
            string id = jwtService.getUserId(bearerToken);
            return Ok();
        }
    }




}