namespace ResumeAnalyzerAPI.AIAgents
{
    public interface IAIAgent
    {
        Task<string> TestConnectionAsync();
        Task<string> AnalyzeResumeAsync(string resumeText, string jobDescription);
    }
}