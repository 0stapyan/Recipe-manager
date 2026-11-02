# Recipe Manager

A command-line recipe and pantry manager written in C# / .NET, built as an exercise in **SOLID principles and design patterns**. No frameworks or external libraries — everything, including the JSON persistence and the event system, is written by hand.

Users log in to their own isolated workspace and manage pantry stock, recipes, meal plans and shopping lists, under the limits of a subscription plan, while earning achievements as they go.

## What it does

- **Pantry** — add, use and list ingredients with quantities and units.
- **Recipes** — create recipes, add ingredients and steps, scale servings, and list what's missing from the pantry.
- **Cooking** — "cook" a recipe, which consumes the required ingredients from the pantry.
- **Planning** — schedule recipes for dates and export a shopping list.
- **Users** — `login <name>` creates or opens an isolated profile; each user's data is saved to disk as JSON.
- **Plans** — a Basic and a Chef plan with different limits on pantry items, recipes and planned entries.
- **Achievements** — unlocked by activity (cook five times, stock 100 kg, use ten unique ingredients) via an event subscriber.

```
Recipe Manager CLI (Enter 'exit' to quit)
Enter command > login alice
success Logged in as alice
Enter command > stock add "Flour" 1000 g
Enter command > recipe add "Pancakes"
Enter command > recipe add-ingredient "Pancakes" "Flour" 300 g
Enter command > action cook "Pancakes"
```

A full example session is in [`demo-session.txt`](demo-session.txt).

## Design — the point of the project

The brief was to practise SOLID and design patterns, so the structure matters more than the feature list. Everything is composed from interfaces and wired together in `Program.cs` (hand-rolled dependency injection).

- **Single responsibility / small classes** — each command (`login`, `stock`, `recipe`, `plan`, …) is its own class implementing `ICommand`; each sub-command (`stock add`, `recipe add-ingredient`, …) implements `ISubCommand`; each recipe action (`cook`, `scale`, `info`, `missing`) implements `IAction`. Parsing is separated from execution.
- **Open/closed** — new commands, sub-commands, actions or achievements are added by writing a class and registering it in a registry, without changing existing code.
- **Dependency inversion** — services depend on interfaces (`IUserStore`, `ILimitsService`, `ICommandParser`, `IEventLogger`), with concrete implementations injected at startup. Storage is behind `IUserStore`, so the JSON file store could be swapped for a database without touching the logic.
- **Patterns used:**
  - **Command** — the command / sub-command hierarchy.
  - **Observer (pub/sub)** — an `EventPublisher` notifies subscribers; one writes a log file, another unlocks achievements, and neither knows about the other.
  - **Adapter** — `PublisherEventLoggerAdapter` presents the publisher as a simple `IEventLogger`.
  - **Decorator** — `AchievementParserDecorator` wraps the command parser to track achievement progress.
  - **Registry** — `CommandRegistry` and `ActionRegistry` map names to handlers.

```
CLI/            command-line parsing (CoreCommandParser)
main/Commands/  one class per command, plus SubCommands/
main/Actions/   recipe actions: cook, scale, info, missing
main/Achievements/  achievements and the event subscriber that unlocks them
Events/         the pub/sub publisher
logs/           file logger, subscriber, and the adapter
persistence/    IUserStore and the JSON file implementation
models/         Recipe, Ingredient, StockItem, PlanItem, …
```

## Build and run

Requires the [.NET 9 SDK](https://dotnet.microsoft.com/download).

```bash
cd repo-root
dotnet run
```

User profiles are created under a `data/` directory at runtime (git-ignored).

## License

[MIT](LICENSE)
