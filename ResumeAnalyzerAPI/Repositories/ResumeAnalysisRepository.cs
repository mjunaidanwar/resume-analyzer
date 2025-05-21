using Microsoft.EntityFrameworkCore;
using ResumeAnalyzerAPI.Data;
using ResumeAnalyzerAPI.Entities;

namespace ResumeAnalyzerAPI.Repositories
{
    public class ResumeAnalysisRepository : IResumeAnalysisRepository
    {
        private readonly ApplicationDbContext _context;

        public ResumeAnalysisRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ResumeAnalysisHistory history)
        {
            await _context.ResumeAnalysisHistories.AddAsync(history);
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _context.ResumeAnalysisHistories.FindAsync(id);
            if (existing != null)
            {
                _context.ResumeAnalysisHistories.Remove(existing);
                await _context.SaveChangesAsync();
            }
        }


        public async Task<IEnumerable<ResumeAnalysisHistory>> GetAllAsync()
        {
            return await _context.ResumeAnalysisHistories
                         .OrderByDescending(r => r.CreatedAt)
                         .ToListAsync();
        }

        public async Task<ResumeAnalysisHistory> GetByIdAsync(int id)
        {
            return await _context.ResumeAnalysisHistories.FindAsync(id);
        }

        public async Task<bool> UpdateAsync(ResumeAnalysisHistory updatedAnalysis)
        {
            var existingAnalysis = await _context.ResumeAnalysisHistories.FindAsync(updatedAnalysis.Id);

            if (existingAnalysis == null)
                return false;

            // Update all relevant properties
            existingAnalysis.CompanyName = updatedAnalysis.CompanyName;
            existingAnalysis.SimilarityScore = updatedAnalysis.SimilarityScore;
            existingAnalysis.MatchingSkills = updatedAnalysis.MatchingSkills;
            existingAnalysis.MissingSkills = updatedAnalysis.MissingSkills;
            existingAnalysis.Recommendations = updatedAnalysis.Recommendations;
            existingAnalysis.Examples = updatedAnalysis.Examples;
            // Do NOT update existingAnalysis.CreatedAt or existingAnalysis.Id

            // Explicitly calling Update is kept for clarity as suggested, though SaveChangesAsync would typically handle it.
            _context.ResumeAnalysisHistories.Update(existingAnalysis); 
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
