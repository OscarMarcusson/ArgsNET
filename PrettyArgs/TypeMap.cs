﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace PrettyArgs
{
	internal class TypeMap<T> where T : class
	{
		readonly Dictionary<string, VariableInfo> variables = new Dictionary<string, VariableInfo>();


		internal TypeMap(T instance, IFormatProvider formatProvider, NumberStyles numberStyles)
		{
			var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.SetField | BindingFlags.Instance);
			var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.Instance);

			foreach(var field in fields)
			{
				var name = field.GetCustomAttribute<ArgumentName>();
				var info = new VariableInfo(instance, field, formatProvider, numberStyles);
				Resolve(field.Name, name, info);
			}
			foreach (var property in properties)
			{
				var name = property.GetCustomAttribute<ArgumentName>();
				var info = new VariableInfo(instance, property, formatProvider, numberStyles);
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
		}


		public bool Set(string key, string value, out string error)
		{
			if(variables.TryGetValue(key, out var variable))
			{
				if(variable.hasSet && !variable.isListType)
				{
					error = "Duplicate";
					return false;
				}
				return variable.Set(value, out error);
			}

			error = "Not found";
			return false;
		}
	}
}
