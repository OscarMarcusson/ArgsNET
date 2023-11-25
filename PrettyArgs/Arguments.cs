using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PrettyArgs
{
	public static class Arguments
	{
		static Dictionary<Type, TypeMap> typeMaps = new Dictionary<Type, TypeMap>();

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
			if(!typeMaps.TryGetValue(typeof(T), out var typeMap))
				typeMaps[typeof(T)] = typeMap = new TypeMap(typeof(T));

			for(int i = 0; i < arguments.Length; i++)
			{
				string key = arguments[i];
				string value = null;

				var assignmentIndex = key.IndexOf('=');
				if(assignmentIndex > -1)
				{
					value = key.Substring(assignmentIndex + 1);
					key = key.Substring(0, assignmentIndex);
				}
				else if(i + 1 < arguments.Length && arguments[i][0] != '"')
				{
					i++;
					value = arguments[i];
				}

				if (!typeMap.Set(t, key, value, out error))
					return t;
			}

			error = string.Empty;
			return t;
		}
	}
}
