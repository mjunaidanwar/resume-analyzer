using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ResumeAnalyzerAPI.Entities;
using ResumeAnalyzerAPI.Models;
using ResumeAnalyzerAPI.Services;
using ResumeAnalyzerAPI.Utils;

namespace ResumeAnalyzerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResumeAnalyzerController : ControllerBase
    {
        private readonly ResumeAnalyzerService _service;

        public ResumeAnalyzerController(ResumeAnalyzerService service)
        {
            _service = service;
        }

        [HttpPost("analyze")]
        public async Task<IActionResult> AnalyzeResume([FromForm] ResumeAnalysisRequest request)
        {
            var resumeText = await FileHelper.ReadFileContentAsync(request.ResumeFile);
            var aiResult = await _service.AnalyzeResumeAsync(resumeText, request.JobDescription);
            try
            {
                var response = JsonConvert.DeserializeObject<ResumeAnalysisResponse>(aiResult);
                await _service.SaveResponseHistoryAsync(response);
                return Ok(response);
            }
            catch (JsonException)
            {
                return BadRequest(new { Error = "Failed to parse response", RawResponse = aiResult });
            }
        }

        [HttpGet("history")]
        public async Task<ResumeAnalysisHistory> GetResumeAnalysisHistoryAsync(int id)
        {
            return await _service.GetResumeAnalysisHistoryAsync(id);
        }

        [HttpGet("history/{id}")]
        public async Task<IEnumerable<ResumeAnalysisHistory>> GetResumeAnalysisHistoryAsync()
        {
            return await _service.GetResumeAnalysisHistoriesAsync();
        }

        [HttpPost("test")]
        public async Task<IActionResult> TestConnection()
        {
            var aiResult = await _service.TestConnectionAsync();
            return Ok(aiResult);
        }
    }
}
