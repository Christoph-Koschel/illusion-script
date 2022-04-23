namespace IllusionScript
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            IlsRepl repl = new IlsRepl();
            repl.Run();
        }
    }
}