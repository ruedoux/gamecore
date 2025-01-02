# GameCore

Minimalistic library designed for game development, providing basic features such as mathematical operations for vectors, logger, test runner, basic database operations, and utility functions to streamline game development.

Made both for fun and as a learning experience.

## Features

- **Vector Math Utilities**
- **NOSQL Database Operations with RAM cache support**
- **Custom Logger**
- **Test Runner**

## Tests

Wrote tests for most important parts of the library to ensure they work as expected. To run tests just `cd tests` from project root, build and run.

## Installation

### Manual reference to .csproj for git project

Add the submodule outside of your .csproj file, example:

```bash
# Add submodule to your project
git submodule add https://github.com/ruedoux/gamecore
git submodule update --init --recursive

# Go to your main project path and reference the submodule
cd /your/project/path
dotnet add reference ../path/to/gamecore/lib/GameCore.csproj
```

### Manual reference to dll

```bash
# Create release .dll
git clone https://github.com/ruedoux/gamecore
cd gamecore/lib
dotnet build -c Release

# Add dll to the project
cd /path/to/the/project
dotnet add package /path/to/compiled/GameCore.dll
```
