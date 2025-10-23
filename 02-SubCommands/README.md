# 🍕 KubePizza Demo 02 - SubCommands & Command Hierarchies

> **Demo Step 2**: Advanced CLI structure with subcommands, command hierarchies, and organized code architecture.

---

## 📖 Overview

This is the second demo in the KubePizza series, showcasing how to build complex command-line interfaces using System.CommandLine's subcommand capabilities. The application demonstrates how to:

- Create hierarchical command structures (commands with subcommands)
- Organize commands in separate classes for maintainability  
- Share common options across command families
- Use command aliases for better user experience
- Implement async command handlers with cancellation support
- Add visual feedback with loading indicators

---

## 🎯 Learning Objectives

After running this demo, you'll understand:

1. **Command Hierarchies**: Building nested command structures like `kubepizza order create`
2. **Code Organization**: Separating commands into dedicated classes and namespaces
3. **Command Inheritance**: Using base classes to share common functionality
4. **Global Options**: Options that apply to multiple commands (like `--output`)
5. **Command Aliases**: Creating shortcuts for frequently used commands
6. **Async Handlers**: Implementing async command processing with proper cancellation

---

## 🏗️ Command Structure

```text
kubepizza
├── order (alias: o)
│   ├── create    - Create a new pizza order
│   └── list      - List existing orders
└── topping (alias: t)
    ├── add       - Add a new topping
    └── list      - List available toppings
```

---

## 🚀 Running the Demo

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) or later
- Terminal/Command Prompt

### Build and Run

```bash
# Navigate to the demo folder
cd 02-SubCommands

# Build the project
dotnet build

# Show help for main command
dotnet run -- --help

# Show help for order subcommand
dotnet run -- order --help

# Show help for specific command
dotnet run -- order create --help
```

### Example Commands

#### Order Management

```bash
# Create a new pizza order
dotnet run -- order create --pizza "margherita" --size "large" --toppings basil,mozzarella

# Create order with different output format
dotnet run -- order create --pizza "diavola" --size "medium" --output json

# List all orders
dotnet run -- order list

# List orders with specific status
dotnet run -- order list --status "open" --output table

# Using aliases (order = o)
dotnet run -- o create --pizza "quattro stagioni" --size "large"
dotnet run -- o list --status "delivered"
```

#### Topping Management

```bash
# List available toppings
dotnet run -- topping list

# Add a new topping
dotnet run -- topping add --name "prosciutto"

# Using aliases (topping = t)
dotnet run -- t list
dotnet run -- t add --name "artichokes"
```

#### Global Options

```bash
# All commands support --output option
dotnet run -- order list --output json
dotnet run -- topping list --output yaml
dotnet run -- order create --pizza "margherita" --output table
```

---

## 🔧 Code Architecture

### Project Structure

| Directory/File | Purpose |
|----------------|---------|
| **Program.cs** | Application entry point and root command initialization |
| **Commands/** | Command implementations organized by feature |
| **Commands/CommandBase.cs** | Base class with shared functionality |
| **Commands/RootCommand.cs** | Main application command setup |
| **Commands/Order/** | Order-related commands (create, list) |
| **Commands/Topping/** | Topping-related commands (add, list) |

### Class Hierarchy

```text
CommandBase (abstract)
├── OrderCommand
│   ├── CreateCommand
│   └── ListCommand
└── ToppingCommand
    ├── AddCommand
    └── ListCommand
```

---

## 🎓 Key Concepts Demonstrated

### 1. Command Base Class

```csharp
internal abstract class CommandBase : Command
{
    protected readonly Option<string> outputOption;

    public CommandBase(string name, string description) : base(name, description)
    {
        outputOption = new Option<string>("--output", ["-o"]);
        outputOption.DefaultValueFactory = _ => "table";
        outputOption.AcceptOnlyFromAmong("table", "json", "yaml");
        
        this.Options.Add(outputOption);
    }
}
```

### 2. Subcommand Structure

```csharp
internal class OrderCommand : CommandBase
{
    public OrderCommand() : base("order", "Manage pizza orders")
    {
        this.Aliases.Add("o");
        
        this.Subcommands.Add(new CreateCommand());
        this.Subcommands.Add(new ListCommand());
    }
}
```

### 3. Async Command Handlers

```csharp
private async Task CommandHandler(ParseResult parseResult, CancellationToken cancellationToken)
{
    var pizza = parseResult.GetRequiredValue(pizzaOption);
    
    await Task.Delay(5000, cancellationToken)
        .WithLoadingIndicator(
            message: "Sending order to server...",
            style: LoadingIndicator.Style.Spinner,
            completionMessage: "Order placed successfully!",
            showTimeTaken: true);
            
    // Process the command...
}
```

### 4. Root Command Setup

```csharp
internal class RootCommand : System.CommandLine.RootCommand
{
    public RootCommand() : base("kubepizza — manage your pizza orders like a pro 🍕")
    {
        this.Subcommands.Add(new OrderCommand());
        this.Subcommands.Add(new ToppingCommand());
    }
}
```

---

## 💡 Advanced Features

### Output Options

The `--output` option is available on all commands through inheritance:

- **table**: Human-readable tabular format (default)
- **json**: Machine-readable JSON format  
- **yaml**: YAML format for configuration files

### Command Aliases

- `order` → `o`: Quick access to order commands
- `topping` → `t`: Quick access to topping commands

### Loading Indicators

The create command demonstrates async operations with visual feedback:

- Spinner animation during processing
- Completion message with timing information
- Proper cancellation token handling

### Input Validation

- **Size validation**: Only accepts "small", "medium", "large"
- **Status filtering**: Validates order status values
- **Required options**: Ensures critical parameters are provided

---

## 🧪 Try It Yourself

### Experiment with Commands

1. **Help System**:

   ```bash
   dotnet run -- --help
   dotnet run -- order --help
   dotnet run -- topping add --help
   ```

2. **Command Aliases**:

   ```bash
   dotnet run -- o create --pizza "margherita"
   dotnet run -- t list
   ```

3. **Output Formats**:

   ```bash
   dotnet run -- order list --output json
   dotnet run -- order list --output yaml
   ```

4. **Async Operations**:

   ```bash
   dotnet run -- order create --pizza "diavola"
   # Watch the loading indicator in action!
   ```

### Code Modifications to Try

1. **Add New Subcommand**: Create an `order delete` command
2. **Add Global Option**: Add a `--verbose` option to CommandBase
3. **New Command Group**: Create a `pizza` command group for managing pizza types
4. **Custom Validation**: Add validation for pizza names
5. **Enhanced Output**: Implement actual JSON/YAML formatting

---

## 🔍 Key Differences from Demo 01

| Aspect | Demo 01 (Basic) | Demo 02 (SubCommands) |
|--------|-----------------|----------------------|
| **Structure** | Single command with options | Hierarchical commands with subcommands |
| **Code Organization** | All code in Program.cs | Separated into dedicated classes |
| **Command Scope** | One main action | Multiple actions organized by domain |
| **Reusability** | No code reuse | Base class for shared functionality |
| **User Experience** | Simple flat commands | Rich CLI with help and organization |
| **Async Support** | Synchronous only | Full async/await with cancellation |

---

## 📚 What's Next?

This demo covers command hierarchies and organization. The series continues with:

- **Demo 03**: Advanced validation and custom validators  
- **Demo 04**: Tab completion and enhanced user experience
- **Demo 05**: Custom help formatting and documentation
- **Demo 06**: Complete application with middleware, DI, and testing

---

## 🔗 Resources

- [System.CommandLine Subcommands](https://learn.microsoft.com/en-us/dotnet/standard/commandline/define-commands#subcommands)
- [Command Organization Patterns](https://learn.microsoft.com/en-us/dotnet/standard/commandline/model-binding)
- [Async Command Handlers](https://learn.microsoft.com/en-us/dotnet/standard/commandline/handle-termination)

---

## 🐛 Troubleshooting

### Common Issues

1. **Subcommand Not Found**:
   - Ensure subcommands are properly added to parent commands
   - Check command names and aliases for typos

2. **Options Not Working**:
   - Verify options are added to the correct command class
   - Check option parsing in command handlers

3. **Async Issues**:
   - Ensure command handlers return `Task`
   - Properly handle `CancellationToken` parameters

### Getting Help

- Use `dotnet run -- --help` for main command help
- Use `dotnet run -- [command] --help` for subcommand help  
- Use `dotnet run -- [command] [subcommand] --help` for specific command help
- Review the Commands folder structure for implementation details

---

## 🎨 Visual Features

This demo includes several UX enhancements:

- **🍕 Emoji indicators** for different operations
- **Loading spinners** during async operations  
- **Colored output** for success messages and status
- **Structured help** with clear command hierarchies
- **Progress feedback** with completion timing