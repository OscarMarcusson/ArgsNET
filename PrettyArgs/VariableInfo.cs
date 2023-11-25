using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace PrettyArgs
{
	internal class VariableInfo
	{
		readonly Type type;
		readonly Type valueType;
		readonly Action<object> setter;
		List<object> list;

		public bool hasSet = false;
		public readonly bool isListType;

		public VariableInfo(object instance, FieldInfo field) : this(field.FieldType)
		{
			setter = (v) => field.SetValue(instance, v);
		}

		public VariableInfo(object instance, PropertyInfo property) : this(property.PropertyType)
		{
			setter = (v) => property.SetValue(instance, v);
		}

		private VariableInfo(Type type)
		{
			this.type = type;
			isListType = typeof(IList).IsAssignableFrom(type);

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

			if (valueType == typeof(sbyte)) return ParseHelper(sbyte.Parse, value, out parsed, out error);
			if (valueType == typeof(short)) return ParseHelper(short.Parse, value, out parsed, out error);
			if (valueType == typeof(int)) return ParseHelper(int.Parse, value, out parsed, out error);
			if (valueType == typeof(long)) return ParseHelper(long.Parse, value, out parsed, out error);
			if (valueType == typeof(byte)) return ParseHelper(byte.Parse, value, out parsed, out error);
			if (valueType == typeof(ushort)) return ParseHelper(ushort.Parse, value, out parsed, out error);
			if (valueType == typeof(uint)) return ParseHelper(uint.Parse, value, out parsed, out error);
			if (valueType == typeof(ulong)) return ParseHelper(ulong.Parse, value, out parsed, out error);
			
			if (valueType == typeof(float)) return ParseHelper(float.Parse, value, out parsed, out error);
			if (valueType == typeof(double)) return ParseHelper(double.Parse, value, out parsed, out error);
			if (valueType == typeof(decimal)) return ParseHelper(decimal.Parse, value, out parsed, out error);

			if (valueType == typeof(DateTime)) return ParseHelper(DateTime.Parse, value, out parsed, out error);
			if (valueType == typeof(DateTimeOffset)) return ParseHelper(DateTimeOffset.Parse, value, out parsed, out error);
			if (valueType == typeof(TimeSpan)) return ParseHelper(TimeSpan.Parse, value, out parsed, out error);

			parsed = default;
			error = $"Invalid type: {valueType.Name}";
			return false;

			
		}

		bool ParseHelper<T>(Func<string, T> parser, string value, out object parsed, out string error)
		{
			try
			{
				parsed = parser(value);
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
