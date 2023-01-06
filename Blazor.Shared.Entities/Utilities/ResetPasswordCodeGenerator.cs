using System.Security.Cryptography;
using System.Text;

namespace Blazor.Shared.Entities.Utilities;

public static class ResetPasswordCodeGenerator
{
    public static readonly char[] Chars =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray(); 

    public static string GetUniqueKey(int size)
    {            
        var data = new byte[4*size];
        using (var crypto = RandomNumberGenerator.Create())
        {
            crypto.GetBytes(data);
        }
            
        var result = new StringBuilder(size);
        for (var i = 0; i < size; i++)
        {
            var rnd = BitConverter.ToUInt32(data, i * 4);
            var idx = rnd % Chars.Length;

            result.Append(Chars[idx]);
        }

        return result.ToString();
    }

    [Obsolete] // .NET6 RNGCryptoServiceProvider is an obsolete library
    public static string GetUniqueKeyOriginal_BIASED(int size)
    {
        var charArray =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
        var data = new byte[size];
        using (var crypto = new RNGCryptoServiceProvider())
        {
            crypto.GetBytes(data);
        }
        var result = new StringBuilder(size);
        foreach (var b in data)
        {
            result.Append(charArray[b % (charArray.Length)]);
        }
        return result.ToString();
    }
}