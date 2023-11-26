using System;
using System.Collections.Generic;
using System.Text;

namespace ArgsNET
{
	public class ArgumentError
	{
		/// <summary> The arguments that where parsed </summary>
		public readonly string[] arguments = Array.Empty<string>();

		/// <summary> The index in the <see cref="arguments"/> array that could not be parsed </summary>
		public readonly int index;


		/// <summary> If the given argument existed this will be its short name, like <c>-v</c> </summary>
		public readonly string shortName;

		/// <summary> If the given argument existed this will be its long name, like <c>--version</c> </summary>
		public readonly string longName;

		/// <summary> The type of error that occured </summary>
		public readonly ArgumentErrorType error;

		/// <summary> If the given argument existed this will be its <see cref="Type"/> </summary>
		public readonly Type type;

		/// <summary> If the given argument existed and is of an <see cref="Array"/>, <see cref="List{T}"/> or other <see cref="IEnumerable{T}"/> variety, this will be the element <see cref="Type"/></summary>
		public readonly Type elementType;


		internal ArgumentError(string[] arguments, int index, string shortName, string longName, ArgumentErrorType error, Type type, Type elementType)
		{
			this.arguments = arguments ?? Array.Empty<string>();
			this.index = index;
			this.shortName = shortName;
			this.longName = longName;
			this.error = error;
			this.type = type;
			this.elementType = elementType == type ? null : elementType;
		}
	}
}
