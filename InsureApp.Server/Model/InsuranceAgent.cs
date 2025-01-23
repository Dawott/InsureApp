using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace InsureApp.Server.Model
{
    public class InsuranceAgent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "Imię jest wymagane!")]
        public required string FirstName { get; set; }
        [Required(ErrorMessage = "Nazwisko jest wymagane!")]
        public required string LastName { get; set; }
        [Required(ErrorMessage = "Podaj Email!")]
        [EmailAddress(ErrorMessage = "Błędny format email")]
        public required string Email { get; set; }
        public string? Licence { get; set; }
        [Required(ErrorMessage = "Nazwa użytkownika jest wymagana!")]
        public string Username { get; set; }
        public string PasswordHash { get; set; }

        public virtual ICollection<InsuranceReport>? HandledReports { get; set; }
    }
}
