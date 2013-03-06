using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GhostscriptSharp.Enums
{
    /// <summary>
    /// Output devices for GhostScript
    /// </summary>
    public enum GhostscriptDevices
    {
        UNDEFINED,
        png16m,
        pnggray,
        png256,
        png16,
        pngmono,
        pngalpha,
        jpeg,
        jpeggray,
        tiffgray,
        tiff12nc,
        tiff24nc,
        tiff32nc,
        tiffsep,
        tiffcrle,
        tiffg3,
        tiffg32d,
        tiffg4,
        tifflzw,
        tiffpack,
        faxg3,
        faxg32d,
        faxg4,
        bmpmono,
        bmpgray,
        bmpsep1,
        bmpsep8,
        bmp16,
        bmp256,
        bmp16m,
        bmp32b,
        pcxmono,
        pcxgray,
        pcx16,
        pcx256,
        pcx24b,
        pcxcmyk,
        psdcmyk,
        psdrgb,
        pdfwrite,
        pswrite,
        epswrite,
        pxlmono,
        pxlcolor
    }

    /// <summary>
    /// Native page sizes
    /// </summary>
    /// <remarks>
    /// Missing 11x17 as enums can't start with a number, and I can't be bothered
    /// to add in logic to handle it - if you need it, do it yourself.
    /// </remarks>
    public enum GhostscriptPageSizes
    {
        UNDEFINED,
        ledger,
        legal,
        letter,
        lettersmall,
        archE,
        archD,
        archC,
        archB,
        archA,
        a0,
        a1,
        a2,
        a3,
        a4,
        a4small,
        a5,
        a6,
        a7,
        a8,
        a9,
        a10,
        isob0,
        isob1,
        isob2,
        isob3,
        isob4,
        isob5,
        isob6,
        c0,
        c1,
        c2,
        c3,
        c4,
        c5,
        c6,
        jisb0,
        jisb1,
        jisb2,
        jisb3,
        jisb4,
        jisb5,
        jisb6,
        b0,
        b1,
        b2,
        b3,
        b4,
        b5,
        flsa,
        flse,
        halfletter
    }

    /// <summary>
    /// Ghostscript error codes
    /// </summary>
    public enum GhostscriptErrorCode : int
    {
        NoErrors = 0,
        e_unknownerror = -1,
        e_dictfull = -2,
        e_dictstackoverflow = -3,
        e_dictstackunderflow = -4,
        e_execstackoverflow = -5,
        e_interrupt = -6,
        e_invalidaccess = -7,
        e_invalidexit = -8,
        e_invalidfileaccess = -9,
        e_invalidfont = -10,
        e_invalidrestore = -11,
        e_ioerror = -12,
        e_limitcheck = -13,
        e_nocurrentpoint = -14,
        e_rangecheck = -15,
        e_stackoverflow = -16,
        e_stackunderflow = -17,
        e_syntaxerror = -18,
        e_timeout = -19,
        e_typecheck = -20,
        e_undefined = -21,
        e_undefinedfilename = -22,
        e_undefinedresult = -23,
        e_unmatchedmark = -24,
        e_VMerror = -25,
        // Level 1 Errors
        e_configurationerror = -26,
        e_undefinedresource = -27,
        e_unregistered = -28,
        // Level 2 Errors
        e_invalidcontext = -29,
        e_invalidid = -30,
        // Level 3 Errors

        e_Fatal = -100,
        e_Quit = -101,
        e_NeedInput = -106,
        e_Info = -110,
        UNKNOWN = -9999
    };
}
