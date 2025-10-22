# 🍕 kubepizza — a playful System.CommandLine demo

> A **kubectl-style pizza orchestrator** built with [.NET System.CommandLine](https://learn.microsoft.com/en-us/dotnet/standard/commandline).  
> Playful, but designed to showcase all the key features of the library — from parsing and validation to tab completion, custom help, and testable command handlers.

---

## 🚀 Overview

**kubepizza** is a demo CLI that simulates managing pizza orders like Kubernetes manages pods.  
It’s built to support a live session or workshop titled:

> _“CLI-first: Modern command-line apps with System.CommandLine”_

Through a series of progressive demos, it illustrates how to create **modern, structured, and testable** command-line tools in .NET.

---

## 🧩 Demo Steps

| Step | Topic | Example Command | Features Highlighted |
|------|--------|-----------------|----------------------|
| **1** | Basic command and parameter parsing | `kubepizza order create --pizza margherita --size large --toppings basil,mozzarella` | Arguments, options, parsing |
| **2** | Commands and subcommands | `kubepizza order list`, `kubepizza topping add` | Nested commands, aliases, global options |
| **3** | Parameter validation | `kubepizza order create --size small --toppings mozzarella,cheddar,parmesan` | Validation rules, error messages |
| **4** | Tab completion | `kubepizza order create --pizza [TAB]` | Dynamic completions, `AddCompletions()` |
| **5** | Custom help | `kubepizza order create --help` | Custom `HelpBuilder`, rich descriptions |
| **6** | Final application | `kubepizza order list --output table` | Middleware, formatting, testable handlers |

---

## 🧠 Learning Goals

- Understand the **core concepts** of System.CommandLine (`RootCommand`, `Option`, `Argument`, `CommandHandler`).
- Learn how to **structure and test** CLI applications in .NET.
- Discover **built-in features**: autocomplete, validation, help generation, and dependency injection.
- Build a **production-grade CLI** using the same library that powers the .NET SDK tools.

---

## ⚙️ Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download) or later  
- A terminal with emoji support (for full flavor 🍕)

### Run the demo

```bash
git clone https://github.com/<your-account>/kubepizza.git
cd kubepizza
dotnet run -- order create --pizza margherita --size large --toppings basil,mozzarella
