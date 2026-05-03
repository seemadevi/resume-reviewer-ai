using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.Extensions.Configuration;

// 1. Load API key from User Secrets
var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var apiKey = config["OpenAI:ApiKey"] ?? throw new InvalidOperationException("OpenAI:ApiKey is missing in User Secrets!");
var model = config["OpenAI:Model"] ?? "gpt-4o-mini";


// 2. Verify model
Console.WriteLine($"Using model: {model} \n");


// 3. Create Semantic Kernel
var builder = Kernel.CreateBuilder();
builder.AddOpenAIChatCompletion(model, apiKey);
var kernel = builder.Build();

// 4. Get chat service
var chat = kernel.GetRequiredService<IChatCompletionService>();


// 5. Setup conversation
var History = new ChatHistory();
History.AddSystemMessage("\"You are a helpful resume coach for software developers. Give short, practical advice.");


// 6. Get user input
Console.Write("Hi Seema");
var userInput = Console.ReadLine() ?? "Hello!";
History.AddUserMessage(userInput);

// 7. Call AI
Console.WriteLine("\nAI Response:");
var response = await chat.GetChatMessageContentAsync(History);
Console.WriteLine(response.Content);

Console.WriteLine("\n First AI Call Successful!");
Console.WriteLine("\n press any key to exit");
Console.ReadKey();



