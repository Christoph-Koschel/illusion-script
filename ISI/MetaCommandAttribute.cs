using System;

namespace IllusionScript.ISI
{
    internal abstract partial class Repl
    {
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
        protected sealed class MetaCommandAttribute : Attribute
        {
            public readonly string name;
            public readonly string description;

            public MetaCommandAttribute(string name, string description)
            {
                this.name = name;
                this.description = description;
            }
        }
    }
}