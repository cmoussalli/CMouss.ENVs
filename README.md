# CMouss.ENVs

A lightweight, cross-platform .NET library for managing application configuration through file-based environment parameters.

[![NuGet](https://img.shields.io/nuget/v/CMouss.ENVs.svg)](https://www.nuget.org/packages/CMouss.ENVs/)
[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

## Overview

CMouss.ENVs provides a static environment manager that loads application-specific configuration parameters from text files stored in platform-specific directories. It supports a hierarchical configuration approach where environment-specific files can override base values.

## Features

- **Cross-Platform Support** - Works on Windows, Linux, and macOS
- **Hierarchical Configuration** - Base configuration with environment-specific overlays
- **Thread-Safe** - All operations are protected with internal locking
- **Simple File Format** - Key=Value pairs with comment support
- **Zero Dependencies** - Uses only built-in .NET libraries

## Installation

Install via NuGet Package Manager:

```bash
dotnet add package CMouss.ENVs
```

Or via Package Manager Console:

```powershell
Install-Package CMouss.ENVs
```

## Quick Start

### 1. Create Configuration Files

Create your configuration directory and files:

**Windows:** `C:\ENVs\MyApp\Base.txt`
**Linux/macOS:** `/etc/envs/MyApp/Base.txt`

```ini
# Base.txt - Default configuration
DatabaseServer=localhost
ApiEndpoint=https://api.example.com
MaxConnections=10
```

### 2. Initialize and Use

```csharp
using CMouss.ENVs;

// Initialize with your app name
ENVManager.UseEnvironment("MyApp");

// Retrieve values
string dbServer = ENVManager.GetValue("DatabaseServer");

// Safe retrieval (no exception if not found)
if (ENVManager.TryGetValue("ApiKey", out string apiKey))
{
    // Use apiKey
}

// Get all parameters
var allParams = ENVManager.GetAll();
```

## Environment Overlays

You can create environment-specific configuration files that override base values:

**Base.txt:**
```ini
DatabaseServer=localhost
LogLevel=Debug
ApiEndpoint=https://api.example.com
```

**Production.txt:**
```ini
DatabaseServer=prod-db.example.com
LogLevel=Warning
```

**Usage:**
```csharp
// Load Base.txt, then overlay with Production.txt
ENVManager.UseEnvironment("MyApp", "Production");

// Returns "prod-db.example.com" (overridden)
string dbServer = ENVManager.GetValue("DatabaseServer");

// Returns "https://api.example.com" (from Base.txt)
string apiEndpoint = ENVManager.GetValue("ApiEndpoint");
```

## Configuration File Format

- One parameter per line in `Key=Value` format
- Lines starting with `#` are treated as comments
- Empty lines are ignored
- Leading and trailing whitespace is trimmed from keys and values

```ini
# This is a comment
DatabaseServer=localhost
ApiKey=your-secret-key

# Another comment
MaxRetries=3
```

## API Reference

### ENVManager (Static Class)

| Method | Description |
|--------|-------------|
| `UseEnvironment(appName)` | Initialize with base configuration only |
| `UseEnvironment(appName, envName)` | Initialize with base + environment overlay |
| `GetValue(paramName)` | Get parameter value (throws `KeyNotFoundException` if not found) |
| `TryGetValue(paramName, out value)` | Safe retrieval returning boolean |
| `GetAll()` | Returns read-only dictionary of all parameters |
| `Reset()` | Clear state for reinitialization |
| `IsInitialized` | Property indicating initialization status |
| `GetBaseDirectory()` | Returns platform-specific base directory path |

## Directory Structure

```
Windows:  C:\ENVs\
Linux:    /etc/envs/
macOS:    /etc/envs/

[BaseDirectory]
└── MyApp/
    ├── Base.txt          (required)
    ├── Development.txt   (optional)
    ├── Staging.txt       (optional)
    └── Production.txt    (optional)
```

## Error Handling

| Exception | Condition |
|-----------|-----------|
| `InvalidOperationException` | Not initialized, or reinitialized without `Reset()` |
| `ArgumentException` | Null or empty parameter names |
| `DirectoryNotFoundException` | Application directory doesn't exist |
| `FileNotFoundException` | Required `Base.txt` file doesn't exist |
| `KeyNotFoundException` | Parameter not found (from `GetValue`) |

## Envs Manager (GUI Tool)

The solution includes a Windows Forms application for managing environment configurations visually. It provides:

- Browse available applications
- View and manage configuration files
- Create new application directories

## Requirements

- .NET 9.0 or later
- Write access to the platform-specific configuration directory

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Author

Caesar Moussalli (CMouss)
