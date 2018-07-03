using System;

namespace DeninaSharp.Core.Documentation
{
    public class DocumentationEventArgs : EventArgs
    {
		// One of these will be NULL, depening on what we've loaded
        public CommandDoc CommandDoc { get; set; }
        public CategoryDoc CategoryDoc { get; set; }

		public DocumentationEventArgs(CommandDoc commandDoc)
        {
            CommandDoc = commandDoc;
        }
        public DocumentationEventArgs(CategoryDoc categoryDoc)
        {
            CategoryDoc = categoryDoc;
        }

    }
}
