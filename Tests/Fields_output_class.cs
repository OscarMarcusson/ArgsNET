using System.Globalization;
using ArgsNET;

namespace Tests
{
	[TestClass]
	public class Fields_output_class
	{
		[TestMethod]
		public void Empty_input_does_nothing()
		{
			var args = Array.Empty<string>();
			var output = Deserialize.Arguments(args).To<Output>(out var errors);
			Assert.AreEqual("", errors, "Did not expect any parse errors");
			Assert.IsNull(output.output);
			Assert.IsFalse(output.version);
			Assert.AreEqual(0, output.age);
			Assert.AreEqual(0m, output.money);
		}

		[TestMethod]
		[DataRow("-v")]
		[DataRow("--version")]
		public void Flags_are_set_to_true(params string[] args)
		{
			var output = Deserialize.Arguments(args).To<Output>(out var errors);
			Assert.AreEqual("", errors, "Did not expect any parse errors");
			Assert.IsNull(output.output);
			Assert.IsTrue(output.version);
			Assert.IsNull(output.inputFiles);
			Assert.AreEqual(0, output.age);
			Assert.AreEqual(0m, output.money);
		}

		[TestMethod]
		[DataRow("-o", "test")]
		[DataRow("-o=test")]
		[DataRow("--output", "test")]
		[DataRow("--output=test")]
		public void Options_strings_are_set(params string[] args)
		{
			var output = Deserialize.Arguments(args).To<Output>(out var errors);
			Assert.AreEqual("", errors, "Did not expect any parse errors");
			Assert.AreEqual("test", output.output);
			Assert.IsFalse(output.version);
			Assert.IsNull(output.inputFiles);
			Assert.AreEqual(0, output.age);
			Assert.AreEqual(0m, output.money);
		}

		[TestMethod]
		public void Options_numbers_are_set()
		{
			var args = new string[]
			{
				"--age", "42",
				"--money", "10.00000001"
			};
			var output = Deserialize.Arguments(args).WithFormatProvider(new NumberFormatInfo { NumberDecimalSeparator = "." }).To<Output>(out var errors);
			Assert.AreEqual("", errors, "Did not expect any parse errors");
			Assert.IsNull(output.output);
			Assert.IsFalse(output.version);
			Assert.IsNull(output.inputFiles);
			Assert.AreEqual(42, output.age);
			Assert.AreEqual(10.00000001m, output.money);
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
			var output = Deserialize.Arguments(args).To<Output>(out var errors);
			Assert.AreEqual("", errors, "Did not expect any parse errors");
			Assert.IsNull(output.output);
			Assert.IsFalse(output.version);
			Assert.IsNotNull(output.inputFiles);
			Assert.AreEqual(3, output.inputFiles.Length, "Expected 3 array elements");
			Assert.AreEqual("test-1", output.inputFiles[0]);
			Assert.AreEqual("test-2", output.inputFiles[1]);
			Assert.AreEqual("test-3", output.inputFiles[2]);
			Assert.AreEqual(0, output.age);
			Assert.AreEqual(0m, output.money);
		}

		class Output
		{
			[ArgumentName("o", "output")]
			public string? output;

			[ArgumentName("v", "version")]
			public bool version;

			[ArgumentName("i", "input")]
			public string[]? inputFiles;

			// automatic "-a" / "--age"
			public int age;

			// automatic "-m" / "--money"
			public decimal money;
		}
	}
}