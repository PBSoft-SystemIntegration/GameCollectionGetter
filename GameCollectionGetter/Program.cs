using System.Text;
using System.Text.Json;

// Containeren kører med portmapping 5000:80.
HttpClient client = new HttpClient();
client.BaseAddress = new Uri("http://localhost:5000/api/games/");

// API'et skriver fx title i JSON, mens C#-property'en hedder Title.
var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };


Console.WriteLine("get [id]");
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

async Task HandlePost(string[] words)
{
    var game = new Game
    {
        Title = words[1],
        Genre = words[2],
        ReleaseYear = int.Parse(words[3])
    };
    // Lav Game-objektet om til JSON, og send det i requestens body.
    string json = JsonSerializer.Serialize(game);
    using var content = new StringContent(json, Encoding.UTF8, "application/json");
    using var response = await client.PostAsync("", content);
    string responseJson = await response.Content.ReadAsStringAsync();
    Console.WriteLine($"HTTP {(int)response.StatusCode}");

    if (!response.IsSuccessStatusCode)
    {
        Console.WriteLine(responseJson);
        return;
    }

    // API'et returnerer det oprettede spil, nu med et id.
    var createdGame = JsonSerializer.Deserialize<GameResponse>(responseJson, jsonOptions)!;
    Console.WriteLine($"Oprettet: {createdGame.Id}: {createdGame.Title}, {createdGame.Genre}, {createdGame.ReleaseYear}");
}

async Task HandlePut(string[] words)
{
    string id = words[1];
    // PUT erstatter alle spillets felter.
    var game = new Game
    {
        Title = words[2],
        Genre = words[3],
        ReleaseYear = int.Parse(words[4])
    };

    string json = JsonSerializer.Serialize(game);
    using var content = new StringContent(json, Encoding.UTF8, "application/json");
    var response = await client.PutAsync(id, content);
    await ShowResponse(response);
}

async Task HandlePatch(string[] words)
{
    string id = words[1];
    // PATCH sender kun det felt, vi vil ændre. Her ændrer vi titlen.
    var changes = new { Title = words[2] };

    string json = JsonSerializer.Serialize(changes);
    using var content = new StringContent(json, Encoding.UTF8, "application/json");
    var response = await client.PatchAsync(id, content);
    await ShowResponse(response);
}

async Task HandleDelete(string[] words)
{
    string id = words[1];
    var response = await client.DeleteAsync(id);
    await ShowResponse(response);
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
