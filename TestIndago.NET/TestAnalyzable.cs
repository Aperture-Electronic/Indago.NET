using Indago.Analyzable;

namespace TestIndago.NET;

[TestClass]
public class TestAnalyzable
{
    [TestMethod]
    public void TestVerilogNumberLiteralParse()
    {
        var number = new AnalyzableIntegerValue("'shF000_1000");
    }
    
    
}