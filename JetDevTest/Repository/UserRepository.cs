using Data;
using Data.Entity;
using JetDevTest.Dto;
using JetDevTest.Repository.IRepository;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JetDevTest.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _config;
        private readonly string _secretKey;

        public UserRepository(ApplicationDbContext db,IConfiguration config)
        {
            this._db = db;
            this._config = config;
            
            _secretKey = _config.GetValue<string>("ApiSettings:Secret");
        }
        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequest)
        {
            var user = _db.Users.FirstOrDefault(u => (u.Email.ToUpper() == loginRequest.Email.ToUpper()) &&
                   u.Password == loginRequest.Password);



            if (user == null)
            {
                return new LoginResponseDTO()
                {
                    Token = "",
                    UserName = null
                };
            }

            //for generating JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(this._secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserName.ToString()),

                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
            {
                Token = tokenHandler.WriteToken(token),
                UserName = user.UserName

            };
            return loginResponseDTO;
        }

        public async Task<User> Register(RegisterRequestDTO registerRequest)
        {   //checkong if user already exists for perticular mail
            User existedUser = _db.Users.Where(u => u.Email == registerRequest.Email).FirstOrDefault();
            //if unique user then register it
            if (existedUser == null)
            {
                User user = new User()
                {
                    UserName = registerRequest.UserName,
                    Email = registerRequest.Email,
                    Password = registerRequest.Password,
                    
                };

                _db.Users.Add(user);
                _db.SaveChanges();
                return user;
                
            }
            return null;
           
        }

        public async Task<string> ForgotPassword(string email)
        {
            User user = _db.Users.Where(u => u.Email == email).FirstOrDefault();
            if(user == null)
            {
                return null;
            }
            //creating random token
            string token = Guid.NewGuid().ToString();
            //adding token to database table
            user.ResetToken = token;
            user.ResetTokenCreationDate = DateTime.Now;
            _db.SaveChanges();
            return token;
        }


        public async Task<ApiResponse> ResetPassword(ResetPasswordRequestDTO resetRequest)
        {
            User user =  _db.Users.Where(u => u.Email == resetRequest.Email).FirstOrDefault();
            ApiResponse response = new ApiResponse();
            if (user == null)
            {
                response.StatusCode = System.Net.HttpStatusCode.NotFound;
                response.IsSuccess = false;
                return response;
            }
            if(resetRequest.Password != resetRequest.ConfirmPassword)
            {
                response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                response.ErrorMessages.Add("Passwords not Matched");
                response.IsSuccess = false;
                return response;
            }
            //here checking if reset creantials are ok or not
            if (user.ResetTokenCreationDate != null
                && user.ResetTokenCreationDate.Value.AddMinutes(30) > DateTime.Now
                && !string.IsNullOrWhiteSpace(user.ResetToken)
                && user.ResetToken == resetRequest.ResetToken)
            {   //here making token again empty and changing the password
                user.ResetToken = string.Empty;
                user.ResetTokenCreationDate = null;
                user.Password = resetRequest.Password;
                
                _db.SaveChanges();

                response.IsSuccess = true;
                response.StatusCode = System.Net.HttpStatusCode.OK;
                return response;
            }
            response.StatusCode = System.Net.HttpStatusCode.OK;
            return response;
        }
    }
}
