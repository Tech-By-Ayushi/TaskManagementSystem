using Microsoft.AspNetCore.Identity;
using System;

class Program
{
    static void Main()
    {
        var hasher = new PasswordHasher<string>();
        var password = "Admin_1234";
        var hash = hasher.HashPassword(null, password);

        Console.WriteLine("Password: " + password);
        Console.WriteLine("Hashed Password: " + hash);

        var result = hasher.VerifyHashedPassword(null, hash, password);
        Console.WriteLine("Verification Result: " + result);
    }
}
