using ResumeAnalyzerAPI.AIAgents;
using ResumeAnalyzerAPI.Data;
using ResumeAnalyzerAPI.Entities;
using ResumeAnalyzerAPI.Models;
using ResumeAnalyzerAPI.Repositories;

namespace ResumeAnalyzerAPI.Services
{
   public class ResumeAnalyzerService
    {
        private readonly IResumeAnalysisRepository _repository;
        private readonly IAIAgent _agent;

        public ResumeAnalyzerService(IConfiguration configuration, IResumeAnalysisRepository repository)
        {
            var agentSetting = configuration["AIAgent"];
            _agent = agentSetting switch
            {
                AIAgent.AzureOpenAI => new AzureOpenAIAgent(configuration),
                AIAgent.OpenAI => new OpenAIAgent(configuration),
                _ => throw new ArgumentException("Unsupported AI Agent specified", nameof(agentSetting))
            };
            _repository = repository;
        }

        public async Task<string> TestConnectionAsync()
        {
            return await _agent.TestConnectionAsync();
        }

        public async Task<string> AnalyzeResumeAsync(string resumeText, string jobDescription)
        {
            return await _agent.AnalyzeResumeAsync(resumeText, jobDescription);
        }

        public async Task SaveResponseHistoryAsync(ResumeAnalysisResponse response)
        {
            var history = new ResumeAnalysisHistory
            {
                SimilarityScore = response.SimilarityScore,
                MatchingSkills = response.MatchingSkills,
                MissingSkills = response.MissingSkills,
                Recommendations = response.Recommendations,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(history);
            await _repository.SaveChangesAsync();
        }

        public async Task<IEnumerable<ResumeAnalysisHistory>> GetResumeAnalysisHistoriesAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<ResumeAnalysisHistory> GetResumeAnalysisHistoryAsync(int historyId)
        {
            return await _repository.GetByIdAsync(historyId);
        }
    }
}
