using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace ArgsNET
{
	internal class TypeMap<T> where T : class
	{
		readonly Dictionary<string, VariableInfo> variables = new Dictionary<string, VariableInfo>();
		readonly Dictionary<string, string> longShortMap = new Dictionary<string, string>();

		internal TypeMap(T instance, IFormatProvider formatProvider, NumberStyles numberStyles)
		{
			var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.SetField | BindingFlags.Instance);
			var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.Instance);

			foreach(var field in fields)
			{
				var name = field.GetCustomAttribute<ArgumentName>();
				var info = new VariableInfo((v) => field.SetValue(instance, v), field.FieldType, formatProvider, numberStyles);
				Resolve(field.Name, name, info);
			}
			foreach (var property in properties)
			{
				var name = property.GetCustomAttribute<ArgumentName>();
				var info = new VariableInfo((v) => property.SetValue(instance, v), property.PropertyType, formatProvider, numberStyles);
				Resolve(property.Name, name, info);
			}
		}

		void Resolve(string name, ArgumentName customNames, VariableInfo info)
		{
			var longName = !string.IsNullOrWhiteSpace(customNames?.longName)
				? $"--{customNames.longName}"
				: NameResolver.ResolveLongNameFromVariableName(name)
				;
			var shortName = !string.IsNullOrWhiteSpace(customNames?.shortName)
				? $"-{customNames.shortName}"
				: NameResolver.ResolveShortNameFromLongName(longName)
				;
			variables[shortName] = info;
			variables[longName] = info;
			longShortMap[shortName] = longName;
			longShortMap[longName] = shortName;
		}


		public bool Set(string key, string value, out ArgumentErrorType error)
		{
			if(variables.TryGetValue(key, out var variable))
			{
				return variable.Set(value, out error);
			}

			error = ArgumentErrorType.NotFound;
			return false;
		}

		internal void ResolveInformation(string key, out string shortName, out string longName, out Type type, out Type elementType)
		{
			if(!variables.TryGetValue(key, out var variable))
			{
				shortName = null;
				longName = null;
				type = null;
				elementType = null;
				return;
			}

			if (key.StartsWith("--"))
			{
				longName = key;
				shortName = longShortMap[key];
			}
			else
			{
				longName = longShortMap[key];
				shortName = key;
			}
			type = variable.type;
			elementType = variable.valueType;
		}
	}
}
