namespace IllusionScript.Runtime.Diagnostics
{
    public sealed class Diagnostic
    {
        public readonly string text;
        public readonly TextSpan span;

        internal Diagnostic(TextSpan span, string text)
        {
            this.text = text;
            this.span = span;
        }

        public override string ToString()
        {
            return text;
        }
    }
}