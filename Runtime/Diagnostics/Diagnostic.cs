namespace IllusionScript.Runtime.Diagnostics
{
    public sealed class Diagnostic
    {
        public readonly string text;
        public readonly TextSpan span;

        internal Diagnostic(string text, TextSpan span)
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