# GameCore

Minimalistic library designed for game development, providing basic features such as mathematical operations for vectors, logger, test runner, basic database operations, and utility functions to streamline game development.

Made both for fun and as a learning experience.

## Features

- **Vector Math Utilities**
- **NOSQL Database Operations with RAM cache support**
- **Custom Logger**
- **Test Runner**

## Tests

Wrote tests for most important parts of the library to ensure they work as expected. To run tests just `cd tests` from project root, build and run. Tests are run automatically from `Main`.

## Installation

Can be added to a project as a git submodule and then referenced using dotnet, example:

```bash
git submodule add <link to this repo>
git submodule update --init --recursive
cd your/project/path
dotnet add reference ../path/to/gamecore/lib/gamecore.csproj
```

Or can be compiled as a dll and then referenced in the project:

```bash
# Create release .dll
git clone <link to this repo>
cd gamecore/lib
dotnet build -c Release

# Add dll to the project
cd /path/to/the/project
dotnet add package /path/to/compiled/gamecore.dll
```
