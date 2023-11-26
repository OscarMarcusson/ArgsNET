# ArgsNET

 A .NET command line argument parser. It can be used without any configuration, as long as you have a class that can receive the data. For example:

```csharp
var arguments = ArgsNET.Deserialize.SystemArguments().To<Arguments>(out var errors);
```

This assumes we have a class along the lines of this:

```csharp
class Arguments
{
    public bool help;
    public string[] input;
    public string output;
}
```

The above would be able to parse any of the following automatically:

| Variable           | CLI arguments     | Accepts value                                |
| ------------------ | ----------------- | -------------------------------------------- |
| `Arguments.help`   | `-h` / `--help`   | No                                           |
| `Arguments.input`  | `-i` / `--input`  | Yes, like `-i index.html index.js index.css` |
| `Arguments.output` | `-o` / `--output` | Yes, like `-o build/index.html`              |
