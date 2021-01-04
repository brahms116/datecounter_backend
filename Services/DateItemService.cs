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
        Task<int> createItem(DateItemInput _input, int userId);
        Task<int> deleteItem(int userId, int itemId);
        Task<DateItem> getCoverItem(int userId);
        Task<IList<DateItem>> getItems(int userId);
        Task<int> setCoverItem(int userId, int itemId);
    }

    public class DateItemService : IDateItemService
    {
        private readonly IConfiguration config;

        public DateItemService(IConfiguration _config)
        {
            config = _config;
        }


        public async Task<int> createItem(DateItemInput _input, int userId)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(config.GetConnectionString("postgres")))
            {
                string sql = "INSERT INTO \"date_item\" (user_id,title,date) VALUES( @userId, @title, @date) ";
                int result = await connection.ExecuteAsync(sql, new { userId = userId, title = _input.title, date = _input.date });
                if (result == 0)
                {
                    throw new InvalidOperationException("CreateError, Sql returned num of rows affected as 0");
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


        public async Task<int> deleteItem(int userId, int itemId)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(config.GetConnectionString("postgres")))
            {
                string sql = "SELECT user_id FROM \"date_item\" WHERE id = @id";
                IEnumerable<DateItem> result = await connection.QueryAsync<DateItem>(sql, new { id = itemId });
                List<DateItem> resultItems = result.ToList();
                if (!resultItems.Any())
                {
                    throw new InvalidOperationException("DeleteError: No such date_item");
                }
                if (resultItems[0].user_id != userId)
                {
                    throw new UnauthorizedAccessException("DeleteError: Resource does not belong to you");
                }
                sql = "DELETE FROM \"date_item\" WHERE id = @id";
                int deleteResult = await connection.ExecuteAsync(sql, new { id = itemId });
                if (deleteResult < 1)
                {
                    throw new InvalidOperationException("DeleteError: No rows were deleted");
                }
                return deleteResult;
            }
        }

        public async Task<int> setCoverItem(int userId, int itemId)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(config.GetConnectionString("postgres")))
            {
                string sql = "Select * FROM \"cover_item\" WHERE user_id = @userId";
                var result = await connection.QueryAsync(sql, new { userId = userId });
                if (result.Any())
                {
                    sql = "UPDATE \"cover_item\" SET date_item_id = @itemId WHERE user_id = @userId";
                    int execResult = await connection.ExecuteAsync(sql, new { itemId = itemId, userId = userId });
                    return execResult;
                }
                else
                {
                    sql = "INSERT INTO \"cover_item\"(user_id,date_item_id) VALUES (@userId,@itemId)";
                    int execResult = await connection.ExecuteAsync(sql, new { itemId = itemId, userId = userId });
                    return execResult;

                }
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