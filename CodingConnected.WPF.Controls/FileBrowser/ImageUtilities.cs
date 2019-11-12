using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

/// <summary>
/// From here: https://stackoverflow.com/questions/2701263/get-the-icon-for-a-given-extension
/// Internals are mostly from here: http://www.codeproject.com/Articles/2532/Obtaining-and-managing-file-and-folder-icons-using
/// Caches all results.
/// </summary>
/// 
namespace CodingConnected.WPF.Controls
{
    public static class ImageUtilities
    {
        private static Dictionary<string, ImageSource> _iconsCache = new Dictionary<string, ImageSource>();

        public static ImageSource GetIconImage(string filePath)
        {
            var ext = Path.GetExtension(filePath);
            if (!_iconsCache.ContainsKey(ext)) _iconsCache.Add(ext, GetRegisteredIcon(filePath).ToImageSource());
            return _iconsCache[ext];
        }

        public static System.Drawing.Icon GetRegisteredIcon(string filePath)
        {
            var shinfo = new SHfileInfo();
            Win32.SHGetFileInfo(filePath, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), Win32.SHGFI_ICON | Win32.SHGFI_SMALLICON);
            return System.Drawing.Icon.FromHandle(shinfo.hIcon);
        }

        /// <summary>
        /// http://stackoverflow.com/a/6580799/1943849
        /// </summary>
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

        [System.Runtime.InteropServices.DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHfileInfo psfi, uint cbSizeFileInfo, uint uFlags);
    }
}