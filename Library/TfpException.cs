using System;

namespace BlendInteractive.Denina.Core
{
    // This exists so that a calling application can differentiate between an exception that the app understands (a mis-parsed command, or an attempt to access a non-existent variable, etc.), opposed to an unhandled exception
    public class TfpException : Exception
    {
        public TfpException()
        {
        }

        public TfpException(string message) : base(message)
        {
        }

        public TfpException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}