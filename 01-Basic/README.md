# 🍕 KubePizza Demo 01 - Basic Command-Line Parsing

> **Demo Step 1**: Introduction to System.CommandLine basics - argument parsing, options, and simple command handlers.

---

## 📖 Overview

This is the first demo in the KubePizza series, showcasing the fundamental concepts of .NET's `System.CommandLine` library. The application demonstrates how to:

- Create a basic command-line interface
- Define and parse command options
- Handle required and optional parameters
- Set default values and validation rules
- Process parsed arguments with a command handler

---

## 🎯 Learning Objectives

After running this demo, you'll understand:

1. **Basic CLI Structure**: How to create a `RootCommand` and add options
2. **Option Types**: Different parameter types (string, boolean, arrays)
3. **Option Configuration**: Required vs optional, default values, aliases
4. **Argument Parsing**: How System.CommandLine processes command-line input
5. **Command Handlers**: Processing parsed arguments and executing business logic

---

## 🚀 Running the Demo

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) or later
- Terminal/Command Prompt

### Build and Run

```bash
# Navigate to the demo folder
cd 01-Basic

# Build the project
dotnet build

# Run with sample commands
dotnet run -- --pizza "margherita" --size "large" --toppings basil,mozzarella --delivery

# Or run without delivery flag
dotnet run -- --pizza "pepperoni" --size "medium" --toppings mushrooms,olives
```

### Example Commands

Try these sample commands to explore different features:

```bash
# Basic order with required pizza type
dotnet run -- --pizza "margherita"

# Order with size and toppings
dotnet run -- --pizza "diavola" --size "large" --toppings "spicy salami,hot peppers"

# Order with delivery option
dotnet run -- --pizza "quattro stagioni" --size "medium" --delivery

# Using size alias (-s)
dotnet run -- --pizza "capricciosa" -s "small" --toppings mushrooms,ham,artichokes

# Multiple toppings (different syntax)
dotnet run -- --pizza "custom" --toppings mushrooms --toppings olives --toppings pepperoni
```

---

## 🔧 Code Structure

### Key Components

| File | Purpose |
|------|---------|
| `Program.cs` | Main application entry point with command definitions |
| `01-Basic.csproj` | Project file with System.CommandLine dependency |
| `Commands.txt` | Sample commands for testing |

### Option Definitions

The demo defines four main options:

1. **`--pizza`** (Required)
   - Type: `string`
   - Purpose: Specifies the pizza type to order
   - Example: `--pizza "margherita"`

2. **`--size`** (Optional)
   - Type: `string`
   - Default: `"medium"`
   - Alias: `-s`
   - Validation: Only accepts "small", "medium", "large"
   - Example: `--size "large"` or `-s "small"`

3. **`--toppings`** (Optional)
   - Type: `string[]`
   - Arity: Zero or more
   - Supports multiple formats: comma-separated or multiple flags
   - Example: `--toppings basil,mozzarella` or `--toppings mushrooms --toppings olives`

4. **`--delivery`** (Optional)
   - Type: `bool`
   - Purpose: Flag for delivery vs pickup
   - Example: `--delivery` (sets to true)

---

## 🎓 Key Concepts Demonstrated

### 1. Option Creation and Configuration

```csharp
var pizzaOption = new Option<string>("--pizza")
{
    Description = "The type of pizza to order (e.g. margherita, diavola).",
    Required = true,
};
```

### 2. Default Values and Aliases

```csharp
var sizeOption = new Option<string>("--size")
{
    Description = "The size of the pizza: small, medium, large.",
};
sizeOption.Aliases.Add("-s");
sizeOption.DefaultValueFactory = (argResult) => "medium";
```

### 3. Input Validation

```csharp
sizeOption.AcceptOnlyFromAmong("small", "medium", "large");
```

### 4. Array Options

```csharp
var toppingsOption = new Option<string[]>("--toppings")
{
    Description = "Comma-separated list of extra toppings.",
    Arity = ArgumentArity.ZeroOrMore
};
toppingsOption.AllowMultipleArgumentsPerToken = true;
```

### 5. Command Handler

```csharp
root.SetAction((ParseResult parseResult) =>
{
    var pizza = parseResult.GetRequiredValue(pizzaOption);
    var size = parseResult.GetValue(sizeOption);
    // Process the order...
});
```

---

## 🧪 Try It Yourself

### Experiment with Edge Cases

1. **Missing Required Parameter**:

   ```bash
   dotnet run -- --size "large"
   # Should show error about missing --pizza
   ```

2. **Invalid Size Value**:

   ```bash
   dotnet run -- --pizza "margherita" --size "extralarge"
   # Should show validation error
   ```

3. **Help Output**:

   ```bash
   dotnet run -- --help
   # Shows automatically generated help
   ```

### Code Modifications to Try

1. Add a new option for `--quantity` with a numeric type
2. Create an option for `--notes` to add special instructions
3. Add validation for pizza names (create a predefined list)
4. Experiment with different default values

---

## 📚 What's Next?

This demo covers the basics, but System.CommandLine offers much more:

- **Demo 02**: Subcommands and command hierarchies
- **Demo 03**: Advanced validation and custom validators  
- **Demo 04**: Tab completion and user experience enhancements
- **Demo 05**: Custom help formatting and documentation
- **Demo 06**: Complete application with middleware and testing

---

## 🔗 Resources

- [System.CommandLine Documentation](https://learn.microsoft.com/en-us/dotnet/standard/commandline/)
- [.NET CLI Guide](https://learn.microsoft.com/en-us/dotnet/core/tools/)
- [Command-Line Best Practices](https://learn.microsoft.com/en-us/dotnet/standard/commandline/syntax)

---

## 🐛 Troubleshooting

### Common Issues

1. **Build Errors**: Ensure you have .NET 10 SDK installed
2. **Package Not Found**: Run `dotnet restore` to restore NuGet packages
3. **Command Not Recognized**: Make sure you're using `--` before your arguments when using `dotnet run`

### Getting Help

- Use `dotnet run -- --help` to see available options
- Check the `Commands.txt` file for working examples
- Review the detailed comments in `Program.cs` for implementation details