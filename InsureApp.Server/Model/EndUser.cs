using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsureApp.Server.Model
{
    public class EndUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "Imię jest wymagane!")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Nazwisko jest wymagane!")]
        public required string LastName { get; set; }
        [Required(ErrorMessage = "Podaj Email!")]
        [EmailAddress(ErrorMessage = "Błędny format email")]
        public required string Email { get; set; }
        public required bool IsActive { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        [Required(ErrorMessage = "Nazwa użytkownika jest wymagana!")]
        public string Username { get; set; }
        public string PasswordHash { get; set; }

        public virtual ICollection<InsuranceReport>? InsuranceReports { get; set; }
    }
}
