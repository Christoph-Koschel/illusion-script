using System.Collections.Generic;
using IllusionScript.Runtime.Diagnostics;
using IllusionScript.Runtime.Interface;
using IllusionScript.Runtime.Lexing;
using IllusionScript.Runtime.Parsing.Nodes;

namespace IllusionScript.Runtime.Parsing
{
    public sealed class SyntaxTree
    {
        public readonly DiagnosticGroup diagnostics;
        public readonly SourceText text;
        public readonly Expression root;

        private SyntaxTree(SourceText text)
        {
            Parser parser = new Parser(text);
            Expression root = parser.ParseText();

            diagnostics = parser.diagnostics;
            this.root = root;
            this.text = text;
        }

        public static Compilation Parse(SourceText source)
        {
            SyntaxTree syntaxTree = new SyntaxTree(source);
            return new Compilation(syntaxTree.diagnostics, syntaxTree);
        }

        public static Compilation Parse(string text)
        {
            SourceText source = new SourceText(text);
            return Parse(source);
        }

        public static IEnumerable<Token> MakeTokens(SourceText text)
        {
            Lexer lexer = new Lexer(text);
            Token token;
            do
            {
                token = lexer.Lex();
                if (token.type != SyntaxType.EOFToken)
                {
                    yield return token;
                }
            } while (token.type != SyntaxType.EOFToken);
        }

        public static IEnumerable<Token> MakeTokens(string text)
        {
            SourceText source = new SourceText(text);
            return MakeTokens(source);
        }
    }
}