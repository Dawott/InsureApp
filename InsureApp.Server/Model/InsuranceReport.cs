using System.ComponentModel.DataAnnotations.Schema;

namespace InsureApp.Server.Model
{
    public class InsuranceReport
    {
        public int Id { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string Status { get; set; } 
        public decimal RequestedCoverageAmount { get; set; }
        public string? AdditionalNotes { get; set; }

        [ForeignKey(nameof(EndUserId))]
        public int EndUserId { get; set; }
        [ForeignKey(nameof(InsuranceAgentId))]
        public int? InsuranceAgentId { get; set; }
        [ForeignKey(nameof(InsuranceTypeId))]
        public int InsuranceTypeId { get; set; }

        public virtual EndUser EndUser { get; set; }
        public virtual InsuranceAgent? InsuranceAgent { get; set; }
        public virtual InsuranceType InsuranceType { get; set; }

        public virtual ICollection<InsuranceDocument> Documents { get; set; }
    }
}
