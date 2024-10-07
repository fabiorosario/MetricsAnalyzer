# MetricsAnalyzer

## Overview
MetricsAnalyzer is a tool designed to analyze and compute various software metrics, helping developers better understand and improve their code quality. The project supports different programming languages, focusing on detecting key software metrics that influence code maintainability.

## Requirements
- .NET Core SDK (3.1 or later)
- Roslyn for C# analysis
- Supported Platforms: Windows, Linux, macOS

## Installation
1. Clone the repository:
    ```bash
    git clone https://github.com/fabiorosario/MetricsAnalyzer.git
    ```
2. Build the solution using .NET Core:
    ```bash
    dotnet build
    ```

## Usage
1. Run the MetricsAnalyzer from the command line:
    ```bash
    dotnet run --project MetricsAnalyzer [options]
    ```
2. Options:
    - `--input <path>`: Path to the source code to analyze
    - `--metrics <list>`: List of metrics to calculate

## Contributing
Feel free to fork the repository and submit pull requests for new features or bug fixes.

## License
MIT License.

