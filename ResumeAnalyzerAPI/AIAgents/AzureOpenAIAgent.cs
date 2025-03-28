using Azure;
using Azure.AI.OpenAI;
using OpenAI.Chat;
using OpenAI.Files;

namespace ResumeAnalyzerAPI.AIAgents
{
    public class AzureOpenAIAgent : IAIAgent
    {
        private readonly AzureOpenAIClient _openAIClient;
        private readonly ChatClient _chatClient;

        public AzureOpenAIAgent(IConfiguration configuration)
        {
            var endpoint = configuration["AzureOpenAI:Endpoint"];
            string apiKey = Environment.GetEnvironmentVariable("AZUREAI_API_KEY");
            var deploymentName = configuration["AzureOpenAI:DeploymentName"];

            _openAIClient = new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
            _chatClient = _openAIClient.GetChatClient(deploymentName);
        }

        public async Task<string> TestConnectionAsync()
        {
            ChatCompletion completion = await _chatClient.CompleteChatAsync(
                [
                    new SystemChatMessage("This is a test."),
                    new SystemChatMessage("Please confirm that the connection to the Azure AI is working."),
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

            Provide:
            - Similarity score (0-100)
            - Matching skills
            - Missing skills
            - Recommendations for improvement";

            ChatCompletion completion = await _chatClient.CompleteChatAsync(
                [
                    new SystemChatMessage("You are a helpful assistant specialized in resume analysis."),
                    new UserChatMessage(prompt)
                ]);

            return completion.Content[0].Text;
        }
    }
}