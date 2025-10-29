# 🍕 KubePizza Demo 03 - Advanced Validation & Business Logic

> **Series**: Building Command Line Applications with System.CommandLine  
> **Focus**: Custom validators, business rule validation, and dependency injection integration  
> **Level**: Intermediate to Advanced

---

## 📖 Overview

This is the third demo in the KubePizza series, showcasing System.CommandLine's advanced validation capabilities. The application demonstrates how to:

- **Custom Validation**: Implement option and command-level validators with business logic
- **Dependency Injection**: Integrate services for validation and data access
- **Business Rules**: Enforce complex cross-parameter validation constraints
- **Custom Parsers**: Handle sophisticated input parsing (comma-separated values)
- **Error Messaging**: Provide clear, actionable validation feedback
- **Service Architecture**: Build maintainable validation systems with proper separation of concerns

---

## 🎯 Learning Objectives

After exploring this demo, you'll understand:

1. **Option-Level Validators**: Custom validation logic for individual parameters
2. **Command-Level Validators**: Cross-parameter business rule enforcement
3. **Service Integration**: Using dependency injection in validation logic
4. **Custom Parsers**: Implementing sophisticated input parsing with validation
5. **Error Handling**: Creating meaningful, contextual error messages
6. **Architecture Patterns**: Building modular, testable validation systems
7. **Performance**: Efficient validation with dependency injection patterns

---

## 🏗️ Project Structure

```text
03-Validators/
├── Program.cs                    # Entry point with DI container setup
├── Commands/
│   ├── CommandBase.cs           # Base class with DI integration
│   ├── RootCommand.cs           # Root command orchestrator
│   ├── Order/                   # Pizza order management commands
│   │   ├── OrderCommand.cs      # Order command group
│   │   ├── CreateCommand.cs     # Create order with advanced validation
│   │   └── ListCommand.cs       # List orders with status filtering
│   └── Topping/                 # Topping management commands
│       ├── ToppingCommand.cs    # Topping command group
│       ├── AddCommand.cs        # Add new toppings
│       └── ListCommand.cs       # List available toppings
├── Properties/
│   └── launchSettings.json     # Launch configuration
├── 03-Validators.csproj         # Project file with dependencies
├── Program.cs                   # Entry point with DI container setup  
└── README.md                    # This documentation
```

---

## 🎯 Validation Architecture

The application implements a multi-layered validation system:

```text
Validation Layers:
├── 🔍 Option-Level Validators
│   ├── Pizza Type → Validates against business catalog
│   ├── Toppings → Validates individual toppings against inventory
│   └── Custom Parsers → Handles complex input formatting
├── ⚖️ Command-Level Validators  
│   └── Business Rules → Cross-parameter validation (size + toppings)
├── 🛠️ Built-in Validators
│   ├── AcceptOnlyFromAmong → Restricts to predefined values
│   └── Required → Enforces mandatory parameters
└── 🏗️ Service Integration
    └── IPizzaCatalog → External data source validation
```

---

## 🚀 Quick Start

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) or later
- Terminal or Command Prompt

### Build and Run

```bash
# Navigate to the demo directory
cd 03-Validators

# Restore dependencies and build the project
dotnet restore
dotnet build

# Explore available commands
dotnet run -- --help

# Test successful validation
dotnet run -- order create --pizza "margherita" --size "medium" --toppings "basil,mozzarella"

# Test validation errors
dotnet run -- order create --pizza "hawaiian" --size "small" --toppings "basil,mozzarella,olive,mushrooms"
```

---

## 🧪 Validation Examples

### ✅ Valid Commands

```bash
# Basic valid order
dotnet run -- order create --pizza "margherita" --size "medium" --toppings "basil,mozzarella"

# Large pizza with multiple toppings
dotnet run -- order create --pizza "diavola" --size "large" --toppings "mozzarella,salami,chili"

# Small pizza within topping limit
dotnet run -- order create --pizza "capricciosa" --size "small" --toppings "basil,olive,mushrooms"

# Delivery option (boolean flag)
dotnet run -- order create --pizza "margherita" --delivery
dotnet run -- order create --pizza "margherita" --delivery true
dotnet run -- order create --pizza "margherita" --delivery false
```

### ❌ Validation Error Examples

#### 1. Invalid Pizza Type

```bash
dotnet run -- order create --pizza "hawaiian"
# ❌ Error: Invalid pizza type 'hawaiian'. 
#    Allowed types are: margherita, diavola, capricciosa, quattroformaggi, vegetariana.
```

#### 2. Invalid Toppings

```bash
dotnet run -- order create --pizza "margherita" --toppings "pineapple,bacon"
# ❌ Error: Invalid toppings: pineapple, bacon. 
#    Allowed toppings are: basil, mozzarella, bufala, olive, mushrooms, onions, peppers, anchovies, artichokes, ham, salami, chili.
```

#### 3. Business Rule Violation

```bash
dotnet run -- order create --pizza "margherita" --size "small" --toppings "basil,mozzarella,olive,mushrooms"
# ❌ Error: Too many toppings for a small size (max 3).
```

#### 4. Invalid Size Option

```bash
dotnet run -- order create --pizza "margherita" --size "family"
# ❌ Error: Argument 'family' not recognized. Must be one of: 'small', 'medium', 'large'
```

#### 5. Invalid Boolean Value

```bash
dotnet run -- order create --pizza "margherita" --delivery maybe
# ❌ Error: Cannot parse argument 'maybe' for --delivery as expected type 'System.Boolean'.
```

---

## 🔧 Key Implementation Patterns

### 1. Option-Level Validation with Service Integration

**Pizza Type Validation**:

```csharp
pizzaOption.Validators.Add(result =>
{
    var pizzaCatalog = this.serviceProvider.GetRequiredService<IPizzaCatalog>();
    var value = result.GetValueOrDefault<string>();
    if (!pizzaCatalog.Pizzas.Contains(value, StringComparer.OrdinalIgnoreCase))
    {
        result.AddError($"Invalid pizza type '{value}'. Allowed types are: {string.Join(", ", pizzaCatalog.Pizzas)}.");
    }
});
```

**Toppings Validation**:

```csharp
toppingsOption.Validators.Add(result =>
{
    var pizzaCatalog = this.serviceProvider.GetRequiredService<IPizzaCatalog>();
    var values = result.GetValueOrDefault<string[]>() ?? Array.Empty<string>();
    var invalidToppings = values
        .Where(t => !pizzaCatalog.AllToppings.Contains(t, StringComparer.OrdinalIgnoreCase))
        .ToArray();
    if (invalidToppings.Length > 0)
    {
        result.AddError($"Invalid toppings: {string.Join(", ", invalidToppings)}. Allowed toppings are: {string.Join(", ", pizzaCatalog.AllToppings)}.");
    }
});
```

### 2. Command-Level Business Rule Validation

```csharp
this.Validators.Add(result =>
{
    var size = result.GetValue(sizeOption) ?? "medium";
    var toppings = result.GetValue(toppingsOption) ?? Array.Empty<string>();

    // Business rule: small pizzas can't have more than 3 toppings
    if (size.Equals("small", StringComparison.OrdinalIgnoreCase) && toppings.Length > 3)
    {
        result.AddError("Too many toppings for a small size (max 3).");
    }
});
```

### 3. Custom Input Parsers

**Comma-Separated Toppings Parser**:

```csharp
toppingsOption.CustomParser = result =>
{
    var allValues = new List<string>();
    foreach (var token in result.Tokens)
    {
        var splits = token.Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        allValues.AddRange(splits);
    }
    return allValues.ToArray();
};
```

### 4. Built-in Validation Features

```csharp
// Restrict to predefined values
sizeOption.AcceptOnlyFromAmong("small", "medium", "large");

// Require mandatory options
pizzaOption.Required = true;

// Default value factories
sizeOption.DefaultValueFactory = (a) => "medium";
deliveryOption.DefaultValueFactory = _ => true;
```

---

## 🏭 Dependency Injection Setup

### Service Registration

```csharp
// Program.cs - DI Container Configuration
var serviceCollection = new ServiceCollection();
serviceCollection.TryAddSingleton<IPizzaCatalog, PizzaCatalog>();
var serviceProvider = serviceCollection.BuildServiceProvider();

var rootCommand = new RootCommand(serviceProvider);
```

### Command Integration

```csharp
// CommandBase.cs - Base class with DI support
public abstract class CommandBase : Command
{
    protected readonly IServiceProvider serviceProvider;
    protected readonly Option<string> outputOption;

    protected CommandBase(string name, string description, IServiceProvider serviceProvider)
        : base(name, description)
    {
        this.serviceProvider = serviceProvider;
        // Configure shared options and validation
    }
}
```

---

## 📊 Business Data Catalog

### Available Pizzas

- **margherita**: Classic tomato and mozzarella
- **diavola**: Spicy with salami and chili
- **capricciosa**: Ham, mushrooms, artichokes, and olives  
- **quattroformaggi**: Four cheese varieties
- **vegetariana**: Fresh vegetable toppings

### Available Toppings

**Full Catalog**: basil, mozzarella, bufala, olive, mushrooms, onions, peppers, anchovies, artichokes, ham, salami, chili

### Business Validation Rules

1. **Pizza Types**: Must exist in the approved catalog
2. **Toppings**: Must be from the available inventory  
3. **Size Constraints**: Small pizzas limited to 3 toppings maximum
4. **Delivery Options**: Boolean flag with default value (true/false)
5. **Output Formats**: Must be table, json, or yaml

---

## 💡 Advanced Features

### Dependency Injection Integration

Commands receive `IServiceProvider` for accessing validation services:

```csharp
public CommandBase(string name, string description, IServiceProvider serviceProvider)
{
    this.serviceProvider = serviceProvider;
    // Configure options and validators...
}
```

### Smart Error Messages

Validation provides context-aware error messages:

- Lists allowed values for invalid choices
- Explains business rule violations clearly
- Suggests corrections where possible

### Modular Validation Architecture

- **Option validators**: Check individual parameters
- **Command validators**: Enforce business rules across parameters
- **Service validators**: Use external data sources for validation
- **Built-in validators**: Leverage System.CommandLine features

---

## 🧪 Hands-On Experiments

### Test Different Validation Scenarios

1. **Pizza Validation**:

   ```bash
   dotnet run -- order create --pizza "california"     # Invalid
   dotnet run -- order create --pizza "MARGHERITA"    # Valid (case insensitive)
   ```

2. **Topping Validation**:

   ```bash
   dotnet run -- order create --pizza "margherita" --toppings "pineapple"    # Invalid
   dotnet run -- order create --pizza "margherita" --toppings "BASIL"        # Valid (case insensitive)
   ```

3. **Custom Parser Testing**:

   ```bash
   dotnet run -- order create --pizza "margherita" --toppings "basil,mozzarella,olive"          # Comma-separated
   dotnet run -- order create --pizza "margherita" --toppings basil --toppings mozzarella,olive  # Mixed format
   ```

4. **Boolean Option Testing**:

   ```bash
   dotnet run -- order create --pizza "margherita" --delivery        # Valid (flag = true)
   dotnet run -- order create --pizza "margherita" --delivery true   # Valid  
   dotnet run -- order create --pizza "margherita" --delivery false  # Valid
   ```

5. **Business Rule Testing**:

   ```bash
   dotnet run -- order create --pizza "margherita" --size "small" --toppings "basil,mozzarella,olive,mushrooms,ham"  # Too many for small
   ```

### Code Enhancement Ideas

1. **Extended Business Rules**: Add minimum topping requirements for large pizzas
2. **Inventory Checking**: Validate topping availability vs. just catalog membership
3. **Price Validation**: Implement cost calculations with budget constraints
4. **Time-Based Rules**: Restrict special pizzas to specific hours
5. **Combination Validation**: Validate pizza and topping compatibility

---

## 🔍 Evolution from Previous Demos

| Feature | Demo 02 (SubCommands) | Demo 03 (Validators) |
|---------|----------------------|---------------------|
| **Validation** | Basic built-in validation | Custom validators with business logic |
| **Data Sources** | Static/hardcoded values | Dynamic validation with services |
| **Architecture** | Simple command structure | Service-oriented with DI container |
| **Error Messages** | Generic framework errors | Custom, contextual feedback |
| **Business Logic** | No business constraints | Complex cross-parameter rules |
| **Input Parsing** | Default parsing only | Custom parsers for complex formats |
| **Service Integration** | No external services | Full DI with business catalogs |

---

## � Dependencies & Technologies

### Core Packages

- **System.CommandLine** (2.0.0-rc.2.25502.107): Advanced CLI framework
- **Microsoft.Extensions.DependencyInjection** (10.0.0-rc.2.25502.107): DI container
- **KubePizza.Core**: Shared business logic and utilities

### Target Framework

- **.NET 10**: Latest .NET version with enhanced performance

### Key Namespaces

- `System.CommandLine`: Core CLI functionality
- `Microsoft.Extensions.DependencyInjection`: Service registration and resolution
- `KubePizza.Core.Services`: Business services and validation logic
- `KubePizza.Core.Utilities`: Console utilities and UI enhancements

---

## �📚 What's Next in the Series

This demo establishes comprehensive validation foundations. The series continues with:

- **Demo 04 - Tab Completion**: Dynamic completions based on validation data
- **Demo 05 - Custom Help**: Enhanced help formatting with validation context
- **Demo 06 - Complete Solution**: Production-ready application with testing and middleware

---

## 🔗 Resources

- [System.CommandLine Validation](https://learn.microsoft.com/en-us/dotnet/standard/commandline/syntax#validators)
- [Custom Validators](https://learn.microsoft.com/en-us/dotnet/standard/commandline/model-binding#custom-validation)
- [Dependency Injection in Console Apps](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection-usage)

---

## 🐛 Troubleshooting

### Common Validation Issues

1. **Service Not Found Errors**:
   - Verify service registration in DI container
   - Check service lifetime (singleton, transient, scoped)
   - Ensure service interfaces match implementations

2. **Validation Not Executing**:
   - Confirm validators are added to correct option/command
   - Check that validation handles null/empty values gracefully
   - Verify validation order (validators run before custom parsers)

3. **Custom Parser Problems**:
   - Handle empty/null tokens in parser logic
   - Use `result.AddError()` for parser validation errors
   - Test with edge cases (empty strings, special characters)

4. **Error Messages Not Displaying**:
   - Ensure `result.AddError()` is called in validation
   - Verify validation executes before command handler
   - Check error message formatting and clarity

### Getting Detailed Help

```bash
# Application structure
dotnet run -- --help

# Order command options  
dotnet run -- order --help

# Create command specifics
dotnet run -- order create --help

# Topping management
dotnet run -- topping --help
```

---

## 💡 Advanced Validation Features

### Smart Error Context

- **Suggested Corrections**: Error messages include valid alternatives
- **Business Context**: Explains why validation rules exist
- **Progressive Disclosure**: Shows relevant validation information based on input

### Performance Optimizations

- **Lazy Service Resolution**: Services loaded only when needed for validation
- **Cached Validations**: Expensive validations cached during single command execution  
- **Efficient Error Collection**: Multiple validation errors collected and reported together

### Extensibility Patterns

- **Pluggable Validators**: Easy addition of new validation rules
- **Service-Based Validation**: External data sources for dynamic validation
- **Composite Validation**: Combining multiple validation strategies

---

## 🎨 Validation Capabilities Showcase

This demo highlights advanced validation features:

- **🔍 Smart Validation**: Context-aware business rule checking with service integration
- **📋 Data-Driven**: Dynamic validation against real business catalogs and inventories
- **🔧 Modular Architecture**: Layered validation system with clear separation of concerns
- **💡 Clear User Feedback**: Actionable error messages with suggestions and context
- **⚡ High Performance**: Efficient validation with dependency injection patterns
- **🛠️ Custom Parsing**: Flexible input handling with integrated validation logic
- **🏗️ Enterprise Ready**: Scalable architecture suitable for production applications