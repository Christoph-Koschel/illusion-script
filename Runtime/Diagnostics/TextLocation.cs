namespace IllusionScript.Runtime.Diagnostics
{
    public struct TextLocation
    {
        public readonly SourceText text;
        public readonly TextSpan span;
        public int startLine => text.GetLineIndex(span.start);
        public int endLine => text.GetLineIndex(span.end);
        public int startCharacter => span.start - text.lines[startLine].start;
        public int endCharacter => span.end - text.lines[startLine].end;

        public TextLocation(SourceText text, TextSpan span)
        {
            this.text = text;
            this.span = span;
        }
    }
}