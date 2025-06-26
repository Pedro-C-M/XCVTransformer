using XCVTransformer.Transformers.Codificators.Implementations;

namespace XCVTransformer.Tests;

[TestClass]
public class MorseTests
{
    [TestMethod]
    public async Task Encode_ReturnsMorseEncodedString()
    {
        var codificator = new MorseCodificator();
        var input = "SOS";
        var expected = "... --- ...";

        var result = await codificator.Encode(input);

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public async Task Decode_ReturnsOriginalString()
    {
        var codificator = new MorseCodificator();
        var input = "... --- ...";
        var expected = "SOS";

        var result = await codificator.Decode(input);

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void GetName_ReturnsMorse()
    {
        var codificator = new MorseCodificator();
        var result = codificator.GetName();

        Assert.AreEqual("Morse", result);
    }

    [TestMethod]
    public async Task Encode_UnsupportedCharacters()
    {
        var codificator = new MorseCodificator();
        var input = "HELLO!";
        var expected = ".... . .-.. .-.. --- ?";

        var result = await codificator.Encode(input);

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public async Task Decode_UnknownMorseSymbols()
    {
        var codificator = new MorseCodificator();
        var input = "... --- ... --..--";  // --..-- no está en diccionario, debería traducir a '?'
        var expected = "SOS?";

        var result = await codificator.Decode(input);

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public async Task Encode_WithSpaces()
    {
        var codificator = new MorseCodificator();
        var input = "HI THERE";
        var expected = ".... .. / - .... . .-. .";

        var result = await codificator.Encode(input);

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public async Task Decode_WithSlashes()
    {
        var codificator = new MorseCodificator();
        var input = ".... .. / - .... . .-. .";
        var expected = "HI THERE";

        var result = await codificator.Decode(input);

        Assert.AreEqual(expected, result);
    }
}
