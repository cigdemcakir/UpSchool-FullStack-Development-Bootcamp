using System;

namespace PasswordGenerator
{
    public class Program
    {
        static void Main(string[] args)
        {
            var Generator = new Generator();

            Generator.Greetings();

            Generator.ReadInputs();

            Generator.Generate();

            Generator.WriteLatestGeneratedPassword();

            Console.ReadLine();
        }
    }
}
