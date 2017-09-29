using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Menu
{
    public class _Menu
    {
        public string Ans { get; set; }
        public int Choice { get; set; }
        public Dictionary<int,string> menuIndexes = new Dictionary<int, string>();

        public void AddMenuIndex(int index, string quote)
        {
            menuIndexes.Add(index, quote);
        }

        public void Start()
        {
            Console.Clear();
            Console.WriteLine("\n\t====================");
            Console.WriteLine("\t");
            Console.WriteLine("\n\t====================");
            foreach (KeyValuePair<int,string> item in menuIndexes)
            {
                Console.WriteLine($"{item.Key} - {item.Value}");
            }

            Console.WriteLine();
           
            Console.WriteLine("\n\t====================");
            Console.Write("\n> Choose menu index: ");
            try
            {
                Choice = Convert.ToInt32(Console.ReadLine());
            }
            catch (Exception err)
            {
                Console.WriteLine("{0} - WrongMenuIndex", err.Message);
            }
        }
        public void Finish()
        {
            Console.WriteLine("\nEnded");
        }
        public bool AllowContinue()
        {

            Console.WriteLine("\n>Continue(y/n)? - ");
            Ans = Console.ReadLine();
            return Ans == "y";
        }
    }
}
