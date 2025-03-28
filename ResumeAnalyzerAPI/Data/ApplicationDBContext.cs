using Microsoft.EntityFrameworkCore;
using ResumeAnalyzerAPI.Entities;

namespace ResumeAnalyzerAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ResumeAnalysisHistory> ResumeAnalysisHistories { get; set; }
    }
}
