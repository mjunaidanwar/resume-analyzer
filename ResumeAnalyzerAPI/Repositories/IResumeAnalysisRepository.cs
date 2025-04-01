using ResumeAnalyzerAPI.Entities;

namespace ResumeAnalyzerAPI.Repositories
{
    public interface IResumeAnalysisRepository
    {
        Task AddAsync(ResumeAnalysisHistory history);
        Task<IEnumerable<ResumeAnalysisHistory>> GetAllAsync();
        Task<ResumeAnalysisHistory> GetByIdAsync(int id);
        Task<bool> UpdateAsync(ResumeAnalysisHistory updatedAnalysis);
        Task SaveChangesAsync();
    }
}