using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace DeninaSharp.Core.Documentation
{
    public class CodeSampleDoc : BaseDoc
    {
        private string input;
        public string Input
        {
            get
            {
                return input;
            }
            set
            {
                input = Eval(value);
            }
        }

        private string command;
        public string Command
        {
            get
            {
                return command;
            }
            set
            {
                command = Eval(value);
            }
        }

        private string output;
        public string Output
        {
            get
            {
                return output;
            }
            set
            {
                output = Eval(value);
            }
        }



    }
}