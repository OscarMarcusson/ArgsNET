using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PrettyArgs
{
	internal class TypeMap
	{
		readonly Dictionary<string, FieldInfo> fields = new Dictionary<string, FieldInfo>();
		readonly Dictionary<string, PropertyInfo> properties = new Dictionary<string, PropertyInfo>();


		public TypeMap(Type type)
		{
			var fields = type.GetFields(BindingFlags.Public | BindingFlags.SetField | BindingFlags.Instance);
			var properties = type.GetProperties(BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.Instance);

			foreach(var field in fields)
			{
				var name = field.GetCustomAttribute<ArgumentName>();
				var shortName = name?.shortName ?? field.Name[0].ToString();
				var longName = name?.longName ?? field.Name;

				this.fields[$"-{shortName}"] = field;
				this.fields[$"--{longName}"] = field;
			}
		}


		public bool Set<T>(T instance, string key, string value, out string error)
		{
			if(fields.TryGetValue(key, out var field))
			{
				if (field.FieldType == typeof(string))
				{
					field.SetValue(instance, value);
					error = null;
					return true;
				}

				error = $"Invalid type: {field.FieldType}";
				return false;
			}

			error = "Not found";
			return false;
		}
	}
}
