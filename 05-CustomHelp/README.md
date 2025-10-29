# 🍕 KubePizza Demo 05 - Custom Help & Enhanced Documentation

> **Demo Step 5**: Advanced help formatting with custom help actions, contextual examples, and enhanced user guidance systems.

---

## 📖 Overview

This is the fifth demo in the KubePizza series, showcasing System.CommandLine's custom help capabilities and documentation features. The application demonstrates how to:

- Create custom help actions that extend default help output
- Implement contextual examples for different commands
- Build hierarchical help systems with rich formatting
- Provide command-specific guidance and usage patterns
- Integrate custom help with existing validation and completion systems
- Design user-friendly documentation that scales with command complexity

---

## 🎯 Learning Objectives

After running this demo, you'll understand:

1. **Custom Help Actions**: Extending default help behavior with additional content
2. **Contextual Examples**: Providing command-specific usage examples
3. **Help Architecture**: Building scalable help systems for complex CLIs
4. **User Guidance**: Creating comprehensive documentation within the application
5. **Help Integration**: Combining help with validation and completion features
6. **Professional Documentation**: Designing help that matches modern CLI standards

---

## 🏗️ Custom Help Architecture

```text
Help System:
├── Default System.CommandLine Help
│   ├── Command Descriptions
│   ├── Option Documentation
│   └── Usage Syntax
├── Custom Help Enhancement
│   ├── Contextual Examples
│   ├── Color-Coded Output
│   └── Hierarchical Example Lookup
└── Integration Features
    ├── Validation-Aware Help
    ├── Completion-Compatible
    └── Service-Driven Content
```

---

## 🚀 Running the Demo

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) or later
- Terminal/Command Prompt

### Build and Explore Help

```bash
# Navigate to the demo folder
cd 05-CustomHelp

# Build the project
dotnet build

# Explore the enhanced help system
dotnet run -- --help
dotnet run -- order --help
dotnet run -- order create --help
dotnet run -- order list --help
```

---

## 🎮 Enhanced Help Examples

### Root Command Help

```bash
dotnet run -- --help
```

**Output includes:**

- Default System.CommandLine help (usage, description, options, commands)
- **Enhanced Examples section** with practical command samples:
  - `kubepizza --help`
  - `kubepizza order --help`
  - `kubepizza order create --pizza margherita --size large --toppings basil,mozzarella`
  - `kubepizza order list --status open --output json`

### Order Command Help

```bash
dotnet run -- order --help
```

**Output includes:**

- Standard order command documentation
- **Context-Specific Examples**:
  - `kubepizza order create --pizza diavola --size medium`
  - `kubepizza order list --status delivered`

### Create Command Help

```bash
dotnet run -- order create --help
```

**Output includes:**

- Detailed option descriptions
- **Action-Specific Examples**:
  - `kubepizza order create --pizza margherita --size large --toppings basil,mozzarella`
  - `kubepizza order create --pizza vegetariana --toppings mushrooms,peppers`

### List Command Help

```bash
dotnet run -- order list --help
```

**Output includes:**

- Filtering and output options
- **Query Examples**:
  - `kubepizza order list --status preparing --output yaml`
  - `kubepizza order list --status all --output table`

---

## 🔧 Code Architecture

### Custom Help Components

| Component | Purpose |
|-----------|---------|
| **CustomHelpAction** | Extends default help with additional content |
| **ExamplesProvider** | Manages contextual examples for commands |
| **Constants** | Centralized example definitions |
| **Enhanced RootCommand** | Integrates custom help into command structure |

### Help Integration Pattern

```csharp
// Root command setup with custom help
for (int i = 0; i < this.Options.Count; i++)
{   
    if (this.Options[i] is HelpOption defaultHelpOption)
    {
        defaultHelpOption.Action = new CustomHelpAction((HelpAction)defaultHelpOption.Action!);
        break;
    }
}
```

---

## 🎓 Key Custom Help Concepts

### 1. Custom Help Action Implementation

```csharp
internal class CustomHelpAction : SynchronousCommandLineAction
{
    private readonly HelpAction _defaultHelp;
    private readonly ExamplesProvider _examplesProvider;

    public override int Invoke(ParseResult parseResult)
    { 
        // Show default help first
        int result = _defaultHelp.Invoke(parseResult);

        // Add custom examples section
        var examples = _examplesProvider.GetExamplesFor(parseResult.CommandResult.Command);
        if (examples.Length > 0)
        {
            Console.WriteLine();
            ConsoleUtility.WriteLine("Examples", ConsoleColor.DarkGreen);
            foreach (var e in examples)
                ConsoleUtility.WriteLine($"\t{e}", ConsoleColor.Blue);
        }

        return result;
    }
}
```

### 2. Hierarchical Example Management

```csharp
internal static Dictionary<string, string[]> Examples = new Dictionary<string, string[]>
{
    ["kubepizza"] = new[]
    {
        "kubepizza --help",
        "kubepizza order create --pizza margherita --size large"
    },
    ["kubepizza order create"] = new[]
    {
        "kubepizza order create --pizza margherita --size large --toppings basil,mozzarella",
        "kubepizza order create --pizza vegetariana --toppings mushrooms,peppers"
    }
};
```

### 3. Command Path Resolution

```csharp
public static string GetPath(Command command)
{
    var parts = new List<string>();
    var current = command;
    while (current is not null)
    {
        parts.Add(current.Name);
        current = current.Parents.FirstOrDefault() as Command;
    }
    parts.Reverse();
    return string.Join(' ', parts);
}
```

---

## 💡 Advanced Help Features

### Contextual Examples

Examples are automatically matched to the specific command being queried:

- **Root Command**: General usage and navigation examples
- **Order Command**: Order management workflows
- **Create Command**: Pizza creation examples with different options
- **List Command**: Filtering and output format examples

### Color-Coded Output

The help system uses consistent color coding:

- **Headers**: Dark Green for section titles ("Examples")
- **Examples**: Blue for example commands
- **Descriptions**: Default colors for standard help content

### Hierarchical Organization

Examples are organized by command path:

```text
kubepizza                    → General usage examples
kubepizza order              → Order management examples
kubepizza order create       → Pizza creation examples  
kubepizza order list         → Order listing examples
```

### Integration with Existing Features

The custom help system works seamlessly with:

- **Validation**: Help shows valid options that align with validators
- **Tab Completion**: Examples demonstrate completable values
- **Business Logic**: Examples use real pizza types and toppings from catalog

---

## 🎨 Help Design Principles

### 1. Progressive Disclosure

Help information increases in specificity as users navigate deeper:

- Root: Overview and navigation
- Commands: Action-specific guidance  
- Subcommands: Detailed usage patterns

### 2. Practical Examples

All examples are:

- **Executable**: Can be run immediately
- **Realistic**: Use actual pizza types and valid options
- **Educational**: Show different feature combinations

### 3. Consistent Formatting

- Clean separation between default help and custom content
- Consistent indentation and color scheme
- Professional appearance matching modern CLI standards

---

## 🧪 Try It Yourself

### Explore Help Hierarchy

1. **Start with Root Help**:

   ```bash
   dotnet run -- --help
   ```

2. **Navigate to Command Help**:

   ```bash
   dotnet run -- order --help
   ```

3. **Explore Specific Actions**:

   ```bash
   dotnet run -- order create --help
   dotnet run -- order list --help
   ```

4. **Test Examples**:

   ```bash
   # Copy examples from help output and run them
   dotnet run -- order create --pizza margherita --size large --toppings basil,mozzarella
   dotnet run -- order list --status preparing --output yaml
   ```

### Code Modifications to Try

1. **Add New Examples**: Extend the Examples dictionary with more scenarios
2. **Custom Formatting**: Modify color schemes or add icons to help output
3. **Dynamic Examples**: Generate examples based on available pizza catalog
4. **Help Sections**: Add additional custom sections (tips, warnings, etc.)
5. **Interactive Help**: Create help that responds to current command state

---

## 📊 Help Content Structure

### Example Categories by Command

| Command Path | Example Focus | Count |
|-------------|---------------|--------|
| **Root (`kubepizza`)** | Navigation and overview | 4 examples |
| **Order (`kubepizza order`)** | Order operations | 2 examples |
| **Create (`kubepizza order create`)** | Pizza creation patterns | 2 examples |
| **List (`kubepizza order list`)** | Filtering and output | 2 examples |

### Content Types

- **Navigation Examples**: Help discovery and command exploration
- **Basic Usage**: Simple, common operation patterns
- **Advanced Usage**: Complex scenarios with multiple options
- **Output Formats**: Different ways to format and filter results

---

## 🔍 Key Differences from Demo 04

| Aspect | Demo 04 (Tab Completion) | Demo 05 (Custom Help) |
|--------|-------------------------|----------------------|
| **User Guidance** | Proactive during input | On-demand documentation |
| **Content Source** | Dynamic from services | Static curated examples |
| **Interaction Model** | Real-time suggestions | Explicit help requests |
| **Learning Support** | Prevents errors | Teaches usage patterns |
| **Documentation** | Implicit through completions | Explicit through examples |
| **Maintenance** | Service-driven updates | Manual example curation |

---

## 📚 What's Next?

This demo covers comprehensive help and documentation systems. The series continues with:

- **Demo 06**: Complete production application with middleware, testing, deployment, and advanced features

---

## 🔗 Resources

- [System.CommandLine Help](https://learn.microsoft.com/en-us/dotnet/standard/commandline/syntax#help)
- [Custom Help Builders](https://learn.microsoft.com/en-us/dotnet/standard/commandline/customize-help)
- [CLI Documentation Best Practices](https://clig.dev/#help)

---

## 🐛 Troubleshooting

### Common Issues

1. **Examples Not Appearing**:
   - Verify the CustomHelpAction is properly registered
   - Check that command paths in Examples dictionary match actual command hierarchy
   - Ensure ExamplesProvider is correctly instantiated

2. **Color Output Issues**:
   - Verify terminal supports ANSI colors
   - Check ConsoleUtility.WriteLine implementation
   - Test with different terminal applications

3. **Help Integration Problems**:
   - Ensure HelpOption is being found and replaced correctly
   - Verify the custom action wraps the original action
   - Check for conflicts with other help customizations

### Help Content Maintenance

1. **Keeping Examples Current**: Update examples when adding new options or commands
2. **Validation Alignment**: Ensure examples use valid values that pass validation
3. **Completion Consistency**: Examples should demonstrate completable values
4. **Testing Examples**: Regularly verify all examples execute successfully

### Getting Help

- Use `--help` at any command level for comprehensive documentation
- Examples in help output are executable and demonstrate real usage
- Check command hierarchy with progressive help exploration
- Review Constants.cs for complete example catalog

---

## 🎨 Documentation Features

This demo showcases professional help capabilities:

- **📚 Contextual Documentation**: Examples matched to specific command contexts
- **🎨 Rich Formatting**: Color-coded output with consistent visual hierarchy
- **🔄 Extensible Architecture**: Easy to add new examples and help sections
- **🎯 Practical Guidance**: Executable examples that teach through demonstration
- **⚡ Professional UX**: Help system that scales with application complexity