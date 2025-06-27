using XCVTransformer.Transformers.Codificators.Implementations;

namespace XCVTransformer.Tests;

[TestClass]
public class EnigmaTests
{
    private EnigmaCodificator enigma;

    [TestInitialize]
    public void Setup()
    {
        enigma = new EnigmaCodificator();
    }

    [TestMethod]
    public async Task EncodeDecode_WorksCorrectly()
    {
        string original = "HELLO WORLD";
        string encrypted = await enigma.Encode(original);
        string decrypted = await enigma.Decode(encrypted);

        Assert.AreEqual(original.ToUpper(), decrypted); // el codificador pasa todo a mayúsculas
    }

    [TestMethod]
    public async Task Encode_IgnoresNonAlphabetic()
    {
        string original = "123! HOLA.";
        var encoded = await enigma.Encode(original);

        Assert.AreEqual("123! QYTE.", encoded);
    }

    [TestMethod]
    public async Task Decode_IgnoresNonAlphabetic()
    {
        string original = "123! HOLA.";
        var decoded = await enigma.Decode(original);

        Assert.AreEqual("123! PMEU.", decoded);
    }

    [TestMethod]
    public async Task Encode_EmptyOrNull()
    {
        Assert.AreEqual("", await enigma.Encode(""));
        Assert.AreEqual("", await enigma.Encode(null));
    }

    [TestMethod]
    public async Task Decode_EmptyOrNull()
    {
        Assert.AreEqual("", await enigma.Decode(""));
        Assert.AreEqual("", await enigma.Decode(null));
    }

    [TestMethod]
    public void GetName_ReturnsCorrect()
    {
        Assert.AreEqual("Encriptación Enigma", enigma.GetName());
    }

    [TestMethod]
    public async Task EncodeDecode_WithLowercase()
    {
        string original = "abcxyz";
        string encrypted = await enigma.Encode(original);
        string decrypted = await enigma.Decode(encrypted);

        Assert.AreEqual(original.ToUpper(), decrypted);
    }
}
