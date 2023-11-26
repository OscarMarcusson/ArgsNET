using System.Globalization;
using ArgsNET;

namespace Tests
{
	[TestClass]
	public class Properties_output_class
	{
		[TestMethod]
		public void Empty_input_does_nothing()
		{
			var args = Array.Empty<string>();
			var output = Deserialize.Arguments(args).To<Output>(out var errors);
			Assert.AreEqual("", errors, "Did not expect any parse errors");
			Assert.IsNull(output.OutputFile);
			Assert.IsFalse(output.Version);
			Assert.AreEqual(0, output.Age);
			Assert.AreEqual(0m, output.Money);
		}

		[TestMethod]
		[DataRow("-v")]
		[DataRow("--version")]
		public void Flags_are_set_to_true(params string[] args)
		{
			var output = Deserialize.Arguments(args).To<Output>(out var errors);
			Assert.AreEqual("", errors, "Did not expect any parse errors");
			Assert.IsNull(output.OutputFile);
			Assert.IsTrue(output.Version);
			Assert.IsNull(output.InputFiles);
			Assert.AreEqual(0, output.Age);
			Assert.AreEqual(0m, output.Money);
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
			Assert.AreEqual("test", output.OutputFile);
			Assert.IsFalse(output.Version);
			Assert.IsNull(output.InputFiles);
			Assert.AreEqual(0, output.Age);
			Assert.AreEqual(0m, output.Money);
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
			Assert.IsNull(output.OutputFile);
			Assert.IsFalse(output.Version);
			Assert.IsNull(output.InputFiles);
			Assert.AreEqual(42, output.Age);
			Assert.AreEqual(10.00000001m, output.Money);
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
			Assert.IsNull(output.OutputFile);
			Assert.IsFalse(output.Version);
			Assert.IsNotNull(output.InputFiles);
			Assert.AreEqual(3, output.InputFiles.Length, "Expected 3 array elements");
			Assert.AreEqual("test-1", output.InputFiles[0]);
			Assert.AreEqual("test-2", output.InputFiles[1]);
			Assert.AreEqual("test-3", output.InputFiles[2]);
			Assert.AreEqual(0, output.Age);
			Assert.AreEqual(0m, output.Money);
		}

		class Output
		{
			[ArgumentName("o", "output")]
			public string? OutputFile { get; set; }

			[ArgumentName("v", "version")]
			public bool Version { get; set; }

			[ArgumentName("i", "input")]
			public string[]? InputFiles { get; set; }

			// automatic "-a" / "--age"
			public int Age { get; set; }

			// automatic "-m" / "--money"
			public decimal Money { get; set; }
		}
	}
}