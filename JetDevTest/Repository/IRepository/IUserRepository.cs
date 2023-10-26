using Data.Entity;
using JetDevTest.Dto;

namespace JetDevTest.Repository.IRepository
{
    public interface IUserRepository
    {
         Task<LoginResponseDTO> Login(LoginRequestDTO loginRequest);

         Task<User> Register(RegisterRequestDTO registerRequest);

        Task<string> ForgotPassword(string email);

        Task<ApiResponse> ResetPassword(ResetPasswordRequestDTO resetRequest);

    }
}
