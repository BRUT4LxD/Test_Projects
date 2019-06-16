using System;

namespace Test
{
    internal static class Encrytpion
    {
        private static void Main(string[] args)
        {
            DateTime date = new DateTime(2011, 11, 11);
            DateTime date2 = new DateTime(2015, 11, 11);
            Console.WriteLine(date.Date.Day.Equals(date2.Date.Day));
            Console.Read();
        }



    }
}