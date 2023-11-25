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
			Assert.AreEqual("", errors, "Did not expect any parse errors");
			Assert.IsNull(output.output);
			Assert.IsFalse(output.version);
		}

		[TestMethod]
		[DataRow("-v")]
		[DataRow("--version")]
		public void Flags_are_set_to_true(params string[] args)
		{
			var output = Arguments.Parse<Output>(args, out var errors);
			Assert.AreEqual("", errors, "Did not expect any parse errors");
			Assert.IsNull(output.output);
			Assert.IsTrue(output.version);
			Assert.IsNull(output.inputFiles);
		}

		[TestMethod]
		[DataRow("-o", "test")]
		[DataRow("-o=test")]
		[DataRow("--output", "test")]
		[DataRow("--output=test")]
		public void Options_are_set_to_value(params string[] args)
		{
			var output = Arguments.Parse<Output>(args, out var errors);
			Assert.AreEqual("", errors, "Did not expect any parse errors");
			Assert.AreEqual("test", output.output);
			Assert.IsFalse(output.version);
			Assert.IsNull(output.inputFiles);
		}

		[TestMethod]
		[DataRow("-i", "test-1", "test-2", "test-3")]                // Space separated list
		[DataRow("-i=test-1,test-2,test-3")]                         // Comma separated list
		[DataRow("-i", "test-1", "-i", "test-2", "-i", "test-3")]    // Multiple arguments
		[DataRow("--input", "test-1", "test-2", "test-3")]                          // Space separated list
		[DataRow("--input=test-1,test-2,test-3")]                                   // Comma separated list
		[DataRow("--input", "test-1", "--input", "test-2", "--input", "test-3")]    // Multiple arguments
		public void Array_options_are_set_to_values(params string[] args)
		{
			var output = Arguments.Parse<Output>(args, out var errors);
			Assert.AreEqual("", errors, "Did not expect any parse errors");
			Assert.IsNull(output.output);
			Assert.IsFalse(output.version);
			Assert.IsNotNull(output.inputFiles);
			Assert.AreEqual(3, output.inputFiles.Length, "Expected 3 array elements");
			Assert.AreEqual("test-1", output.inputFiles[0]);
			Assert.AreEqual("test-2", output.inputFiles[1]);
			Assert.AreEqual("test-3", output.inputFiles[2]);
		}

		class Output
		{
			[ArgumentName("o", "output")]
			public string? output;

			[ArgumentName("v", "version")]
			public bool version;

			[ArgumentName("i", "input")]
			public string[]? inputFiles;
		}
	}
}