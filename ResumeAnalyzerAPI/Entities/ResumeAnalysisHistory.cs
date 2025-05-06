namespace ResumeAnalyzerAPI.Entities
{
    public class ResumeAnalysisHistory
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public double SimilarityScore { get; set; }
        public List<string> MatchingSkills { get; set; } = new List<string>();
        public List<string> MissingSkills { get; set; } = new List<string>();
        public string Recommendations { get; set; } = string.Empty;
        public List<string> Examples { get; set; } = new List<string>();
        public DateTime CreatedAt { get; set; }
    }
}