using PrettyArgs;

namespace Tests
{
	[TestClass]
	public class Fields_output_class
	{
		[TestMethod]
		public void Empty_input_does_nothing()
		{
			var args = Array.Empty<string>();
			var output = Arguments.Parse<Output>(args, out var errors);
			Assert.AreEqual(0, errors.Length, "Did not expect any parse errors");
			Assert.IsNull(output.output);
			Assert.IsFalse(output.version);
		}

		[TestMethod]
		[DataRow("-v")]
		[DataRow("--version")]
		public void Flags_are_set_to_true(params string[] args)
		{
			var output = Arguments.Parse<Output>(args, out var errors);
			Assert.AreEqual(0, errors.Length, "Did not expect any parse errors");
			Assert.IsNull(output.output);
			Assert.IsTrue(output.version);
		}

		[TestMethod]
		[DataRow("-o", "test")]
		[DataRow("-o=test")]
		[DataRow("--output test")]
		[DataRow("--output=test")]
		public void Options_are_set_to_value(params string[] args)
		{
			var output = Arguments.Parse<Output>(args, out var errors);
			Assert.AreEqual(0, errors.Length, "Did not expect any parse errors");
			Assert.AreEqual("test", output.output);
			Assert.IsFalse(output.version);
		}

		class Output
		{
			public string? output;
			public bool version;
		}
	}
}