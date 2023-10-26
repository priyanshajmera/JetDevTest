using System.ComponentModel.DataAnnotations;

namespace JetDevTest.Dto
{
    public class ResetPasswordRequestDTO
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
        [Required]
        public string ResetToken { get; set; }
    }
}
