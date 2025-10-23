# 🍕 KubePizza Demo 03 - Advanced Validation & Business Logic

> **Demo Step 3**: Comprehensive validation strategies with custom validators, business rule enforcement, and dependency injection integration.

---

## 📖 Overview

This is the third demo in the KubePizza series, showcasing System.CommandLine's advanced validation capabilities. The application demonstrates how to:

- Implement custom option validators with business logic
- Create command-level validators for complex business rules
- Integrate dependency injection for validation services
- Validate data against external catalogs and inventories
- Combine multiple validation strategies for robust input checking
- Provide clear, actionable error messages to users

---

## 🎯 Learning Objectives

After running this demo, you'll understand:

1. **Option Validators**: Creating custom validation logic for individual options
2. **Command Validators**: Implementing cross-option validation rules
3. **Dependency Injection**: Using services within validation logic
4. **Business Logic Integration**: Validating against external data sources
5. **Error Handling**: Providing meaningful feedback for validation failures
6. **Service Architecture**: Building modular, testable validation systems

---

## 🏗️ Validation Architecture

```text
Validation Layers:
├── Option-Level Validators
│   ├── Pizza Type Validation (against catalog)
│   └── Toppings Validation (against available inventory)
├── Command-Level Validators
│   └── Business Rule Validation (size + toppings constraints)
└── Built-in Validators
    ├── AcceptOnlyFromAmong (size validation)
    └── Required (mandatory options)
```

---

## 🚀 Running the Demo

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) or later
- Terminal/Command Prompt

### Build and Run

```bash
# Navigate to the demo folder
cd 03-Validators

# Build the project
dotnet build

# Test valid commands
dotnet run -- order create --pizza "margherita" --size "large" --toppings basil,mozzarella

# Test validation errors
dotnet run -- order create --pizza "invalid" --size "small" --toppings basil,mozzarella,ham,chili
```

---

## 🧪 Validation Examples

### Valid Commands

```bash
# Valid pizza from catalog
dotnet run -- order create --pizza "margherita" --size "medium" --toppings basil,mozzarella

# Valid toppings from inventory
dotnet run -- order create --pizza "diavola" --size "large" --toppings mozzarella,chili

# Small pizza with allowed toppings (≤3)
dotnet run -- order create --pizza "capricciosa" --size "small" --toppings basil,olive,mushrooms
```

### Validation Error Examples

#### 1. Invalid Pizza Type

```bash
dotnet run -- order create --pizza "hawaiian"
# Error: Invalid pizza type 'hawaiian'. Allowed types are: margherita, diavola, capricciosa, quattroformaggi, vegetariana.
```

#### 2. Invalid Toppings

```bash
dotnet run -- order create --pizza "margherita" --toppings pineapple,bacon
# Error: Invalid toppings: pineapple, bacon. Allowed toppings are: basil, mozzarella, bufala, olive, mushrooms, onions, peppers, anchovies, artichokes, ham, salami, chili.
```

#### 3. Business Rule Violation

```bash
dotnet run -- order create --pizza "margherita" --size "small" --toppings basil,mozzarella,olive,mushrooms
# Error: Too many toppings for a small size (max 3).
```

#### 4. Invalid Size

```bash
dotnet run -- order create --pizza "margherita" --size "family"
# Error: Argument 'family' not recognized. Must be one of: 'small', 'medium', 'large'
```

---

## 🔧 Code Architecture

### Project Structure

| Directory/File | Purpose |
|----------------|---------|
| **Program.cs** | DI container setup and application entry point |
| **Commands/** | Command implementations with validation logic |
| **Commands/CommandBase.cs** | Base class with DI integration |
| **KubePizza.Core/Services/** | Business services and data catalogs |

### Dependency Injection Setup

```csharp
var serviceCollection = new ServiceCollection();
serviceCollection.TryAddSingleton<IPizzaCatalog, PizzaCatalog>();
var serviceProvider = serviceCollection.BuildServiceProvider();

var rootCommand = new RootCommand(serviceProvider);
```

### Validation Services

| Service | Purpose |
|---------|---------|
| **IPizzaCatalog** | Provides pizza types and topping inventories |
| **PizzaCatalog** | Implementation with business data and recommendations |

---

## 🎓 Key Validation Concepts

### 1. Option-Level Validation (Pizza Type)

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

### 2. Option-Level Validation (Toppings)

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

### 3. Command-Level Validation (Business Rules)

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

### 4. Built-in Validation

```csharp
// Size validation using built-in validator
sizeOption.AcceptOnlyFromAmong("small", "medium", "large");

// Required option validation
pizzaOption.Required = true;
```

---

## 📊 Pizza Catalog Data

### Available Pizzas

- **margherita**: Classic tomato and mozzarella
- **diavola**: Spicy with salami and chili
- **capricciosa**: Ham, mushrooms, artichokes, and olives
- **quattroformaggi**: Four cheese varieties
- **vegetariana**: Vegetable toppings

### Available Toppings

**In Stock**: basil, mozzarella, olive, mushrooms, onions, salami  
**Full Catalog**: basil, mozzarella, bufala, olive, mushrooms, onions, peppers, anchovies, artichokes, ham, salami, chili

### Business Rules

1. **Pizza Types**: Must be from the approved catalog
2. **Toppings**: Must be from the available inventory
3. **Size Constraints**: Small pizzas limited to 3 toppings maximum
4. **Output Formats**: Must be table, json, or yaml

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

## 🧪 Try It Yourself

### Experiment with Validation

1. **Test Pizza Validation**:

   ```bash
   dotnet run -- order create --pizza "california"
   dotnet run -- order create --pizza "MARGHERITA"  # Case insensitive
   ```

2. **Test Topping Validation**:

   ```bash
   dotnet run -- order create --pizza "margherita" --toppings "pineapple"
   dotnet run -- order create --pizza "margherita" --toppings "BASIL"  # Case insensitive
   ```

3. **Test Business Rules**:

   ```bash
   dotnet run -- order create --pizza "margherita" --size "small" --toppings "basil,mozzarella,olive,mushrooms,ham"
   ```

4. **Test Combined Validation**:

   ```bash
   dotnet run -- order create --pizza "invalid" --size "jumbo" --toppings "pineapple,bacon"
   ```

### Code Modifications to Try

1. **Add New Business Rule**: Large pizzas require at least 2 toppings
2. **Inventory Validation**: Check if toppings are in stock vs. just in catalog
3. **Price Validation**: Add cost calculation and budget limits
4. **Time-based Rules**: Restrict certain pizzas to specific hours
5. **Combo Validation**: Validate pizza + topping combinations

---

## 🔍 Key Differences from Demo 02

| Aspect | Demo 02 (SubCommands) | Demo 03 (Validators) |
|--------|----------------------|---------------------|
| **Validation** | Basic built-in validation only | Custom validators with business logic |
| **Data Sources** | Static/hardcoded values | Dynamic validation against services |
| **Dependency Injection** | No DI container | Full DI integration with services |
| **Error Messages** | Generic System.CommandLine errors | Custom, contextual error messages |
| **Business Logic** | No business rules | Complex business rule validation |
| **Architecture** | Simple command structure | Service-oriented with validation layers |

---

## 📚 What's Next?

This demo covers comprehensive validation strategies. The series continues with:

- **Demo 04**: Tab completion and dynamic completions based on validation data
- **Demo 05**: Custom help formatting with validation information
- **Demo 06**: Complete application with testing, middleware, and production features

---

## 🔗 Resources

- [System.CommandLine Validation](https://learn.microsoft.com/en-us/dotnet/standard/commandline/syntax#validators)
- [Custom Validators](https://learn.microsoft.com/en-us/dotnet/standard/commandline/model-binding#custom-validation)
- [Dependency Injection in Console Apps](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection-usage)

---

## 🐛 Troubleshooting

### Common Issues

1. **Service Not Found**:
   - Ensure services are registered in the DI container
   - Verify service lifetime registration (singleton, transient, scoped)

2. **Validation Not Triggered**:
   - Check that validators are added to the correct option or command
   - Ensure validation logic handles null/empty values properly

3. **Error Messages Not Showing**:
   - Verify `result.AddError()` is called in validation logic
   - Check that validation runs before command execution

### Getting Help

- Use `dotnet run -- --help` for command structure
- Use `dotnet run -- order create --help` for specific command help
- Review validation error messages for specific guidance
- Check the PizzaCatalog service for available data

---

## 🎨 Validation Features

This demo showcases advanced validation capabilities:

- **🔍 Smart Validation**: Context-aware business rule checking
- **📋 Data-Driven**: Validation against real business catalogs
- **🔧 Modular Design**: Layered validation architecture
- **💡 Clear Feedback**: Actionable error messages with suggestions
- **⚡ Performance**: Efficient validation with dependency injection