using System.Net;
using System.Security.Cryptography;

namespace server.Classes;

internal static class AuthorizationHelper
{
    private const string AuthCookieName = "HardMonAuth";

    public static void Logout(IRequestCookieCollection requestCookies, IResponseCookies responseCookies)
    {
        if (requestCookies.ContainsKey(AuthCookieName))
        {
            responseCookies.Delete(AuthCookieName);
        }
    }

    public static void Login(UserDto user, IResponseCookies responseCookies)
    {
        var expireAt = DateTime.UtcNow.AddDays(10).ToFileTimeUtc();
        var rawToken = $"{user.Id}-{expireAt}-{Environment.TickCount}"; //todo: add other values
        var token = rawToken.Encrypt();
        responseCookies.Append(AuthCookieName, token, new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.Now.AddDays(10), //todo: move days to method parameters
        });
    }

    public static int ValidateCookie(IRequestCookieCollection requestCookies)
    {
        if (!requestCookies.TryGetValue(AuthCookieName, out var cookie))
            throw new DomainException(HttpStatusCode.Unauthorized, "Unauthorized");

        try
        {
            var decrypted = cookie.Decrypt();
            var tokenValues = decrypted.Split('-');
            if (tokenValues.Length >= 2)
            {
                if (int.TryParse(tokenValues[0], out var userId) && long.TryParse(tokenValues[1], out var expireAtValue))
                {
                    if (DateTime.FromFileTimeUtc(expireAtValue) < DateTime.UtcNow)
                        throw new DomainException(HttpStatusCode.Unauthorized, "Token expired");
                    return userId;
                }
            }

            throw new DomainException(HttpStatusCode.Unauthorized, "Invalid token content");
        }
        catch
        {
            throw new DomainException(HttpStatusCode.Unauthorized, "Invalid token");
        }
    }

    #region Encryption

    private static byte[] aesKey = "63P5RtO7BZU+jhjD3tQbO8qU2ab486vb".FromBase64Bytes();
    private static byte[] aesIv = "MZRNmm5AHcM=".FromBase64Bytes();

    private static string Encrypt(this string plainText)
    {
        if (string.IsNullOrWhiteSpace(plainText))
            return null;
        byte[] encrypted;
        using (var alg = TripleDES.Create())
        {
            alg.Key = aesKey;
            alg.IV = aesIv;
            var encryptor = alg.CreateEncryptor(alg.Key, alg.IV);
            using (var msEncrypt = new MemoryStream())
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            {
                using (var swEncrypt = new StreamWriter(csEncrypt))
                    swEncrypt.Write(plainText);
                encrypted = msEncrypt.ToArray();
            }
        }

        return encrypted.ToBase64();
    }

    private static string Decrypt(this string cipherText)
    {
        if (cipherText == null || cipherText.Length <= 0)
            return null;
        string plaintext;
        using (var alg = TripleDES.Create())
        {
            alg.Key = aesKey;
            alg.IV = aesIv;
            var decryptor = alg.CreateDecryptor(alg.Key, alg.IV);
            using (var msDecrypt = new MemoryStream(cipherText.FromBase64Bytes()))
            using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            using (var srDecrypt = new StreamReader(csDecrypt))
                plaintext = srDecrypt.ReadToEnd();
        }

        return plaintext;
    }

    #endregion

    public static string GetHash(this string value)
    {
        byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(value);
        byte[] hashBytes = SHA256.HashData(inputBytes);
        return Convert.ToHexString(hashBytes);
    }

    #region string to byte and back extension methods

    private static byte[] FromBase64Bytes(this string str)
    {
        return string.IsNullOrEmpty(str) ? null : Convert.FromBase64String(str);
    }

    private static string ToBase64(this byte[] data)
    {
        if (data == null || data.Length == 0) return null;
        return Convert.ToBase64String(data);
    }

    #endregion
}