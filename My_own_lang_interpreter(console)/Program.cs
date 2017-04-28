using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My_own_lang_interpreter_console_
{
    class Program
    {
        static void Main(string[] args)
        {
            Interpreter a = new Interpreter("int a = 5;int b;");            
            Console.WriteLine(string.Join(";",a.token_list));
            Console.WriteLine(a.run());
            Console.ReadKey();
        }
    }
}
