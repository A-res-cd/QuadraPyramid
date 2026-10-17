# Quadra Pyramid

A small, standalone mathematics game prototype for Grade 10 students of Nueva Ecija High School, Philippines. It demonstrates the appearance, navigation, questions, feedback, scoring, and proposed pyramid learning journey. It is a concept for client review, not the final production game.

The project was created with the official standalone template:

```powershell
dotnet new blazorwasm -o QuadraPyramidPrototype --no-restore
```

No Unity files were imported, referenced, or reused.

## Technologies used

| Technology | What it does here |
| --- | --- |
| C# | Chooses screens, loads questions, checks answers, and calculates scores. |
| Blazor WebAssembly | Runs the C# application inside a normal browser using WebAssembly, a format browsers can execute. |
| Razor | Combines HTML and C# in a `.razor` file. `@if` chooses what to display; `@onclick` calls a C# method when a button is pressed. |
| HTML | Gives the screens their structure: headings, paragraphs, and buttons. |
| CSS | Sets colors, spacing, button styles, pyramid shapes, and mobile layouts. |
| JSON | Stores question text, choices, answers, and explanations outside the game code. |
| .NET SDK | Provides commands to create, build, and run the application. |

The only NuGet packages are Blazor WebAssembly and its development server, both supplied by the template. There are no external CSS frameworks, JavaScript gameplay scripts, databases, APIs, or component libraries. `blazor.webassembly.js` is Microsoft's required framework loader, not game logic.

## Project structure

```text
QuadraPyramid_Prototype/
├── .gitignore
├── QuadraPyramidPrototype/
│   ├── Pages/
│   │   └── Home.razor                 All five screens and game logic
│   ├── Models/
│   │   └── QuestionData.cs            LevelData and QuestionData classes
│   ├── Layout/
│   │   └── MainLayout.razor           Displays the main page
│   ├── Properties/
│   │   └── launchSettings.json        Local development addresses
│   ├── wwwroot/
│   │   ├── css/app.css                All styling and CSS artwork
│   │   ├── data/level1.json           Factoring questions
│   │   ├── data/level2.json           Quadratic equation questions
│   │   ├── data/level3.json           Quadratic inequality questions
│   │   └── index.html                Browser entry page and Blazor loader
│   ├── Program.cs                    Starts Blazor and sets up HttpClient
│   ├── App.razor                     Standard router for the home page
│   ├── _Imports.razor                Shared C# namespace imports
│   ├── QuadraPyramidPrototype.csproj  .NET version and template dependencies
│   └── README.md                     This student guide
└── verification/                     Optional developer checks, not part of the game
    ├── Verification.csproj
    └── Program.cs
```

`bin` and `obj` are generated when you build. Do not edit them. The optional verification program uses built-in .NET features and adds no third-party packages to the game.

## How to run

1. Download the **.NET 8 SDK** for your computer from [Microsoft's .NET 8 download page](https://dotnet.microsoft.com/en-us/download/dotnet/8.0). Choose the SDK, not only the runtime. On Windows, run the installer and reopen your terminal afterward. The prototype targets `net8.0`; it was built here with SDK `8.0.424`.
2. Install and open Visual Studio Code. Use **File → Open Folder** to open `QuadraPyramid_Prototype`.
3. Select **Terminal → New Terminal**. Check your installation:

   ```powershell
   dotnet --info
   dotnet new list blazorwasm
   ```

4. From the outer workspace folder, run:

   ```powershell
   cd QuadraPyramidPrototype
   dotnet restore
   dotnet build
   dotnet run
   ```

5. Open the address printed after `Now listening on:`. The template's HTTP profile uses `http://localhost:5170`. Use the address printed by your terminal if it differs. Wait for the initial browser download of the .NET runtime.
6. Stop the server with **Ctrl+C**.

For a predictable HTTP address without opening a browser automatically, run this instead of `dotnet run`:

```powershell
dotnet run --no-launch-profile --urls http://localhost:5170
```

If port 5170 is busy, change it to 5171. Do not open `index.html` directly from File Explorer: Blazor and its JSON files must be served over HTTP. The development server serves static application files; it is not a backend API. Internet is needed for the first package restore. The app uses no external fonts or online services during gameplay.

If `dotnet` is not recognized, install the SDK and restart VS Code. If the standalone template is missing from an SDK installation, try `dotnet new install Microsoft.AspNetCore.Components.WebAssembly.Templates::8.0.30`, then check `dotnet new list blazorwasm` again. An ordinary .NET 8 SDK installation already includes it. No separate WebAssembly workload is required for this ordinary development build.

## How the game works

1. **Main menu:** Play opens the pyramid. Instructions explains the rules.
2. **Pyramid:** select Level 1 (Factoring), Level 2 (Quadratic Equations), or Level 3 (Quadratic Inequalities). All levels are open for demonstration.
3. **Questions:** select one of four choices. The game immediately shows whether it is correct, marks the correct choice, and explains the mathematics. All answer buttons then become disabled.
4. **Next Question:** becomes available after answering. On the last question it is labelled **See Results**.
5. **Results:** shows the level name, correct answers out of total answered, and points. Retry Level starts a fresh attempt. Next Level loads the next topic. Back to Pyramid lets you choose again.
6. **Pyramid completion:** finishing Level 3 shows “Congratulations! You have reached the top of Quadra Pyramid!” There is no Level 4 button.

Each correct answer earns 10 points. Each incorrect answer earns 0. There are no timers, lives, penalties, or passing requirements. Each sample level has two questions, so its maximum is 20 points. Each attempt starts at 0. Refreshing the browser resets the game. Leaving a question screen abandons that attempt when another level is started.

The demonstration treats finishing Level 3 as reaching the summit even when selected directly. It does not track whether the other levels were completed. Progression rules can be revised after client feedback.

## How to edit questions

Open one of the JSON files in `wwwroot/data`. Each file has a level number, title, and array (list) of questions. For example:

```json
{
  "level": 1,
  "title": "Factoring",
  "questions": [
    {
      "id": 1,
      "question": "Factor x² + 5x + 6.",
      "choices": [
        "(x + 2)(x + 3)",
        "(x + 1)(x + 6)",
        "(x − 2)(x − 3)",
        "(x + 3)(x + 3)"
      ],
      "answer": 0,
      "explanation": "2 and 3 add up to 5 and multiply to 6."
    }
  ]
}
```

- Change the text inside quotation marks to edit a question.
- Keep exactly four choices and exactly one mathematically correct choice.
- `answer` is the position of the correct choice, starting at **0**: first = 0, second = 1, third = 2, fourth = 3.
- Add a question by copying one full question object into the `questions` list. Separate question objects with commas and use a new `id`.
- Do not add a comma after the last item. JSON does not allow comments.
- Keep the file saved as UTF-8 so `²`, `−`, `≤`, and `≥` display properly.
- Check both the correct answer and every other choice. An equivalent form can accidentally create a second correct answer.
- The question counter, progress bar, score maximum, and results use the number of questions in the JSON. If you add questions, also update the pyramid's sample “2 questions per level” caption in `Home.razor`.

`LevelData` represents one entire file. `QuestionData` represents one question. Their C# properties match the JSON fields; the standard JSON loader ignores differences in letter case. `Questions` and `Choices` are simple arrays. The game checks basic data shape when loading and displays a retryable message if loading fails. A human still needs to review the mathematics.

### Mathematics review of the included samples

| Level / question | Correct choice | Why it is unique |
| --- | --- | --- |
| 1 / 1 | A: `(x + 2)(x + 3)` | Expands to `x² + 5x + 6`; the other choices have a different middle or constant term. |
| 1 / 2 | C: `(x − 3)(x + 3)` | Expands to `x² − 9`; the other choices have a nonzero middle term or a different constant. |
| 2 / 1 | B: `x = 2 or x = 3` | `(x − 2)(x − 3) = 0`; only this choice contains the complete solution set. |
| 2 / 2 | D: `x = 4 or x = −4` | Both square to 16; the single-root choices are incomplete and ±8 square to 64. |
| 3 / 1 | C: `−3 < x < 3` | The square must be below 9; the endpoints are excluded and outside values fail. |
| 3 / 2 | A: `x ≤ −2 or x ≥ 2` | The square must be at least 4; both outside intervals and both endpoints are required. |

## How to change the design

Edit `wwwroot/css/app.css`. The `:root` block at the top defines reusable colors such as `--cream`, `--ink`, and `--gold`. `.primary` and `.secondary` style the main buttons. `.hero` and `.question-card` control the main layouts. `.level-tier` styles the three clickable levels. `.pyramid-art` and `.art-tier` draw the decorative menu pyramid with CSS.

The media queries at the bottom adjust spacing and stack items on narrow screens. There are no image downloads, external fonts, CSS frameworks, or animation libraries. For screen wording, edit the HTML portions of `Home.razor`.

## Basic code explanation

All game methods live in the `@code` section at the bottom of `Pages/Home.razor`.

| Variable | Purpose |
| --- | --- |
| `currentScreen` | A string: `menu`, `instructions`, `levels`, `question`, or `results`. The matching `@if` block is displayed. |
| `currentLevel` | The selected level number, 1–3. |
| `questionIndex` | The current question's array position, starting at 0. Add 1 for the number shown to the player. |
| `score` | Points in this attempt. |
| `correctAnswers` | Number of correct answers in this attempt. |
| `selectedAnswer` | The selected choice's position; −1 means nothing has been selected. |
| `hasAnswered` | A Boolean (`true` or `false`) that prevents a second answer. |
| `levelData` | The level loaded from JSON. `null` means no data is loaded yet. |
| `isLoading` | Stops duplicate level loads and displays a loading message. |
| `errorMessage` | A friendly message when the data cannot be loaded. |

| Method | Purpose |
| --- | --- |
| `ShowMenu`, `ShowInstructions`, `ShowLevels` | Set the string that chooses the visible screen. |
| `StartLevel` | Loads and checks JSON, resets the attempt, and opens the questions screen. |
| `CheckAnswer` | Ignores repeat answers, compares the choice index with `Answer`, and adds 10 points when correct. |
| `NextQuestion` | Moves forward only after answering. Calls `ShowResults` after the last question. |
| `ShowResults` | Displays results using the current attempt values. |
| `RetryLevel` | Calls `StartLevel` with the same level number. |
| `NextLevel` | Calls `StartLevel` with the current level number plus 1. |
| `GetChoiceClass` | Returns a CSS class for the correct answer or selected wrong answer after answering. |

`Task` means a method can finish later. `async` and `await` allow the browser to keep working while the JSON download completes. `HttpClient` is .NET's built-in way to request a file over HTTP. `GetFromJsonAsync<LevelData>` downloads the file and converts its JSON into a C# object. `@inject HttpClient Http` gives the page the client configured by the template in `Program.cs`; no custom service is needed.

`try/catch/finally` handles loading: `try` attempts the download, `catch` displays a friendly error, and `finally` always clears the loading flag. `@onclick="() => StartLevel(1)"` means “call this method with the number 1 when clicked.” The loop uses a local `choiceIndex` so each answer button remembers its own index.

The standard `App.razor` router opens this one page at `/`. The five game screens are conditional sections inside that page, not five separate routes.

## Verification

Performed in the development environment:

- `dotnet restore QuadraPyramidPrototype`: succeeded.
- `dotnet build QuadraPyramidPrototype --no-restore`: succeeded with 0 warnings and 0 errors.
- `dotnet run --project verification`: **58 checks passed**, calling the actual compiled `Home` component's C# methods, using the real JSON files through a test HTTP handler.
- Checks cover all three levels, JSON-to-model loading, answer indices, menu navigation, scoring, repeated-click protection, unanswered-question protection, correct/wrong highlighting classes, final-question results, zero-score completion, retry resets, next-level loading, Level 3 completion state, missing/malformed data errors, and retry recovery.
- Started the real local development server. HTTP requests returned **200** for `/`, `/css/app.css`, all three `/data/levelN.json` files, `/_framework/blazor.webassembly.js`, and `/_framework/blazor.boot.json`.
- Reviewed all six mathematics problems and explanations as shown in the table above.

Run the optional checks from the outer workspace folder:

```powershell
dotnet run --project verification
```

These checks do not automate a browser. In particular, checking a completion state and message in the source is not proof that it rendered. No browser automation tool was available in this session. The following still need manual verification:

1. Open the app on a desktop browser. Confirm the menu, pyramid illustration, and buttons are readable.
2. Open Instructions, use Back, then Play. Confirm all three pyramid buttons work.
3. Try a correct and an incorrect answer. Confirm feedback and explanation display, answer buttons disable, and Next Question enables only after answering.
4. Finish each level. Confirm displayed scores match your answers and See Results opens the results screen.
5. Try Retry Level, Next Level, Back to Pyramid, and the header menu button.
6. Finish Level 3. Confirm the full congratulations message appears and there is no Next Level button.
7. Resize to a phone width (for example 375 pixels), and check all five screens for overflow, clipping, and readable choices. Test keyboard Tab, Enter, and Space as well.
8. Refresh the page and confirm the app returns to the menu with a new attempt when a level is selected.

## Prototype vs. actual development

This project demonstrates the proposed game concept so the client can give feedback before actual development. The final technology stack and game engine have **not** been selected. Using C# and Blazor here does not commit the final game to these technologies.

Known limits: only six sample questions, all levels unlocked, no saved progress, and Level 3 completion stands for reaching the top even if other levels were skipped. There are no accounts, student records, teacher tools, research data collection, analytics, timers, lives, multiplayer, audio, or Unity integration. The initial Blazor runtime download takes time. This is a local browser prototype, not a packaged mobile or desktop application.

## Possible research defense questions

**Why did you create a prototype?**
To let the client try the proposed screens and learning flow before deciding what to build in the final game.

**Why did you use C#?**
The prototype brief requires C# for gameplay. Blazor WebAssembly lets our C# code run in a browser without a game engine. It does not decide the final development stack.

**What is the purpose of JSON?**
JSON keeps the questions separate from the gameplay. We can change question text, answers, and explanations without rewriting the answer-checking method.

**How does the game check answers?**
The clicked button supplies its index, from 0 to 3. The game compares it with the question's `answer` value in JSON. Equal values mean the answer is correct.

**How does the game calculate scores?**
It adds 10 for each correct answer and adds nothing for an incorrect answer. `hasAnswered` stops a question from awarding points twice. Starting or retrying a level resets the score.

**How does pyramid progression work?**
The proposed order is Factoring, Quadratic Equations, then Quadratic Inequalities. Results has a Next Level button for Levels 1 and 2. All levels are selectable in this demo, and there is no passing-score rule.

**What is the difference between this prototype and the final application?**
This prototype has sample content and temporary state for a demonstration. The final application's features, full question set, progression rules, and technologies will depend on client decisions and further development.

**Does the game collect student data?**
No. It keeps the current attempt in memory only. Refreshing the page clears it.
