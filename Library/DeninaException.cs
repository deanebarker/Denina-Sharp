using BlendInteractive.Denina.Core;
using System;
using System.Collections.Generic;

namespace DeninaSharp.Core
{
    // This exists so that a calling application can differentiate between an exception that the app understands (a mis-parsed command, or an attempt to access a non-existent variable, etc.), opposed to an unhandled exception
    public class DeninaException : Exception
    {
        public string CurrentCommandText { get; set; }
        public string CurrentCommandName { get; set; }

        public List<DebugEntry> DebugData { get; set; }


        public DeninaException()
        {
            Init();
        }

        public DeninaException(string message) : base(message)
        {
            Init();
        }

        public DeninaException(string message, Exception inner) : base(message, inner)
        {
            Init();
        }

        private void Init()
        {
            DebugData = new List<DebugEntry>();
        }
    }
}