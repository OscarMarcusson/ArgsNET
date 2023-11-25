using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace PrettyArgs
{
	public class ArgumentDeserializationContext
	{
		readonly string[] arguments;
		NumberStyles? numberStyles;
		IFormatProvider formatProvider;

		internal ArgumentDeserializationContext(string[] arguments)
		{
			this.arguments = arguments;
		}




		public ArgumentDeserializationContext WithNumberStyles(NumberStyles numberStyles)
		{
			if (this.numberStyles != null)
				throw new NotSupportedException("The number styles has already been set");

			this.numberStyles = numberStyles;
			return this;
		}

		public ArgumentDeserializationContext WithFormatProvider(IFormatProvider formatProvider)
		{
			if (this.formatProvider != null)
				throw new NotSupportedException("The format provider has already been set");

			this.formatProvider = formatProvider;
			return this;
		}

		public T To<T>(out string error) where T : class, new()
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
				if (assignmentIndex > -1)
				{
					value = key.Substring(assignmentIndex + 1);
					key = key.Substring(0, assignmentIndex);
					Split(optionArgs, value);
				}
				else while (i + 1 < arguments.Length && arguments[i + 1][0] != '-')
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
				else if (optionArgs.Count > 0)
				{
					foreach (var split in optionArgs)
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
			for (int i = 0; i < value.Length; i++)
			{
				if (value[i] == ',')
				{
					if (previous != i)
					{
						builder.Add(value.Substring(previous, i - previous));
						previous = i + 1;
					}
				}
			}

			if (previous < value.Length - 1)
				builder.Add(value.Substring(previous));
		}
	}
}
