using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVMtoBytecode
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] bytes = System.IO.File.ReadAllBytes("C:\\NeoDeveloping\\SmartTransit\\TicketManager\\bin\\Debug\\TicketManager.avm");
            string str = System.Text.Encoding.Default.GetString(bytes);
            Console.WriteLine($"Execution result: {str}");
            Console.ReadLine();
        }
    }
}
