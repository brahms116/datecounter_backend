using System;
using System.Security.Cryptography;
using System.Text;
using Npgsql;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;
using System.Security.Claims;
using Dapper;
using System.Linq;
using datecounter.Models;
namespace datecounter.Services
{
    public interface IUserService
    {
        Task<string> login(UserLoginInput _input);
        Task<User> registerUser(UserRegisterInput input);
    }



    // Class UserService is responsible for performing actions related to user data, such as login/register
    // Methods will throw InvalidOperationExecption on the case that email is already taken or that password is incorrect
    public class UserService : IUserService
    {

        private IConfiguration _config;
        private readonly IJwtService _jwtService;

        public UserService(IConfiguration configuration, IJwtService jwtService)
        {
            _config = configuration;
            _jwtService = jwtService;
        }

        private async Task<bool> _isEmailTaken(string email, NpgsqlConnection cnn)
        {
            string sql = "SELECT * FROM \"user\" WHERE email = @email";
            var result = await cnn.QueryAsync(sql, new { email = email });
            return result.Any();
        }

        public async Task<User> registerUser(UserRegisterInput input)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_config.GetConnectionString("postgres")))
            {
                if (await _isEmailTaken(input.email, connection))
                {
                    throw new ArgumentException("Email is already taken");
                }
                else
                {
                    //salt and hash passwords
                    byte[] salt = new byte[16];
                    new RNGCryptoServiceProvider().GetBytes(salt);
                    Rfc2898DeriveBytes generator = new Rfc2898DeriveBytes(input.password, salt);
                    byte[] hash = generator.GetBytes(16);

                    //password hash has the salt from 0-15 and the hash result from 16-31
                    byte[] passwordhash = new byte[32];
                    Array.Copy(salt, 0, passwordhash, 0, 16);
                    Array.Copy(hash, 0, passwordhash, 16, 16);

                    //store in db
                    string sql = "INSERT INTO \"user\" (email,password) VALUES( @email,@password) RETURNING*";
                    User result = await connection.QueryFirstOrDefaultAsync<User>(sql, new { email = input.email, password = Convert.ToBase64String(passwordhash) });
                    return result;
                }
            }
        }

        public async Task<string> login(UserLoginInput _input)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_config.GetConnectionString("postgres")))
            {

                string sql = "Select * from \"user\" where email= @email";
                User resultUser = await connection.QueryFirstOrDefaultAsync<User>(sql, new { email = _input.email });
                if (resultUser == null)
                {
                    throw new UnauthorizedAccessException("Incorrect Credentials");
                }
                else
                {
                    byte[] hashbytes = Convert.FromBase64String(resultUser.password);
                    byte[] salt = new byte[16];
                    Array.Copy(hashbytes, 0, salt, 0, 16);
                    Rfc2898DeriveBytes generator = new Rfc2898DeriveBytes(_input.password, salt);
                    byte[] inputPasswordBytes = generator.GetBytes(16);
                    for (int i = 0; i < 16; i++)
                    {
                        if (hashbytes[16 + i] != inputPasswordBytes[i])
                        {
                            throw new UnauthorizedAccessException("Incorrect Credentials");
                        }
                    }

                    //generate token and store user id in it
                    return _jwtService.generateToken(resultUser.id.ToString());
                }

            }
        }

    }
}