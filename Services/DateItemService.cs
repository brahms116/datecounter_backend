using System;
using System.Collections.Generic;
using Npgsql;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Dapper;
using System.Linq;
using datecounter.Models;



namespace datecounter.Services{
    public interface IDateItemService
    {
        Task<DateItem> createItem(DateItemInput _input, int userId);
        Task<DateItem> deleteItem(int userId, int itemId);
        Task<DateItem> getCoverItem(int userId);
        Task<IList<DateItem>> getItems(int userId);
        Task<DateItem> setCoverItem(int userId, int itemId);
    }

    public class DateItemService : IDateItemService
    {
        private readonly IConfiguration config;

        public DateItemService(IConfiguration _config)
        {
            config = _config;
        }


        public async Task<DateItem> createItem(DateItemInput _input, int userId)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(config.GetConnectionString("postgres")))
            {
                string sql = "INSERT INTO \"date_item\" (user_id,title,date) VALUES( @userId, @title, @date) RETURNING*";
                DateItem result = await connection.QueryFirstOrDefaultAsync<DateItem>(sql, new { userId = userId, title = _input.title, date = _input.date });
                if (result == null)
                {
                    throw new InvalidOperationException("Sql returned num of rows affected as 0");
                }
                return result;
            }

        }

        public async Task<IList<DateItem>> getItems(int userId)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(config.GetConnectionString("postgres")))
            {
                string sql = "SELECT * FROM \"date_item\" WHERE user_id = @user_id ORDER BY date DESC";
                IEnumerable<DateItem> result = await connection.QueryAsync<DateItem>(sql, new { user_id = userId });
                return result.ToList();
            }
        }


        public async Task<DateItem> deleteItem(int userId, int itemId)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(config.GetConnectionString("postgres")))
            {
                string sql = "SELECT user_id FROM \"date_item\" WHERE id = @id";
                IEnumerable<DateItem> result = await connection.QueryAsync<DateItem>(sql, new { id = itemId });
                List<DateItem> resultItems = result.ToList();
                if (!resultItems.Any())
                {
                    throw new InvalidOperationException("No such date item");
                }
                if (resultItems[0].user_id != userId)
                {
                    throw new UnauthorizedAccessException("Resource does not belong to you");
                }
                sql = "DELETE FROM \"date_item\" WHERE id = @id RETURNING *";
                DateItem deleteResult = await connection.QueryFirstOrDefaultAsync<DateItem>(sql, new { id = itemId });
                if (deleteResult == null)
                {
                    throw new InvalidOperationException("No rows were deleted");
                }
                return deleteResult;
            }
        }

        public async Task<DateItem> setCoverItem(int userId, int itemId)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(config.GetConnectionString("postgres")))
            {
                string sql = "Select * FROM \"cover_item\" WHERE user_id = @userId";
                IEnumerable<DateItem> result = await connection.QueryAsync<DateItem>(sql, new { userId = userId });
                DateItem item = await connection.QueryFirstOrDefaultAsync<DateItem>("Select * FROM \"date_item\" WHERE id= @itemId", new { itemId = itemId });
                if (item == null) throw new InvalidOperationException("No Such Item Exists");
                if (item.user_id != userId) throw new UnauthorizedAccessException("The item does not belong to you");
                if (result.Any())
                {
                    // if(result.ToList()[0].user_id!=userId){
                    //     throw new UnauthorizedAccessException("SetCoverItemError: The item does not belong to you");
                    // }
                    if(result.First().id == itemId) return result.First();
                    sql = "UPDATE \"cover_item\" SET date_item_id = @itemId WHERE user_id = @userId";
                    int updateResult = await connection.ExecuteAsync(sql, new { itemId = itemId, userId = userId });
                    if(updateResult==0){
                        throw new InvalidOperationException("No rows were updated");
                    }
                }
                else
                {
                    sql = "INSERT INTO \"cover_item\"(user_id,date_item_id) VALUES (@userId,@itemId)";
                    int createResult = await connection.ExecuteAsync(sql, new { itemId = itemId, userId = userId });
                    if(createResult==0){
                        throw new InvalidOperationException("No rows were inserted");
                    }

                }
                return item;
            }
        }

        public async Task<DateItem> getCoverItem(int userId)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(config.GetConnectionString("postgres")))
            {
                string sql = "SELECT * FROM \"date_item\" WHERE id = (SELECT date_item_id FROM \"cover_item\" WHERE user_id = @userId)";
                DateItem coverItem = await connection.QueryFirstOrDefaultAsync<DateItem>(sql, new { userId = userId });
                return coverItem;
            }
        }



    }
}