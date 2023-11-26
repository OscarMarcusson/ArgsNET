using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArgsNET
{
	public static class Deserialize
	{
		public static ArgumentDeserializationContext SystemArguments()
		{
			var args = Environment.GetCommandLineArgs();
			return Arguments(args.Skip(1).ToArray());
		}

		public static ArgumentDeserializationContext String(string rawArgumentString)
		{
			var tokens = new List<string>();
			var builder = new StringBuilder(rawArgumentString.Length);
			for(int i = 0; i < rawArgumentString.Length; i++)
			{
				switch (rawArgumentString[i])
				{
					case ' ':
					case '\t':
						if (builder.Length == 0)
							continue;

						tokens.Add(builder.ToString());
						builder.Clear();
						break;

					case '"':
						if(builder.Length > 0)
						{
							builder.Append(rawArgumentString[i]);
							continue;
						}
						for (i++; i < rawArgumentString.Length; i++)
						{
							if (rawArgumentString[i] == '"')
							{
								tokens.Add(builder.ToString());
								builder.Clear();
								break;
							}
							builder.Append(rawArgumentString[i]);
						}
						break;

					default:
						builder.Append(rawArgumentString[i]);
						break;
				}
			}

			if(builder.Length > 0)
				tokens.Add(builder.ToString());

			return Arguments(tokens.ToArray());
		}

		public static ArgumentDeserializationContext Arguments(string[] arguments)
		{
			return new ArgumentDeserializationContext(arguments);
		}
	}
}
