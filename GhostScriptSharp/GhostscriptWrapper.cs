using System;
using System.Collections.Generic;
using System.Drawing;
using GhostscriptSharp.Enums;
using GhostscriptSharp.Settings;

namespace GhostscriptSharp
{
    /// <summary>
    /// Wraps the Ghostscript API with a C# interface
    /// </summary>
    public class GhostscriptWrapper
    {
        #region Globals

        private static readonly string[] ARGS = new[] {
				
                "-q",                       // Keep gs from writing information to standard output
                "-dQUIET",
               
                "-dPARANOIDSAFER",          // Run this command in safe mode
                "-dBATCH",                  // Keep gs from going into interactive mode
                "-dNOPAUSE",                // Do not prompt and pause for each page
                "-dNOPROMPT",               // Disable prompts for user interaction           
                "-dMaxBitmap=500000000",    // Set high for better performance
				"-dNumRenderingThreads=4",  // Multi-core, come-on!
                
                // Configure the output anti-aliasing, resolution, etc
                "-dAlignToPixels=0",
                "-dGridFitTT=0",
                "-dTextAlphaBits=4",
                "-dGraphicsAlphaBits=4"
		};

        #endregion


        /// <summary>
        /// Generates a thumbnail jpg for the pdf at the input path and saves it 
        /// at the output path
        /// </summary>
        public static void GeneratePageThumb(string inputPath, string outputPath, int page, int dpix, int dpiy, int width = 0, int height = 0)
        {
            GeneratePageThumbs(inputPath, outputPath, page, page, dpix, dpiy, width, height);
        }

        /// <summary>
        /// Generates a collection of thumbnail jpgs for the pdf at the input path 
        /// starting with firstPage and ending with lastPage.
        /// Put "%d" somewhere in the output path to have each of the pages numbered
        /// </summary>
        public static void GeneratePageThumbs(string inputPath, string outputPath, int firstPage, int lastPage, int dpix, int dpiy, int width = 0, int height = 0)
        {
            API.GhostScript.CallAPI(GetArgs(inputPath, outputPath, firstPage, lastPage, dpix, dpiy, width, height));
        }

        /// <summary>
        /// Rasterises a postscript file into selected format
        /// </summary>
        /// <param name="inputPath">Postscript file to convert</param>
        /// <param name="outputPath">Destination file</param>
        /// <param name="settings">Conversion settings</param>
        public static void GenerateOutput(string inputPath, string outputPath, GhostscriptSettings settings)
        {
            API.GhostScript.CallAPI(GetArgs(inputPath, outputPath, settings));
        }

        /// <summary>
        /// Returns an array of arguments to be sent to the Ghostscript API
        /// </summary>
        /// <param name="inputPath">Path to the source file</param>
        /// <param name="outputPath">Path to the output file</param>
        /// <param name="firstPage">The page of the file to start on</param>
        /// <param name="lastPage">The page of the file to end on</param>
        private static string[] GetArgs(string inputPath, string outputPath, int firstPage, int lastPage, int dpix, int dpiy, int width, int height)
        {
            // To maintain backwards compatibility, this method uses previous hardcoded values.
            GhostscriptSettings s = new GhostscriptSettings();
            s.Device = GhostscriptDevices.jpeg;
            s.Pages.Start = firstPage;
            s.Pages.End = lastPage;
            s.Resolution = new System.Drawing.Size(dpix, dpiy);

            GhostscriptPageSize pageSize = new GhostscriptPageSize();
            if (width == 0 && height == 0)
            {
                pageSize.Native = GhostscriptPageSizes.a7;
            }
            else
            {
                pageSize.Manual = new Size(width, height);
            }
            s.Size = pageSize;

            return GetArgs(inputPath, outputPath, s);
        }

        /// <summary>
        /// Returns an array of arguments to be sent to the Ghostscript API
        /// </summary>
        /// <param name="inputPath">Path to the source file</param>
        /// <param name="outputPath">Path to the output file</param>
        /// <param name="settings">API parameters</param>
        /// <returns>API arguments</returns>
        private static string[] GetArgs(string inputPath, string outputPath, GhostscriptSettings settings)
        {
            if (settings.Device == GhostscriptDevices.UNDEFINED)
                throw new ArgumentException("An output device must be defined for Ghostscript", "GhostscriptSettings.Device");

            if (settings.Pages.AllPages == false && (settings.Pages.Start <= 0 && settings.Pages.End < settings.Pages.Start))
                throw new ArgumentException("Pages to be printed must be defined.", "GhostscriptSettings.Pages");

            if (settings.Resolution.IsEmpty)
                throw new ArgumentException("An output resolution must be defined", "GhostscriptSettings.Resolution");

            if (settings.Size.Native == GhostscriptPageSizes.UNDEFINED && settings.Size.Manual.IsEmpty)
                throw new ArgumentException("Page size must be defined", "GhostscriptSettings.Size");

            List<string> args = new List<string>(ARGS);

            // Output device
            args.Add(String.Format("-sDEVICE={0}", settings.Device));

            // Pages to output
            if (settings.Pages.AllPages)
            {
                args.Add("-dFirstPage=1");
            }
            else
            {
                args.Add(String.Format("-dFirstPage={0}", settings.Pages.Start));
                if (settings.Pages.End >= settings.Pages.Start)
                {
                    args.Add(String.Format("-dLastPage={0}", settings.Pages.End));
                }
            }

            // Page size
            if (settings.Size.Native == GhostscriptPageSizes.UNDEFINED)
            {
                args.Add(String.Format("-dDEVICEWIDTHPOINTS={0}", settings.Size.Manual.Width));
                args.Add(String.Format("-dDEVICEHEIGHTPOINTS={0}", settings.Size.Manual.Height));
                args.Add("-dFIXEDMEDIA");
                args.Add("-dPDFFitPage");
            }
            else
            {
                args.Add(String.Format("-sPAPERSIZE={0}", settings.Size.Native.ToString()));
            }

            // Page resolution
            args.Add(String.Format("-dDEVICEXRESOLUTION={0}", settings.Resolution.Width));
            args.Add(String.Format("-dDEVICEYRESOLUTION={0}", settings.Resolution.Height));

            // Password
            if (settings.Password != null)
            {
                args.Add(String.Format("-sPDFPassword={0}", settings.Password));
            }

            // Files
            args.Add(String.Format("-sOutputFile={0}", outputPath));
            args.Add(inputPath);

            return args.ToArray();
        }
    }
}