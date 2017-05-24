using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My_own_lang_interpreter_console_
{
    class InterpreterIOConsole : Interpreter
    {
        public InterpreterIOConsole(string text) : base(text)
        {            
        }
        public override void read_function()
        {
            string ans = Console.ReadLine();
            Tokenizer tmp = new Tokenizer(ans);
            Eat(token_type.READ_FUNCTION);
            Eat(token_type.L_PAR);
            Token var_token = CurrentToken;
            Eat(token_type.ID);
            Eat(token_type.R_PAR);
            Token value_token = tmp.GetNextToken();
            if (VarTypes[var_token.value.ToString()] == value_token.type)
                VarValues[var_token.value.ToString()] = value_token.value;
            else
                Error();
        }
        public override void write_function()
        {
            Console.WriteLine(base.write());            
        }
        public override string Run()
        {
            Console.WriteLine("Program is running");
            string res = base.Run();
            Console.WriteLine("Program ended");
            Console.ReadKey();
            return res;
        }
    }
}
