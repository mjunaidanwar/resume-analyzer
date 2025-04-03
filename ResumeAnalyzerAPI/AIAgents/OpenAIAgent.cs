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
            var model = configuration["OpenAI:Model"];
            var apiKey = configuration["OpenAI:Key"];
            _openAIClient = new OpenAIClient(apiKey);
            _chatClient = _openAIClient.GetChatClient(model);
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
            string prompt = $@"Analyze this resume text:
            {resumeText}

            Against this job description:
            {jobDescription}

            Return the analysis strictly in the following JSON format only:

            {{
            ""similarityScore"": 0,
            ""matchingSkills"": [""skill1"", ""skill2""],
            ""missingSkills"": [""skill3"", ""skill4""],
            ""recommendations"": ""your recommendations here""
            }}";

            ChatCompletion completion = await _chatClient.CompleteChatAsync(
                [
                    new SystemChatMessage("You are a helpful assistant specialized in resume analysis."),
                    new UserChatMessage(prompt)
                ]);

            return completion.Content[0].Text;
        }
    }
}