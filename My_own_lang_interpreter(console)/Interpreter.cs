using System;
using System.Collections.Generic;



namespace My_own_lang_interpreter_console_
{
    class Interpreter
    {
        private const double delta = 1E-10;        
        private int current_position;
        public List<Token> token_list;
        private Dictionary<string, object> var_values = new Dictionary<string, object>();
        private Dictionary<string, token_type> var_types = new Dictionary<string, token_type>();
        private Token current_token
        {
            get
            {
                return token_list[current_position];
            }
        }
        public Interpreter(string text)
        {
            current_position = 0;
            Tokenizer lexer = new Tokenizer(text);
            token_list = lexer.make_list();                          
        }
        private void error(string explain = "")
        {
            throw new Exception(explain);
        }
        private void eat(token_type expected_type)
        {
            if (current_token.compare_type(expected_type))            
                current_position++;           
            else
                error(string.Format("Expected token type:{0} Acual token type:{1}",expected_type.ToString(),current_token.type.ToString()));
        }
        public string run()
        {
            while (!current_token.compare_type(token_type.EOF))
            {
                sentense();
                eat(token_type.SEMI);
            }
            return string.Join(";",var_types)+Environment.NewLine+ string.Join(";", var_values);
        }
        private void block()
        {            
            eat(token_type.L_FIGURE);
            while (!current_token.compare_type(token_type.R_FIGURE))
            {
                sentense();
                eat(token_type.SEMI);                
            }
            eat(token_type.R_FIGURE);
        }
        private void skip_block()
        {
            eat(token_type.L_FIGURE);
            while (!current_token.compare_type(token_type.R_FIGURE))                            
                eat(current_token.type);            
            eat(token_type.R_FIGURE);
        }
        private void sentense()
        {
            Console.WriteLine(current_token.type.ToString());
            Console.WriteLine(current_token.value);
            if (current_token.is_type())         
                declaration_sentense();           
            else if (current_token.compare_type(token_type.ID))
                equation_sentense();
            else if (current_token.compare_type(token_type.IF))
                if_sentense();
            else if (current_token.compare_type(token_type.WHILE))
                while_sentense();
            else if (current_token.is_function())
                function_call_sentense();
            else
                error();
        }
        private void skip_sentense()
        {
            while (!current_token.compare_type(token_type.SEMI))
                eat(current_token.type);            
        }
        private void declaration_sentense()
        {
            Token var_type = current_token;
            eat(var_type.type);
            if (var_type.compare_type(token_type.TYPE_BOOL))
                var_type.type = token_type.BOOL;
            else if (var_type.compare_type(token_type.TYPE_INT))
                var_type.type = token_type.INTEGER;
            else if (var_type.compare_type(token_type.TYPE_DBL))
                var_type.type = token_type.DOUBLE;
            else if (var_type.compare_type(token_type.TYPE_CHAR))
                var_type.type = token_type.CHAR;
            else if (var_type.compare_type(token_type.TYPE_STRING))
                var_type.type = token_type.STRING;
            do
            {
                if (current_token.compare_type(token_type.COMA))
                    eat(token_type.COMA);
                if (current_token.compare_type(token_type.ID))
                {
                    var_types.Add((string)current_token.value,var_type.type);
                    if (token_list[current_position + 1].compare_type(token_type.EQUAL))
                        equation_sentense();
                    else                    
                        eat(token_type.ID);                    
                }                
            } while (current_token.compare_type(token_type.COMA));
        }
        private void equation_sentense()
        {
            Token l_variable = current_token;
            token_type l_variable_type;
            if (var_types.TryGetValue((string)l_variable.value, out l_variable_type))
            {                
                eat(token_type.ID);
                eat(token_type.EQUAL);
                Token r_variable = expression();
                if (r_variable.compare_type(l_variable_type))
                    var_values[(string)l_variable.value] = r_variable.value;
                else
                    error(l_variable_type.ToString()+"!=" + r_variable.type.ToString());
            }
            else
                error();
        }
        private Token expression()
        {
            if (current_token.compare_type(token_type.STRING))
            {
                Token tmp = current_token;
                eat(token_type.STRING);
                return tmp;
            }
            else
                return bool_expr();
        }
        private Token bool_expr()
        {
            Token res = bool_term();
            while (current_token.compare_type(token_type.DIZ))
            {
                eat(token_type.DIZ);
                Token tmp = bool_term();
                res.value = Convert.ToBoolean(res.value) || Convert.ToBoolean(tmp.value);
            }
            return res;
        }
        Token bool_term()
        {
            Token res = bool_fact();
            while (current_token.compare_type(token_type.CON))
            {
                eat(token_type.CON);
                Token tmp = bool_fact();
                res.value = Convert.ToBoolean(res.value) && Convert.ToBoolean(tmp.value);
            }
            return res;
        }
        Token bool_fact()
        {
            Token tmp;
            if (current_token.compare_type(token_type.BOOL))
            {
                tmp = current_token;
                eat(tmp.type);
                return tmp;
            }
            else if (current_token.compare_type(token_type.L_PAR))
            {
                eat(token_type.L_PAR);
                tmp = bool_expr();
                eat(token_type.R_PAR);
                return tmp;
            }
            else
            {
                tmp = real_expr();
                if ((current_token.compare_type(token_type.LESS)) || (current_token.compare_type(token_type.MORE)) || (current_token.compare_type(token_type.EQUAL_BOOL)))
                {
                    Token tmp2;
                    if (current_token.compare_type(token_type.MORE))
                    {
                        eat(token_type.MORE);
                        tmp2 = real_expr();
                        if (Convert.ToDouble(tmp.value) > Convert.ToDouble(tmp2.value))
                            return new Token(token_type.BOOL, true);
                        else
                            return new Token(token_type.BOOL, false);
                    }
                    else if (current_token.compare_type(token_type.LESS))
                    {
                        eat(token_type.LESS);
                        tmp2 = real_expr();
                        if (Convert.ToDouble(tmp.value) < Convert.ToDouble(tmp2.value))
                            return new Token(token_type.BOOL, true);
                        else
                            return new Token(token_type.BOOL, false);
                    }
                    else
                    {
                        eat(token_type.EQUAL_BOOL);
                        tmp2 = real_expr();
                        if (Math.Abs(Convert.ToDouble(tmp.value) - Convert.ToDouble(tmp2.value)) < delta)
                            return new Token(token_type.BOOL, true);
                        else
                            return new Token(token_type.BOOL, false);
                    }
                }
                return tmp;
            }

        }
        Token real_expr()
        {
            Token res = term();
            if ((res.compare_type(token_type.INTEGER)) || (res.compare_type(token_type.DOUBLE)))
            {
                while (current_token.compare_type(token_type.PLUS) || (current_token.compare_type(token_type.MINUS)))
                {
                    int sign = 1;
                    if (current_token.compare_type(token_type.MINUS))                    
                        sign = -1;                    
                    eat(current_token.type);
                    Token tmp = term();
                    if (tmp.compare_type(token_type.DOUBLE) || res.compare_type(token_type.DOUBLE))
                    {
                        res.value = Convert.ToDouble(res.value) + Convert.ToDouble(tmp.value) * sign;
                        res.type = token_type.DOUBLE;
                    }
                    else
                        res.value = Convert.ToInt64(res.value) + Convert.ToInt64(tmp.value) * sign;
                }
            }
            return res;
        }
        Token term()
        {
            Token res = power();
            if ((res.compare_type(token_type.INTEGER)) || (res.compare_type(token_type.DOUBLE)))
            {
                while (current_token.compare_type(token_type.DIV) || current_token.compare_type(token_type.MUL))
                {
                    token_type sign = current_token.type;
                    eat(sign);
                    Token tmp = power();
                    if (sign == token_type.MUL)
                    {
                        res.value = Convert.ToDouble(res.value) * Convert.ToDouble(tmp.value);
                        res.type = token_type.DOUBLE;
                    }
                    else
                    {
                        res.value = Convert.ToDouble(res.value) / Convert.ToDouble(tmp.value);
                        res.type = token_type.DOUBLE;
                    }
                }
            }
            return res;
        }
        Token power()
        {
            Token res = fact();
            if (res.compare_type(token_type.INTEGER) || res.compare_type(token_type.DOUBLE))
            {
                while (current_token.compare_type( token_type.POW))
                {
                    eat(token_type.POW);
                    Token tmp = fact();
                    res.value = Math.Pow(Convert.ToDouble(res.value), Convert.ToDouble(tmp.value));
                    res.type = token_type.DOUBLE;
                }
            }
            return res;
        }
        Token fact()
        {
            Token tmp;
            if (current_token.compare_type(token_type.INTEGER))
            {
                tmp = current_token;
                eat(token_type.INTEGER);
                return tmp;
            }
            else if (current_token.compare_type(token_type.DOUBLE))
            {
                tmp = current_token;
                eat(token_type.DOUBLE);
                return tmp;
            }
            else if (current_token.compare_type(token_type.ID))
            {
                token_type t_type = token_type.EOF;
                object t_value;
                if (var_values.TryGetValue((string)current_token.value, out t_value) && var_types.TryGetValue((string)current_token.value, out t_type))
                {
                    eat(token_type.ID);
                    return new Token(t_type, t_value);
                }
                else
                {
                    error();
                    return current_token;
                }
            }
            else if (current_token.compare_type(token_type.L_PAR))
            {
                eat(token_type.L_PAR);
                tmp = real_expr();
                eat(token_type.R_PAR);
                return tmp;
            }
            else
                error();
            return current_token;
        }
        private void if_sentense()
        {
            eat(token_type.IF);
            eat(token_type.L_PAR);
            Token result = bool_expr();
            eat(token_type.R_PAR);
            if (result.compare_type(token_type.BOOL))
            {
                if (Convert.ToBoolean(result.value))
                {
                    if (current_token.compare_type(token_type.L_FIGURE))
                        block();
                    else
                        sentense();
                }
                else
                {
                    if (current_token.compare_type(token_type.L_FIGURE))
                        skip_block();
                    else
                        skip_sentense();
                }                                          
                
                if (current_token.compare_type(token_type.ELSE))
                {
                    eat(token_type.ELSE);
                    if (!Convert.ToBoolean(result.value))
                    {
                        if (current_token.compare_type(token_type.L_FIGURE))
                            block();
                        else
                            sentense();
                    }
                    else
                    {
                        if (current_token.compare_type(token_type.L_FIGURE))
                            skip_block();
                        else
                            skip_sentense();
                    }
                }                
            }
            else            
                error();
            
        }
        private void while_sentense()
        {
            int begin = current_position;
            eat(token_type.WHILE);
            eat(token_type.L_PAR);
            Token result = bool_expr();
            eat(token_type.R_PAR);            
            if (result.compare_type(token_type.BOOL))
            {
                if (Convert.ToBoolean(result.value))
                {
                    if (current_token.compare_type(token_type.L_FIGURE))
                        block();
                    else
                        sentense();
                    current_position = begin;
                    while_sentense();
                }
                else
                {
                    if (current_token.compare_type(token_type.L_FIGURE))
                        skip_block();
                    else
                        skip_sentense();
                }
                    
                                          
            }
            else
                error();

            }
        private void function_call_sentense()
        {
            if (current_token.compare_type(token_type.WRITE_FUNCTION))               
                throw new NotImplementedException();
            else if (current_token.compare_type(token_type.READ_FUNCTION))                
                throw new NotImplementedException();
        }        
    }
}
