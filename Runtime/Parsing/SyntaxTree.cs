using System.Collections.Generic;
using System.Collections.Immutable;
using IllusionScript.Runtime.Diagnostics;
using IllusionScript.Runtime.Lexing;
using IllusionScript.Runtime.Parsing.Nodes;

namespace IllusionScript.Runtime.Parsing
{
    public class SyntaxTree
    {
        public readonly SourceText text;
        public readonly CompilationUnit root;
        public readonly ImmutableArray<Diagnostic> diagnostics;

        private SyntaxTree(SourceText text)
        {
            Parser parser = new Parser(text);
            CompilationUnit root = parser.ParseCompilationUnit();
            
            diagnostics = parser.Diagnostics.ToImmutableArray();
            this.text = text;
            this.root = root;
        }

        public static SyntaxTree Parse(SourceText text)
        {
            return new SyntaxTree(text);
        }

        public static SyntaxTree Parse(string text)
        {
            SourceText sourceText = SourceText.From(text);
            return Parse(sourceText);
        }

        public static IEnumerable<Token> ParseTokens(string text)
        {
            SourceText sourceText = SourceText.From(text);
            return ParseTokens(sourceText);
        }

        public static IEnumerable<Token> ParseTokens(SourceText text)
        {
            Lexer lexer = new Lexer(text);
            while (true)
            {
                Token token = lexer.Lex();
                if (token.type == SyntaxType.EOFToken)
                {
                    break;
                }

                yield return token;
            }
        }
    }
}