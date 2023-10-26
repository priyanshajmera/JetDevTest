using System.ComponentModel.DataAnnotations;

namespace Data.Entity
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Email { get; set; }

        public string Password { get; set; }

        public string ResetToken { get; set; }

        public DateTime? ResetTokenCreationDate { get; set; }
    }
}
