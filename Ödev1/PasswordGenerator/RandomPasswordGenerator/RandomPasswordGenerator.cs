using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PasswordGenerator
{
    internal class RandomPasswordGenerator
    {
        private string checkNumbers, checkUppercase, checkLowercase, checkSpecialCharacters, passwordLength;
        private string specialCharacters = "!#$%&'()*+,-./:;<=>?@[]^_{}|~";
        private string outputPassword = "";

        Random randomObject = new Random();

        private enum AsciiNo
        {
            NumberStart = 48,
            NumberEnd = 57,
            UppercaseStart = 65,
            UppercaseEnd = 90,
            LowercaseStart = 97,
            LowercaseEnd = 122
        }

        private void startScreen()
        {
            for (int i = 0; i <= 55; i++) { Console.Write("*"); }
            Console.WriteLine("\n*Welcome to the B E S T P A S S W O R D M A N A G E R !*");
            for (int i = 0; i <= 55; i++) { Console.Write("*"); }
        }

        public void Generator()
        {
            this.startScreen();
            
            Console.WriteLine("\n\nDo u want to Include Numbers? y/n");
            checkNumbers = Console.ReadLine();
            checkNumbers = query(checkNumbers);

            Console.WriteLine("OK! How about uppercase characters? y/n");
            checkUppercase = Console.ReadLine();
            checkUppercase = query(checkUppercase);

            Console.WriteLine("Very Nice! How about lowercase characters? y/n");
            checkLowercase = Console.ReadLine();
            checkLowercase = query(checkLowercase);

            Console.WriteLine("All right! We are almost done. Would you also want to add special characters? y/n");
            checkSpecialCharacters = Console.ReadLine();
            checkSpecialCharacters = query(checkSpecialCharacters);

            Console.WriteLine("Great! Lastly. How long do you want to keep your password length? If an invalid value is entered, the default length will be accepted as '8'.");
            passwordLength = Console.ReadLine();
            passwordLength = SayiMi(passwordLength);

            for (int i = 0; i <= 54; i++) { Console.Write("-"); }

            List<char> SpecialCharacter = specialCharacters.ToCharArray().ToList();
            List<char> Password = new List<char>();


            if (checkNumbers == "y" && checkNumbers == "Y") { for (int i = (int)AsciiNo.NumberStart; i <= (int)AsciiNo.NumberEnd; i++) { Password.Add(Convert.ToChar(i)); } }
            if (checkUppercase == "y" && checkNumbers == "Y") { for (int i = (int)AsciiNo.UppercaseStart; i <= (int)AsciiNo.UppercaseEnd; i++) { Password.Add(Convert.ToChar(i)); } }
            if (checkLowercase == "y" && checkNumbers == "Y") { for (int i = (int)AsciiNo.LowercaseStart; i <= (int)AsciiNo.LowercaseEnd; i++) { Password.Add(Convert.ToChar(i)); } }
            if (checkSpecialCharacters == "y") { for (int i = 0; i < SpecialCharacter.Count; i++) { Password.Add(SpecialCharacter[i]); } }

            for (int i = 0; i < Convert.ToInt32(passwordLength); i++)
            {
                outputPassword += Password[randomObject.Next(Password.Count)];
            }

            Console.WriteLine("\n" + outputPassword);
            for (int i = 0; i <= 54; i++) { Console.Write("-"); }
            Console.ReadLine();
        }
        private string SayiMi(string value)
        {
            foreach (char chr in value)
            {
                if (!Char.IsNumber(chr)) return "8";
            }
            return value;
        }
        private string query(string answer)
        {
            return (answer != "y" && answer != "Y") ? "n" : answer;
        }
    }
}
