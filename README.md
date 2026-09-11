# GameCollectionGetter – startprojekt

På `master` er **kun GET implementeret**. POST, PUT, PATCH og DELETE har tomme Handle-metoder med TODO'er.
Den færdige løsning ligger på `OtherKeywords`.

## Det virker allerede

```text
get
get 0
exit
```

## Opgave

Implementér de fire resterende metoder med den almindelige `Game`-klasse:

| Kommando, som skal implementeres | Request | Forventet succes |
|---|---|---|
| `post TestGame Action 2026` | Game | 201 med oprettet spil og Location |
| `put 2 UpdatedGame Adventure 2025` | Game | 204 uden svartekst |
| `patch 2 NewTitle` | Objekt med kun Title | 204 uden svartekst |
| `delete 2` | Ingen request body | 204 uden svartekst |

Brug id'et fra POST-svaret; 2 er kun et eksempel. Deserialisér POST-svaret til GameResponse, og vis objektets properties. Ved PUT, PATCH og DELETE vises status med ShowResponse.
Afprøv bagefter ændringerne med GET. Kommandoerne i tabellen sender endnu ingen requests på denne branch.

## Start

Start GameCollection-containeren med portmapping `5000:80`.
API-dokumentation og kontrakter: http://localhost:5000/scalar

```powershell
dotnet run --project GameCollectionGetter
```

Klienten bruger `http://localhost:5000/api/games/`. Ret BaseAddress i Program.cs, hvis du bruger en anden port.

## Game og JSON

`Game.cs` er en almindelig klasse med Title, Genre og ReleaseYear.
POST og PUT bruger samme klasse. API'ets C#-klassenavne behøver ikke være de samme som klientens;
det er felterne i JSON, der skal passe sammen.

Løsningen viser trinene direkte: opret Game, kald JsonSerializer.Serialize,
læg JSON i StringContent og send med PostAsync eller PutAsync.
GET læser først svaret som JSON-tekst med ReadAsStringAsync. Derefter bruges
JsonSerializer.Deserialize til GameResponse eller List<GameResponse>.
Koden udskriver objektets properties: Id, Title, Genre og ReleaseYear.
GameResponse er en almindelig klasse, ligesom Game, men indeholder også API'ets id.

POST-øvelsen følger samme dataflow i begge retninger:
1. Opret et Game-objekt fra brugerens input.
2. Serialisér objektet med JsonSerializer.Serialize og send JSON til API'et.
3. Læs svarets JSON og deserialisér med JsonSerializer.Deserialize<GameResponse>.
4. Brug det oprettede objekts Id og øvrige properties.

jsonOptions gør deserialiseringen uafhængig af store/små bogstaver, så JSON-feltet title matcher Title.

PATCH-eksemplet ændrer kun titlen og bruger derfor `new { Title = words[2] }`.
Et Game-objekt med kun Title udfyldt ville også sende Genre og ReleaseYear med standardværdier.
Begge modeller er almindelige klasser med properties. Der bruges ingen records.

## Koden

Switch-blokken kalder HandleGet, HandlePost, HandlePut, HandlePatch og HandleDelete.
`words[0]` er kommandoen, og `words[1]` er første argument.
Koden forudsætter korrekt input. Titel og genre skrives uden mellemrum.
Forkert input eller en stoppet API kan afslutte programmet. Validering og try/catch kan tilføjes som ekstra øvelse.
Data i API'et nulstilles ved genstart af containeren.
