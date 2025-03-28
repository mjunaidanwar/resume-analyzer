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

        public async Task<IEnumerable<ResumeAnalysisHistory>> GetAllAsync()
        {
            return await _context.ResumeAnalysisHistories.ToListAsync();
        }

        public async Task<ResumeAnalysisHistory> GetByIdAsync(int id)
        {
            return await _context.ResumeAnalysisHistories.FindAsync(id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
