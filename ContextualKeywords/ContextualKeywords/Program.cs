using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextualKeywords
{
    class Program
    {
        static void Main(string[] args)
        {
            // Yield return makes the function an Iterable.
            // Yield can be used to create own iteration.
            // Yield does not create object until it is called. See TheGang function
            // Usually used on functions with loops and turn it into Iterable.

            Console.WriteLine("** First Test **");
            var sentence = "This is a test to see how yield works.";
            foreach(var word in PerWord(sentence))
            {
                Console.WriteLine(word);
            }

            Console.WriteLine("** Second Test **");
            var foundPrimeNumbers = GetPrimeNumbers(10);
            Console.WriteLine($"Length of Prime Numbers using the function with yield is {foundPrimeNumbers.Count()}");
            foreach (var primeNumbers in foundPrimeNumbers)
            {
                Console.WriteLine(primeNumbers);
            }

            Console.WriteLine("** Third Test **");
            // This line does not create the other objects defined in the function. Only the first one
            Console.WriteLine($"First Member using function: {TheGang().First().name}");

            // Since Lily is the 3rd member of the list, the function needed to create objects for Barney and Ted, 1st and 2nd member respectively.
            Console.WriteLine($"Is Lily in the Gang? -> {TheGang().Any(_ => _.name.Equals("Lily"))}");

            // We are getting the count of the list, so it will create objects for all of the members
            Console.WriteLine($"How many members of the Gang? -> {TheGang().Count()}");

            // We are getting the last member so it will create objects for all the members
            Console.WriteLine($"The Last Member: {TheGang().Last().name}");

            Console.WriteLine("** Fourth Test **");
            // Does not create any objects yet
            var gangMembers = TheGang();

            // Assigning it to a variable has the same behavior as using the function directly.
            Console.WriteLine($"First Member using variable: {gangMembers.First().name}");
            Console.WriteLine($"Is Lily in the Gang? using variable -> {gangMembers.Any(_ => _.name.Equals("Lily"))}");

            Console.ReadLine();
        }

        static IEnumerable<string> PerWord(string sentence)
        {
            var separatedWords = sentence.Split(' ');

            foreach(var word in separatedWords)
            {
                yield return word;
            }
        }

        static IEnumerable<int> GetPrimeNumbers(int length)
        {
            var foundPrimeNumbers = 0;
            for (var i = 1; foundPrimeNumbers < length; ++i)
            {
                if (IsPrime(i))
                {
                    foundPrimeNumbers++;
                    yield return i;
                }
                   
            }
        }

        static bool IsPrime(int number)
        {
            if (number == 1) return false;
            if (number == 2) return true;

            var limit = Math.Ceiling(Math.Sqrt(number));

            for (var i = 2; i <= limit; ++i)
                if (number % i == 0)
                    return false;
            return true;
        }

        static IEnumerable<Person> TheGang()
        {
            var names = new List<string> { "Barney", "Ted", "Lily", "Marshall", "Robin"};
            for (var i = 0; i < 5; ++i)
            {
                Console.WriteLine(names[i]);
                yield return new Person(names[i], 20 + i);
            }
        }
    }

    public class Person
    {
        public string name { get; }
        public int age { get; }

        public Person(string name, int age)
        {
            this.name = name;
            this.age = age;
        }
    }
}
