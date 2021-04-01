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
        public async Task<ActionResult<DateItemResponse>> Create(DateItemInput _input){
            try{
                string bearerToken = Request.Headers["Authorization"].ToString().Split(" ").Last();
                int id = int.Parse(jwtService.getUserId(bearerToken));
                DateItem result = await datetimeService.createItem(_input,id); 
            return CreatedAtAction("Create",new DateItemResponse{isSuccess=true,payload= new DateItemResponseData{item=result}});
            }catch(UnauthorizedAccessException ex){
                return Unauthorized(new DateItemResponse{isSuccess=false,error=new ApiError{msg=ex.Message}});
            }catch(Exception ex){
                return BadRequest(new DateItemResponse{isSuccess=false,error=new ApiError{msg=ex.Message}});
            }
        }

        [HttpGet]
        [Route("items")]
        public async Task<ActionResult<GetItemsResponse>> GetItems(){
            try{
                string bearerToken = Request.Headers["Authorization"].ToString().Split(" ").Last();
                int userId = int.Parse(jwtService.getUserId(bearerToken));
                IList<DateItem>  result = await datetimeService.getItems(userId);
                DateItem coverItem = await datetimeService.getCoverItem(userId);
                return Ok(new GetItemsResponse{isSuccess=true, payload= new GetItemsResponseData{coverItem=coverItem, items=result}});
            }catch(UnauthorizedAccessException ex){
                return Unauthorized(ex.Message);
            }catch(Exception ex){
                return BadRequest(ex.Message);
            }
            
        }

        [HttpDelete]
        [Route("delete/{id?}")]
        public async Task<ActionResult<DateItemResponse>> Delete(int id){
            try{
                string bearerToken = Request.Headers["Authorization"].ToString().Split(" ").Last();
                int userId = int.Parse(jwtService.getUserId(bearerToken));
                DateItem result = await datetimeService.deleteItem(userId,id);
                return Ok(new DateItemResponse{isSuccess=true,payload= new DateItemResponseData{item=result}});
            }catch(UnauthorizedAccessException ex){
                return Unauthorized(new DateItemResponse{isSuccess=false,error=new ApiError{msg=ex.Message}});
            }catch(Exception ex){
                return BadRequest(new DateItemResponse{isSuccess=false,error=new ApiError{msg=ex.Message}});
            }
            
        }

        [HttpPost]
        [Route("coveritem")]
        public async Task<ActionResult<DateItemResponse>> SetCoverItem(SetCoverItemInput _input){
            try{
                string bearerToken = Request.Headers["Authorization"].ToString().Split(" ").Last();
                int userId = int.Parse(jwtService.getUserId(bearerToken));
                DateItem result = await datetimeService.setCoverItem(userId,_input.id);
                return Ok(new DateItemResponse{isSuccess=true,payload= new DateItemResponseData{item=result}});
            }catch(UnauthorizedAccessException ex){
                return Unauthorized(new DateItemResponse{isSuccess=false,error=new ApiError{msg=ex.Message}});
            }catch(Exception ex){
                return BadRequest(new DateItemResponse{isSuccess=false,error=new ApiError{msg=ex.Message}});
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
