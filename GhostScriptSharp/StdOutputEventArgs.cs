using System;
using System.Collections.Generic;
using System.Text;

namespace GhostscriptSharp
{
    public class StdOutputEventArgs : EventArgs
    {
        protected readonly string _output;
        public string Output { get { return _output; } }

        public StdOutputEventArgs(string output) : base()
        {
            _output = output;
        }
    }
}