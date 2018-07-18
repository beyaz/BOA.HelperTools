using System;
using BOAPlugins.PropertyGeneration;
using BOAPlugins.SearchProcedure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BOAPlugins.Test.PropertyGeneration
{
    [TestClass]
    public class PropertyGeneratorTest
    {
        #region Properties
        static Func<string, string, bool> Compare => new SpaceCaseInsensitiveComparator().Compare;
        #endregion

        #region Public Methods
        [TestMethod]
        public void Test_ArrayTypes()
        {
            var expected =
                @"#region byte[] A
byte[] _a;
public byte[] A
{
    get{ return _a; }
    set
	{
		if ( _a != value )
		{
			_a = value;
			OnPropertyChanged(""A"");

        }
    }
}
#endregion";
            var result = new PropertyGenerator().Generate("byte[]", "A");

            Assert.IsTrue(Compare(expected, result));
        }

        [TestMethod]
        public void Test1()
        {
            var expected = @"#region byte A
byte _a;
public byte A
{
    get{ return _a; }
    set
	{
		if ( _a != value )
		{
			_a = value;
			OnPropertyChanged(""A"");

        }
    }
}
#endregion";
            var result = new PropertyGenerator().Generate("byte", "A");

            Assert.IsTrue(Compare(expected, result));
        }
        #endregion
    }
}