using Moq;
using System.Text;
using XCVTransformer.Transformers.Codificators.Implementations;
using XCVTransformer.Transformers.Codificators;
using System.Security.Cryptography;

namespace XCVTransformer.Tests;

[TestClass]
public class AESTests
{
    private AESCodificator aes;
    private MockKeyStorage mockKeyStorage;

    [TestInitialize]
    public void Setup()
    {
        // Clave e IV válidos para AES-256 (32 y 16 bytes)
        byte[] key = Encoding.UTF8.GetBytes("0123456789ABCDEF0123456789ABCDEF");
        byte[] iv = Encoding.UTF8.GetBytes("1234567890ABCDEF");

        // Setup mock manual para devolver la clave e IV fijos
        mockKeyStorage = new MockKeyStorage(() => Task.FromResult((key, iv)));

        aes = new AESCodificator(mockKeyStorage);
    }

    [TestMethod]
    public async Task EncodeDecode_WorksCorrectly()
    {
        string original = "Texto de prueba";
        string encrypted = await aes.Encode(original);
        string decrypted = await aes.Decode(encrypted);

        Assert.AreEqual(original, decrypted);
    }

    [TestMethod]
    public async Task Decode_InvalidBase64()
    {
        await Assert.ThrowsExceptionAsync<FormatException>(() =>
            aes.Decode("not base64"));
    }

    [TestMethod]
    public void GetName_ReturnsCorrect()
    {
        Assert.AreEqual("Encriptación AES", aes.GetName());
    }

    [TestMethod]
    public async Task Decode_InvalidEncryptedContent()
    {
        string corrupt = Convert.ToBase64String(Encoding.UTF8.GetBytes("invalid encrypted data"));

        await Assert.ThrowsExceptionAsync<CryptographicException>(() =>
            aes.Decode(corrupt));
    }

    [TestMethod]
    public async Task Encode_InvalidKeySize()
    {
        byte[] shortKey = Encoding.UTF8.GetBytes("SHORTKEY"); 
        byte[] iv = Encoding.UTF8.GetBytes("1234567890ABCDEF");

        mockKeyStorage = new MockKeyStorage(() => Task.FromResult((shortKey, iv)));
        aes = new AESCodificator(mockKeyStorage);

        await Assert.ThrowsExceptionAsync<CryptographicException>(() =>
            aes.Encode("mensaje"));
    }

    [TestMethod]
    public async Task Encode_InvalidIVSize()
    {
        byte[] key = Encoding.UTF8.GetBytes("0123456789ABCDEF0123456789ABCDEF");
        byte[] badIV = Encoding.UTF8.GetBytes("IVBAD"); // menos de 16 bytes

        mockKeyStorage = new MockKeyStorage(() => Task.FromResult((key, badIV)));
        aes = new AESCodificator(mockKeyStorage);

        await Assert.ThrowsExceptionAsync<CryptographicException>(() =>
            aes.Encode("mensaje"));
    }

    [TestMethod]
    public async Task KeyStorage_ThrowsException()
    {
        // Mock que lanza excepción simulando fallo en vault
        mockKeyStorage = new MockKeyStorage(() => throw new Exception("Vault error"));
        aes = new AESCodificator(mockKeyStorage);

        await Assert.ThrowsExceptionAsync<Exception>(() =>
            aes.Encode("data"));
    }
}
