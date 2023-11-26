using ArgsNET;

namespace Tests
{
	[TestClass]
	public class Args_from_string
	{
		[TestMethod]
		public void Flags()
		{
			var output = Deserialize.String("--flag").To<Output>(out var errors);
			Assert.IsNull(errors, "Did not expect any parse errors");
			Assert.IsTrue(output.flag);
			Assert.IsNull(output.option);
			Assert.IsNull(output.arrayOption);
		}

		[TestMethod]
		public void Option()
		{
			var output = Deserialize.String("--option value").To<Output>(out var errors);
			Assert.IsNull(errors, "Did not expect any parse errors");
			Assert.IsFalse(output.flag);
			Assert.AreEqual("value", output.option);
			Assert.IsNull(output.arrayOption);
		}

		[TestMethod]
		[DataRow("--array-option value-1 value-2 value-3")]
		[DataRow("--array-option=value-1,value-2,value-3")]
		[DataRow("-ao value-1 -ao value-2 -ao value-3")]
		public void Array(string args)
		{
			var output = Deserialize.String(args).To<Output>(out var errors);
			Assert.IsNull(errors, "Did not expect any parse errors");
			Assert.IsFalse(output.flag);
			Assert.IsNull(output.option);
			Assert.IsNotNull(output.arrayOption);
			Assert.AreEqual(3, output.arrayOption.Length);
			Assert.AreEqual("value-1", output.arrayOption[0]);
			Assert.AreEqual("value-2", output.arrayOption[1]);
			Assert.AreEqual("value-3", output.arrayOption[2]);
		}

		class Output
		{
			public string? option;
			public bool flag;
			public string[]? arrayOption;
		}
	}
}