using XCVTransformer.Transformers.Codificators.Implementations;

namespace XCVTransformer.Tests;

[TestClass]
public class ROT13Tests
{
    [TestMethod]
    public async Task Encode_ReturnsROT13EncodedString()
    {
        var codificator = new ROT13Codification();
        var input = "HelloWorld";
        var expected = "UryybJbeyq";

        var result = await codificator.Encode(input);

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public async Task Decode_ReturnsOriginalString()
    {
        var codificator = new ROT13Codification();
        var input = "UryybJbeyq";
        var expected = "HelloWorld";

        var result = await codificator.Decode(input);

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public async Task EncodeDecode_AreSymmetric()
    {
        var codificator = new ROT13Codification();
        var original = "TheQuickBrownFox123!";
        var encoded = await codificator.Encode(original);
        var decoded = await codificator.Decode(encoded);

        Assert.AreEqual(original, decoded);
    }

    [TestMethod]
    public void GetName_ReturnsROT13()
    {
        var codificator = new ROT13Codification();
        var result = codificator.GetName();

        Assert.AreEqual("ROT13", result);
    }

    [TestMethod]
    public async Task Encode_NonAlphabeticCharactersUnchanged()
    {
        var codificator = new ROT13Codification();
        var input = "1234!@#$%^&*()_+-=[]{}|;':,.<>/?";
        var expected = input;  // No cambia nada

        var result = await codificator.Encode(input);

        Assert.AreEqual(expected, result);
    }


}
