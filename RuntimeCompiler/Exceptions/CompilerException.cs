using System;
using System.Runtime.Serialization;

namespace RuntimeCompilerApi
{
    [Serializable]
    public class CompilerException : Exception
    {
        public CompilerException()
        {

        }

        public CompilerException(string message) : base(message)
        {

        }

        public CompilerException(string message, Exception exception) : base(message, exception)
        {

        }

        protected CompilerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
