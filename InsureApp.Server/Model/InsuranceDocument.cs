using System.ComponentModel.DataAnnotations.Schema;

namespace InsureApp.Server.Model
{
    public class InsuranceDocument
    {
        public int Id { get; set; }
        
        public required string FileName { get; set; }
        public required string FilePath { get; set; }
        public string? FileType { get; set; }
        public string? Description { get; set; }
        public DateTime UploadDate { get; set; }

        [ForeignKey("InsuranceReportId")]
        public int InsuranceReportId { get; set; }
        public virtual InsuranceReport InsuranceReport { get; set; }
    }
}
