using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GhostscriptSharp.Enums;

namespace GhostscriptSharp.Settings
{
    public class GhostscriptDeviceUtils
    {
        private GhostscriptDeviceUtils()
        {
        }

        public static IEnumerable<string> GetDefaultArgs(GhostscriptDevices device)
        {
            switch (device)
            {
                case GhostscriptDevices.jpeg:
                    return new[] { 
                  "-dAlignToPixels=0",
                  "-dGridFitTT=0",
                  "-dTextAlphaBits=4",
                  "-dGraphicsAlphaBits=4"
               };
                default:
                    return new string[0];
            }
        }
    }
}
