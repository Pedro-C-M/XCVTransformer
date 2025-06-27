using XCVTransformer.Transformers.Codificators.Implementations;

namespace XCVTransformer.Tests;

[TestClass]
public class InversorTests
{
    private Inversor inversor;

    [TestInitialize]
    public void Setup()
    {
        inversor = new Inversor();
    }

    [TestMethod]
    public async Task EncodeDecode_WorksCorrectly()
    {
        string original = "Texto de prueba";
        string encoded = await inversor.Encode(original);
        string decoded = await inversor.Decode(encoded);

        Assert.AreEqual(original, decoded);
    }

    [TestMethod]
    public async Task Encode_EmptyInput()
    {
        string result = await inversor.Encode("");
        Assert.AreEqual("", result);
    }

    [TestMethod]
    public async Task Encode_WhitespaceOnly()
    {
        string input = "   ";
        string result = await inversor.Encode(input);
        Assert.AreEqual("   ", result); // los espacios siguen siendo espacios, solo invertidos
    }

    [TestMethod]
    public async Task Encode_SpecialCharacters()
    {
        string input = "!@#123abc";
        string expected = "cba321#@!";
        string result = await inversor.Encode(input);

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void GetName_ReturnsCorrect()
    {
        Assert.AreEqual("Invertir orden", inversor.GetName());
    }
}
