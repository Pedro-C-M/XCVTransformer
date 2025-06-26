using ABI.System;
using XCVTransformer.Transformers.Codificators.Implementations;

namespace XCVTransformer.Tests;

[TestClass]
public class HexadecimalTests
{
    [TestMethod]
    public async Task Encode_ReturnsHexEncodedString()
    {
        var codificator = new HexCodificator();
        var input = "Hola";
        var expected = "48 6F 6C 61";

        var result = await codificator.Encode(input);

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public async Task Decode_ReturnsOriginalString()
    {
        var codificator = new HexCodificator();
        var input = "48 6F 6C 61";
        var expected = "Hola";

        var result = await codificator.Decode(input);

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void GetName_ReturnsHexadecimal()
    {
        var codificator = new HexCodificator();
        var result = codificator.GetName();

        Assert.AreEqual("Hexadecimal", result);
    }

    [TestMethod]
    public async Task Decode_InvalidCharacters()
    {
        var codificator = new HexCodificator();
        var invalidInput = "4G 6Z";

        await Assert.ThrowsExceptionAsync<FormatException>(async () =>
        {
            await codificator.Decode(invalidInput);
        });
    }

    [TestMethod]
    public async Task Decode_OddLengthHex()
    {
        var codificator = new HexCodificator();
        var invalidInput = "486 F";

        await Assert.ThrowsExceptionAsync<OverflowException>(async () =>
        {
            await codificator.Decode(invalidInput);
        });
    }

    [TestMethod]
    public async Task Decode_InvalidSeparator()
    {
        var codificator = new HexCodificator();
        var invalidInput = "48-6F-6C";

        await Assert.ThrowsExceptionAsync<FormatException>(async () =>
        {
            await codificator.Decode(invalidInput);
        });
    }

    [TestMethod]
    public async Task Decode_EmptyOrExtraSpaces()
    {
        var codificator = new HexCodificator();
        var invalidInput = "48   6F    6L";

        await Assert.ThrowsExceptionAsync<FormatException>(async () =>
        {
            await codificator.Decode(invalidInput);
        });
    }
}
