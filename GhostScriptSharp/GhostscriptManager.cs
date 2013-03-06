using System;
using System.Collections.Generic;

using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using GhostscriptSharp.Settings;
using GhostscriptSharp.Enums;
using GhostscriptSharp.API;

namespace GhostscriptSharp
{
    public class GhostscriptManager : IDisposable
    {
        public const Int32 GS_Revision_Size_Bytes = 16;

        #region Globals

        protected static object resourceLock = new object();

        protected static String[] _defaultArgs;

        protected static String[] DefaultArgs
        {
            get
            {
                if (_defaultArgs == null)
                {
                    _defaultArgs = new String[] {
                  "-dPARANOIDSAFER",      // Run in safe mode
                  "-dBATCH",              // Exit after completing commands
                  "-dNOPAUSE",            // Do not pause for each page
                  "-dNOPROMPT"            // Don't prompt for user input
               };
                }
                return _defaultArgs;
            }
        }

        protected static String ghostscriptLibraryPath;
        public static String GhostscriptLibraryPath
        {
            get { return String.IsNullOrEmpty(ghostscriptLibraryPath) ? String.Empty : ghostscriptLibraryPath; }
            set { ghostscriptLibraryPath = value; }
        }

        protected static GhostscriptManager _instance;
        public static GhostscriptManager GetInstance()
        {
            lock (resourceLock)
            {
                if (_instance == null)
                {
                    _instance = new GhostscriptManager();
                }
                return _instance;
            }
        }

        #endregion

        protected IntPtr libraryHandle;

        protected bool revisionInfoLoaded;
        protected String productName;
        protected String copyright;
        protected Int32 revision;
        protected Int32 revisionDate;

        protected GhostscriptSettings settings;
        public GhostscriptSettings Settings
        {
            get { return settings; }
        }

        protected GhostscriptManager()
        {
            revisionInfoLoaded = false;
            libraryHandle = IntPtr.Zero;

            LoadGhostscriptLibrary();

            this.settings = new GhostscriptSettings();
        }

        protected void LoadGhostscriptLibrary()
        {
            NativeFunctions.SetDllDirectory(GhostscriptLibraryPath);
            libraryHandle = NativeFunctions.LoadLibrary(Environment.Is64BitProcess ? "gsdll32.dll" : "gsdll64.dll");
        }

        protected void UnloadGhostscriptLibrary()
        {
            if (libraryHandle != IntPtr.Zero)
            {
                NativeFunctions.FreeLibrary(libraryHandle);
                libraryHandle = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Run the Ghostscript interpreter providing the output file and input file(s)
        /// </summary>
        /// <param name="outputPath">The path to create the output file. Put '%d' the path to create multiple numbered files, one for each page</param>
        /// <param name="inputPaths">One or more input files</param>
        public void DoConvert(String outputPath, params String[] inputPaths)
        {
            bool is64 = Environment.Is64BitProcess;

            IntPtr gsInstancePtr;
            lock (resourceLock)
            {
                if (is64)
                    NativeFunctions.CreateAPIInstance64(out gsInstancePtr, IntPtr.Zero);
                else
                    NativeFunctions.CreateAPIInstance32(out gsInstancePtr, IntPtr.Zero);
                try
                {
                    if (StdOut != null || StdErr != null)
                    {
                        GhostScript.StdoutCallback stdout;
                        #region Set StdOut
                        if (StdOut != null)
                        {
                            stdout = (caller_handle, buf, len) =>
                            {
                                StdOut(this, new StdOutputEventArgs(buf.Substring(0, len)));
                                return len;
                            };
                        }
                        else
                        {
                            stdout = EmptyStdoutCallback;
                        }
                        #endregion
                        GhostScript.StdoutCallback stderr;
                        #region Set StdErr
                        if (StdErr != null)
                        {
                            stderr = (caller_handle, buf, len) =>
                            {
                                StdOut(this, new StdOutputEventArgs(buf.Substring(0, len)));
                                return len;
                            };
                        }
                        else
                        {
                            stderr = EmptyStdoutCallback;
                        }
                        #endregion
                        if (is64)
                            NativeFunctions.Set_Stdio64(gsInstancePtr, EmptyStdinCallback, stdout, stderr);
                        else
                            NativeFunctions.Set_Stdio32(gsInstancePtr, EmptyStdinCallback, stdout, stderr);
                    }
                    String[] args = null;
                    {
                        List<String> lArgs = new List<string>();
                        lArgs.Add("GhostscriptSharp"); // First arg is ignored, corresponds to argv[0]
                        lArgs.AddRange(GhostscriptManager.DefaultArgs);
                        lArgs.AddRange(this.Settings.GetGhostscriptArgs());
                        lArgs.Add(String.Format("-sOutputFile={0}", outputPath));
                        lArgs.AddRange(inputPaths);
                        args = lArgs.ToArray();
                    }
                    int result;

                    if (is64)
                        result = NativeFunctions.InitAPI64(gsInstancePtr, args.Length, args);
                    else
                        result = NativeFunctions.InitAPI32(gsInstancePtr, args.Length, args);

                    if (result < 0)
                    {
                        throw new GhostscriptException("Ghostscript conversion error", result);
                    }
                }
                finally
                {
                    if (is64)
                    {
                        NativeFunctions.ExitAPI64(gsInstancePtr);
                        NativeFunctions.DeleteAPIInstance64(gsInstancePtr);
                    }
                    else
                    {
                        NativeFunctions.ExitAPI32(gsInstancePtr);
                        NativeFunctions.DeleteAPIInstance32(gsInstancePtr);
                    }
                }
            }
        }

        #region Revision Info Properties

        /// <summary>
        /// Name of the product obtained from the Ghostscript DLL e.g. "GPL Ghostscript"
        /// </summary>
        public String ProductName
        {
            get
            {
                if (!revisionInfoLoaded)
                {
                    LoadRevisionInfo();
                }
                return productName;
            }
        }

        /// <summary>
        /// Copyright Information obtained from the Ghostscript DLL
        /// </summary>
        public String Copyright
        {
            get
            {
                if (!revisionInfoLoaded)
                {
                    LoadRevisionInfo();
                }
                return copyright;
            }
        }

        /// <summary>
        /// Revision Number of the Ghostscript DLL e.g. 871 for v8.71
        /// </summary>
        public Int32 Revision
        {
            get
            {
                if (!revisionInfoLoaded)
                {
                    LoadRevisionInfo();
                }
                return revision;
            }
        }

        /// <summary>
        /// Revision Date of the Ghostscript DLL in the format yyyyMMdd
        /// </summary>
        public Int32 RevisionDate
        {
            get
            {
                if (!revisionInfoLoaded)
                {
                    LoadRevisionInfo();
                }
                return revisionDate;
            }
        }

        /// <summary>
        /// Get Ghostscript Library revision info
        /// </summary>
        /// <param name="strProduct"></param>
        /// <param name="strCopyright"></param>
        /// <param name="intRevision"></param>
        /// <param name="intRevisionDate"></param>
        protected void LoadRevisionInfo()
        {
            NativeFunctions.GS_Revision rev;
            if (Environment.Is64BitProcess)
                NativeFunctions.GetRevision64(out rev, GS_Revision_Size_Bytes);
            else
                NativeFunctions.GetRevision32(out rev, GS_Revision_Size_Bytes);

            this.productName = rev.strProduct;
            this.copyright = rev.strCopyright;
            this.revision = rev.intRevision;
            this.revisionDate = rev.intRevisionDate;
            this.revisionInfoLoaded = true;
        }

        #endregion

        #region stdin, stdout, stderr handlers

        //public delegate void StdinReader(StringBuilder input);
        public delegate void StdOutputHandler(object sender, StdOutputEventArgs args);

        //public event StdinReader StdIn;
        public event StdOutputHandler StdOut;
        public event StdOutputHandler StdErr;

        /// <summary>
        /// "Default" implementation of StdinCallback - gives Ghostscript EOF whenever it requests input
        /// </summary>
        /// <param name="caller_handle"></param>
        /// <param name="buf"></param>
        /// <param name="len"></param>
        /// <returns>0 (EOF) whenever GS requests input</returns>
        protected GhostScript.StdinCallback EmptyStdinCallback = (caller_handle, buf, len) =>
        {
            return 0; // return EOF always
        };

        /// <summary>
        /// "Default" implementation of StdoutCallback - does nothing with output, returns all characters handled
        /// </summary>
        /// <param name="caller_handle"></param>
        /// <param name="buf"></param>
        /// <param name="len"></param>
        /// <returns>len (the number of characters handled) whenever GS outputs anything</returns>
        protected GhostScript.StdoutCallback EmptyStdoutCallback = (caller_handle, buf, len) =>
        {
            return len; // return all bytes handled
        };

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            UnloadGhostscriptLibrary();
        }

        #endregion

        #region Convenience Methods

        /// <summary>
        /// Convert a postscript file to a pdf
        /// </summary>
        /// <param name="outputPath">The path to create the output file. Put '%d' the path to create multiple numbered files, one for each page</param>
        /// <param name="inputPaths">One or more input files</param>
        public static void PsToPdf(String outputPath, params String[] inputPaths)
        {
            GhostscriptManager gsm = GhostscriptManager.GetInstance();
            bool libraryLoaded = (gsm.libraryHandle != IntPtr.Zero);
            if (!libraryLoaded)
            {
                gsm.LoadGhostscriptLibrary();
            }
            GhostscriptSettings oldSettings = gsm.Settings;
            gsm.settings = new GhostscriptSettings();
            gsm.Settings.Device = GhostscriptDevices.pdfwrite;
            gsm.Settings.Pages.AllPages = true;
            gsm.Settings.Quiet = true;
            gsm.DoConvert(outputPath, inputPaths);
            if (!libraryLoaded)
            {
                gsm.UnloadGhostscriptLibrary();
            }
            gsm.settings = oldSettings;
        }

        #endregion
    }
}
