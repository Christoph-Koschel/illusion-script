using System;
using System.Text;
using IllusionScript.Runtime.Diagnostics;
using IllusionScript.Runtime.Parsing;

namespace IllusionScript.Runtime.Lexing
{
    public sealed class Lexer
    {
        private SourceText source;
        public readonly DiagnosticGroup diagnostics;
        private int position;
        private char current => Peek(0);
        private char next => Peek(1);

        internal Lexer(SourceText source)
        {
            this.source = source;
            diagnostics = new DiagnosticGroup();
            position = 0;
        }

        private char Peek(int offset)
        {
            int index = position + offset;

            if (index >= source.text.Length)
            {
                return '\0';
            }

            return source.text[index];
        }

        public Token Lex()
        {
            int start = position;

            switch (current)
            {
                case '\0':
                    return new Token(SyntaxType.EOFToken, start, "", null);
                case '\r':
                case '\n':
                case '\t':
                case ' ':
                    return CreateWhitespaces();
                case ',':
                    position++;
                    return new Token(SyntaxType.CommaToken, start, ",", null);
                case '"':
                    return CreateString();
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return CreateNumber();
                case '+':
                    position++;
                    if (current == '=')
                    {
                        position++;
                        return new Token(SyntaxType.PlusEqualsToken, start, "+=", null);
                    }

                    return new Token(SyntaxType.PlusToken, start, "+", null);
                case '-':
                    position++;
                    if (current == '=')
                    {
                        position++;
                        return new Token(SyntaxType.MinusEqualsToken, start, "-=", null);
                    }

                    return new Token(SyntaxType.MinusToken, start, "-", null);
                case '*':
                    position++;
                    if (current == '*')
                    {
                        position++;
                        if (current == '=')
                        {
                            position++;
                            return new Token(SyntaxType.DoubleStarEqualsToken, start, "**=", null);
                        }

                        return new Token(SyntaxType.DoubleStarToken, start, "**", null);
                    }
                    else if (current == '=')
                    {
                        position++;
                        return new Token(SyntaxType.StarEqualsToken, start, "*=", null);
                    }

                    return new Token(SyntaxType.StarToken, start, "*", null);
                case '/':
                    position++;
                    if (current == '=')
                    {
                        position++;
                        return new Token(SyntaxType.SlashEqualsToken, start, "/=", null);
                    }

                    return new Token(SyntaxType.SlashToken, start, "/", null);
                case '%':
                    position++;
                    if (current == '=')
                    {
                        position++;
                        return new Token(SyntaxType.PercentEqualsToken, start, "%=", null);
                    }

                    return new Token(SyntaxType.PercentToken, start, "%", null);
                case '(':
                    position++;
                    return new Token(SyntaxType.LParenToken, start, "(", null);
                case ')':
                    position++;
                    return new Token(SyntaxType.RParenToken, start, ")", null);
                case '{':
                    position++;
                    return new Token(SyntaxType.LBraceToken, start, "{", null);
                case '}':
                    position++;
                    return new Token(SyntaxType.RBraceToken, start, "}", null);
                case '&':
                    position++;
                    if (current == '&')
                    {
                        position++;
                        return new Token(SyntaxType.DoubleAndToken, start, "&&", null);
                    }
                    else if (current == '=')
                    {
                        position++;
                        return new Token(SyntaxType.AndEqualsToken, start, "&=", null);
                    }

                    return new Token(SyntaxType.AndToken, start, "&", null);
                case '|':
                    position++;
                    if (current == '|')
                    {
                        position++;
                        return new Token(SyntaxType.DoubleSplitToken, start, "||", null);
                    }
                    else if (current == '=')
                    {
                        position++;
                        return new Token(SyntaxType.SplitEqualsToken, start, "|=", null);
                    }

                    return new Token(SyntaxType.SplitToken, start, "|", null);
                case '^':
                    position++;
                    if (current == '=')
                    {
                        position++;
                        return new Token(SyntaxType.HatEqualsToken, start, "^=", null);
                    }

                    return new Token(SyntaxType.HatToken, start, "^", null);
                case '=':
                    position++;
                    if (current == '=')
                    {
                        position++;
                        return new Token(SyntaxType.DoubleEqualsToken, start, "==", null);
                    }

                    return new Token(SyntaxType.EqualsToken, start, "=", null);
                case '!':
                    position++;
                    if (current == '=')
                    {
                        position++;
                        return new Token(SyntaxType.BangEqualsToken, start, "!=", null);
                    }

                    return new Token(SyntaxType.BangToken, start, "!", null);
                case ':':
                    position++;
                    return new Token(SyntaxType.ColonToken, start, ":", null);
                case '<':
                    position++;
                    if (current == '=')
                    {
                        position++;
                        return new Token(SyntaxType.LessEqualsToken, start, "<=", null);
                    }

                    return new Token(SyntaxType.LessToken, start, "<", null);

                case '>':
                    position++;
                    if (current == '=')
                    {
                        position++;
                        return new Token(SyntaxType.GreaterEqualsToken, start, ">=", null);
                    }

                    return new Token(SyntaxType.GreaterToken, start, ">", null);
                case '~':
                    position++;
                    return new Token(SyntaxType.TildeToken, start, "~", null);
                default:
                    if (char.IsLetter(current))
                    {
                        return CreateText();
                    }

                    break;
            }

            diagnostics.ReportBadCharacter(start, current);
            position++;
            return new Token(SyntaxType.BadToken, start, current.ToString(), null);
        }

        private Token CreateString()
        {
            int start = position;
            // Skip the current quote
            position++;

            StringBuilder sb = new StringBuilder();
            bool done = false;

            while (!done)
            {
                switch (current)
                {
                    case '\0':
                    case '\r':
                    case '\n':
                        TextSpan span = new TextSpan(start, 1);
                        diagnostics.ReportUnterminatedString(span);
                        done = true;
                        break;
                    case '"':
                        done = true;
                        position++;
                        break;
                    case '\\':
                        if (next == '"')
                        {
                            sb.Append('"');
                            position += 2;
                        }
                        else if (next == '\\')
                        {
                            sb.Append('\\');
                        }

                        break;
                    default:
                        sb.Append(current);
                        position++;
                        break;
                }
            }

            string value = sb.ToString();

            return new Token(SyntaxType.StringToken, start, '"' + value.Replace("\"", "\\\"") + '"', value);
        }

        private Token CreateNumber()
        {
            StringBuilder builder = new StringBuilder();
            int start = position;

            while (char.IsDigit(current))
            {
                builder.Append(current);
                position++;
            }

            if (int.TryParse(builder.ToString(), out int number))
            {
                return new Token(SyntaxType.NumberToken, start, builder.ToString(), number);
            }
            else
            {
                diagnostics.ReportInvalidNumber(new TextSpan(start, builder.Length), builder.ToString(), typeof(int));
                return new Token(SyntaxType.BadToken, start, builder.ToString(), null);
            }
        }

        private Token CreateWhitespaces()
        {
            int start = position;
            StringBuilder builder = new StringBuilder();

            while (char.IsWhiteSpace(current))
            {
                builder.Append(current);
                position++;
            }

            return new Token(SyntaxType.WhiteSpaceToken, start, builder.ToString(), null);
        }

        private Token CreateText()
        {
            int start = position;
            StringBuilder builder = new StringBuilder();

            while (char.IsLetter(current))
            {
                builder.Append(current);
                position++;
            }

            SyntaxType type = GetKeywordType(builder.ToString());

            return new Token(type, start, builder.ToString(), null);
        }

        public static string? GetText(SyntaxType type)
        {
            switch (type)
            {
                case SyntaxType.PlusToken:
                    return "+";
                case SyntaxType.PlusEqualsToken:
                    return "+=";
                case SyntaxType.MinusToken:
                    return "-";
                case SyntaxType.MinusEqualsToken:
                    return "-=";
                case SyntaxType.DoubleStarToken:
                    return "**";
                case SyntaxType.StarToken:
                    return "*";
                case SyntaxType.StarEqualsToken:
                    return "*=";
                case SyntaxType.SlashToken:
                    return "/";
                case SyntaxType.SlashEqualsToken:
                    return "/=";
                case SyntaxType.PercentToken:
                    return "%";
                case SyntaxType.PercentEqualsToken:
                    return "%=";
                case SyntaxType.BangToken:
                    return "!";
                case SyntaxType.EqualsToken:
                    return "=";
                case SyntaxType.TildeToken:
                    return "~";
                case SyntaxType.LessToken:
                    return "<";
                case SyntaxType.LessEqualsToken:
                    return "<=";
                case SyntaxType.GreaterToken:
                    return ">";
                case SyntaxType.GreaterEqualsToken:
                    return ">=";
                case SyntaxType.AndToken:
                    return "&";
                case SyntaxType.DoubleAndToken:
                    return "&&";
                case SyntaxType.AndEqualsToken:
                    return "&=";
                case SyntaxType.SplitToken:
                    return "|";
                case SyntaxType.SplitEqualsToken:
                    return "|=";
                case SyntaxType.DoubleSplitToken:
                    return "||";
                case SyntaxType.HatToken:
                    return "^";
                case SyntaxType.HatEqualsToken:
                    return "^=";
                case SyntaxType.DoubleEqualsToken:
                    return "==";
                case SyntaxType.BangEqualsToken:
                    return "!=";
                case SyntaxType.LParenToken:
                    return "(";
                case SyntaxType.RParenToken:
                    return ")";
                case SyntaxType.LBraceToken:
                    return "{";
                case SyntaxType.RBraceToken:
                    return "}";
                case SyntaxType.ColonToken:
                    return ":";
                case SyntaxType.CommaToken:
                    return ",";
                case SyntaxType.BreakKeyword:
                    return "break";
                case SyntaxType.ContinueKeyword:
                    return "continue";
                case SyntaxType.ElseKeyword:
                    return "else";
                case SyntaxType.FalseKeyword:
                    return "false";
                case SyntaxType.ForKeyword:
                    return "for";
                case SyntaxType.FunctionKeyword:
                    return "define";
                case SyntaxType.IfKeyword:
                    return "if";
                case SyntaxType.LetKeyword:
                    return "let";
                case SyntaxType.ReturnKeyword:
                    return "return";
                case SyntaxType.ToKeyword:
                    return "to";
                case SyntaxType.TrueKeyword:
                    return "true";
                case SyntaxType.WhileKeyword:
                    return "while";
                case SyntaxType.DoKeyword:
                    return "do";
                default:
                    Console.WriteLine(type + " is not defined in GetText");
                    return null;
            }
        }

        public static SyntaxType GetKeywordType(string text)
        {
            return text switch
            {
                "true" => SyntaxType.TrueKeyword,
                "false" => SyntaxType.FalseKeyword,
                "break" => SyntaxType.BreakKeyword,
                "continue" => SyntaxType.ContinueKeyword,
                "define" => SyntaxType.FunctionKeyword,
                "do" => SyntaxType.DoKeyword,
                "else" => SyntaxType.ElseKeyword,
                "for" => SyntaxType.ForKeyword,
                "if" => SyntaxType.IfKeyword,
                "let" => SyntaxType.LetKeyword,
                "return" => SyntaxType.ReturnKeyword,
                "to" => SyntaxType.ToKeyword,
                "while" => SyntaxType.WhileKeyword,
                _ => SyntaxType.IdentifierToken
            };
        }
    }
}