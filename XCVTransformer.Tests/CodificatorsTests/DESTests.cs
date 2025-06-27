using System.Security.Cryptography;
using System.Text;
using XCVTransformer.Transformers.Codificators.Implementations;

namespace XCVTransformer.Tests;

[TestClass]
public class DESTests
{
    private DESCodificator des;
    private MockKeyStorage mockKeyStorage;

    [TestInitialize]
    public void Setup()
    {
        byte[] key = Encoding.UTF8.GetBytes("12345678"); // 8 bytes
        byte[] iv = Encoding.UTF8.GetBytes("ABCDEFGH");  // 8 bytes

        mockKeyStorage = new MockKeyStorage(() => Task.FromResult((key, iv)));
        des = new DESCodificator(mockKeyStorage);
    }

    [TestMethod]
    public async Task EncodeDecode_WorksCorrectly()
    {
        string original = "Texto de prueba";
        string encrypted = await des.Encode(original);
        string decrypted = await des.Decode(encrypted);

        Assert.AreEqual(original, decrypted);
    }

    [TestMethod]
    public async Task Decode_InvalidBase64()
    {
        await Assert.ThrowsExceptionAsync<FormatException>(() =>
            des.Decode("%%%not base64%%%"));
    }

    [TestMethod]
    public void GetName_ReturnsCorrect()
    {
        Assert.AreEqual("Encriptación DES", des.GetName());
    }

    [TestMethod]
    public async Task Decode_InvalidEncryptedContent()
    {
        // Texto válido en Base64 pero contenido inválido para DESCryptoStream
        string corrupt = Convert.ToBase64String(Encoding.UTF8.GetBytes("invalid"));
        await Assert.ThrowsExceptionAsync<CryptographicException>(() =>
            des.Decode(corrupt));
    }

    [TestMethod]
    public async Task Encode_InvalidKeySize()
    {
        byte[] badKey = Encoding.UTF8.GetBytes("BAD"); // Menos de 8 bytes
        byte[] iv = Encoding.UTF8.GetBytes("ABCDEFGH");

        mockKeyStorage = new MockKeyStorage(() => Task.FromResult((badKey, iv)));
        des = new DESCodificator(mockKeyStorage);

        await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
            des.Encode("mensaje"));
    }

    [TestMethod]
    public async Task Encode_InvalidIVSize()
    {
        byte[] key = Encoding.UTF8.GetBytes("12345678");
        byte[] badIV = Encoding.UTF8.GetBytes("IV"); // Menos de 8 bytes

        mockKeyStorage = new MockKeyStorage(() => Task.FromResult((key, badIV)));
        des = new DESCodificator(mockKeyStorage);

        await Assert.ThrowsExceptionAsync<CryptographicException>(() =>
            des.Encode("mensaje"));
    }

    [TestMethod]
    public async Task Decode_InvalidLenght()
    {
        // Texto cifrado truncado que no es múltiplo de bloque
        string base64Invalid = Convert.ToBase64String(new byte[] { 0x00, 0x01, 0x02 });
        await Assert.ThrowsExceptionAsync<CryptographicException>(() =>
            des.Decode(base64Invalid));
    }

    [TestMethod]
    public async Task KeyStorage_ThrowsException()
    {
        mockKeyStorage = new MockKeyStorage(() => throw new Exception("Vault error"));
        des = new DESCodificator(mockKeyStorage);

        await Assert.ThrowsExceptionAsync<Exception>(() =>
            des.Encode("data"));
    }
}
