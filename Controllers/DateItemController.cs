using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using datecounter.Models;
using datecounter.Services;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace datecounter.Controllers{


    [ApiController]
    [Route("api/dateitem")]
    [Authorize]
    public class DateItemController:ControllerBase{
        private readonly IJwtService jwtService;
        private readonly IDateItemService datetimeService;

        public DateItemController(IJwtService jwtService,IDateItemService datetimeService)
        {
            this.jwtService = jwtService;
            this.datetimeService = datetimeService;
        }

        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<int>> Create(DateItemInput _input){
            try{
                string bearerToken = Request.Headers["Authorization"].ToString().Split(" ").Last();
                int id = int.Parse(jwtService.getUserId(bearerToken));
                int result = await datetimeService.createItem(_input,id); 
            return CreatedAtAction("Create",result);
            }catch(UnauthorizedAccessException ex){
                return Unauthorized(ex.Message);
            }catch(Exception ex){
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("items")]
        public async Task<ActionResult<GetItemResponse>> GetItems(){
            try{
                string bearerToken = Request.Headers["Authorization"].ToString().Split(" ").Last();
                int userId = int.Parse(jwtService.getUserId(bearerToken));
                IList<DateItem>  result = await datetimeService.getItems(userId);
                DateItem coverItem = await datetimeService.getCoverItem(userId);
                return Ok(new GetItemResponse{coverItem=coverItem,items=result});
            }catch(UnauthorizedAccessException ex){
                return Unauthorized(ex.Message);
            }catch(Exception ex){
                return BadRequest(ex.Message);
            }
            
        }

        [HttpDelete]
        [Route("delete/{id?}")]
        public async Task<ActionResult<int>> Delete(int id){
            try{
                string bearerToken = Request.Headers["Authorization"].ToString().Split(" ").Last();
                int userId = int.Parse(jwtService.getUserId(bearerToken));
                int result = await datetimeService.deleteItem(userId,id);
                return Ok(result);
            }catch(UnauthorizedAccessException ex){
                return Unauthorized(ex.Message);
            }catch(Exception ex){
                return BadRequest(ex.Message);
            }
            
        }

        [HttpPost]
        [Route("coveritem")]
        public async Task<ActionResult<int>> SetCoverItem([FromBody]int id){
            try{
                string bearerToken = Request.Headers["Authorization"].ToString().Split(" ").Last();
                int userId = int.Parse(jwtService.getUserId(bearerToken));
                int result = await datetimeService.setCoverItem(userId,id);
                return result;
            }catch(UnauthorizedAccessException ex){
                return Unauthorized(ex.Message);
            }catch(Exception ex){
                return BadRequest(ex.Message);
            }
            
        }

        // [HttpGet]
        // [Route("coveritem")]
        // public async Task<ActionResult<DateItem>> GetCoverItem(){
        //     string bearerToken = Request.Headers["Authorization"].ToString().Split(" ").Last();
        //     int userId = int.Parse(jwtService.getUserId(bearerToken));
        //     DateItem result = await datetimeService.getCoverItem(userId);
        //     return result;
        // }
    }
}
