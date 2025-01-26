using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsureApp.Server.Model
{
    public class InsuranceReport
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime SubmissionDate { get; set; }
        public required string Status { get; set; } 
        public required string Description { get; set; }
        public string? DecisionReason { get; set; }
        public string? AdditionalNotes { get; set; }

        [ForeignKey("EndUserId")]
        public int? EndUserId { get; set; }
        [ForeignKey("InsuranceAgentId")]
        public int? InsuranceAgentId { get; set; }
        [ForeignKey("InsuranceTypeId")]
        public int? InsuranceTypeId { get; set; }

        public EndUser? EndUser { get; set; }
        public InsuranceAgent? InsuranceAgent { get; set; }
        public InsuranceType? InsuranceType { get; set; }

        public virtual ICollection<InsuranceDocument>? Documents { get; set; }
    }
}
