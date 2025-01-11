namespace InsureApp.Server.Model
{
    public class InsuranceType
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<InsuranceReport>? InsuranceReports { get; set; }
    }
}
