﻿using System;
using IllusionScript.Runtime.Diagnostics;
using IllusionScript.Runtime.Parsing;

namespace IllusionScript.Runtime.Lexing
{
    public class Lexer : SyntaxFacts
    {
        private readonly SourceText text;
        private readonly DiagnosticGroup diagnostics;

        private int position;

        private SyntaxType type;
        private int start;
        private object value;

        private char current => Peek(0);

        internal Lexer(SourceText text)
        {
            diagnostics = new DiagnosticGroup();
            this.text = text;
        }


        internal DiagnosticGroup Diagnostics() => diagnostics;

        private char Peek(int offset)
        {
            int index = position + offset;

            if (index >= text.Length)
            {
                return '\0';
            }

            return text[index];
        }

        public Token Lex()
        {
            start = position;
            type = SyntaxType.BadToken;
            value = null;


            switch (current)
            {
                case '\0':
                    type = SyntaxType.EOFToken;
                    break;
                case ':':
                    type = SyntaxType.ColonToken;
                    position++;
                    break;
                case '~':
                    type = SyntaxType.TildeToken;
                    position++;
                    break;
                case ',':
                    type = SyntaxType.CommaToken;
                    position++;
                    break;
                case '+':
                    type = SyntaxType.PlusToken;
                    position++;
                    if (current == '=')
                    {
                        position++;
                        type = SyntaxType.PlusEqualsToken;
                    }

                    break;
                case '-':
                    type = SyntaxType.MinusToken;
                    position++;
                    if (current == '=')
                    {
                        position++;
                        type = SyntaxType.MinusEqualsToken;
                    }

                    break;
                case '*':
                    type = SyntaxType.StarToken;
                    position++;
                    if (current == '*')
                    {
                        position++;
                        if (current == '=')
                        {
                            position++;
                            type = SyntaxType.DoubleStarEqualsToken;
                        }
                        else
                        {
                            type = SyntaxType.DoubleStarToken;
                        }
                    }
                    else if (current == '=')
                    {
                        position++;
                        type = SyntaxType.StarEqualsToken;
                    }

                    break;
                case '/':
                    type = SyntaxType.SlashToken;
                    position++;
                    if (current == '=')
                    {
                        position++;
                        type = SyntaxType.SlashEqualsToken;
                    }

                    break;
                case '%':
                    type = SyntaxType.PercentToken;
                    position++;
                    if (current == '=')
                    {
                        position++;
                        type = SyntaxType.PercentEqualsToken;
                    }

                    break;
                case '^':
                    type = SyntaxType.HatToken;
                    position++;
                    if (current == '=')
                    {
                        position++;
                        type = SyntaxType.HatEqualsToken;
                    }

                    break;
                case '(':
                    type = SyntaxType.LParenToken;
                    position++;
                    break;
                case ')':
                    type = SyntaxType.RParenToken;
                    position++;
                    break;
                case '{':
                    type = SyntaxType.LBraceToken;
                    position++;
                    break;
                case '}':
                    type = SyntaxType.RBraceToken;
                    position++;
                    break;
                case '&':
                    type = SyntaxType.AndToken;
                    position++;
                    if (current == '&')
                    {
                        position++;
                        type = SyntaxType.DoubleAndToken;
                    }
                    else if (current == '=')
                    {
                        position++;
                        type = SyntaxType.AndEqualsToken;
                    }

                    break;
                case '|':
                    type = SyntaxType.SplitToken;
                    position++;
                    if (current == '|')
                    {
                        position++;
                        type = SyntaxType.DoubleSplitToken;
                    }
                    else if (current == '=')
                    {
                        position++;
                        type = SyntaxType.SplitEqualsToken;
                    }

                    break;
                case '=':
                    position++;
                    if (current != '=')
                    {
                        type = SyntaxType.EqualsToken;
                    }
                    else
                    {
                        type = SyntaxType.DoubleEqualsToken;
                        position++;
                    }

                    break;
                case '<':
                    type = SyntaxType.LessToken;
                    position++;
                    if (current == '=')
                    {
                        position++;
                        type = SyntaxType.LessEqualsToken;
                    } else if (current == '<')
                    {
                        position++;
                        type = SyntaxType.DoubleLessToken;
                    }

                    break;
                case '>':
                    type = SyntaxType.GreaterToken;
                    position++;
                    if (current == '=')
                    {
                        position++;
                        type = SyntaxType.GreaterEqualsToken;
                    } else if (current == '>')
                    {
                        position++;
                        type = SyntaxType.DoubleGreaterToken;
                    }

                        break;
                case '!':
                    position++;
                    if (current != '=')
                    {
                        type = SyntaxType.BangToken;
                    }
                    else
                    {
                        position++;
                        type = SyntaxType.BangEqualsToken;
                    }

                    break;
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
                    GenerateNumber();
                    break;
                case ' ':
                case '\t':
                case '\n':
                case '\r':
                    GenerateWhiteSpace();
                    break;
                default:
                    if (char.IsLetter(current))
                    {
                        GenerateKeyword();
                    }
                    else
                    {
                        diagnostics.ReportBadCharacter(position, current);
                        position++;
                    }

                    break;
            }


            string text = GetText(type);
            if (text == null)
            {
                int length = position - start;
                text = this.text.ToString(start, length);
            }

            return new Token(type, start, text, value);
        }

        private void GenerateKeyword()
        {
            while (char.IsLetter(current))
            {
                position++;
            }

            int length = position - start;
            string text = this.text.ToString(start, length);
            type = GetKeywordType(text);
        }

        private void GenerateWhiteSpace()
        {
            while (char.IsWhiteSpace(current))
            {
                position++;
            }

            type = SyntaxType.WhiteSpaceToken;
        }

        private void GenerateNumber()
        {
            while (char.IsDigit(current))
            {
                position++;
            }

            int length = position - start;
            string text = this.text.ToString(start, length);
            if (!int.TryParse(text, out int value))
            {
                diagnostics.ReportInvalidNumber(new TextSpan(start, length), text, typeof(int));
            }

            this.value = value;
            type = SyntaxType.NumberToken;
        }
    }
}