# 🍕 KubePizza Demo 04 - Tab Completion & Dynamic Auto-Complete

> **Demo Step 4**: Interactive tab completion with smart suggestions, context-aware completions, and dynamic data-driven auto-complete features.

---

## 📖 Overview

This is the fourth demo in the KubePizza series, showcasing System.CommandLine's powerful tab completion capabilities. The application demonstrates how to:

- Implement dynamic tab completion for command options
- Create context-aware completions based on user input
- Build smart filtering and suggestion systems
- Use business data to drive completion suggestions
- Integrate tab completion with validation for consistent UX
- Provide intelligent recommendations based on pizza selections

---

## 🎯 Learning Objectives

After running this demo, you'll understand:

1. **Dynamic Completions**: Creating data-driven tab completion sources
2. **Context Awareness**: Completions that adapt based on other user inputs
3. **Smart Filtering**: Completion suggestions that filter based on partial input
4. **Business Logic Integration**: Using services to provide intelligent suggestions
5. **User Experience**: How tab completion improves CLI usability
6. **Performance Considerations**: Efficient completion source implementations

---

## 🏗️ Tab Completion Architecture

```text
Completion System:
├── Static Completions
│   ├── Size Options (small, medium, large)
│   └── Output Formats (table, json, yaml)
├── Dynamic Completions
│   ├── Pizza Types (from catalog service)
│   └── Smart Toppings (filtered by pizza selection)
└── Context-Aware Features
    ├── Partial Match Filtering
    ├── Case-Insensitive Matching
    └── Already-Selected Exclusion
```

---

## 🚀 Running the Demo

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) or later
- Terminal/Command Prompt with tab completion support
- **Bash, PowerShell, or Zsh** (required for tab completion features)

### Build and Setup

```bash
# Navigate to the demo folder
cd 04-TabCompletion

# Build the project
dotnet build

# Enable tab completion (choose your shell)
# For Bash:
source <(dotnet run -- [suggest] --registration bash)

# For PowerShell:
dotnet run -- [suggest] --registration powershell | Out-String | Invoke-Expression

# For Zsh:
source <(dotnet run -- [suggest] --registration zsh)
```

---

## 🎮 Interactive Tab Completion Examples

### Basic Tab Completion

```bash
# Try tab completion for pizza types
kubepizza order create --pizza [TAB]
# Shows: margherita, diavola, capricciosa, quattroformaggi, vegetariana

# Partial matching works too
kubepizza order create --pizza mar[TAB]
# Shows: margherita

# Case-insensitive matching
kubepizza order create --pizza DIAR[TAB]
# Shows: diavola
```

### Context-Aware Topping Suggestions

```bash
# Basic toppings (shows all available)
kubepizza order create --pizza margherita --toppings [TAB]
# Shows: basil, mozzarella, olive, mushrooms, onions, salami

# Smart recommendations based on pizza type
kubepizza order create --pizza margherita --toppings [TAB]
# Prioritizes: basil, mozzarella, bufala (recommended for margherita)

# Excludes already selected toppings
kubepizza order create --pizza margherita --toppings basil --toppings [TAB]
# Shows: mozzarella, olive, mushrooms, onions, salami (basil excluded)
```

### Size and Output Completions

```bash
# Size completion
kubepizza order create --pizza margherita --size [TAB]
# Shows: small, medium, large

# Output format completion
kubepizza order create --pizza margherita --output [TAB]
# Shows: table, json, yaml
```

---

## 🔧 Code Architecture

### Tab Completion Sources

#### 1. Static Completion (Size)

```csharp
sizeOption.CompletionSources.Add((context) =>
{
    var sizes = new[] { "small", "medium", "large" };
    return sizes
        .Where(s => s.Contains(context.WordToComplete, StringComparison.OrdinalIgnoreCase))
        .Select(s => new CompletionItem(s));
});
```

#### 2. Dynamic Completion (Pizza Types)

```csharp
pizzaOption.CompletionSources.Add((context) =>
{
    var pizzaCatalog = this.serviceProvider.GetRequiredService<IPizzaCatalog>();
    return pizzaCatalog.Pizzas
        .Where(p => p.Contains(context.WordToComplete, StringComparison.OrdinalIgnoreCase))
        .Select(p => new CompletionItem(p));
});
```

#### 3. Context-Aware Completion (Smart Toppings)

```csharp
toppingsOption.CompletionSources.Add((context) =>
{
    var parse = context.ParseResult;
    var chosenPizza = parse.GetValue(pizzaOption);
    var alreadyTyped = parse.GetValue(toppingsOption) ?? Array.Empty<string>();

    var pizzaCatalog = this.serviceProvider.GetRequiredService<IPizzaCatalog>();

    IEnumerable<string> toppings = string.IsNullOrWhiteSpace(chosenPizza)
           ? pizzaCatalog.AllToppings
           : pizzaCatalog.GetRecommendedToppingsFor(chosenPizza);

    var remaining = toppings.Except(alreadyTyped, StringComparer.OrdinalIgnoreCase);

    return remaining.Select(t => new CompletionItem(t));
});
```

---

## 🎓 Key Completion Concepts

### 1. Completion Sources

Tab completion sources are functions that return completion suggestions:

```csharp
option.CompletionSources.Add((context) =>
{
    // Access current user input
    var wordToComplete = context.WordToComplete;
    
    // Access parse result for context
    var parseResult = context.ParseResult;
    
    // Return completion items
    return suggestions.Select(s => new CompletionItem(s));
});
```

### 2. Context Awareness

Completions can access the current parse state to provide smart suggestions:

```csharp
var chosenPizza = parse.GetValue(pizzaOption);
var alreadyTyped = parse.GetValue(toppingsOption) ?? Array.Empty<string>();
```

### 3. Filtering and Matching

Built-in filtering for partial matches and case-insensitive completion:

```csharp
return items
    .Where(item => item.Contains(context.WordToComplete, StringComparison.OrdinalIgnoreCase))
    .Select(item => new CompletionItem(item));
```

---

## 📊 Smart Completion Features

### Pizza-Specific Toppings

The completion system provides intelligent topping suggestions based on pizza selection:

| Pizza Type | Recommended Toppings |
|------------|---------------------|
| **margherita** | basil, mozzarella, bufala |
| **diavola** | mozzarella, chili, onions |
| **capricciosa** | artichokes, ham, mushrooms, olive |
| **quattroformaggi** | mozzarella, bufala |
| **vegetariana** | mushrooms, peppers, onions, olive |

### Completion Behaviors

- **Partial Matching**: Type "mar" and get "margherita"
- **Case Insensitive**: "DIAR" matches "diavola"
- **Exclusion Logic**: Already selected toppings don't appear again
- **Context Filtering**: Toppings change based on pizza selection
- **Inventory Aware**: Only shows available toppings from stock

---

## 💡 Advanced Features

### Service Integration

Completion sources can access dependency injection services:

```csharp
var pizzaCatalog = this.serviceProvider.GetRequiredService<IPizzaCatalog>();
```

### Smart Filtering

Multiple levels of filtering for relevant suggestions:

1. **Business Logic**: Only show recommended toppings for selected pizza
2. **Availability**: Filter by inventory/stock status
3. **User Input**: Match partial input with case insensitivity
4. **Context**: Exclude already selected options

### Performance Optimization

- **Lazy Evaluation**: Completions calculated only when needed
- **Efficient Filtering**: LINQ-based filtering for performance
- **Service Caching**: DI services provide cached business data

---

## 🧪 Try It Yourself

### Test Tab Completion

1. **Basic Pizza Completion**:

   ```bash
   kubepizza order create --pizza [TAB]
   kubepizza order create --pizza m[TAB]
   kubepizza order create --pizza MAR[TAB]
   ```

2. **Context-Aware Toppings**:

   ```bash
   # Without pizza context
   kubepizza order create --toppings [TAB]
   
   # With pizza context
   kubepizza order create --pizza margherita --toppings [TAB]
   kubepizza order create --pizza diavola --toppings [TAB]
   ```

3. **Exclusion Logic**:

   ```bash
   kubepizza order create --pizza margherita --toppings basil --toppings [TAB]
   # Note: basil should not appear in suggestions
   ```

4. **Multiple Options**:

   ```bash
   kubepizza order create --pizza margherita --size [TAB] --output [TAB]
   ```

### Code Modifications to Try

1. **Add Description to Completions**: Include help text in `CompletionItem`
2. **Priority Sorting**: Order suggestions by popularity or relevance
3. **Fuzzy Matching**: Implement more advanced matching algorithms
4. **Custom Completion**: Add completion for new options like `--quantity`
5. **Rich Completions**: Add icons or categories to completion items

---

## 🔍 Key Differences from Demo 03

| Aspect | Demo 03 (Validators) | Demo 04 (Tab Completion) |
|--------|---------------------|-------------------------|
| **User Experience** | Validation after input | Guidance during input |
| **Interaction Model** | Error-driven feedback | Proactive suggestions |
| **Data Usage** | Validation against catalogs | Completion from catalogs |
| **Performance Impact** | Validation on submission | Real-time completion generation |
| **User Guidance** | Error messages after mistakes | Prevention of mistakes |
| **Development Complexity** | Validation logic | Completion logic + context awareness |

---

## 📚 What's Next?

This demo covers interactive tab completion and user experience enhancements. The series continues with:

- **Demo 05**: Custom help formatting and comprehensive documentation
- **Demo 06**: Complete application with middleware, testing, and production features

---

## 🔗 Resources

- [System.CommandLine Completions](https://learn.microsoft.com/en-us/dotnet/standard/commandline/tab-completion)
- [Tab Completion Registration](https://learn.microsoft.com/en-us/dotnet/standard/commandline/tab-completion#enable-tab-completion)
- [Custom Completion Sources](https://learn.microsoft.com/en-us/dotnet/standard/commandline/tab-completion#custom-completions)

---

## 🐛 Troubleshooting

### Common Issues

1. **Tab Completion Not Working**:
   - Ensure you've registered tab completion for your shell
   - Verify your terminal supports tab completion
   - Check that you're using the correct shell registration command

2. **Completions Not Showing**:
   - Verify completion sources are properly added to options
   - Check that services are registered in DI container
   - Ensure completion logic handles null/empty values

3. **Performance Issues**:
   - Review completion source efficiency
   - Consider caching expensive operations
   - Optimize filtering logic for large datasets

### Shell-Specific Setup

#### Bash

```bash
# Add to ~/.bashrc for permanent setup
echo 'source <(kubepizza [suggest] --registration bash)' >> ~/.bashrc
```

#### PowerShell

```powershell
# Add to PowerShell profile for permanent setup
Add-Content $PROFILE 'kubepizza [suggest] --registration powershell | Out-String | Invoke-Expression'
```

#### Zsh

```bash
# Add to ~/.zshrc for permanent setup
echo 'source <(kubepizza [suggest] --registration zsh)' >> ~/.zshrc
```

### Getting Help

- Use `kubepizza --help` for command structure
- Use `kubepizza [suggest] --help` for tab completion setup help
- Test completion with simple commands first before complex scenarios
- Check shell configuration if tab completion doesn't activate

---

## 🎨 User Experience Features

This demo showcases advanced UX capabilities:

- **🚀 Proactive Guidance**: Suggestions appear before errors occur
- **🧠 Smart Context**: Completions adapt to user's current input state
- **⚡ Real-Time Feedback**: Instant suggestions as you type
- **🔍 Intelligent Filtering**: Relevant suggestions based on business logic
- **🎯 Precision**: Exact matches for partial input with case flexibility