using System;
using System.Collections.Generic;



namespace My_own_lang_interpreter_console_
{
    abstract class Interpreter
    {
        protected const double delta = 1E-10;
        protected int CurrentPosition;
        protected Token[] TokenList;
        protected Dictionary<string, object> VarValues = new Dictionary<string, object>();
        protected Dictionary<string, token_type> VarTypes = new Dictionary<string, token_type>();
        protected Token CurrentToken
        {
            get
            {
                return TokenList[CurrentPosition];
            }
        }
        public Interpreter(string text)
        {
            CurrentPosition = 0;
            Tokenizer lexer = new Tokenizer(text);
            TokenList = lexer.MakeTokenArray();                          
        }
        protected void Error(string explain = "")
        {
            throw new Exception(explain);
        }
        protected void Eat(token_type expected_type)
        {
            if (CurrentToken.compare_type(expected_type))            
                CurrentPosition++;           
            else
                Error(string.Format("Expected token type:{0} Acual token type:{1}",expected_type.ToString(),CurrentToken.type.ToString()));
        }
        public virtual string Run()
        {
            while (!CurrentToken.compare_type(token_type.EOF))
            {
                Sentense();
                Eat(token_type.SEMI);
            }
            return string.Join(";",VarTypes)+Environment.NewLine+ string.Join(";", VarValues);
        }
        protected void Block()
        {            
            Eat(token_type.L_FIGURE);
            while (!CurrentToken.compare_type(token_type.R_FIGURE))
            {
                Sentense();
                Eat(token_type.SEMI);                
            }
            Eat(token_type.R_FIGURE);
        }
        protected void SkipBlock()
        {
            Eat(token_type.L_FIGURE);
            while (!CurrentToken.compare_type(token_type.R_FIGURE))
            {
                if (CurrentToken.compare_type(token_type.L_FIGURE))
                    SkipBlock();
                else
                    Eat(CurrentToken.type);
            }        
            Eat(token_type.R_FIGURE);
        }       
        protected void Sentense()
        {    
            if (CurrentToken.is_type())         
                DeclarationSentense();           
            else if (CurrentToken.compare_type(token_type.ID))
                EquationSentense();
            else if (CurrentToken.compare_type(token_type.IF))
                IfSentense();
            else if (CurrentToken.compare_type(token_type.WHILE))
                WhileSentense();
            else if (CurrentToken.is_function())
                FunctionCallSentense();
            else
                Error();
        }
        protected void SkipSentense()
        {
            while (!CurrentToken.compare_type(token_type.SEMI))
                Eat(CurrentToken.type);            
        }
        protected void DeclarationSentense()
        {
            Token var_type = new Token(CurrentToken.type);
            Eat(var_type.type);
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
                if (CurrentToken.compare_type(token_type.COMA))
                    Eat(token_type.COMA);
                if (CurrentToken.compare_type(token_type.ID))
                {
                    VarTypes.Add((string)CurrentToken.value,var_type.type);
                    if (TokenList[CurrentPosition + 1].compare_type(token_type.EQUAL))
                        EquationSentense();
                    else                    
                        Eat(token_type.ID);                    
                }                
            } while (CurrentToken.compare_type(token_type.COMA));
        }
        protected void EquationSentense()
        {
            Token l_variable = CurrentToken;
            token_type l_variable_type;
            if (VarTypes.TryGetValue((string)l_variable.value, out l_variable_type))
            {                
                Eat(token_type.ID);
                Eat(token_type.EQUAL);
                Token r_variable = Expression();
                if (r_variable.compare_type(l_variable_type))
                    VarValues[(string)l_variable.value] = r_variable.value;
                else
                    Error(l_variable_type.ToString()+"!=" + r_variable.type.ToString());
            }
            else
                Error();
        }
        protected Token Expression()
        {
            if (CurrentToken.compare_type(token_type.STRING))
            {
                Token tmp = CurrentToken;
                Eat(token_type.STRING);
                return tmp;
            }
            else
                return BoolExpr();
        }
        protected Token BoolExpr()
        {
            Token res = BoolTerm();
            while (CurrentToken.compare_type(token_type.DIZ))
            {
                Eat(token_type.DIZ);
                Token tmp = BoolTerm();
                res.value = Convert.ToBoolean(res.value) || Convert.ToBoolean(tmp.value);
            }
            return res;
        }
        protected Token BoolTerm()
        {
            Token res = BoolFact();
            while (CurrentToken.compare_type(token_type.CON))
            {
                Eat(token_type.CON);
                Token tmp = BoolFact();
                res.value = Convert.ToBoolean(res.value) && Convert.ToBoolean(tmp.value);
            }
            return res;
        }
        protected Token BoolFact()
        {
            Token tmp;
            if (CurrentToken.compare_type(token_type.BOOL))
            {
                tmp = CurrentToken;
                Eat(tmp.type);
                return tmp;
            }
            else if (CurrentToken.compare_type(token_type.L_PAR))
            {
                Eat(token_type.L_PAR);
                tmp = BoolExpr();
                Eat(token_type.R_PAR);
                return tmp;
            }
            else if (CurrentToken.compare_type(token_type.NOT))
            {
                Eat(token_type.NOT);
                tmp = BoolFact();
                if (tmp.compare_type(token_type.BOOL))
                    tmp.value = !Convert.ToBoolean(tmp.value);
                else
                    Error();
                return tmp;
            }
            else
            {                          
                tmp = RealExpr();
                if ((CurrentToken.compare_type(token_type.LESS)) || (CurrentToken.compare_type(token_type.MORE)) || (CurrentToken.compare_type(token_type.EQUAL_BOOL)))
                {
                    Token tmp2;
                    if (CurrentToken.compare_type(token_type.MORE))
                    {
                        Eat(token_type.MORE);
                        tmp2 = RealExpr();
                        if (Convert.ToDouble(tmp.value) > Convert.ToDouble(tmp2.value))
                            return new Token(token_type.BOOL, true);
                        else
                            return new Token(token_type.BOOL, false);
                    }
                    else if (CurrentToken.compare_type(token_type.LESS))
                    {
                        Eat(token_type.LESS);
                        tmp2 = RealExpr();
                        if (Convert.ToDouble(tmp.value) < Convert.ToDouble(tmp2.value))
                            return new Token(token_type.BOOL, true);
                        else
                            return new Token(token_type.BOOL, false);
                    }
                    else
                    {
                        Eat(token_type.EQUAL_BOOL);
                        tmp2 = RealExpr();
                        if (Math.Abs(Convert.ToDouble(tmp.value) - Convert.ToDouble(tmp2.value)) < delta)
                            return new Token(token_type.BOOL, true);
                        else
                            return new Token(token_type.BOOL, false);
                    }
                }
                return tmp;
            }

        }
        protected Token RealExpr()
        {
            Token res = Term();
            if ((res.compare_type(token_type.INTEGER)) || (res.compare_type(token_type.DOUBLE)))
            {
                while (CurrentToken.compare_type(token_type.PLUS) || (CurrentToken.compare_type(token_type.MINUS)))
                {
                    int sign = 1;
                    if (CurrentToken.compare_type(token_type.MINUS))                    
                        sign = -1;                    
                    Eat(CurrentToken.type);
                    Token tmp = Term();
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
        protected Token Term()
        {
            Token res = Power();
            if ((res.compare_type(token_type.INTEGER)) || (res.compare_type(token_type.DOUBLE)))
            {
                while (CurrentToken.compare_type(token_type.DIV) || CurrentToken.compare_type(token_type.MUL))
                {
                    token_type sign = CurrentToken.type;
                    Eat(sign);
                    Token tmp = Power();
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
        protected Token Power()
        {
            Token res = Fact();
            if (res.compare_type(token_type.INTEGER) || res.compare_type(token_type.DOUBLE))
            {
                while (CurrentToken.compare_type( token_type.POW))
                {
                    Eat(token_type.POW);
                    Token tmp = Fact();
                    res.value = Math.Pow(Convert.ToDouble(res.value), Convert.ToDouble(tmp.value));
                    res.type = token_type.DOUBLE;
                }
            }
            return res;
        }
        protected Token Fact()
        {
            Token tmp;
            if (CurrentToken.compare_type(token_type.INTEGER))
            {
                tmp = CurrentToken;
                Eat(token_type.INTEGER);
                return tmp;
            }
            else if (CurrentToken.compare_type(token_type.DOUBLE))
            {
                tmp = CurrentToken;
                Eat(token_type.DOUBLE);
                return tmp;
            }
            else if (CurrentToken.compare_type(token_type.ID))
            {
                token_type t_type = token_type.EOF;
                object t_value;
                if (VarValues.TryGetValue((string)CurrentToken.value, out t_value) && VarTypes.TryGetValue((string)CurrentToken.value, out t_type))
                {
                    Eat(token_type.ID);
                    return new Token(t_type, t_value);
                }
                else
                {
                    Error();
                    return CurrentToken;
                }
            }
            else if (CurrentToken.compare_type(token_type.L_PAR))
            {
                Eat(token_type.L_PAR);
                tmp = RealExpr();
                Eat(token_type.R_PAR);
                return tmp;
            }
            else
                Error();
            return CurrentToken;
        }
        protected void IfSentense()
        {
            Eat(token_type.IF);
            Eat(token_type.L_PAR);
            Token result = BoolExpr();
            Eat(token_type.R_PAR);
            if (result.compare_type(token_type.BOOL))
            {
                if (Convert.ToBoolean(result.value))
                {
                    if (CurrentToken.compare_type(token_type.L_FIGURE))
                        Block();
                    else
                        Sentense();
                }
                else
                {
                    if (CurrentToken.compare_type(token_type.L_FIGURE))
                        SkipBlock();
                    else
                        SkipSentense();
                }                                          
                
                if (CurrentToken.compare_type(token_type.ELSE))
                {
                    Eat(token_type.ELSE);
                    if (!Convert.ToBoolean(result.value))
                    {
                        if (CurrentToken.compare_type(token_type.L_FIGURE))
                            Block();
                        else
                            Sentense();
                    }
                    else
                    {
                        if (CurrentToken.compare_type(token_type.L_FIGURE))
                            SkipBlock();
                        else
                            SkipSentense();
                    }
                }                
            }
            else            
                Error();
            
        }        
        protected void WhileSentense()
        {
            int begin = CurrentPosition;
            Eat(token_type.WHILE);
            Eat(token_type.L_PAR);
            Token result = BoolExpr();
            Eat(token_type.R_PAR);
            if (!result.compare_type(token_type.BOOL))
                Error();
            if(Convert.ToBoolean(result.value))
            {
                if (CurrentToken.compare_type(token_type.L_FIGURE))
                    Block();
                else
                    Sentense();
                Eat(token_type.SEMI);
                CurrentPosition = begin;
                WhileSentense();
            }
            else
            {
                if (CurrentToken.compare_type(token_type.L_FIGURE))
                    SkipBlock();
                else
                    SkipSentense();
            }
            
        }
        protected void FunctionCallSentense()
        {
            if (CurrentToken.compare_type(token_type.WRITE_FUNCTION))
                write_function();
            else if (CurrentToken.compare_type(token_type.READ_FUNCTION))                
                read_function();
        }        
        protected string write()
        {
            Eat(token_type.WRITE_FUNCTION);
            Eat(token_type.L_PAR);
            Token tmp = Expression();
            Eat(token_type.R_PAR);
            return tmp.value.ToString();
        }
        public abstract void write_function();
        public abstract void read_function();

    }
}
