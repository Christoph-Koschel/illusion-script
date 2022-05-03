namespace IllusionScript.ISI
{
    internal static class Program
    {
        internal static string[] args;

        public static void Main(string[] args)
        {
            Program.args = args;
            IlsRepl repl = new IlsRepl();
            repl.Run();

            // Disable console closing
            end: // <<-- TODO Remove
            goto end; // <<-- TODO Remove
        }
    }
}