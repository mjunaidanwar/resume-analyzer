using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ResumeAnalyzerAPI.Entities;
using ResumeAnalyzerAPI.Models;
using ResumeAnalyzerAPI.Services;
using ResumeAnalyzerAPI.Utils;
using Microsoft.Extensions.Logging; // Added for ILogger

namespace ResumeAnalyzerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class ResumeAnalyzerController : ControllerBase
    {
        private readonly ResumeAnalyzerService _service;
        private readonly ILogger<ResumeAnalyzerController> _logger; // Added for logging

        public ResumeAnalyzerController(ResumeAnalyzerService service, ILogger<ResumeAnalyzerController> logger) // Added logger injection
        {
            _service = service;
            _logger = logger; // Store logger
        }

        [HttpPost("analyze")]
        [AllowAnonymous]
        public async Task<IActionResult> AnalyzeResume([FromForm] ResumeAnalysisRequest request)
        {
            string resumeText;
            try
            {
                resumeText = await FileHelper.ReadFileContentAsync(request.ResumeFile);
            }
            catch (NotSupportedException ex)
            {
                _logger.LogWarning(ex, "Unsupported file format uploaded: {FileName}", request.ResumeFile.FileName);
                return BadRequest(new { Error = "Unsupported file format. Please upload a .txt, .pdf, or .docx file.", Detail = ex.Message });
            }
            catch (Exception ex) // Catches other errors from FileHelper, e.g. parsing issues
            {
                _logger.LogError(ex, "Error reading or parsing resume file: {FileName}", request.ResumeFile.FileName);
                return BadRequest(new { Error = "Error reading or parsing the resume file. Please ensure the file is not corrupted or password-protected.", Detail = ex.Message });
            }

            string aiResult;
            try
            {
                aiResult = await _service.AnalyzeResumeAsync(resumeText, request.JobDescription);
            }
            catch (ArgumentException ex) when (ex.ParamName == "jobDescription" || (ex.Message != null && ex.Message.ToLower().Contains("job description")))
            {
                _logger.LogWarning(ex, "Invalid job description provided for file: {FileName}", request.ResumeFile.FileName);
                return BadRequest(new { Error = ex.Message });
            }
            // It's good practice to catch other specific exceptions the service might throw if they need unique handling.
            // For now, the global exception handler will catch other unexpected service errors.

            try
            {
                var response = JsonConvert.DeserializeObject<ResumeAnalysisResponse>(aiResult);
                await _service.SaveResponseHistoryAsync(response);
                return Ok(response);
            }
            catch (JsonException ex) // It's good practice to log this too
            {
                _logger.LogError(ex, "Failed to parse AI response for file: {FileName}", request.ResumeFile.FileName);
                return BadRequest(new { Error = "Failed to parse response from AI service.", RawResponse = aiResult });
            }
        }

        [HttpGet("history/{id}")]
        [AllowAnonymous]
        public async Task<ResumeAnalysisHistory> GetResumeAnalysisHistoryAsync(int id)
        {
            return await _service.GetResumeAnalysisHistoryAsync(id);
        }

        [HttpGet("history")]
        [AllowAnonymous]
        public async Task<IEnumerable<ResumeAnalysisHistory>> GetResumeAnalysisHistoryAsync()
        {
            return await _service.GetResumeAnalysisHistoriesAsync();
        }

        // TODO: Security Review - [AllowAnonymous] is used here. 
        // In a production environment, this endpoint should be protected with appropriate authorization
        // (e.g., require authentication and check if the user has permission to update this history).
        [HttpPut("history")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateResumeAnalysisHistoryAsync(ResumeAnalysisHistory updatedResumeAnalysisHistory)
        {
            var result = await _service.UpdateResumeAnalysisHistoryAsync(updatedResumeAnalysisHistory);
            return Ok(result);
        }

        // TODO: Security Review - [AllowAnonymous] is used here.
        // In a production environment, this endpoint should be protected with appropriate authorization
        // (e.g., require authentication and check if the user has permission to delete this history).
        [HttpDelete("history/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteResumeAnalysisHistoryAsync(int id)
        {
            await _service.DeleteResumeAnalysisHistoryAsync(id);
            return Ok();
        }

        [HttpPost("test")]
        [AllowAnonymous]
        public async Task<IActionResult> TestConnection()
        {
            var aiResult = await _service.TestConnectionAsync();
            return Ok(aiResult);
        }
    }
}
