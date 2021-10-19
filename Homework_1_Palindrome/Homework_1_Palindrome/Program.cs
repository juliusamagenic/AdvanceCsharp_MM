using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Homework_1_Palindrome
{
    class Program
    {
        static void Main(string[] args)
        {
            int entryCount = 10;
            Console.WriteLine($"Enter {entryCount} words/phrases and it will output a list of entries that are Palindrome:");
            var palindromeCandidates = new List<string>();
            for (int i = 0; i < entryCount; ++i)
                palindromeCandidates.Add(Console.ReadLine());

            Console.WriteLine();
            Console.WriteLine("Palindromes found in the list:");
            foreach (var entry in GetPalindromeEntries(palindromeCandidates))
            {
                Console.WriteLine(entry);
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static IEnumerable<string> GetPalindromeEntries(List<string> entries)
        {
            var replaceRegex = new Regex(@"\s+|'|,");
            foreach (var entry in entries)
            {
                var trimmedEntry = replaceRegex.Replace(entry, "");
                var reversed = new string(trimmedEntry.Reverse().ToArray());
                if (trimmedEntry.Equals(reversed, StringComparison.OrdinalIgnoreCase))
                    yield return entry;
            }
        }
    }
}
