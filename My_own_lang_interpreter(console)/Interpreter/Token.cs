using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace my_own_lang_interpretator
{
    enum token_type
    {
        EOF,
        SEMI,
        PLUS,
        ASSIGN,
        EQUAL_BOOL,
        ID,
        BOOL,
        BOOL_TYPE,
        DOUBLE_TYPE,
        INT_TYPE,
        STRING_TYPE,
        CHAR_TYPE,
        WRITE_FUNCTION,
        READ_FUNCTION,
        IF,
        WHILE,
        INTEGER,
        DOUBLE,
        CHAR,
        STRING,
        COMA,
        EQUAL,
        DIZ,
        CON,
        L_PAR,
        R_PAR,
        LESS,
        MORE,
        MINUS,
        MUL,
        DIV,
        POW,
        COLON,
        DOT,
        L_FIGURE,
        QUOTE,
        R_FIGURE,
        D_QUOTE,
        ELSE,
    }
    class Token
    {
        public token_type type;
        public object value;
        public Token(token_type type,object value = null)
        {
            this.type = type;
            this.value = value;
        }
        public bool compare_type(token_type compare_type)
        {
            return compare_type == type;
        }
        public bool is_type()
        {
            return (this.type == token_type.BOOL_TYPE) || (type == token_type.DOUBLE_TYPE) || (type == token_type.INT_TYPE) || (type == token_type.CHAR_TYPE)|| (type == token_type.STRING_TYPE);
        }
        public bool is_function()
        {
            return (type == token_type.WRITE_FUNCTION) || (type == token_type.READ_FUNCTION);
        }
        public override string ToString()
        {
            if (compare_type(token_type.DOUBLE) || compare_type(token_type.INTEGER) || compare_type(token_type.STRING) || compare_type(token_type.BOOL))
                return value.ToString();
            else
                return "NotValue";
        }
    }
}
