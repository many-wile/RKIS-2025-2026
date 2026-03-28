using System.Security.Cryptography;
using System.Text;

namespace TodoList;

public static class PayloadCrypto
{
	private static readonly byte[] AesKey =
	[
		0x45, 0x12, 0x99, 0x1C, 0x7B, 0x3A, 0x2F, 0x88,
		0x10, 0x44, 0xBB, 0x5D, 0x9A, 0x8E, 0x22, 0x77,
		0x11, 0x55, 0x33, 0x66, 0x99, 0xAA, 0xCC, 0xEE,
		0xDD, 0xFF, 0x00, 0x12, 0x34, 0x56, 0x78, 0x90
	];

	private static readonly byte[] AesIv =
	[
		0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF,
		0xFE, 0xDC, 0xBA, 0x98, 0x76, 0x54, 0x32, 0x10
	];

	public static Aes CreateAes()
	{
		Aes aes = Aes.Create();
		aes.Key = AesKey;
		aes.IV = AesIv;
		return aes;
	}

	public static byte[] EncryptString(string plainText)
	{
		using Aes aes = CreateAes();
		using var output = new MemoryStream();

		using (var cryptoStream = new CryptoStream(output, aes.CreateEncryptor(), CryptoStreamMode.Write))
		using (var writer = new StreamWriter(cryptoStream, Encoding.UTF8))
		{
			writer.Write(plainText ?? string.Empty);
		}

		return output.ToArray();
	}

	public static string DecryptToString(byte[] encryptedPayload)
	{
		if (encryptedPayload == null || encryptedPayload.Length == 0)
		{
			return string.Empty;
		}

		using Aes aes = CreateAes();
		using var input = new MemoryStream(encryptedPayload);
		using var cryptoStream = new CryptoStream(input, aes.CreateDecryptor(), CryptoStreamMode.Read);
		using var reader = new StreamReader(cryptoStream, Encoding.UTF8);
		return reader.ReadToEnd();
	}
}