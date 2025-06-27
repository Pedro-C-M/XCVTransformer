using XCVTransformer.Transformers.Codificators.Implementations;

namespace XCVTransformer.Tests;

[TestClass]
public class CharCountTests
{
    private CharCount charCount;

    [TestInitialize]
    public void Setup()
    {
        charCount = new CharCount();
    }

    [TestMethod]
    public async Task Encode_ReturnsCorrectCharacterCount()
    {
        string input = "Hola mundo";
        string result = await charCount.Encode(input);

        Assert.AreEqual("10", result);
    }

    [TestMethod]
    public async Task Decode_ReturnsCorrectCharacterCount()
    {
        string input = "1234567890"; // 10 caracteres
        string result = await charCount.Decode(input);

        Assert.AreEqual("10", result);
    }

    [TestMethod]
    public async Task Encode_EmptyString()
    {
        string result = await charCount.Encode("");
        Assert.AreEqual("0", result);
    }

    [TestMethod]
    public async Task Decode_EmptyString()
    {
        string result = await charCount.Decode("");
        Assert.AreEqual("0", result);
    }

    [TestMethod]
    public void GetName_ReturnsCorrect()
    {
        Assert.AreEqual("Número de caracteres", charCount.GetName());
    }
}
