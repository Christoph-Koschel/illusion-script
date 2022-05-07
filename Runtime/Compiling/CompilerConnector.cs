namespace IllusionScript.Runtime.Compiling
{
    public abstract class CompilerConnector
    {
        protected readonly string baseDir;
        public readonly string hashID;
        public string name { get; }
        private static int hashIdCount = 0x11;

        protected CompilerConnector(string baseDir)
        {
            hashIdCount <<= 0x1;
            hashID = "0x" + hashIdCount.ToString("X");

            this.baseDir = baseDir;
        }

        public abstract void BuildOutput();

        public abstract void Build();
    }
}