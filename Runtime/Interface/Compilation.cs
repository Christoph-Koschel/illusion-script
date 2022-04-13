using System.Collections.Generic;
using IllusionScript.Runtime.Diagnostics;
using IllusionScript.Runtime.Lexing;
using IllusionScript.Runtime.Parsing;

namespace IllusionScript.Runtime.Interface
{
    public sealed class Compilation
    {
        public readonly DiagnosticGroup diagnostics;
        public readonly IEnumerable<Token> tokens;

        private Compilation(DiagnosticGroup diagnostics, IEnumerable<Token> tokens)
        {
            this.diagnostics = diagnostics;
            this.tokens = tokens;
        }

        public static Compilation Parse(SourceText source)
        {
            Parser parser = new Parser(source);

            return new Compilation(parser.diagnostics, parser.tokens);
        }

        public static Compilation Parse(string text)
        {
            SourceText source = new SourceText(text);
            return Parse(source);
        }
    }
}