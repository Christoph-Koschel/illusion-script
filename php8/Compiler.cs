using System.IO;
using IllusionScript.Runtime.Compiling;

namespace IllusionScript.Compiler.PHP8
{
    public class Compiler : CompilerConnector
    {
        public Compiler(string baseDir) : base(baseDir)
        {
        }

        public override void BuildOutput()
        {
            string baseOutput = Path.Combine(baseDir, "out", name);

            if (!Directory.Exists(baseOutput))
            {
                Directory.CreateDirectory(baseOutput);
            }
        }

        public override void Build()
        {
            
        }
    }
}