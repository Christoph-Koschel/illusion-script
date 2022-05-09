using System.IO;
using IllusionScript.Runtime.Binding;

namespace IllusionScript.Runtime.Compiling
{
    public abstract class CompilerConnector
    {
        protected string baseDir;
        protected TextWriter writer;
        public readonly string hashID;
        public abstract string name { get; }
        private static int hashIdCount = 0x11;

        protected CompilerConnector()
        {
            writer = new StringWriter();
            hashIdCount <<= 0x1;
            hashID = "0x" + hashIdCount.ToString("X");
        }

        public void setBaseDir(string baseDir)
        {
            this.baseDir = baseDir;
        }

        public abstract bool BuildOutput();

        public abstract bool BuildCore();

        public abstract bool Build(Compilation compilation, BoundProgram program);

        public abstract bool CleanUp();

        public abstract string SyscallBinder();

        public void setOutput(TextWriter writer)
        {
            this.writer = writer;
        }

        public static int DetectTarget(string tar, int def)
        {
            return tar.ToLower() switch
            {
                "exe" => Executable,
                "lib" => Library,
                "library" => Library,
                _ => def
            };
        }

        public const int Executable = 0;
        public const int Library = 1;
    }
}