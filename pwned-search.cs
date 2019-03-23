namespace pwned_search
{
    using System;
    using System.IO;
    using System.Net;
    using System.Security.Cryptography;
    using System.Text;

    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.Write("Enter the password to check or 'x' to exit: ");

            var input = Console.ReadLine();

            while (input != "x")
            {
                SHA1 sha = new SHA1CryptoServiceProvider();
                var data = sha.ComputeHash(Encoding.ASCII.GetBytes(input));

                // Loop through each byte of the hashed data 
                // and format each one as a hexadecimal string.
                var sBuilder = new StringBuilder();
                for (var i = 0; i < data.Length; i++) sBuilder.Append(data[i].ToString("x2"));
                var result = sBuilder.ToString().ToUpper();
                Console.WriteLine($"The SHA-1 hash of {input} is: {result}");

                try
                {
                    // get a list of all the possible passwords where the first 5 digits of the hash are the same
                    var url = "https://api.pwnedpasswords.com/range/" + result.Substring(0, 5);
                    var request = WebRequest.Create(url);
                    var response = request.GetResponse().GetResponseStream();
                    var reader = new StreamReader(response);

                    // look at each possibility and compare the rest of the hash to see if there is a match
                    var hashToCheck = result.Substring(5);

                    var resultCount = 0;
                    var timesFound = 0;

                    var line = reader.ReadLine();

                    while (line != null)
                    {
                        var parts = line.Split(':');

                        resultCount++;

                        if (parts[0] == hashToCheck) timesFound = int.Parse(parts[1]);

                        line = reader.ReadLine();
                    }

                    Console.WriteLine($"Search result returned {resultCount} matching hashes.");
                    Console.WriteLine($"Password has been compromised {timesFound} times.");
                }
                catch
                {
                    Console.WriteLine("An error occured querying pwnedpasswords.com API.");
                }

                Console.Write("Enter another password to check for 'x' to exit: ");

                input = Console.ReadLine();
            }
        }
    }
}