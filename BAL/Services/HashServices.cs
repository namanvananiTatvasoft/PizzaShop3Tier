using System.Security.Cryptography;
using System.Text;
using BAL.Interfaces;

namespace BAL.Services;

public class HashServices : IHashServices
{
    public string HashPassword(string password){
        SHA256 sha256 = SHA256.Create();

        byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        StringBuilder builder = new StringBuilder();

        for (int i = 0; i < bytes.Length; i++)
        {
            builder.Append(bytes[i].ToString("x2"));
        }

        return builder.ToString();
    }
}
