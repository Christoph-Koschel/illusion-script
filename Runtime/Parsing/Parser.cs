using System.Collections.Generic;
using IllusionScript.Runtime.Diagnostics;
using IllusionScript.Runtime.Lexing;

namespace IllusionScript.Runtime.Parsing
{
    internal sealed class Parser
    {
        public readonly DiagnosticGroup diagnostics;
        public readonly IEnumerable<Token> tokens;

        public Parser(SourceText source)
        {
            Lexer lexer = new Lexer(source);
            List<Token> tokens = new List<Token>();
            do
            {
                tokens.Add(lexer.Lex());
            } while (tokens[^1].type != SyntaxType.EOFToken);

            diagnostics = new DiagnosticGroup();
            diagnostics.AddRange(lexer.diagnostics);

            this.tokens = tokens.ToArray();
        }
    }
}