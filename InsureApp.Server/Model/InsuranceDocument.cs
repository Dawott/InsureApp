using System.ComponentModel.DataAnnotations.Schema;

namespace InsureApp.Server.Model
{
    public class InsuranceDocument
    {
        public int Id { get; set; }
        
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public DateTime UploadDate { get; set; }

        [ForeignKey(nameof(InsuranceReportId))]
        public int InsuranceReportId { get; set; }
        public virtual InsuranceReport InsuranceReport { get; set; }
    }
}
