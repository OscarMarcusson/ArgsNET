using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArgsNET;

namespace Tests
{
	[TestClass]
	public class Long_name_resolver
	{
		[TestMethod]
		[DataRow("example", "--example")]
		[DataRow("snakeCaseName", "--snake-case-name")]
		[DataRow("PascalCaseName", "--pascal-case-name")]
		public void Letters_only(string input, string expected)
		{
			var output = NameResolver.ResolveLongNameFromVariableName(input);
			Assert.AreEqual(expected, output);
		}

		[TestMethod]
		[DataRow("example1", "--example-1")]
		[DataRow("snakeCase123Name", "--snake-case-123-name")]
		[DataRow("PascalCase123Name", "--pascal-case-123-name")]
		public void Letters_and_digits(string input, string expected)
		{
			var output = NameResolver.ResolveLongNameFromVariableName(input);
			Assert.AreEqual(expected, output);
		}

		[TestMethod]
		[DataRow("example-1-", "--example-1")]
		[DataRow("example---1", "--example-1")]
		[DataRow("----example-1-", "--example-1")]
		[DataRow("snake----Case123--Name-", "--snake-case-123-name")]
		[DataRow("Pascal----Case123--Name--", "--pascal-case-123-name")]
		public void Hyphens_are_trimmed_to_one(string input, string expected)
		{
			var output = NameResolver.ResolveLongNameFromVariableName(input);
			Assert.AreEqual(expected, output);
		}
	}
}
