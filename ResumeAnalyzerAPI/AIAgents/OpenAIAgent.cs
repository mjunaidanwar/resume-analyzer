using System.Text;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Files;

namespace ResumeAnalyzerAPI.AIAgents
{
    public class OpenAIAgent : IAIAgent
    {
        private readonly ChatClient _chatClient;
        private readonly OpenAIClient _openAIClient;
        private readonly OpenAIFileClient _fileClient;
        public OpenAIAgent(IConfiguration configuration)
        {
            var model = configuration["OpenAI:Model"];
            string apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            _openAIClient = new OpenAIClient(apiKey);
            _chatClient = _openAIClient.GetChatClient(model);
            _fileClient = _openAIClient.GetOpenAIFileClient();
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

        /// <summary>
        /// Future story: Function to store Resume in OpenAI.
        /// </summary>
        /// <param name="resumeText"></param>
        /// <returns>File ID</returns>
        public async Task<string> StoreResumeAsync(string resumeText)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(resumeText)))
            {
                var fileResponse = await _fileClient.UploadFileAsync(
                    stream, 
                    "resume-junaid", 
                    FileUploadPurpose.FineTune);
                return fileResponse.Value.Id;
            }
        }
    }
}