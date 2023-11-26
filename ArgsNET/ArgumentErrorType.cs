using System;
using System.Collections.Generic;
using System.Text;

namespace ArgsNET
{
	public enum ArgumentErrorType
	{
		/// <summary> There was no error </summary>
		None = 0,

		/// <summary> A normal field or property was given multiple values, which is only supported on <see cref="Array"/> or <see cref="List{T}"/></summary>
		Duplicate,

		/// <summary> A <see cref="bool"/> was given a value </summary>
		FlagReceivedValue,

		/// <summary> An argument option was given a value, but the input string value could not be converted to the correct <see cref="Type"/> </summary>
		InvalidValue,

		/// <summary> An argument was given that did not exist on the output <see langword="class"/> </summary>
		NotFound,

		/// <summary> The field or property was of a <see cref="Type"/> that could not be parsed </summary>
		TypeNotSupported,
	}
}
