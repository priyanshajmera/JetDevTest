using JetDevTest.Dto;
using JetDevTest.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace JetDevTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepo;
        protected ApiResponse _response;

        public UserController(IUserRepository userRepo)
        {
            this._userRepo = userRepo;
            _response = new ApiResponse();
        }
        //endpoint for register
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDTO registerRequest)
        {
         
            var user = await this._userRepo.Register(registerRequest);
            if (user == null)
            {
                _response.StatusCode = HttpStatusCode.Ambiguous;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("User Already Exists");
                return BadRequest(_response);
            }
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;

            return Ok(_response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDTO loginRequest)
        {
            var loginResponse = await this._userRepo.Login(loginRequest);
            if (loginResponse.UserName == null || string.IsNullOrEmpty(loginResponse.Token))
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Either Password is wrong or user not exist!!!");
                return BadRequest(_response);
            }
            
            return Ok(loginResponse);

        }


        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(string Email)
        {
            var response = await this._userRepo.ForgotPassword(Email);
            if (response == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("No user Found");
                return BadRequest("No User Exists");
            }
            return Ok();

        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequestDTO resetRequest)
        {
            ApiResponse response = await _userRepo.ResetPassword(resetRequest);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound(_response);
            }
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
