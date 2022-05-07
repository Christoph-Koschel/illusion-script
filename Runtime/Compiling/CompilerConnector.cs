using IllusionScript.Runtime.Binding;

namespace IllusionScript.Runtime.Compiling
{
    public abstract class CompilerConnector
    {
        protected string baseDir;
        public readonly string hashID;
        public string name { get; }
        private static int hashIdCount = 0x11;

        protected CompilerConnector()
        {
            hashIdCount <<= 0x1;
            hashID = "0x" + hashIdCount.ToString("X");
        }

        public void setBaseDir(string baseDir)
        {
            this.baseDir = baseDir;
        }

        public abstract bool BuildOutput();

        internal abstract bool Build(BoundProgram program);

        public abstract bool CleanUp();
    }
}