namespace InsureApp.Server.Model
{
    public class InsuranceAgent
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public string? Licence { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }

        public virtual ICollection<InsuranceReport>? HandledReports { get; set; }
    }
}
