using System;
using System.Collections.Generic;

namespace Blockchain_sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            Block b = new Block(1, new List<Transaction>());

            Console.WriteLine(b.GetHashCode());
            Console.ReadLine();
            
        }
    }
}
