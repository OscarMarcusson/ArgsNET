using System;
using System.Collections.Generic;
using System.Text;

namespace PrettyArgs
{
	public static class Arguments
	{
		public static T ParseSystemArguments<T>(out string[] errors) where T : class, new()
		{
			var args = Environment.GetCommandLineArgs();
			return Parse<T>(args, out errors);
		}

		public static T Parse<T>(string rawArgumentString, out string[] errors) where T : class, new()
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

			return Parse<T>(tokens.ToArray(), out errors);
		}

		public static T Parse<T>(string[] arguments, out string[] errors) where T : class, new()
		{
			var t = new T();
			errors = Array.Empty<string>();
			return t;
		}
	}
}
