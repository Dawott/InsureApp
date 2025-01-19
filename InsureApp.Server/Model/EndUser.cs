namespace InsureApp.Server.Model
{
    public class EndUser
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required bool IsActive { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }

        public virtual ICollection<InsuranceReport>? InsuranceReports { get; set; }
    }
}
