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

## Flags

All booleans are understood as flags. A flag does not accept a value, and is instead called just with the argument, like `-v` or `--help`. When this happens, the relevant boolean field or property will be set to true.

Note that flags may only be set once, multiple arguments of the same flag, like `-h -h`, will cause a parse error to be returned, marking the second instance as incorrect.

## Options

All non-booleans are understood as options. These may accept one or multiple values, depending on if they are arrays (any other collection), or if they are a regular value like just a string or int. The regular values may only be set once, just like flags. Arrays and lists, however, may be set as many times as desired. This can be done with any of the following methods:

| Multi-value method | Example                             |
| ------------------ | ----------------------------------- |
| Space separated    | `-i value1 value2 value3`           |
| Comma separated    | `-i=value1,value2,value3`           |
| Multiple arguments | `-i value1` `-i value2` `-i value3` |
