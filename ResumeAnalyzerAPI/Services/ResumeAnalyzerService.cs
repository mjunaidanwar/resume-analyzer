using ResumeAnalyzerAPI.AIAgents;
using ResumeAnalyzerAPI.Models;

namespace ResumeAnalyzerAPI.Services
{
   public class ResumeAnalyzerService
    {
        private readonly IAIAgent _agent;

        public ResumeAnalyzerService(IConfiguration configuration)
        {
            var agentSetting = configuration["AIAgent"];
            _agent = agentSetting switch
            {
                AIAgent.AzureOpenAI => new AzureOpenAIAgent(configuration),
                AIAgent.OpenAI => new OpenAIAgent(configuration),
                _ => throw new ArgumentException("Unsupported AI Agent specified", nameof(agentSetting))
            };
        }

        public async Task<string> TestConnectionAsync()
        {
            return await _agent.TestConnectionAsync();
        }

        public async Task<string> AnalyzeResumeAsync(string resumeText, string jobDescription)
        {
            return await _agent.AnalyzeResumeAsync(resumeText, jobDescription);
        }
    }
}
