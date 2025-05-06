using Newtonsoft.Json;

namespace ResumeAnalyzerAPI.Models
{
    public class ResumeAnalysisResponse
    {
        [JsonProperty("similarityScore")]
        public double SimilarityScore { get; set; }
        
        [JsonProperty("matchingSkills")]
        public List<string> MatchingSkills { get; set; } = new List<string>();
        
        [JsonProperty("missingSkills")]
        public List<string> MissingSkills { get; set; } = new List<string>();
        
        [JsonProperty("recommendations")]
        public string Recommendations { get; set; } = string.Empty;

        [JsonProperty("examples")]
        public List<string> Examples { get; set; } = new List<string>();
    }
}
