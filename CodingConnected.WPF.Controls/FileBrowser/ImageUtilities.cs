using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CodingConnected.WPF.Controls
{
    public static class ImageUtilities
    {
        private static readonly Dictionary<string, ImageSource> IconsCache = new Dictionary<string, ImageSource>();

        public static ImageSource GetIconImage(string filePath)
        {
            var ext = Path.GetExtension(filePath);
            if (!IconsCache.ContainsKey(ext))
            {
                // folder
                if (ext == "")
                    IconsCache.Add(ext, GetRegisteredIcon(filePath, true).ToImageSource());
                // file
                else
                    IconsCache.Add(ext, GetRegisteredIcon(filePath).ToImageSource());
            }
            return IconsCache[ext];
        }

        public static Icon GetRegisteredIcon(string filePath, bool folder = false)
        {
            var shinfo = new SHfileInfo();
            var flags = folder
                // folder: use simple method, since extra flag give wring icons
                ? Win32.SHGFI_ICON | Win32.SHGFI_SMALLICON
                // file: extra flags allow for non-existent files
                : Win32.SHGFI_ICON | Win32.SHGFI_SMALLICON | Win32.SHGFI_SYSICONINDEX | Win32.SHGFI_USEFILEATTRIBUTES;
            Win32.SHGetFileInfo(filePath, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), flags);
            return Icon.FromHandle(shinfo.hIcon);
        }

        public static ImageSource ToImageSource(this Icon icon)
        {
            var imageSource = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            return imageSource;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SHfileInfo
    {
        public IntPtr hIcon;
        public int iIcon;
        public uint dwAttributes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szDisplayName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    }
    
    internal sealed class Win32
    {
        public const uint SHGFI_ICON = 0x100;
        public const uint SHGFI_LARGEICON = 0x0; // large
        public const uint SHGFI_SMALLICON = 0x1; // small
        public const uint SHGFI_SYSICONINDEX = 0x000004000;
        public const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;

       [System.Runtime.InteropServices.DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHfileInfo psfi, uint cbSizeFileInfo, uint uFlags);
    }
}