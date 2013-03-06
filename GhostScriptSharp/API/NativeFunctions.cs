using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace GhostscriptSharp.API
{
    class NativeFunctions
    {
        #region Hooks into the kernel32 DLL (for loading unmanaged code)

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern int FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern int SetDllDirectory(string lpPathName);

        #endregion

        #region Hooks into 32 bit Ghostscript DLL

        /// <summary>
        /// Returns the revision number and strings of the Ghostscript interpreter library.
        /// </summary>
        /// <param name="pr"></param>
        /// <param name="len"></param>
        /// <returns>0 for success, something else for error</returns>
        [DllImport("gsdll32.dll", EntryPoint = "gsapi_revision")]
        internal static extern Int32 GetRevision32(out GS_Revision pr, Int32 len);

        /// <summary>
        /// Create a new instance of Ghostscript. The Ghostscript API supports only one instance.
        /// </summary>
        /// <param name="pinstance">Instance pointer, provided to most other API calls</param>
        /// <param name="caller_handle">Will be provided to callback functions</param>
        /// <returns></returns>
        [DllImport("gsdll32.dll", EntryPoint = "gsapi_new_instance")]
        internal static extern Int32 CreateAPIInstance32(out IntPtr pinstance, IntPtr caller_handle);

        /// <summary>
        /// Destroy an instance of Ghostscript. If Ghostscript has been initialized with InitAPI, you must call ExitAPI before this.
        /// </summary>
        /// <param name="instance">The instance given by CreateAPIInstance</param>
        [DllImport("gsdll32.dll", EntryPoint = "gsapi_delete_instance")]
        internal static extern void DeleteAPIInstance32(IntPtr instance);

        /// <summary>
        /// Set the callback functions for stdio
        /// </summary>
        /// <param name="instance">The instance given by CreateAPIInstance</param>
        /// <param name="stdin">The callback for stdin reads. Should return the number of characters read, 0 for EOF, or -1 for error.</param>
        /// <param name="stdout">The callback for stdout writes. Should return the number of characters written.</param>
        /// <param name="stderr">The callback for stderr writes. Should return the number of characters written.</param>
        /// <returns></returns>
        [DllImport("gsdll32.dll", EntryPoint = "gsapi_set_stdio")]
        internal static extern Int32 Set_Stdio32(IntPtr instance, GhostScript.StdinCallback stdin, GhostScript.StdoutCallback stdout, GhostScript.StdoutCallback stderr);

        /// <summary>
        /// Initialize and run the interpreter with arguments. See Ghostscript API for details on arguments.
        /// </summary>
        /// <param name="instance">The instance given by CreateAPIInstance</param>
        /// <param name="argc">The number of arguments supplied</param>
        /// <param name="argv">Array of arguments supplied. Argv[0] is ignored (as in C programs)</param>
        /// <returns></returns>
        [DllImport("gsdll32.dll", EntryPoint = "gsapi_init_with_args")]
        internal static extern Int32 InitAPI32(IntPtr instance, Int32 argc, string[] argv);

        /// <summary>
        /// After initializing the interpreter, clients may pass it strings to be interpreted. To pass input in arbitrary chunks, first call RunStringBegin, then RunStringContinue as many times as desired, stopping if it returns anything other than e_NeedInput. Then call RunStringEnd to indicate end of file.
        /// </summary>
        /// <param name="instance">The instance given by CreateAPIInstance</param>
        /// <param name="user_errors">0 if errors should be handled normally, if set negative, the function will directly return error codes bypassing the interpreted language</param>
        /// <param name="pexit_code">Set to the exit code for the interpreted in case of quit or fatal error</param>
        /// <returns>Error code corresponding to GhostscriptErrorCode</returns>
        [DllImport("gsdll32.dll", EntryPoint = "gsapi_run_string_begin")]
        internal static extern Int32 RunStringBegin32(IntPtr instance, Int32 user_errors, out Int32 pexit_code);

        /// <summary>
        /// After initializing the interpreter, clients may pass it strings to be interpreted. To pass input in arbitrary chunks, first call RunStringBegin, then RunStringContinue as many times as desired, stopping if it returns anything other than e_NeedInput. Then call RunStringEnd to indicate end of file.
        /// </summary>
        /// <param name="instance">The instance given by CreateAPIInstance</param>
        /// <param name="str">The string to interpret</param>
        /// <param name="length">The length of the string to interpret. This must be no greater than 65535</param>
        /// <param name="user_errors">0 if errors should be handled normally, if set negative, the function will directly return error codes bypassing the interpreted language</param>
        /// <param name="pexit_code">Set to the exit code for the interpreted in case of quit or fatal error</param>
        /// <returns>Error code corresponding to GhostscriptErrorCode, or e_NeedInput when ready for another string</returns>
        [DllImport("gsdll32.dll", EntryPoint = "gsapi_run_string_continue")]
        internal static extern Int32 RunStringContinue32(IntPtr instance, String str, UInt32 length, Int32 user_errors, out Int32 pexit_code);

        /// <summary>
        /// After initializing the interpreter, clients may pass it strings to be interpreted. To pass input in arbitrary chunks, first call RunStringBegin, then RunStringContinue as many times as desired, stopping if it returns anything other than e_NeedInput. Then call RunStringEnd to indicate end of file.
        /// </summary>
        /// <param name="instance">The instance given by CreateAPIInstance</param>
        /// <param name="user_errors">0 if errors should be handled normally, if set negative, the function will directly return error codes bypassing the interpreted language</param>
        /// <param name="pexit_code">Set to the exit code for the interpreted in case of quit or fatal error</param>
        /// <returns>Error code corresponding to GhostscriptErrorCode</returns>
        [DllImport("gsdll32.dll", EntryPoint = "gsapi_run_string_end")]
        internal static extern Int32 RunStringEnd32(IntPtr instance, Int32 user_errors, out Int32 pexit_code);

        /// <summary>
        /// After initializing the interpreter, clients may pass it strings to be interpreted. To pass input in arbitrary chunks, first call RunStringBegin, then RunStringContinue as many times as desired, stopping if it returns anything other than e_NeedInput. Then call RunStringEnd to indicate end of file.
        /// </summary>
        /// <param name="instance">The instance given by CreateAPIInstance</param>
        /// <param name="str">The string to interpret</param>
        /// <param name="length">The length of the string to interpret. This must be no greater than 65535</param>
        /// <param name="user_errors">0 if errors should be handled normally, if set negative, the function will directly return error codes bypassing the interpreted language</param>
        /// <param name="pexit_code">Set to the exit code for the interpreted in case of quit or fatal error</param>
        /// <returns>Error code corresponding to GhostscriptErrorCode</returns>
        [DllImport("gsdll32.dll", EntryPoint = "gsapi_run_string_with_length")]
        internal static extern Int32 RunStringWithLength32(IntPtr instance, String str, UInt32 length, Int32 user_errors, out Int32 pexit_code);

        /// <summary>
        /// After initializing the interpreter, clients may pass it strings to be interpreted. To pass input in arbitrary chunks, first call RunStringBegin, then RunStringContinue as many times as desired, stopping if it returns anything other than e_NeedInput. Then call RunStringEnd to indicate end of file.
        /// </summary>
        /// <param name="instance">The instance given by CreateAPIInstance</param>
        /// <param name="str">The string to interpret. There is a 65535 charactere limit</param>
        /// <param name="user_errors">0 if errors should be handled normally, if set negative, the function will directly return error codes bypassing the interpreted language</param>
        /// <param name="pexit_code">Set to the exit code for the interpreted in case of quit or fatal error</param>
        /// <returns>Error code corresponding to GhostscriptErrorCode</returns>
        [DllImport("gsdll32.dll", EntryPoint = "gsapi_run_string")]
        internal static extern Int32 RunString32(IntPtr instance, String str, Int32 user_errors, out Int32 pexit_code);

        /// <summary>
        /// After initializing the interpreter, clients may pass it strings to be interpreted. To pass input in arbitrary chunks, first call RunStringBegin, then RunStringContinue as many times as desired, stopping if it returns anything other than e_NeedInput. Then call RunStringEnd to indicate end of file.
        /// </summary>
        /// <param name="instance">The instance given by CreateAPIInstance</param>
        /// <param name="file_name">File name of the file to interpret</param>
        /// <param name="user_errors">0 if errors should be handled normally, if set negative, the function will directly return error codes bypassing the interpreted language</param>
        /// <param name="pexit_code">Set to the exit code for the interpreted in case of quit or fatal error</param>
        /// <returns>Error code corresponding to GhostscriptErrorCode</returns>
        [DllImport("gsdll32.dll", EntryPoint = "gsapi_run_file")]
        internal static extern Int32 RunFile32(IntPtr instance, String file_name, Int32 user_errors, out Int32 pexit_code);

        /// <summary>
        /// Exit the interpreter. This must be called if InitAPI has been called, just before calling DeleteAPIInstance
        /// </summary>
        /// <param name="instance">The instance given by CreateAPIInstance</param>
        /// <returns></returns>
        [DllImport("gsdll32.dll", EntryPoint = "gsapi_exit")]
        internal static extern Int32 ExitAPI32(IntPtr instance);

        #endregion

        #region Hooks into 64 bit Ghostscript DLL

        /// <summary>
        /// Returns the revision number and strings of the Ghostscript interpreter library.
        /// </summary>
        /// <param name="pr"></param>
        /// <param name="len"></param>
        /// <returns>0 for success, something else for error</returns>
        [DllImport("gsdll64.dll", EntryPoint = "gsapi_revision")]
        internal static extern Int32 GetRevision64(out GS_Revision pr, Int32 len);

        /// <summary>
        /// Create a new instance of Ghostscript. The Ghostscript API supports only one instance.
        /// </summary>
        /// <param name="pinstance">Instance pointer, provided to most other API calls</param>
        /// <param name="caller_handle">Will be provided to callback functions</param>
        /// <returns></returns>
        [DllImport("gsdll64.dll", EntryPoint = "gsapi_new_instance")]
        internal static extern Int32 CreateAPIInstance64(out IntPtr pinstance, IntPtr caller_handle);

        /// <summary>
        /// Destroy an instance of Ghostscript. If Ghostscript has been initialized with InitAPI, you must call ExitAPI before this.
        /// </summary>
        /// <param name="instance">The instance given by CreateAPIInstance</param>
        [DllImport("gsdll64.dll", EntryPoint = "gsapi_delete_instance")]
        internal static extern void DeleteAPIInstance64(IntPtr instance);

        /// <summary>
        /// Set the callback functions for stdio
        /// </summary>
        /// <param name="instance">The instance given by CreateAPIInstance</param>
        /// <param name="stdin">The callback for stdin reads. Should return the number of characters read, 0 for EOF, or -1 for error.</param>
        /// <param name="stdout">The callback for stdout writes. Should return the number of characters written.</param>
        /// <param name="stderr">The callback for stderr writes. Should return the number of characters written.</param>
        /// <returns></returns>
        [DllImport("gsdll64.dll", EntryPoint = "gsapi_set_stdio")]
        internal static extern Int32 Set_Stdio64(IntPtr instance, GhostScript.StdinCallback stdin, GhostScript.StdoutCallback stdout, GhostScript.StdoutCallback stderr);

        /// <summary>
        /// Initialize and run the interpreter with arguments. See Ghostscript API for details on arguments.
        /// </summary>
        /// <param name="instance">The instance given by CreateAPIInstance</param>
        /// <param name="argc">The number of arguments supplied</param>
        /// <param name="argv">Array of arguments supplied. Argv[0] is ignored (as in C programs)</param>
        /// <returns></returns>
        [DllImport("gsdll64.dll", EntryPoint = "gsapi_init_with_args")]
        internal static extern Int32 InitAPI64(IntPtr instance, Int32 argc, string[] argv);

        /// <summary>
        /// After initializing the interpreter, clients may pass it strings to be interpreted. To pass input in arbitrary chunks, first call RunStringBegin, then RunStringContinue as many times as desired, stopping if it returns anything other than e_NeedInput. Then call RunStringEnd to indicate end of file.
        /// </summary>
        /// <param name="instance">The instance given by CreateAPIInstance</param>
        /// <param name="user_errors">0 if errors should be handled normally, if set negative, the function will directly return error codes bypassing the interpreted language</param>
        /// <param name="pexit_code">Set to the exit code for the interpreted in case of quit or fatal error</param>
        /// <returns>Error code corresponding to GhostscriptErrorCode</returns>
        [DllImport("gsdll64.dll", EntryPoint = "gsapi_run_string_begin")]
        internal static extern Int32 RunStringBegin64(IntPtr instance, Int32 user_errors, out Int32 pexit_code);

        /// <summary>
        /// After initializing the interpreter, clients may pass it strings to be interpreted. To pass input in arbitrary chunks, first call RunStringBegin, then RunStringContinue as many times as desired, stopping if it returns anything other than e_NeedInput. Then call RunStringEnd to indicate end of file.
        /// </summary>
        /// <param name="instance">The instance given by CreateAPIInstance</param>
        /// <param name="str">The string to interpret</param>
        /// <param name="length">The length of the string to interpret. This must be no greater than 65535</param>
        /// <param name="user_errors">0 if errors should be handled normally, if set negative, the function will directly return error codes bypassing the interpreted language</param>
        /// <param name="pexit_code">Set to the exit code for the interpreted in case of quit or fatal error</param>
        /// <returns>Error code corresponding to GhostscriptErrorCode, or e_NeedInput when ready for another string</returns>
        [DllImport("gsdll64.dll", EntryPoint = "gsapi_run_string_continue")]
        internal static extern Int32 RunStringContinue64(IntPtr instance, String str, UInt32 length, Int32 user_errors, out Int32 pexit_code);

        /// <summary>
        /// After initializing the interpreter, clients may pass it strings to be interpreted. To pass input in arbitrary chunks, first call RunStringBegin, then RunStringContinue as many times as desired, stopping if it returns anything other than e_NeedInput. Then call RunStringEnd to indicate end of file.
        /// </summary>
        /// <param name="instance">The instance given by CreateAPIInstance</param>
        /// <param name="user_errors">0 if errors should be handled normally, if set negative, the function will directly return error codes bypassing the interpreted language</param>
        /// <param name="pexit_code">Set to the exit code for the interpreted in case of quit or fatal error</param>
        /// <returns>Error code corresponding to GhostscriptErrorCode</returns>
        [DllImport("gsdll64.dll", EntryPoint = "gsapi_run_string_end")]
        internal static extern Int32 RunStringEnd64(IntPtr instance, Int32 user_errors, out Int32 pexit_code);

        /// <summary>
        /// After initializing the interpreter, clients may pass it strings to be interpreted. To pass input in arbitrary chunks, first call RunStringBegin, then RunStringContinue as many times as desired, stopping if it returns anything other than e_NeedInput. Then call RunStringEnd to indicate end of file.
        /// </summary>
        /// <param name="instance">The instance given by CreateAPIInstance</param>
        /// <param name="str">The string to interpret</param>
        /// <param name="length">The length of the string to interpret. This must be no greater than 65535</param>
        /// <param name="user_errors">0 if errors should be handled normally, if set negative, the function will directly return error codes bypassing the interpreted language</param>
        /// <param name="pexit_code">Set to the exit code for the interpreted in case of quit or fatal error</param>
        /// <returns>Error code corresponding to GhostscriptErrorCode</returns>
        [DllImport("gsdll64.dll", EntryPoint = "gsapi_run_string_with_length")]
        internal static extern Int32 RunStringWithLength64(IntPtr instance, String str, UInt32 length, Int32 user_errors, out Int32 pexit_code);

        /// <summary>
        /// After initializing the interpreter, clients may pass it strings to be interpreted. To pass input in arbitrary chunks, first call RunStringBegin, then RunStringContinue as many times as desired, stopping if it returns anything other than e_NeedInput. Then call RunStringEnd to indicate end of file.
        /// </summary>
        /// <param name="instance">The instance given by CreateAPIInstance</param>
        /// <param name="str">The string to interpret. There is a 65535 charactere limit</param>
        /// <param name="user_errors">0 if errors should be handled normally, if set negative, the function will directly return error codes bypassing the interpreted language</param>
        /// <param name="pexit_code">Set to the exit code for the interpreted in case of quit or fatal error</param>
        /// <returns>Error code corresponding to GhostscriptErrorCode</returns>
        [DllImport("gsdll64.dll", EntryPoint = "gsapi_run_string")]
        internal static extern Int32 RunString64(IntPtr instance, String str, Int32 user_errors, out Int32 pexit_code);

        /// <summary>
        /// After initializing the interpreter, clients may pass it strings to be interpreted. To pass input in arbitrary chunks, first call RunStringBegin, then RunStringContinue as many times as desired, stopping if it returns anything other than e_NeedInput. Then call RunStringEnd to indicate end of file.
        /// </summary>
        /// <param name="instance">The instance given by CreateAPIInstance</param>
        /// <param name="file_name">File name of the file to interpret</param>
        /// <param name="user_errors">0 if errors should be handled normally, if set negative, the function will directly return error codes bypassing the interpreted language</param>
        /// <param name="pexit_code">Set to the exit code for the interpreted in case of quit or fatal error</param>
        /// <returns>Error code corresponding to GhostscriptErrorCode</returns>
        [DllImport("gsdll64.dll", EntryPoint = "gsapi_run_file")]
        internal static extern Int32 RunFile64(IntPtr instance, String file_name, Int32 user_errors, out Int32 pexit_code);

        /// <summary>
        /// Exit the interpreter. This must be called if InitAPI has been called, just before calling DeleteAPIInstance
        /// </summary>
        /// <param name="instance">The instance given by CreateAPIInstance</param>
        /// <returns></returns>
        [DllImport("gsdll64.dll", EntryPoint = "gsapi_exit")]
        internal static extern Int32 ExitAPI64(IntPtr instance);

        #endregion

        [StructLayout(LayoutKind.Sequential)]
        internal struct GS_Revision
        {
            internal string strProduct;
            internal string strCopyright;
            internal Int32 intRevision;
            internal Int32 intRevisionDate;
        }
    }
}
