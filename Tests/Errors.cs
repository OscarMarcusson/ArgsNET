using ArgsNET;

namespace Tests
{
	[TestClass]
	public class Errors
	{
		[TestMethod]
		public void Flags_not_found()
		{
			var args = new[] { "-v" };
			_ = Deserialize.Arguments(args).To<Output>(out var errors);
			Assert.IsNotNull(errors, "Expected an error");
			Assert.AreEqual(ArgumentErrorType.NotFound, errors.error);
		}

		[TestMethod]
		public void Options_not_found()
		{
			var args = new[] { "-i", "value" };
			_ = Deserialize.Arguments(args).To<Output>(out var errors);
			Assert.IsNotNull(errors, "Expected an error");
			Assert.AreEqual(ArgumentErrorType.NotFound, errors.error);
		}

		[TestMethod]
		[DataRow(2, "-o", "test-1", "test-2")]
		[DataRow(0, "-o=test1,test2")]
		[DataRow(2, "-o", "test1", "-o", "test2")]
		public void Options_is_not_array(int errorIndex, params string[] args)
		{
			_ = Deserialize.Arguments(args).To<Output>(out var errors);
			Assert.IsNotNull(errors, "Expected an error");
			Assert.AreEqual(ArgumentErrorType.Duplicate, errors.error);
			Assert.AreEqual(errorIndex, errors.index);
			Assert.AreEqual("-o", errors.shortName);
			Assert.AreEqual("--option", errors.longName);
			Assert.AreEqual(typeof(string), errors.type);
			Assert.IsNull(errors.elementType);
		}

		class Output
		{
			public string? option;
			public bool flag;
			public string[]? arrayOption;
		}
	}
}