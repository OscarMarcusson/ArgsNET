using System;
using System.Collections.Generic;
using System.Globalization;

namespace ArgsNET
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

		public ArgumentDeserializationContext WithCultureInfo(string culture)
		{
			return WithFormatProvider(CultureInfo.GetCultureInfo(culture));
		}
		public ArgumentDeserializationContext WithFormatProvider(IFormatProvider formatProvider)
		{
			if (this.formatProvider != null)
				throw new NotSupportedException("The format provider has already been set");

			this.formatProvider = formatProvider;
			return this;
		}

		public T To<T>(out ArgumentError error) where T : class, new()
		{
			var t = new T();
			var typeMap = new TypeMap<T>(
				t,
				formatProvider ?? CultureInfo.InstalledUICulture,
				numberStyles ?? NumberStyles.Any
				);
			var optionArgs = new List<string>();

			for (int i = 0; i < arguments.Length; i++)
			{
				optionArgs.Clear();
				var keyIndex = i;
				string key = arguments[keyIndex];
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
					if (!TrySet(typeMap, key, "", i, keyIndex, false, out error))
						return t;
				}
				// Options
				else if (optionArgs.Count > 0)
				{
					foreach (var split in optionArgs)
					{
						if (!TrySet(typeMap, key, split, i, keyIndex, optionArgs.Count > 1, out error))
							return t;
					}
				}
				// Flag
				else
				{
					if (!TrySet(typeMap, key, null, i, keyIndex, false, out error))
						return t;
				}
			}

			error = default;
			return t;
		}

		// Setter wrapper
		bool TrySet<T>(TypeMap<T> typeMap, string key, string value, int index, int keyIndex, bool isMultipleArgs, out ArgumentError error) where T : class
		{
			if (!typeMap.Set(key, value, out var errorType))
			{
				var errorIndex = errorType == ArgumentErrorType.Duplicate && !isMultipleArgs ? keyIndex : index;
				typeMap.ResolveInformation(key, out var shortName, out var longName, out var type, out var elementType);
				error = new ArgumentError(arguments, errorIndex, shortName, longName, errorType, type, elementType);
				return false;
			}
			else
			{
				error = default;
				return true;
			}
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
