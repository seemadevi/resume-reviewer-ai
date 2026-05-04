//using Microsoft.SemanticKernel;
//using Microsoft.SemanticKernel.ChatCompletion;
//using Microsoft.Extensions.Configuration;

//// 1. Load API key from User Secrets
//var config = new ConfigurationBuilder()
//    .AddUserSecrets<Program>()
//    .Build();

//var apiKey = config["OpenAI:ApiKey"] ?? throw new InvalidOperationException("OpenAI:ApiKey is missing in User Secrets!");
//var model = config["OpenAI:Model"] ?? "gpt-4o-mini";


//// 2. Verify model
//Console.WriteLine($"Using model: {model} \n");


//// 3. Create Semantic Kernel
//var builder = Kernel.CreateBuilder();
//builder.AddOpenAIChatCompletion(model, apiKey);
//var kernel = builder.Build();

//// 4. Get chat service
//var chat = kernel.GetRequiredService<IChatCompletionService>();


//// 5. Setup conversation
//var History = new ChatHistory();  // ↓ History setup (conversation memory)
//History.AddSystemMessage("\"You are a helpful resume coach for software developers. Give short, practical advice.");// System rules

//// 6. Get user input
//Console.Write("Hi Seema \n");
//var userInput = Console.ReadLine() ?? "Hello!";
//History.AddUserMessage(userInput);  // तुमचा prompt

//// 7. Call AI
//Console.WriteLine("\n AI Response:");
//// ↓ हीच line = "AI Call"
//var response = await chat.GetChatMessageContentAsync(History);
////                       ↑
////                  Internet वरून OpenAI ला request जाते!

//// ↓ Response मिळाल्यावर print करा
//Console.WriteLine(response.Content);   // ← AI चा repl

//Console.WriteLine("\n First AI Call Successful!");
//Console.WriteLine("\n press any key to exit");
//Console.ReadKey();


using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;


Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.InputEncoding = System.Text.Encoding.UTF8;       //मग Marathi properly दिसेल!
// 1. Load API key
var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var apiKey = config["OpenAI:ApiKey"]
    ?? throw new InvalidOperationException("API key missing!");
var model = config["OpenAI:Model"] ?? "gpt-4o-mini";

// 2. Setup Semantic Kernel
var builder = Kernel.CreateBuilder();
builder.AddOpenAIChatCompletion(model, apiKey);
var kernel = builder.Build();
var chat = kernel.GetRequiredService<IChatCompletionService>();

// 3. Create ONE history that LIVES across multiple turns!
var history = new ChatHistory();
history.AddSystemMessage("You are a friendly resume coach. Remember user's details and refer back to them. Keep replies short.");

// 4. Welcome message
Console.WriteLine("🤖 AI Resume Coach Ready!");
Console.WriteLine("Type 'exit' to quit\n");

// 5. THE MAGIC LOOP! ✨
while (true)
{
    Console.Write("तुम्ही: ");
    var userInput = Console.ReadLine();

    // Exit check
    if (string.IsNullOrWhiteSpace(userInput) || userInput.ToLower() == "exit")
    {
        Console.WriteLine("\n👋 Bye! Good luck with your job hunt!");
        break;
    }

    // Add user's message to history
    history.AddUserMessage(userInput);

    // Get AI response
    var response = await chat.GetChatMessageContentAsync(history);

    // 🔑 IMPORTANT: Add AI's response BACK to history
    // (otherwise AI forgets what it just said!)
    history.AddAssistantMessage(response.Content ?? "");

    // Print AI response
    Console.WriteLine($"\n🤖 AI: {response.Content}\n");

    // Show token count (educational!)
    Console.WriteLine($"   [Total messages in memory: {history.Count}]\n");
}
