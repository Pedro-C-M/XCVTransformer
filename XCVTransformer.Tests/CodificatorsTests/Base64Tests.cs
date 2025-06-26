using XCVTransformer.Transformers.Codificators.Implementations;

namespace XCVTransformer.Tests;

[TestClass]
public class Base64Tests
{

    [TestMethod]
    public async Task Encode_ReturnsBase64EncodedString()
    {
        var codificator = new Base64Codificator();
        var input = "Hola mundo";
        var expected = "SG9sYSBtdW5kbw==";

        var result = await codificator.Encode(input);

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public async Task Decode_ReturnsOriginalString()
    {
        var codificator = new Base64Codificator();
        var input = "SG9sYSBtdW5kbw==";
        var expected = "Hola mundo";

        var result = await codificator.Decode(input);

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void GetName_ReturnsBase64()
    {
        var codificator = new Base64Codificator();

        var result = codificator.GetName();

        Assert.AreEqual("Base64", result);
    }

    [TestMethod]
    public async Task Decode_InvalidChars()
    {
        var codificator = new Base64Codificator();

        await Assert.ThrowsExceptionAsync<FormatException>(async () =>
        {
            await codificator.Decode("####");
        });
    }

    [TestMethod]
    public async Task Decode_InvalidLenght()
    {
        var codificator = new Base64Codificator();

        await Assert.ThrowsExceptionAsync<FormatException>(async () =>
        {
            await codificator.Decode("a");
        });
    }

    [TestMethod]
    public async Task Decode_InvalidFormat()
    {
        var codificator = new Base64Codificator();

        await Assert.ThrowsExceptionAsync<FormatException>(async () =>
        {
            await codificator.Decode("12345===");
        });
    }

}
