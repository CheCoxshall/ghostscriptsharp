using System;
using System.Collections.Generic;

using System.Text;
using System.Runtime.InteropServices;
using GhostscriptSharp.Enums;

namespace GhostscriptSharp
{
    public class GhostscriptException : ExternalException
    {
        protected readonly GhostscriptErrorCode gsErrorCode;
        public GhostscriptErrorCode GsErrorCode
        {
            get { return gsErrorCode; }
        }

        public GhostscriptException()
            : base()
        {
        }

        public GhostscriptException(string message)
            : base(message)
        {
            gsErrorCode = GhostscriptErrorCode.UNKNOWN;
        }

        public GhostscriptException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public GhostscriptException(string message, int errorCode)
            : base(message, errorCode)
        {
            //ex.ErrorCode
            if (Enum.IsDefined(typeof(GhostscriptErrorCode), errorCode))
            {
                gsErrorCode = (GhostscriptErrorCode)errorCode;
            }
            else
            {
                gsErrorCode = GhostscriptErrorCode.UNKNOWN;
            }
        }
    }
}