using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using QuadraPyramidPrototype.Models;
using QuadraPyramidPrototype.Pages;

// Developer-only checks. These call the real component methods, without a browser
// or third-party test packages. Reflection lets gameplay fields stay private.
string projectFolder = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../QuadraPyramidPrototype"));
var game = new Home();
var handler = new LevelFileHandler(projectFolder);
using var http = new HttpClient(handler) { BaseAddress = new Uri("http://localhost/") };
typeof(Home).GetProperty("Http", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(game, http);
int checks = 0;

object? Call(string name, params object[] args) => typeof(Home).GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(game, args);
T Get<T>(string name) => (T)typeof(Home).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(game)!;
async Task CallAsync(string name, params object[] args) => await (Task)Call(name, args)!;
void Check(bool condition, string description)
{
    if (!condition) throw new Exception("FAILED: " + description);
    checks++;
    Console.WriteLine("PASS: " + description);
}

Check(Get<string>("currentScreen") == "menu", "Initial screen is main menu");
Call("ShowInstructions");
Check(Get<string>("currentScreen") == "instructions", "Instructions navigation");
Call("ShowLevels");
Check(Get<string>("currentScreen") == "levels", "Pyramid navigation");
Call("ShowMenu");
Check(Get<string>("currentScreen") == "menu", "Menu navigation");

int[][] expectedAnswers = { new[] { 0, 2 }, new[] { 1, 3 }, new[] { 2, 0 } };
for (int level = 1; level <= 3; level++)
{
    await CallAsync("StartLevel", level);
    var data = Get<LevelData>("levelData");
    Check(data.Level == level && data.Questions.Length == 2, $"Level {level}: JSON deserializes into real C# models");
    Check(data.Questions.All(q => q.Choices.Length == 4 && q.Choices.Distinct().Count() == 4 && !string.IsNullOrWhiteSpace(q.Explanation)), $"Level {level}: four distinct choices and explanations");
    Check(data.Questions.Select(q => q.Answer).SequenceEqual(expectedAnswers[level - 1]), $"Level {level}: answer keys match independently reviewed mathematics");
    Check(Get<int>("score") == 0 && Get<int>("questionIndex") == 0 && !Get<bool>("hasAnswered"), $"Level {level}: starts clean");
    Call("NextQuestion");
    Check(Get<int>("questionIndex") == 0, $"Level {level}: cannot skip unanswered question");

    for (int i = 0; i < data.Questions.Length; i++)
    {
        Call("CheckAnswer", data.Questions[i].Answer);
        Check(Get<int>("score") == (i + 1) * 10 && Get<bool>("hasAnswered"), $"Level {level}, question {i + 1}: correct answer earns ten");
        Check((string)Call("GetChoiceClass", data.Questions[i].Answer)! == "choice-correct", $"Level {level}, question {i + 1}: correct answer is highlighted");
        Call("CheckAnswer", data.Questions[i].Answer);
        Call("CheckAnswer", (data.Questions[i].Answer + 1) % 4);
        Check(Get<int>("score") == (i + 1) * 10 && Get<int>("selectedAnswer") == data.Questions[i].Answer, $"Level {level}, question {i + 1}: repeated answers cannot change score or choice");
        Call("NextQuestion");
    }
    Check(Get<string>("currentScreen") == "results" && Get<int>("correctAnswers") == 2 && Get<int>("score") == 20, $"Level {level}: final question opens results with correct totals");
    await CallAsync("RetryLevel");
    Check(Get<string>("currentScreen") == "question" && Get<int>("score") == 0 && Get<int>("correctAnswers") == 0 && Get<int>("selectedAnswer") == -1 && Get<int>("questionIndex") == 0 && !Get<bool>("hasAnswered"), $"Level {level}: retry resets all attempt state");
    Call("CheckAnswer", (data.Questions[0].Answer + 1) % 4);
    Check(Get<int>("score") == 0 && Get<bool>("hasAnswered"), $"Level {level}: wrong answer earns zero");
    Check((string)Call("GetChoiceClass", data.Questions[0].Answer)! == "choice-correct" && (string)Call("GetChoiceClass", Get<int>("selectedAnswer"))! == "choice-incorrect", $"Level {level}: wrong choice and correction are highlighted");
    Call("NextQuestion");
    Call("CheckAnswer", (data.Questions[1].Answer + 1) % 4);
    Call("NextQuestion");
    Check(Get<string>("currentScreen") == "results" && Get<int>("score") == 0, $"Level {level}: no passing score required");
    if (level < 3)
    {
        await CallAsync("NextLevel");
        Check(Get<int>("currentLevel") == level + 1 && Get<int>("score") == 0, $"Level {level}: next level loads and resets score");
    }
}

string homeSource = File.ReadAllText(Path.Combine(projectFolder, "Pages/Home.razor"));
Check(Get<int>("currentLevel") == 3 && Get<string>("currentScreen") == "results" && homeSource.Contains("Congratulations! You have reached the top of Quadra Pyramid!"), "Level 3 completion state and message exist (rendering needs browser check)");
handler.Fail = true;
await CallAsync("RetryLevel");
Check(Get<string>("errorMessage").Length > 0 && !Get<bool>("isLoading"), "Missing data shows recoverable error and releases loading flag");
handler.Fail = false;
handler.Malformed = true;
await CallAsync("RetryLevel");
Check(Get<string>("errorMessage").Length > 0 && !Get<bool>("isLoading"), "Malformed JSON shows recoverable error");
handler.Malformed = false;
await CallAsync("RetryLevel");
Check(Get<string>("errorMessage") == "" && Get<string>("currentScreen") == "question", "Retry recovers after data error");
Console.WriteLine($"\n{checks} checks passed. Browser rendering and responsive layout still need manual verification.");

class LevelFileHandler(string projectFolder) : HttpMessageHandler
{
    public bool Fail { get; set; }
    public bool Malformed { get; set; }
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (Fail) return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
        string fileName = Path.GetFileName(request.RequestUri!.AbsolutePath);
        string json = Malformed ? "{broken" : File.ReadAllText(Path.Combine(projectFolder, "wwwroot/data", fileName));
        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json, Encoding.UTF8, "application/json") });
    }
}
