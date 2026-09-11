using System.Text;
using System.Text.Json;

// Containeren kører med portmapping 5000:80.
HttpClient client = new HttpClient();
client.BaseAddress = new Uri("http://localhost:5000/api/games/");

// API'et skriver fx title i JSON, mens C#-property'en hedder Title.
var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };


Console.WriteLine("get [id]");
Console.WriteLine("POST, PUT, PATCH og DELETE er opgaver og endnu ikke implementeret.");
Console.WriteLine("post <title> <genre> <year>");
Console.WriteLine("put <id> <title> <genre> <year>");
Console.WriteLine("patch <id> <title>");
Console.WriteLine("delete <id>");
Console.WriteLine("exit");
Console.WriteLine("Skriv titel og genre uden mellemrum.");

while (true)
{
    string? input = Console.ReadLine();
    if (input == null || input == "exit") break;
    if (input == "") continue;

    string[] words = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    if (words.Length == 0) continue;

    switch (words[0])
    {
        case "get":
            await HandleGet(words);
            break;
        case "post":
            await HandlePost(words);
            break;
        case "put":
            await HandlePut(words);
            break;
        case "patch":
            await HandlePatch(words);
            break;
        case "delete":
            await HandleDelete(words);
            break;
        default:
            Console.WriteLine("Ukendt kommando.");
            break;
    }
}

async Task HandleGet(string[] words)
{
    // get henter alle spil. get 0 henter spillet med id 0.
    string id = "";
    if (words.Length > 1) id = words[1];

    using var response = await client.GetAsync(id);
    if (!response.IsSuccessStatusCode)
    {
        await ShowResponse(response);
        return;
    }

    string json = await response.Content.ReadAsStringAsync();
    Console.WriteLine($"HTTP {(int)response.StatusCode}");

    // JSON-teksten bliver til objekter, hvis properties vi kan arbejde med.
    if (id == "")
    {
        var games = JsonSerializer.Deserialize<List<GameResponse>>(json, jsonOptions)!;
        foreach (var game in games)
        {
            Console.WriteLine($"{game.Id}: {game.Title}, {game.Genre}, {game.ReleaseYear}");
        }
    }
    else
    {
        var game = JsonSerializer.Deserialize<GameResponse>(json, jsonOptions)!;
        Console.WriteLine($"{game.Id}: {game.Title}, {game.Genre}, {game.ReleaseYear}");
    }
}

Task HandlePost(string[] words)
{
    // TODO: Opret Game, serialisér til JSON, og send med POST.
    // TODO: Læs svarets JSON og deserialisér til GameResponse.
    Console.WriteLine("POST er ikke implementeret endnu.");
    return Task.CompletedTask;
}

Task HandlePut(string[] words)
{
    // TODO: Opret et Game-objekt, send med PUT, og vis svaret.
    Console.WriteLine("PUT er ikke implementeret endnu.");
    return Task.CompletedTask;
}

Task HandlePatch(string[] words)
{
    // TODO: Opret et objekt med kun Title, send med PATCH, og vis svaret.
    Console.WriteLine("PATCH er ikke implementeret endnu.");
    return Task.CompletedTask;
}

Task HandleDelete(string[] words)
{
    // TODO: Implementér DELETE.
    Console.WriteLine("DELETE er ikke implementeret endnu.");
    return Task.CompletedTask;
}

// API'et svarer med en statuskode og eventuelt en tekst.
// PUT, PATCH og DELETE giver 204 uden tekst, når de lykkes.
async Task ShowResponse(HttpResponseMessage response)
{
    Console.WriteLine($"HTTP {(int)response.StatusCode} {response.ReasonPhrase}");
    string text = await response.Content.ReadAsStringAsync();
    Console.WriteLine(text);
    response.Dispose();
}
