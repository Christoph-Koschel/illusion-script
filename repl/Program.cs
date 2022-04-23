using System;

namespace IllusionScript
{
    internal static class Program
    {
        internal static string[] args;
        
        public static void Main(string[] args)
        {
            Program.args = args;
            try
            {
                IlsRepl repl = new IlsRepl();
                repl.Run();
            }
            catch (Exception err)
            {
                Console.WriteLine(err);
            } 

            // Disable console closing
            end: // <<-- TODO Remove
            goto end; // <<-- TODO Remove
        }
    }
}