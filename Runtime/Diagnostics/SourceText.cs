using System.Collections.Immutable;

namespace IllusionScript.Runtime.Diagnostics
{
    public sealed partial class SourceText
    {
        private readonly string text;
        public readonly ImmutableArray<TextLine> lines;

        public int GetLineIndex(int position)
        {
            int lower = 0;
            int upper = lines.Length - 1;
        
            while (lower <= upper)
            {
                int index = lower + (upper - lower) / 2;
                int start = lines[index].start;
        
                if (position == start)
                {
                    return index;
                }
                else if (position > start)
                {
                    upper = index - 1;
                }
                else
                {
                    lower = index + 1;
                }
            }
        
            return lower - 1;
        }

        private SourceText(string text)
        {
            this.text = text;
            lines = ParseLines(this, text);
        }

        private ImmutableArray<TextLine> ParseLines(SourceText sourceText, string text)
        {
            ImmutableArray<TextLine>.Builder result = ImmutableArray.CreateBuilder<TextLine>();

            int position = 0;
            int lineStart = 0;

            while (position < text.Length)
            {
                int lineBreakWidth = GetLineBreakWidth(text, position);

                if (lineBreakWidth == 0)
                {
                    position++;
                }
                else
                {
                    AddLine(result, sourceText, position, lineStart, lineBreakWidth);

                    position += lineBreakWidth;
                    lineStart = position;
                }
            }

            if (position >= lineStart)
                AddLine(result, sourceText, position, lineStart, 0);

            return result.ToImmutable();
        }

        private static void AddLine(ImmutableArray<TextLine>.Builder result, SourceText sourceText, int position,
            int lineStart, int lineBreakWidth)
        {
            int lineLength = position - lineStart;
            int lineLengthIncludingLineBreak = lineLength + lineBreakWidth;
            TextLine line = new TextLine(sourceText, lineStart, lineLength, lineLengthIncludingLineBreak);
            result.Add(line);
        }

        private static int GetLineBreakWidth(string text, int position)
        {
            char c = text[position];
            char l = position + 1 >= text.Length ? '\0' : text[position + 1];

            if (c == '\r' && l == '\n')
                return 2;

            if (c is '\r' or '\n')
                return 1;

            return 0;
        }


        public override string ToString() => text;

        public string ToString(int start, int length) => text.Substring(start, length);

        public string ToString(TextSpan span) => text.Substring(span.start, span.length);
        public char this[int index] => text[index];

        public int Length => text.Length;

        public static SourceText From(string text)
        {
            return new SourceText(text);
        }
    }
}