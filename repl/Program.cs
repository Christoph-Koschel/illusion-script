namespace IllusionScript
{
    internal static class Program
    {
        internal static string[] args;
        
        public static void Main(string[] args)
        {
            Program.args = args;
            IlsRepl repl = new IlsRepl();
            repl.Run();
        }
    }
}