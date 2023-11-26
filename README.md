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

## Detailed information

### Flags

All booleans are understood as flags. A flag does not accept a value, and is instead called just with the argument, like `-v` or `--help`. When this happens, the relevant boolean field or property will be set to true.

Note that flags may only be set once, multiple arguments of the same flag, like `-h -h`, will cause a parse error to be returned, marking the second instance as incorrect.

### Options

All non-booleans are understood as options. These may accept one or multiple values, depending on if they are arrays (any other collection), or if they are a regular value like just a string or int. The regular values may only be set once, just like flags. Arrays and lists, however, may be set as many times as desired. This can be done with any of the following methods:

| Multi-value method | Example                             |
| ------------------ | ----------------------------------- |
| Space separated    | `-i value1 value2 value3`           |
| Comma separated    | `-i=value1,value2,value3`           |
| Multiple arguments | `-i value1` `-i value2` `-i value3` |

### Argument names

If nothing else is specified the argument names are resolved by using the field or property name into a long and short version. The long names are resolved first, and will be formatted as this:

| Variable name      | Argument name           |
| ------------------ | ----------------------- |
| example            | --example               |
| FieldExample       | --field-example         |
| PropertyExample    | --property-example      |
| nameWith123Numbers | --name-with-123-numbers |

The short name, meanwhile, is resolved by using the long name above and trimming it to only use the first letter of each section, like this:

| Long name               | Short name |
| ----------------------- | ---------- |
| --example               | -e         |
| --really-verbose-name   | -rvn       |
| --name-with-123-numbers | -nw1n      |

#### Custom argument names

It is possible to define custom names, instead of using the automatic name resolver by using the `ArgumentName` attribute:

```csharp
[ArgumentName("o", "output")]
public string output;

[ArgumentName("r", "hot-reload")]
public bool hotReload;
```

Note that we do not add any hyphens at the start of the name. Those will be added automatically.

It is also possible to only set the long argument, and have the short one be automatically generated:

```csharp
[ArgumentName("output")] // Short = "-o"
public string output;

[ArgumentName("hot-reload")] // Short = "-hr"
public bool hotReload;
```

### Deserializer options

There are three ways to deserialize arguments:

| Method          | Example                                              | Description                                 |
| --------------- | ---------------------------------------------------- | ------------------------------------------- |
| SystemArguments | `Deserialize.SystemArguments()`                      | Uses the arguments given to the application |
| String          | `Deserialize.String("-o example.txt")`               | Uses a raw string                           |
| Arguments       | `Deserialize.Arguments(new []{ "-o", "example.txt")` | Uses a string array                         |

In most cases the regular `SystemArguments` method will suffice, but in some cases you have different handling depending on the first argument, like `npm install` or `git switch`, where the first argument is not a flag. In these cases it's recommended to implement a switch on the first argument, and then use the `Arguments` method with the something like `Deserialize.Arguments(args.Skip(1).ToArray())`.