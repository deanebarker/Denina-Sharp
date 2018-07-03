using System;

namespace DeninaSharp.Core.Documentation
{
    public class DocumentationEventArgs : EventArgs
    {
		// One of these will be NULL, depening on what we've loaded
        public FilterDoc FilterDoc { get; set; }
        public CategoryDoc CategoryDoc { get; set; }

		public DocumentationEventArgs(FilterDoc filterDoc)
        {
            FilterDoc = filterDoc;
        }
        public DocumentationEventArgs(CategoryDoc categoryDoc)
        {
            CategoryDoc = categoryDoc;
        }

    }
}
