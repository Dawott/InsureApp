namespace InsureApp.Server.Model
{
    public class InsuranceAgent
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Licence { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }

        public virtual ICollection<InsuranceReport>? HandledReports { get; set; }
    }
}
