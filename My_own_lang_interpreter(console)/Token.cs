using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My_own_lang_interpreter_console_
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
        TYPE_BOOL,
        TYPE_DBL,
        TYPE_INT,
        TYPE_STRING,
        TYPE_CHAR,
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
            return (this.type == token_type.TYPE_BOOL) || (type == token_type.TYPE_DBL) || (type == token_type.TYPE_INT) || (type == token_type.TYPE_CHAR)|| (type == token_type.TYPE_STRING);
        }
        public bool is_function()
        {
            return (type == token_type.WRITE_FUNCTION) || (type == token_type.READ_FUNCTION);
        }
        public bool is_value()
        {
            return compare_type(token_type.DOUBLE) || compare_type(token_type.INTEGER) || compare_type(token_type.STRING) || compare_type(token_type.BOOL);               
        }
        public override string ToString()
        {
            if (this.is_value())
                return value.ToString();
            else
                return this.type.ToString();
        }
    }
}
