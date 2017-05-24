using System;
using System.Collections.Generic;


namespace My_own_lang_interpreter_console_
{
    class Tokenizer
    {
        const char EndSymbol = '$';
        string Text;
        int CurrentPosition;
        private char CurrentChar
        {
            get
            {
                if (CurrentPosition < Text.Length)
                    return Text[CurrentPosition];
                else
                    return EndSymbol;
            }
        }
        Dictionary<char, Token> SingleToken = new Dictionary<char, Token>
        {
            ['+'] = new Token(token_type.PLUS),
            ['-'] = new Token(token_type.MINUS),
            ['*'] = new Token(token_type.MUL),
            ['/'] = new Token(token_type.DIV),
            ['('] = new Token(token_type.L_PAR),
            [')'] = new Token(token_type.R_PAR),
            [':'] = new Token(token_type.COLON),
            [';'] = new Token(token_type.SEMI),
            ['.'] = new Token(token_type.DOT),
            [','] = new Token(token_type.COMA),
            ['='] = new Token(token_type.EQUAL),
            ['^'] = new Token(token_type.POW),
            ['|'] = new Token(token_type.DIZ),
            ['&'] = new Token(token_type.CON),
            ['>'] = new Token(token_type.MORE),
            ['<'] = new Token(token_type.LESS),
            ['{'] = new Token(token_type.L_FIGURE),
            ['}'] = new Token(token_type.R_FIGURE),
            ['!'] = new Token(token_type.NOT),
        };
        Dictionary<string, Token> ReservedWord = new Dictionary<string, Token>
        {
            ["IF"] = new Token(token_type.IF),
            ["ELSE"] = new Token(token_type.ELSE),
            ["WHILE"] = new Token(token_type.WHILE),
            ["WRITE"] = new Token(token_type.WRITE_FUNCTION),
            ["READ"] = new Token(token_type.READ_FUNCTION),
            ["INT"] = new Token(token_type.TYPE_INT),
            ["DOUBLE"] = new Token(token_type.TYPE_DBL),
            ["BOOL"] = new Token(token_type.TYPE_BOOL),
            ["CHAR"] = new Token(token_type.TYPE_CHAR),
            ["STRING"] = new Token(token_type.TYPE_STRING),
            ["TRUE"] = new Token(token_type.BOOL, true),
            ["FALSE"] = new Token(token_type.BOOL, false),
        };
        public Tokenizer(string text)
        {
            Text = text;
            CurrentPosition = 0;
        }
        void Advance()
        {
            CurrentPosition++;
        }
        char peek()
        {
            if (CurrentPosition + 1 < Text.Length)
                return Text[CurrentPosition + 1];
            else
                return EndSymbol;
        }
        void SkipSpace()
        {
            while (((CurrentChar == ' ') || (CurrentChar == '\n') || (CurrentChar == '\r')) && (CurrentChar != EndSymbol))
                Advance();
        }
        Token Number()
        {
            string res = "";
            while (char.IsDigit(CurrentChar))
            {
                res += CurrentChar;
                Advance();
            }
            if (CurrentChar == '.')
            {
                res += ',';
                Advance();
                while (char.IsDigit(CurrentChar))
                {
                    res += CurrentChar;
                    Advance();
                }
                return new Token(token_type.DOUBLE, double.Parse(res));
            }
            else
                return new Token(token_type.INTEGER, int.Parse(res));
        }
        Token StringToken()
        {
            string res = "";
            Advance();
            while (CurrentChar != '"')
            {
                res += CurrentChar;
                Advance();
            }
            Advance();
            return new Token(token_type.STRING,res);
        }
        Token IDToken()
        {
            string res = "";
            if (CurrentChar == '_')
            {
                res += CurrentChar;
                Advance();
            }
            if (char.IsLetter(CurrentChar))
            {
                while (char.IsLetterOrDigit(CurrentChar))
                {
                    res += CurrentChar;
                    Advance();
                }
                Token tmp = new Token(token_type.ID, res);
                if (ReservedWord.TryGetValue(res.ToUpper(), out tmp))
                    return tmp;
                else
                    return new Token(token_type.ID, res);
            }
                else
            {
                Error();
                return new Token(token_type.ID, "WRONG ID");
            }
        }
        public Token GetNextToken()
        {
            SkipSpace();
            Token res = null;
            if (char.IsDigit(CurrentChar))
                res = Number();
            else if (char.IsLetter(CurrentChar) || (CurrentChar == '_'))
                res = IDToken();
            else if (CurrentChar == '\"')
                res = StringToken();            
            else if ((CurrentChar.ToString() + peek().ToString()) == "==")
            {
                Advance();
                Advance();
                res = new Token(token_type.EQUAL_BOOL);
            }
            else if (SingleToken.TryGetValue(CurrentChar, out res))            
                Advance();            
            else if (CurrentChar == EndSymbol)
                res = new Token(token_type.EOF);
            else            
                Error();         
            return res;
        }
        public Token[] MakeTokenArray()
        {
            List<Token> result = new List<Token>();
            Token tmp;
            do
            {
                tmp = GetNextToken();
                result.Add(tmp);
            }
            while (!tmp.compare_type(token_type.EOF));            
            return result.ToArray();
        }                       
        void Error()
        {
            throw new Exception("Error in Toeknizer:" + CurrentChar);
        }
    }
    
}
