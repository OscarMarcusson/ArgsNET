using System;
using System.Collections.Generic;
using System.Text;

namespace PrettyArgs
{
	/// <summary>
	///		A custom name for the argument.
	///		<para>This name should be <b>without</b> the "<c>-</c>" in front of it, those will be added automatically.</para>
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class ArgumentName : Attribute
	{
		public readonly string shortName;
		public readonly string longName;

		public ArgumentName(string longName)
		{
			if (string.IsNullOrWhiteSpace(longName)) throw new ArgumentException(emptyName, nameof(longName));
			if(ContainsIllegalCharacters(longName)) throw new ArgumentException(string.Format(invalidName, longName), nameof(longName));
			if (longName[0] == '-') throw new ArgumentException(hyphenAtStartErrlor, nameof(longName));

			this.longName = longName;
			shortName = NameResolver.ResolveShortNameFromLongName(longName).Substring(1);
			for(int i = 1; i < longName.Length; i++)
			{
				if (longName[i] == '-' && i + 1 < longName.Length) 
				{
					i++;
					shortName += longName[i];
				}
			}
		}

		public ArgumentName(string shortName, string longName)
		{
			if (string.IsNullOrWhiteSpace(shortName)) throw new ArgumentException(emptyName, nameof(shortName));
			if (string.IsNullOrWhiteSpace(longName)) throw new ArgumentException(emptyName, nameof(longName));
			if (ContainsIllegalCharacters(shortName)) throw new ArgumentException(string.Format(invalidName, shortName), nameof(shortName));
			if (ContainsIllegalCharacters(longName)) throw new ArgumentException(string.Format(invalidName, longName), nameof(longName));
			if (shortName[0] == '-') throw new ArgumentException(hyphenAtStartErrlor, nameof(shortName));
			if (longName[0] == '-') throw new ArgumentException(hyphenAtStartErrlor, nameof(longName));

			this.shortName = shortName;
			this.longName = longName;
		}


		private static bool ContainsIllegalCharacters(string name)
		{
			for(int i = 0; i < name.Length; i++)
			{
				if (!char.IsLetterOrDigit(name[i]) && name[i] != '-' && name[i] != '_')
					return true;
			}
			return false;
		}

		const string emptyName = "Empty names are not allowed";
		const string invalidName = "The name \"{0}\" contains invalid characters";
		const string hyphenAtStartErrlor = "Hyphen prefixes (\"-\") should not be included in the name, they will be added automatically";
	}
}
