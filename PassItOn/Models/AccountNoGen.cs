using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace PassItOn.Models
{
    public class AccountNoGen
    {
        // Generate a random number between two numbers  
        public int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        // Generate a random string with a given size  
        public string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }

        // Generate a random password  
        public string RandomAccountNo()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("P");
            builder.Append(RandomString(1, false));
            builder.Append(RandomNumber(10000, 99999));
            builder.Append(RandomString(3, false));
            return builder.ToString();
        }
    }
}