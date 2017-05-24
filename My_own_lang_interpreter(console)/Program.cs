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
            Console.WriteLine("Введите Путь к коду программы:");
            string path = Console.ReadLine();
            string text = System.IO.File.ReadAllText("test.txt");
            Interpreter a = new InterpreterIOConsole(text);            
            a.Run();
        }
    }
}
