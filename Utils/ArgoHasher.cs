using System.Security.Policy;
using System.Text;
using Konscious.Security.Cryptography;

namespace dofdir_komek.Utils;

public class ArgoHasher
{
    public static string Hash(string text)
    {
        if (string.IsNullOrEmpty(text)) return "";

        byte[] b = Encoding.ASCII.GetBytes(text);

        var argon = new Argon2i(b);
        argon.DegreeOfParallelism= 1;
        argon.Iterations = 2;
        argon.MemorySize = 1024;
        var hash = argon.GetBytes(16);

        return Encoding.ASCII.GetString(hash);
    }
}