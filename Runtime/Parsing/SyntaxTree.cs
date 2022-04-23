using System.Collections.Generic;
using System.Collections.Immutable;
using IllusionScript.Runtime.Diagnostics;
using IllusionScript.Runtime.Lexing;
using IllusionScript.Runtime.Parsing.Nodes;
using Microsoft.VisualBasic;

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
        
        public static ImmutableArray<Token> ParseTokens(string text,out ImmutableArray<Diagnostic> diagnostics)
        {
            SourceText sourceText = SourceText.From(text);
            return ParseTokens(sourceText, out diagnostics);
        }


        public static ImmutableArray<Token> ParseTokens(SourceText text, out ImmutableArray<Diagnostic> diagnostics)
        {
            IEnumerable<Token> LexTokens(Lexer lexer)
            {
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

            Lexer l = new Lexer(text);
            ImmutableArray<Token> result = LexTokens(l).ToImmutableArray();
            diagnostics = l.Diagnostics().ToImmutableArray();
            return result;
        }
    }
}