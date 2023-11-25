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
		readonly Action<object> setter;
		List<object> list;

		public bool hasSet = false;
		public readonly bool isListType;

		public VariableInfo(object instance, FieldInfo field)
		{
			type = field.FieldType;
			setter = (v) => field.SetValue(instance, v);
			isListType = ResolveIsListType(type);
		}

		public VariableInfo(object instance, PropertyInfo property)
		{
			type = property.PropertyType;
			setter = (v) => property.SetValue(instance, v);
			isListType = ResolveIsListType(type);
		}

		static bool ResolveIsListType(Type type) => typeof(IList).IsAssignableFrom(type);


		public bool Set(string value, out string error)
		{
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
			parsed = value;
			error = default;

			//error = $"Invalid type: {variable.FieldType}";
			//return false;

			return true;
		}

		void SetNewValue(object value)
		{
			setter(value);
			hasSet = true;
		}
	}
}
