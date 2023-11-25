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

		class Empty { }
	}
}