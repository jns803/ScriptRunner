# ScriptRunner PowerToys Run Plugin

ScriptRunner is a powerful and flexible plugin for [PowerToys Run](https://learn.microsoft.com/en-us/windows/powertoys/)
that allows you to quickly search and execute some pre-configured scripts
directly from the PowerToys Run launcher. Whether you're working
with good ol' batch files, PowerShell scripts or even shell scripts (assumes you have bit git-bash installed).
If you want, you could even configure a custom interpreter.
ScriptRunner makes it easy to manage and
execute your scripts with minimal effort.

## Features

- Script Discovery: Automatically loads and filters scripts based on your search query.
- Customizable Configuration: Define script paths, working directories, arguments, and interpreters in a JSON configuration file.
- Dynamic Theme Support: Adapts icon paths based on the current PowerToys theme (light or dark).
- Context Menu Integration: (Optional) Add custom context menu actions for your scripts.
- Error Handling: Provides feedback when configuration files are missing or invalid.
- Cross-Script Support: Supports multiple script types, including:
  - Batch files
  - PowerShell scripts
  - Shell scripts
  - Custom interpreters

## Installation

1. Download the latest release of the from the releases page.
2. Extract the zip file's contents to %LocalAppData%\Microsoft\PowerToys\PowerToys Run\Plugins
3. Restart PowerToys.

## Configuration JSON

1. Create a JSON configuration file to define your scripts. Below is an example configuration:

```json
[
  {
    "Name": "Example Script",
    "ScriptPath": "C:\\Scripts\\example.bat",
    "WorkingDirectory": "C:\\Scripts",
    "Arguments": "--example-arg",
  },
  {
    "Name": "Custom Python Script",
    "ScriptPath": "C:\\Scripts\\script.py",
    "WorkingDirectory": null,
    "Arguments": "",
    "Interpreter": "python"
  }
]
```

1. Open PowerToys Run settings, go to the ScriptRunner section and enter the path to your configuration file.

## Usage

1. Open PowerToys Run (Alt + Space by default).
2. Type your script's name or a keyword to search for it.
3. Select the script from the results to execute it.

## Contributing

Contributions are welcome! Feel free to open issues or submit pull requests to improve the plugin.

## License

This project is licensed under the LICENSE.