using PrettyArgs;

namespace Tests
{
	[TestClass]
	public class Empty_output_class
	{
		[TestMethod]
		public void Empty_input_does_nothing()
		{
			var args = Array.Empty<string>();
			_ = Deserialize.Arguments(args).To<Empty>(out var errors);
			Assert.AreEqual("", errors, "Did not expect any parse errors");
		}

		[TestMethod]
		[DataRow("-h")]
		[DataRow("-v -h")]
		[DataRow("--verbose-version")]
		[DataRow("--verbose-help")]
		public void Flags_return_error(params string[] args)
		{
			_ = Deserialize.Arguments(args).To<Empty>(out var errors);
			Assert.IsTrue(errors.Length > 0, "Expected an error");
			Assert.IsTrue(errors.Contains("not found", StringComparison.OrdinalIgnoreCase));
		}

		[TestMethod]
		[DataRow("-o", "test")]
		[DataRow("-o=test")]
		[DataRow("--verbose-output test")]
		[DataRow("--verbose-output=test")]
		public void Options_return_error(params string[] args)
		{
			_ = Deserialize.Arguments(args).To<Empty>(out var errors);
			Assert.IsTrue(errors.Length > 0, "Expected an error");
			Assert.IsTrue(errors.Contains("not found", StringComparison.OrdinalIgnoreCase));
		}

		class Empty { }
	}
}