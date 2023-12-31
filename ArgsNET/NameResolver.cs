﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("Tests")]
namespace ArgsNET
{
	internal static class NameResolver
	{
		public static string ResolveShortNameFromLongName(string name)
		{
			var builder = new StringBuilder(6);
			builder.Append('-');
			var previous = '-';
			for(int i = 0; i < name.Length; i++)
			{
				if (name[i] == '-')
				{
					previous = '-';
				}
				else
				{
					if (previous == '-')
						builder.Append(name[i]);
					previous = name[i];
				}
			}
			return builder.ToString();
		}

		public static string ResolveLongNameFromVariableName(string name)
		{
			var output = new StringBuilder(name.Length + 6);
			output.Append("--");
			var last = '-';
			for (int i = 0; i < name.Length; i++)
			{
				if (char.IsDigit(last) || char.IsUpper(last))
				{
					output.Append('-');
					last = '-';
				}

				// Catch any `-` or `_` that may already exist for whatever reason, ensure we only get a single `-`
				if (name[i] == '-' || name[i] == '_')
				{
					if (last != '-' && last != '_')
					{
						last = '-';
						output.Append(last);
					}
				}
				// Uppercases are always lowercased and prefixed as `-c`
				else if (char.IsUpper(name[i]))
				{
					ReadWhile(char.IsUpper, ref i);
				}
				// Digits are always inserted as `-c-`
				else if (char.IsDigit(name[i]))
				{
					ReadWhile(char.IsDigit, ref i);
				}
				// Everything else is just printed as is
				else
				{
					last = name[i];
					output.Append(last);
				}
			}

			return output.ToString().TrimEnd('-');


			void ReadWhile(Func<char, bool> condition, ref int i)
			{
				if (last != '-')
					output.Append('-');

				for (; i < name.Length; i++)
				{
					if (!condition(name[i]))
					{
						i--;
						break;
					}
					last = char.ToLower(name[i]);
					output.Append(last);
				}
			}
		}
	}
}
