using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitClicky.Core.Extensions;

namespace Clicky.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Input: {0}".FormatWith(args[0]));
        }
    }
}
