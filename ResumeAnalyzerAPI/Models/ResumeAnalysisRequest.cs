namespace ResumeAnalyzerAPI.Models
{
    public class ResumeAnalysisRequest
    {
        public IFormFile ResumeFile { get; set; }
        public string JobDescription { get; set; } = string.Empty;
    }
}
