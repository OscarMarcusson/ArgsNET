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
			_ = Arguments.Parse<Empty>(args, out var errors);
			Assert.AreEqual(0, errors.Length, "Did not expect any parse errors");
		}

		[TestMethod]
		[DataRow("-h")]
		[DataRow("-v -h")]
		[DataRow("--verbose-version")]
		[DataRow("--verbose-help")]
		public void Flags_return_error(params string[] args)
		{
			_ = Arguments.Parse<Empty>(args, out var errors);
			Assert.AreEqual(1, errors.Length, "Expected one error");
		}

		[TestMethod]
		[DataRow("-o", "test")]
		[DataRow("-o=test")]
		[DataRow("--verbose-output test")]
		[DataRow("--verbose-output=test")]
		public void Options_return_error(params string[] args)
		{
			_ = Arguments.Parse<Empty>(args, out var errors);
			Assert.AreEqual(1, errors.Length, "Expected one error");
		}

		class Empty { }
	}
}