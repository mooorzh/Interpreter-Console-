using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace my_own_lang_interpretator
{
    class Tokenizer
    {        
            const char end_symbol = '$';
            string TEXT;
            int POS;
            char CUR_CHAR;
            Dictionary<char, Token> SINGLE_TOKENS = new Dictionary<char, Token>
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
                ['>'] = new Token(token_type.LESS),
                ['<'] = new Token(token_type.LESS),
                ['{'] = new Token(token_type.L_FIGURE),
                ['}'] = new Token(token_type.R_FIGURE),                
            };
            Dictionary<string, Token> RESERVED_WORDS = new Dictionary<string, Token>
            {
                ["IF"] = new Token(token_type.IF),
                ["ELSE"] = new Token(token_type.ELSE),
                ["WHILE"] = new Token(token_type.WHILE),
                ["WRITE"] = new Token(token_type.WRITE_FUNCTION),
                ["READ"] = new Token(token_type.READ_FUNCTION),
                ["INT"] = new Token(token_type.INT_TYPE),
                ["DOUBLE"] = new Token(token_type.DOUBLE_TYPE),
                ["BOOL"] = new Token(token_type.BOOL_TYPE),
                ["CHAR"] = new Token(token_type.CHAR_TYPE),
                ["STRING"] = new Token(token_type.STRING_TYPE),
                ["TRUE"] = new Token(token_type.BOOL, true),
                ["FALSE"] = new Token(token_type.BOOL, false),

            };
            public Tokenizer(string text)
            {
                TEXT = text;
                POS = 0;
                try
                {
                    CUR_CHAR = TEXT[POS];
                }
                catch
                {
                    CUR_CHAR = end_symbol;
                }
            }
            void advance()
            {
                POS++;
                if (POS < TEXT.Length)
                    CUR_CHAR = TEXT[POS];
                else
                    CUR_CHAR = end_symbol;
            }
            char peek()
            {
                if (POS + 1 < TEXT.Length)
                    return TEXT[POS + 1];
                else
                    return end_symbol;
            }
            void skip_space()
            {
                while ((CUR_CHAR == ' ') && (CUR_CHAR != end_symbol) || ((CUR_CHAR == '\n') && (CUR_CHAR != end_symbol)) || ((CUR_CHAR == '\r') && (CUR_CHAR != end_symbol)))
                    advance();
            }
            Token number()
            {
                string res = "";

                while (char.IsDigit(CUR_CHAR))
                {
                    res += CUR_CHAR;
                    advance();
                }
                if (CUR_CHAR == '.')
                {
                    res += ',';
                    advance();
                    while (char.IsDigit(CUR_CHAR))
                    {
                        res += CUR_CHAR;
                        advance();
                    }
                    return new Token(token_type.DOUBLE, double.Parse(res));
                }
                else
                    return new Token(token_type.INTEGER, int.Parse(res));

            }
            Token string_token()
            {
            string res = "";
            advance();
            while (CUR_CHAR != '"')
            {
                res += CUR_CHAR;
                advance();
            }
            advance();
            return new Token(token_type.STRING,res);
            }
            Token id_token()
            {

                string res = "";
                if (CUR_CHAR == '_')
                {
                    res += CUR_CHAR;
                    advance();
                }
                if (char.IsLetter(CUR_CHAR))
                {
                    while (char.IsLetterOrDigit(CUR_CHAR))
                    {
                        res += CUR_CHAR;
                        advance();
                    }
                    Token tmp = new Token(token_type.ID, res);
                    if (RESERVED_WORDS.TryGetValue(res.ToUpper(), out tmp))
                        return tmp;
                    else
                        return new Token(token_type.ID, res);
                }
                else
                {
                    error();
                    return new Token(token_type.ID, "WRONG ID");
                }
            }
            public Token get_next_token()
            {
                while (char.IsWhiteSpace(CUR_CHAR) || (CUR_CHAR == '\n') || (CUR_CHAR == '\r'))
                    skip_space();


                Token res = null;
            if (char.IsDigit(CUR_CHAR))
                return number();
            else if (char.IsLetter(CUR_CHAR) || (CUR_CHAR == '_'))
                return id_token();
            else if (CUR_CHAR == '\"')
                return string_token();
            else if (string.Format("{0}{1}", CUR_CHAR, peek()) == ":=")
            {
                advance();
                advance();
                return new Token(token_type.ASSIGN);
            }
            else if ((CUR_CHAR.ToString() + peek().ToString()) == "==")
            {
                advance();
                advance();
                return new Token(token_type.EQUAL_BOOL);
            }
            else if (SINGLE_TOKENS.TryGetValue(CUR_CHAR, out res))
            {
                advance();
                return res;
            }
            else if (CUR_CHAR == end_symbol)
                return new Token(token_type.EOF);
            else
            {
                error();
                return null;
            }
            }
            public List<Token> make_list()
        {
            List<Token> result = new List<Token>();
            Token tmp;
            do
            {
                tmp = get_next_token();
                result.Add(tmp);
            }
            while (!tmp.compare_type(token_type.EOF));            
            return result;
        }
            void error()
            {
                throw new Exception("Error in Toeknizer:" + CUR_CHAR);
            }
        }
    
}
