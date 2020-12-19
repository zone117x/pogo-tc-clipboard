using System;
using System.Runtime.InteropServices;

static class OsxClipboard
{
    static IntPtr nsString = objc_getClass("NSString");
    static IntPtr nsPasteboard = objc_getClass("NSPasteboard");
    static IntPtr nsStringPboardType;
    static IntPtr generalPasteboard;
    static IntPtr initWithUtf8Register = sel_registerName("initWithUTF8String:");
    static IntPtr allocRegister = sel_registerName("alloc");
    static IntPtr stringForTypeRegister = sel_registerName("stringForType:");
    static IntPtr utf8Register = sel_registerName("UTF8String");
    static IntPtr generalPasteboardRegister = sel_registerName("generalPasteboard");
    static IntPtr changeCountRegister = sel_registerName("changeCount");
        

    static OsxClipboard()
    {
        nsStringPboardType = objc_msgSend(objc_msgSend(nsString, allocRegister), initWithUtf8Register, "NSStringPboardType");
        generalPasteboard = objc_msgSend(nsPasteboard, generalPasteboardRegister);
    }

    public static long GetChangeCount()
    {
        var ptr = objc_msgSend(generalPasteboard, changeCountRegister);
        return ptr.ToInt64();
    }

    public static string GetText()
    {
        var ptr = objc_msgSend(generalPasteboard, stringForTypeRegister, nsStringPboardType);
        var charArray = objc_msgSend(ptr, utf8Register);
        return Marshal.PtrToStringAnsi(charArray);
    }

    [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
    static extern IntPtr objc_getClass(string className);

    [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
    static extern IntPtr objc_msgSend(IntPtr receiver, IntPtr selector);

    [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
    static extern IntPtr objc_msgSend(IntPtr receiver, IntPtr selector, string arg1);

    [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
    static extern IntPtr objc_msgSend(IntPtr receiver, IntPtr selector, IntPtr arg1);

    [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
    static extern IntPtr sel_registerName(string selectorName);
}
