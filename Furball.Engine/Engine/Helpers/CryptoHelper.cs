using System;
using System.Security.Cryptography;

namespace Furball.Engine.Engine.Helpers; 

public static class CryptoHelper {
    private static readonly MD5    Md5    = new MD5CryptoServiceProvider();
    private static          SHA256 _sha256 = new SHA256CryptoServiceProvider();
    private static readonly SHA384 Sha384 = new SHA384CryptoServiceProvider();
    private static readonly SHA512 Sha512 = new SHA512CryptoServiceProvider();

    public static string GetMd5(byte[] bytes) {
        byte[] hash = Md5.ComputeHash(bytes);

        string hashString = string.Empty;
        for (int index = 0; index < hash.Length; index++) {
            byte x = hash[index];
            hashString += $"{x:x2}";
        }

        return hashString;
    }

    public static string GetSha256(byte[] bytes) {
        byte[] hash = _sha256.ComputeHash(bytes);

        string hashString = string.Empty;
        for (int index = 0; index < hash.Length; index++) {
            byte x = hash[index];
            hashString += $"{x:x2}";
        }

        return hashString;
    }

    public static string GetSha384(byte[] bytes) {
        byte[] hash = Sha384.ComputeHash(bytes);

        string hashString = string.Empty;
        for (int index = 0; index < hash.Length; index++) {
            byte x = hash[index];
            hashString += $"{x:x2}";
        }

        return hashString;
    }

    public static string GetSha512(byte[] bytes) {
        byte[] hash = Sha512.ComputeHash(bytes);

        string hashString = string.Empty;
        for (int index = 0; index < hash.Length; index++) {
            byte x = hash[index];
            hashString += $"{x:x2}";
        }

        return hashString;
    }

    public static byte Crc8(byte[] data, int dataLimit) {
        byte sum = 0;
        unchecked// Let overflow occur without exceptions
        {
            for (int index = 0; index < Math.Min(dataLimit, data.Length); index++) {
                byte b = data[index];
                sum += b;
            }
        }
        return sum;
    }

    public static ushort Crc16(byte[] data, int dataLimit) {
        ushort sum = 0;
        unchecked// Let overflow occur without exceptions
        {
            for (int index = 0; index < Math.Min(dataLimit, data.Length); index++) {
                byte b = data[index];
                sum += b;
            }
        }
        return sum;
    }

    public static uint Crc32(byte[] data, int dataLimit) {
        uint sum = 0;
        unchecked// Let overflow occur without exceptions
        {
            for (int index = 0; index < Math.Min(dataLimit, data.Length); index++) {
                byte b = data[index];
                sum += b;
            }
        }
        return sum;
    }

    public static ulong Crc64(byte[] data, int dataLimit) {
        ulong sum = 0;
        unchecked// Let overflow occur without exceptions
        {
            for (int index = 0; index < Math.Min(dataLimit, data.Length); index++) {
                byte b = data[index];
                sum += b;
            }
        }
        return sum;
    }
}