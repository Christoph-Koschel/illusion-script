using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using IllusionScript.Runtime.Diagnostics;
using IllusionScript.Runtime.Parsing;

namespace IllusionScript.Runtime.Extension
{
    public static class TextWriterExtensions
    {
        private static bool IsConsole(this TextWriter writer)
        {
            if (writer == Console.Out)
                return !Console.IsOutputRedirected;

            if (writer == Console.Error)
                return !Console.IsErrorRedirected &&
                       !Console.IsOutputRedirected;

            if (writer is IndentedTextWriter iw && iw.InnerWriter.IsConsole())
                return true;

            return false;
        }

        private static void SetForeground(this TextWriter writer, ConsoleColor color)
        {
            if (writer.IsConsole())
                Console.ForegroundColor = color;
        }

        private static void ResetColor(this TextWriter writer)
        {
            if (writer.IsConsole())
                Console.ResetColor();
        }

        public static void WriteKeyword(this TextWriter writer, SyntaxType type)
        {
            string? text = SyntaxFacts.GetText(type);

            writer.WriteKeyword(text);
        }

        public static void WriteKeyword(this TextWriter writer, string text)
        {
            writer.SetForeground(ConsoleColor.Blue);
            writer.Write(text);
            writer.ResetColor();
        }

        public static void WriteIdentifier(this TextWriter writer, string text)
        {
            writer.SetForeground(ConsoleColor.Gray);
            writer.Write(text);
            writer.ResetColor();
        }

        public static void WriteNumber(this TextWriter writer, string text)
        {
            writer.SetForeground(ConsoleColor.DarkMagenta);
            writer.Write(text);
            writer.ResetColor();
        }

        public static void WriteString(this TextWriter writer, string text)
        {
            writer.SetForeground(ConsoleColor.Cyan);
            writer.Write(text);
            writer.ResetColor();
        }

        public static void WriteSpace(this TextWriter writer)
        {
            writer.WritePunctuation(" ");
        }

        public static void WritePunctuation(this TextWriter writer, SyntaxType kind)
        {
            string? text = SyntaxFacts.GetText(kind);
            writer.WritePunctuation(text);
        }

        public static void WritePunctuation(this TextWriter writer, string text)
        {
            writer.SetForeground(ConsoleColor.DarkGray);
            writer.Write(text);
            writer.ResetColor();
        }

        public static void WriteDiagnostics(this TextWriter writer, IEnumerable<Diagnostic> diagnostics,
            SyntaxTree tree)
        {
            foreach (Diagnostic diagnostic in diagnostics)
            {
                var span = diagnostic.location.span;
                int lineIndex = tree.text.GetLineIndex(span.start);
                // TextLine line = text.lines[lineIndex];
                // int lineNumber = lineIndex + 1;
                // int character = diagnostic.span.start - line.start + 1;
                //
                // Console.ForegroundColor = ConsoleColor.DarkRed;
                // Console.Write($"({lineNumber}, {character}): ");
                Console.WriteLine(diagnostic);
                Console.ResetColor();

                // TextSpan prefixSpan = TextSpan.FromBounds(line.start, diagnostic.span.start);
                // TextSpan suffixSpan = TextSpan.FromBounds(diagnostic.span.end, line.end);
                //
                // string prefix = syntaxThree.text.ToString(prefixSpan);
                // string error = syntaxThree.text.ToString(diagnostic.span);
                // string suffix = syntaxThree.text.ToString(suffixSpan);
                //
                // Console.Write(prefix);
                //
                // Console.ForegroundColor = ConsoleColor.DarkRed;
                // Console.Write(error);
                // Console.ResetColor();
                //
                // Console.Write(suffix);
                Console.Write("\n\n");
            }
        }
    }
}