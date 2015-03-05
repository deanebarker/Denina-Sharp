using System;

namespace DeninaSharp.Core
{
    // This exists so that a calling application can differentiate between an exception that the app understands (a mis-parsed command, or an attempt to access a non-existent variable, etc.), opposed to an unhandled exception
    public class DeninaException : Exception
    {
        public DeninaException()
        {
        }

        public DeninaException(string message) : base(message)
        {
        }

        public DeninaException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}