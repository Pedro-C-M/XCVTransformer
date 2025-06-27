using Org.BouncyCastle.Crypto;
using System.Text;
using XCVTransformer.Transformers.Codificators.Implementations;

namespace XCVTransformer.Tests;

[TestClass]
public class BlowFishTests
{
    private BlowfishCodificator blowfish;
    private MockKeyStorage mockKeyStorage;

    [TestInitialize]
    public void Setup()
    {
        byte[] key = Encoding.UTF8.GetBytes("12345678"); // Blowfish mínimo: 4 bytes, aquí 8
        byte[] iv = Encoding.UTF8.GetBytes("ABCDEFGH");  // Blowfish CBC necesita IV de 8 bytes

        mockKeyStorage = new MockKeyStorage(() => Task.FromResult((key, iv)));
        blowfish = new BlowfishCodificator(mockKeyStorage);
    }

    [TestMethod]
    public async Task EncodeDecode_WorksCorrectly()
    {
        string original = "Texto de prueba";
        string encrypted = await blowfish.Encode(original);
        string decrypted = await blowfish.Decode(encrypted);

        Assert.AreEqual(original, decrypted);
    }

    [TestMethod]
    public async Task Decode_InvalidBase64()
    {
        await Assert.ThrowsExceptionAsync<FormatException>(() =>
            blowfish.Decode("not base64!"));
    }

    [TestMethod]
    public void GetName_ReturnsCorrect()
    {
        Assert.AreEqual("Encriptación Blowfish", blowfish.GetName());
    }

    [TestMethod]
    public async Task Decode_CorruptedEncryptedContent()
    {
        string corrupt = Convert.ToBase64String(Encoding.UTF8.GetBytes("invalid"));
        await Assert.ThrowsExceptionAsync<DataLengthException>(() =>
            blowfish.Decode(corrupt));
    }

    [TestMethod]
    public async Task Encode_InvalidKeySize()
    {
        byte[] badKey = Encoding.UTF8.GetBytes("123"); // < 4 bytes
        byte[] iv = Encoding.UTF8.GetBytes("ABCDEFGH");

        mockKeyStorage = new MockKeyStorage(() => Task.FromResult((badKey, iv)));
        blowfish = new BlowfishCodificator(mockKeyStorage);

        await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
            blowfish.Encode("mensaje"));
    }

    [TestMethod]
    public async Task Encode_InvalidIVSize()
    {
        byte[] key = Encoding.UTF8.GetBytes("12345678");
        byte[] badIV = Encoding.UTF8.GetBytes("IV"); // < 8 bytes

        mockKeyStorage = new MockKeyStorage(() => Task.FromResult((key, badIV)));
        blowfish = new BlowfishCodificator(mockKeyStorage);

        await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
            blowfish.Encode("mensaje"));
    }

    [TestMethod]
    public async Task KeyStorage_ThrowsException()
    {
        mockKeyStorage = new MockKeyStorage(() => throw new Exception("Vault error"));
        blowfish = new BlowfishCodificator(mockKeyStorage);

        await Assert.ThrowsExceptionAsync<Exception>(() =>
            blowfish.Encode("data"));
    }
}
