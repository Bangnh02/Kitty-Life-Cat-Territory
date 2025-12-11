using System;
using System.Linq;
using System.Security.Cryptography;

namespace Avelog
{
	public class HashUtils
	{
		private static HashAlgorithm hashProvider = new SHA256CryptoServiceProvider();

		public static int HashSizeByte()
		{
			return hashProvider.HashSize / 8;
		}

		public static byte[] Compute(byte[] inputData)
		{
			return hashProvider.ComputeHash(inputData);
		}

		public static byte[] Compute(int inputData)
		{
			return Compute(BitConverter.GetBytes(inputData));
		}

		public static bool Compare(byte[] hash1, byte[] hash2)
		{
			if (hash1 == null || hash2 == null || hash1.Length != hash2.Length)
			{
				return false;
			}
			return hash1.SequenceEqual(hash2);
		}
	}
}
