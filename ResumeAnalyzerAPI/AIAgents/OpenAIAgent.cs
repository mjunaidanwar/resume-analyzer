using OpenAI;
using OpenAI.Chat;

namespace ResumeAnalyzerAPI.AIAgents
{
    public class OpenAIAgent : IAIAgent
    {
        private readonly ChatClient _chatClient;
        private readonly OpenAIClient _openAIClient;
        public OpenAIAgent(IConfiguration configuration)
        {
            _openAIClient = new OpenAIClient("Your API Key");
            _chatClient = _openAIClient.GetChatClient("o4-mini");
        }

        public async Task<string> TestConnectionAsync()
        {
            ChatCompletion completion = await _chatClient.CompleteChatAsync(
                [
                    new SystemChatMessage("This is a test."),
                    new SystemChatMessage("Please confirm that the connection to the OpenAI API is working."),
                    new SystemChatMessage("Let the user know that thay can use the resume analyzer service by submitting resume and job description from below.")
                ]);
            if (completion.Content == null) {return "Failed to connect, please try again later";}
            return $"{completion.Content[0].Text}";
        }

        /// <summary>
        /// Method to analyze resume.
        /// </summary>
        /// <param name="resumeText">Parsed resume text</param>
        /// <param name="jobDescription">Parsed Job Description</param>
        /// <returns>structured string containing the analyzed resume result</returns>
        public async Task<string> AnalyzeResumeAsync(string resumeText, string jobDescription)
        {
            string prompt = $@"
                You are an AI resume‑to‑job‑description matcher.  
                Return **only** valid JSON that matches the template below‑–no extra keys, no explanatory text.

                • ‘similarityScore’ **must** be an integer **0 ‑ 100** (0 = no match, 100 = perfect match).  
                • ‘examples’ must contain **at least two** bullet points (feel free to provide more if helpful).

                Template (order and exact property names required):

                {{
                ""similarityScore"": 0,
                ""matchingSkills"": [""skill1"", ""skill2""],
                ""missingSkills"": [""skill3"", ""skill4""],
                ""recommendations"": ""your recommendations here"",
                ""examples"": [""resumeBulletPoint1"", ""resumeBulletPoint2""]
                }}

                Analyze this **resume text**:
                {resumeText}

                Against this **job description**:
                {jobDescription}
                ";

            ChatCompletion completion = await _chatClient.CompleteChatAsync(
                [
                    new SystemChatMessage("You are a helpful assistant specialized in resume analysis."),
                    new UserChatMessage(prompt)
                ]);

            return completion.Content[0].Text;
        }
    }
}