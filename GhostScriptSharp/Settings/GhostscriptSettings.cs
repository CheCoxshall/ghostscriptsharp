using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using GhostscriptSharp.Enums;

namespace GhostscriptSharp
{
    /// <summary>
    /// Ghostscript settings
    /// </summary>
    public class GhostscriptSettings : ICloneable
    {
        #region Constants

        public const GhostscriptDevices DefaultDevice = GhostscriptDevices.pdfwrite;
        public const GhostscriptPageSizes DefaultPageSize = GhostscriptPageSizes.a4;

        #endregion

        protected System.Drawing.Size _resolution;
        protected int _numRenderingThreads = -1;

        public bool Quiet { get; set; }

        public GhostscriptDevices Device { get; set; }

        public Settings.GhostscriptPages Pages { get; set; }

        public System.Drawing.Size Resolution { get; set; }

        public Settings.GhostscriptPageSize Size { get; set; }

        public string Password { get; set; }

        /// <summary>
        /// Number of threads to use for rendering. Default is the number of logical processors on the system.
        /// </summary>
        public int NumRenderingThreads
        {
            get
            {
                if (_numRenderingThreads < 1)
                {
                    try
                    {
                        _numRenderingThreads = System.Environment.ProcessorCount;
                    }
                    catch // if the user doesn't have access...
                    {
                        _numRenderingThreads = 1;
                    }
                }
                return _numRenderingThreads;
            }
            set
            {
                _numRenderingThreads = value;
            }
        }

        public GhostscriptSettings()
        {
            this.Quiet = true;
            this.Device = DefaultDevice;
            this.Pages = new Settings.GhostscriptPages();
            this.Size = new Settings.GhostscriptPageSize();
            this.Size.Native = DefaultPageSize;
        }

        public IEnumerable<String> GetGhostscriptArgs()
        {
            List<String> args = new List<string>();

            // Quiet?
            if (this.Quiet)
            {
                args.Add("-q");
                args.Add("-dQUIET");
            }

            // Output device
            String sDevice = (this.Device == GhostscriptDevices.UNDEFINED) ? DefaultDevice.ToString() : this.Device.ToString();
            args.Add(String.Format("-sDEVICE={0}", sDevice));

            // Pages to output
            if (this.Pages.AllPages)
            {
                args.Add("-dFirstPage=1");
            }
            else
            {
                args.Add(String.Format("-dFirstPage={0}", this.Pages.Start));
                if (this.Pages.End >= this.Pages.Start)
                {
                    args.Add(String.Format("-dLastPage={0}", this.Pages.End));
                }
            }

            // Page size
            if (this.Size.Native == GhostscriptPageSizes.UNDEFINED && this.Size.Manual != System.Drawing.Size.Empty)
            {
                args.Add(String.Format("-dDEVICEWIDTHPOINTS={0}", this.Size.Manual.Width));
                args.Add(String.Format("-dDEVICEHEIGHTPOINTS={0}", this.Size.Manual.Height));
            }
            else
            {
                String sSize = (this.Size.Native == GhostscriptPageSizes.UNDEFINED) ? GhostscriptSettings.DefaultPageSize.ToString() : this.Size.Native.ToString();
                args.Add(String.Format("-sPAPERSIZE={0}", sSize));
            }

            // Page resolution
            if (this.Resolution != System.Drawing.Size.Empty)
            {
                args.Add(String.Format("-dDEVICEXRESOLUTION={0}", this.Resolution.Width));
                args.Add(String.Format("-dDEVICEYRESOLUTION={0}", this.Resolution.Height));
            }

            // Multithreaded Rendering
            if (NumRenderingThreads > 1)
            {
                args.Add(String.Format("-dNumRenderingThreads={0}", NumRenderingThreads));
            }

            // Additional device args
            args.AddRange(Settings.GhostscriptDeviceUtils.GetDefaultArgs(this.Device));

            return args;
        }

        #region ICloneable Members

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion
    }
}