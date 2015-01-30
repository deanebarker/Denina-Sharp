using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.HtmlControls;

namespace BlendInteractive.TextFilterPipeline.Core
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
