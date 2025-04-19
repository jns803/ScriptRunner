# ScriptRunner PowerToys Run Plugin

ScriptRunner is a powerful and flexible plugin for [PowerToys Run](https://learn.microsoft.com/en-us/windows/powertoys/)
that allows you to quickly search and execute some pre-configured scripts
directly from the PowerToys Run launcher.

Whether you're working with good ol' batch files, PowerShell scripts,
shell scripts  or even a custom interpreter, ScriptRunner got you covered.

## Features

- Script Discovery: Automatically loads and filters scripts based on your search query.
- Customizable JSON Configuration: Define script paths, working directories, arguments, ... in a JSON configuration file.
- Cross-Script Support: Supports multiple script types, including:
  - Batch files
  - PowerShell scripts
  - Shell scripts (assumes git-bash is installed in `"C:\Program Files\Git\git-bash.exe`)
  - Custom interpreters

## Installation

1. Download the latest release of the from the releases page.
2. Extract the zip file's contents to `%LocalAppData%\Microsoft\PowerToys\PowerToys Run\Plugins`
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
    "Interpreter": "python"
  }
]
```

1. Open PowerToys Run settings, go to the ScriptRunner section and enter the path to your configuration file.

## Usage

1. Open PowerToys Run (Alt + Space by default).
2. Type `$` to trigger the ScriptRunner plugin.
3. Type your script's name to search for it.
4. Select the script from the results and it will be executed.

## Contributing

Contributions are welcome! Feel free to open issues or submit pull requests to improve the plugin.
