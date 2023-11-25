using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PrettyArgs
{
	public static class Arguments
	{
		public static T ParseSystemArguments<T>(out string error) where T : class, new()
		{
			var args = Environment.GetCommandLineArgs();
			return Parse<T>(args, out error);
		}

		public static T Parse<T>(string rawArgumentString, out string error) where T : class, new()
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

			return Parse<T>(tokens.ToArray(), out error);
		}

		public static T Parse<T>(string[] arguments, out string error) where T : class, new()
		{
			var t = new T();
			var typeMap = new TypeMap<T>(t);
			var optionArgs = new List<string>();

			for (int i = 0; i < arguments.Length; i++)
			{
				optionArgs.Clear();
				string key = arguments[i];
				string value = null;

				var assignmentIndex = key.IndexOf('=');
				if(assignmentIndex > -1)
				{
					value = key.Substring(assignmentIndex + 1);
					key = key.Substring(0, assignmentIndex);
					Split(optionArgs, value);
				}
				else while(i + 1 < arguments.Length && arguments[i + 1][0] != '-')
				{
					i++;
					value = arguments[i];
					optionArgs.Add(value);
				}

				// Empty values may be set intentionally, for some reason, so special handling for it
				if (value == "\"\"")
				{
					if (!typeMap.Set(t, key, "", out error))
						return t;
				}
				// Options
				else if(optionArgs.Count > 0)
				{
					foreach(var split in optionArgs)
					{
						if (!typeMap.Set(t, key, split, out error))
							return t;
					}
				}
				// Flag
				else
				{
					if (!typeMap.Set(t, key, null, out error))
						return t;
				}
			}

			error = string.Empty;
			return t;
		}


		static void Split(List<string> builder, string value)
		{
			// TODO:: Escape \"
			var previous = 0;
			for(int i = 0; i < value.Length; i++)
			{
				if (value[i] == ',')
				{
					if(previous != i)
					{
						builder.Add(value.Substring(previous, i - previous));
						previous = i + 1;
					}
				}
			}

			if(previous < value.Length - 1)
				builder.Add(value.Substring(previous));
		}
	}
}
