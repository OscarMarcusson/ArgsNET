using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace ArgsNET
{
	internal class VariableInfo
	{
		readonly Type type;
		readonly Type valueType;
		readonly Action<object> setter;
		readonly IFormatProvider formatProvider;
		readonly NumberStyles numberStyles;
		List<object> list;

		public bool hasSet = false;
		public readonly bool isListType;

		public VariableInfo(object instance, FieldInfo field, IFormatProvider formatProvider, NumberStyles numberStyles)
			: this(field.FieldType, formatProvider, numberStyles)
		{
			setter = (v) => field.SetValue(instance, v);
		}

		public VariableInfo(object instance, PropertyInfo property, IFormatProvider formatProvider, NumberStyles numberStyles)
			: this(property.PropertyType, formatProvider, numberStyles)
		{
			setter = (v) => property.SetValue(instance, v);
		}

		private VariableInfo(Type type, IFormatProvider formatProvider, NumberStyles numberStyles)
		{
			this.type = type;
			isListType = typeof(IList).IsAssignableFrom(type);
			this.formatProvider = formatProvider;
			this.numberStyles = numberStyles;

			if (type.IsArray) valueType = type.GetElementType();
			else if (isListType) throw new NotImplementedException();
			else valueType = type;
		}


		public bool Set(string value, out string error)
		{
			if(type == typeof(bool))
			{
				if(!string.IsNullOrWhiteSpace(value))
				{
					error = "Did not expect a value";
					return false;
				}
				
				SetNewValue(true);
				error = default;
				return true;
			}

			if (type.IsArray)
			{
				if (!TryParse(value, out var parsed, out error))
					return false;

				if (list is null)
					list = new List<object>();

				list.Add(parsed);

				var output = Array.CreateInstance(type.GetElementType(), list.Count);
				for (int i = 0; i < output.Length; i++)
					output.SetValue(list[i], i);

				SetNewValue(output);
				return true;
			}

			//if(isListType && !hasSet)
			//{
			//	hasSet = true;
			//		list = Array.CreateInstance(type.GetElementType(), 1);
			//	else
			//		list = Activator.CreateInstance(type) as IList;
			//}

			else
			{
				if (!TryParse(value, out var parsed, out error))
					return false;

				SetNewValue(parsed);
				return true;
			}
		}

		bool TryParse(string value, out object parsed, out string error)
		{
			error = default;
			
			if(valueType == typeof(string))
			{
				parsed = value;
				return true;
			}
			if (valueType == typeof(char))
			{
				if (value.Length > 1)
				{
					parsed = default;
					error = "Too many characters";
					return false;
				}	
				
				parsed = value[0];
				return true;
			}

			if (valueType == typeof(sbyte)) return ParseHelper(() => sbyte.Parse(value, numberStyles, formatProvider), out parsed, out error);
			if (valueType == typeof(short)) return ParseHelper(() => short.Parse(value, numberStyles, formatProvider), out parsed, out error);
			if (valueType == typeof(int)) return ParseHelper(() => int.Parse(value, numberStyles, formatProvider), out parsed, out error);
			if (valueType == typeof(long)) return ParseHelper(() => long.Parse(value, numberStyles, formatProvider), out parsed, out error);
			if (valueType == typeof(byte)) return ParseHelper(() => byte.Parse(value, numberStyles, formatProvider), out parsed, out error);
			if (valueType == typeof(ushort)) return ParseHelper(() => ushort.Parse(value, numberStyles, formatProvider), out parsed, out error);
			if (valueType == typeof(uint)) return ParseHelper(() => uint.Parse(value, numberStyles, formatProvider), out parsed, out error);
			if (valueType == typeof(ulong)) return ParseHelper(() => ulong.Parse(value, numberStyles, formatProvider), out parsed, out error);
			
			if (valueType == typeof(float)) return ParseHelper(() => float.Parse(value, numberStyles, formatProvider), out parsed, out error);
			if (valueType == typeof(double)) return ParseHelper(() => double.Parse(value, numberStyles, formatProvider), out parsed, out error);
			if (valueType == typeof(decimal)) return ParseHelper(() => decimal.Parse(value, numberStyles, formatProvider), out parsed, out error);

			if (valueType == typeof(DateTime)) return ParseHelper(() => DateTime.Parse(value, formatProvider), out parsed, out error);
			if (valueType == typeof(DateTimeOffset)) return ParseHelper(() => DateTimeOffset.Parse(value, formatProvider), out parsed, out error);
			if (valueType == typeof(TimeSpan)) return ParseHelper(() => TimeSpan.Parse(value, formatProvider), out parsed, out error);

			parsed = default;
			error = $"Invalid type: {valueType.Name}";
			return false;

			
		}

		bool ParseHelper<T>(Func<T> parser, out object parsed, out string error)
		{
			try
			{
				parsed = parser();
				error = default;
				return true;
			}
			catch
			{
				parsed = default;
				error = "Invalid value";
				return false;
			}
		}


		void SetNewValue(object value)
		{
			setter(value);
			hasSet = true;
		}
	}
}
