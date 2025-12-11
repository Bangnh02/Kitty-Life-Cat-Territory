namespace CleanSourceTool.Editor.FileBinary
{
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

class AdvancedFileModifier
{
    public static bool AreFilesBinaryIdentical(string filePath1, string filePath2)
    {
        // Kiểm tra kích thước file trước
        FileInfo fileInfo1 = new FileInfo(filePath1);
        FileInfo fileInfo2 = new FileInfo(filePath2);

        if (fileInfo1.Length != fileInfo2.Length)
            return false;

        // So sánh từng byte
        using (FileStream fs1 = File.OpenRead(filePath1))
        using (FileStream fs2 = File.OpenRead(filePath2))
        {
            int byte1, byte2;
            do
            {
                byte1 = fs1.ReadByte();
                byte2 = fs2.ReadByte();

                if (byte1 != byte2)
                    return false;

            } while (byte1 != -1 && byte2 != -1);
        }

        return true;
    }
    
    
    public static bool AreFilesMetadataIdentical(string filePath1, string filePath2)
    {
        // So sánh ngày tạo
        if (File.GetCreationTimeUtc(filePath1) != File.GetCreationTimeUtc(filePath2))
            return false;

        // So sánh ngày chỉnh sửa
        if (File.GetLastWriteTimeUtc(filePath1) != File.GetLastWriteTimeUtc(filePath2))
            return false;

        // So sánh ngày truy cập
        if (File.GetLastAccessTimeUtc(filePath1) != File.GetLastAccessTimeUtc(filePath2))
            return false;

        // So sánh thuộc tính file
        if (File.GetAttributes(filePath1) != File.GetAttributes(filePath2))
            return false;

        return true;
    }
    
    public static void ModifyFileCompletely(string filePath, int paddingSize = 256)
    {
        // Đọc dữ liệu gốc từ file
        byte[] originalData = File.ReadAllBytes(filePath);

        using (MemoryStream memoryStream = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(memoryStream))
            {
                // 4. Ghi dữ liệu gốc
                writer.Write(originalData);
                
                writer.Write($"Timestamp:{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
                writer.Write($"FileID:{GenerateRandomString(16)}");
                writer.Write($"Checksum:{ComputeChecksum(originalData)}");

                // 2. Thêm padding ngẫu nhiên
                byte[] padding = GenerateRandomBytes(paddingSize);
                writer.Write(padding);
                
                // 3. Thêm metadata mã hóa
                string metadata = $"Encrypted:{GenerateRandomString(32)}";
                byte[] encryptedMetadata = EncryptMetadata(metadata, GenerateEncryptionKey());
                writer.Write(encryptedMetadata);

                // Ghi lại toàn bộ dữ liệu mới vào file cũ
                File.WriteAllBytes(filePath, memoryStream.ToArray());
            }
        }

        // 6. Thay đổi metadata hệ thống file
        ModifyFileSystemMetadata(filePath);

        Console.WriteLine("File đã được chỉnh sửa hoàn toàn với metadata, padding và header.");
    }
    
    private static string GenerateEncryptionKey(int size = 16) // 16 bytes = 128-bit
    {
        byte[] key = new byte[size];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(key);
        }
        return Convert.ToBase64String(key);
    }

    private static byte[] GenerateRandomBytes(int length)
    {
        byte[] bytes = new byte[length];
        Random random = new Random();
        random.NextBytes(bytes);
        return bytes;
    }

    public static string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        Random random = new Random();
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < length; i++)
        {
            builder.Append(chars[random.Next(chars.Length)]);
        }
        return builder.ToString();
    }

    private static string ComputeChecksum(byte[] data)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hash = sha256.ComputeHash(data);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }

    private static byte[] EncryptMetadata(string metadata, string key)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.GenerateIV();

            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(aes.IV, 0, aes.IV.Length);

                using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    byte[] metadataBytes = Encoding.UTF8.GetBytes(metadata);
                    cs.Write(metadataBytes, 0, metadataBytes.Length);
                    cs.FlushFinalBlock();
                }

                return ms.ToArray();
            }
        }
    }

    private static uint ComputeCrc32(string chunkType, byte[] chunkData)
    {
        // CRC-32 tính toán checksum
        byte[] chunkTypeBytes = Encoding.ASCII.GetBytes(chunkType);
        byte[] combined = new byte[chunkTypeBytes.Length + chunkData.Length];
        Buffer.BlockCopy(chunkTypeBytes, 0, combined, 0, chunkTypeBytes.Length);
        Buffer.BlockCopy(chunkData, 0, combined, chunkTypeBytes.Length, chunkData.Length);

        uint crc = 0xffffffff;
        foreach (byte b in combined)
        {
            crc ^= (uint)b;
            for (int i = 0; i < 8; i++)
            {
                if ((crc & 1) != 0)
                {
                    crc = (crc >> 1) ^ 0xedb88320;
                }
                else
                {
                    crc >>= 1;
                }
            }
        }

        return ~crc;
    }

    private static void ModifyFileSystemMetadata(string filePath)
    {
        // Đặt thời gian tạo file
        DateTime creationTime = DateTime.UtcNow.AddMinutes(-10);
        File.SetCreationTimeUtc(filePath, creationTime);

        // Đặt thời gian chỉnh sửa
        DateTime modifiedTime = DateTime.UtcNow;
        File.SetLastWriteTimeUtc(filePath, modifiedTime);

        // Đặt thời gian truy cập
        DateTime accessTime = DateTime.UtcNow.AddMinutes(-5);
        File.SetLastAccessTimeUtc(filePath, accessTime);

        // Thay đổi thuộc tính file (ẩn file)
        File.SetAttributes(filePath, FileAttributes.Normal);
    }
}

}