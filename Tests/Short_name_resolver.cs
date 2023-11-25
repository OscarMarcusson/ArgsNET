using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrettyArgs;

namespace Tests
{
	[TestClass]
	public class Short_name_resolver
	{
		[TestMethod]
		[DataRow("--example", "-e")]
		[DataRow("--snake-case-name", "-scn")]
		[DataRow("--pascal-case-name", "-pcn")]
		public void Letters_only(string input, string expected)
		{
			var output = NameResolver.ResolveShortNameFromLongName(input);
			Assert.AreEqual(expected, output);
		}

		[TestMethod]
		[DataRow("--example-1", "-e1")]
		[DataRow("--snake-case-123-name", "-sc1n")]
		[DataRow("--pascal-case-123-name", "-pc1n")]
		public void Letters_and_digits(string input, string expected)
		{
			var output = NameResolver.ResolveShortNameFromLongName(input);
			Assert.AreEqual(expected, output);
		}
	}
}
