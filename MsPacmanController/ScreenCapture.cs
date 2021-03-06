#region License Information

/**********************************************************************************
Shared Source License for Cropper
Copyright 9/07/2004 Brian Scott
http://blogs.geekdojo.net/brian

This license governs use of the accompanying software ('Software'), and your
use of the Software constitutes acceptance of this license.

You may use the Software for any commercial or noncommercial purpose,
including distributing derivative works.

In return, we simply require that you agree:
1. Not to remove any copyright or other notices from the Software. 
2. That if you distribute the Software in source code form you do so only
   under this license (i.e. you must include a complete copy of this license
   with your distribution), and if you distribute the Software solely in
   object form you only do so under a license that complies with this
   license.
3. That the Software comes "as is", with no warranties. None whatsoever.
   This means no express, implied or statutory warranty, including without
   limitation, warranties of merchantability or fitness for a particular
   purpose or any warranty of title or non-infringement. Also, you must pass
   this disclaimer on whenever you distribute the Software or derivative
   works.
4. That no contributor to the Software will be liable for any of those types
   of damages known as indirect, special, consequential, or incidental
   related to the Software or this license, to the maximum extent the law
   permits, no matter what legal theory it’s based on. Also, you must pass
   this limitation of liability on whenever you distribute the Software or
   derivative works.
5. That if you sue anyone over patents that you think may apply to the
   Software for a person's use of the Software, your license to the Software
   ends automatically.
6. That the patent rights, if any, granted in this license only apply to the
   Software, not to any derivative works you make.
7. That the Software is subject to U.S. export jurisdiction at the time it
   is licensed to you, and it may be subject to additional export or import
   laws in other places.  You agree to comply with all such laws and
   regulations that may apply to the Software after delivery of the software
   to you.
8. That if you are an agency of the U.S. Government, (i) Software provided
   pursuant to a solicitation issued on or after December 1, 1995, is
   provided with the commercial license rights set forth in this license,
   and (ii) Software provided pursuant to a solicitation issued prior to
   December 1, 1995, is provided with “Restricted Rights” as set forth in
   FAR, 48 C.F.R. 52.227-14 (June 1987) or DFAR, 48 C.F.R. 252.227-7013
   (Oct 1988), as applicable.
9. That your rights under this License end automatically if you breach it in
   any way.
10.That all rights not expressly granted to you in this license are reserved.

**********************************************************************************/

#endregion

#region Using Directives

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Text;

#endregion


namespace MsPacmanController
{
	/// <summary>
	/// Class for getting images of the desktop.
	/// </summary>
	/// <threadsafety static="false" instance="false"/>
	/// <note type="caution">This class is not thread safe.</note> 
	/// <remarks>This class has been scaled back to the essentials for capturing a segment of 
	/// the desktop in order to keep Cropper as small as possible.</remarks>
	internal sealed class NativeMethods
	{
		private NativeMethods() { }

		#region Dll Imports

		[DllImport("user32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = false)]
		internal static extern IntPtr WindowFromPoint(POINT point);

		[DllImport("user32.dll")]
		internal static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = false)]
		internal static extern IntPtr GetDesktopWindow();

		[DllImport("user32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = false)]
		internal static extern IntPtr GetWindowDC(IntPtr hwnd);

		[DllImport("user32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = false)]
		internal static extern IntPtr GetDC(IntPtr hwnd);

		[DllImport("user32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = false)]
		internal static extern int ReleaseDC(IntPtr hwnd, IntPtr dc);

		[DllImport("gdi32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = false)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool BitBlt(IntPtr hDestDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, Int32 dwRop);

		[DllImport("user32.dll")]
		internal static extern int GetWindowRgn(IntPtr hWnd, IntPtr hRgn);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

		[DllImport("gdi32.dll")]
		internal static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nReghtRect, int nBottomRect);

		[DllImport("gdi32.dll")]
		internal static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nReghtRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

		[DllImport("user32.dll")]
		internal static extern ulong GetWindowLongA(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		[DllImport("user32.dll")]
		static extern int GetSystemMetrics(int smIndex);

		[DllImport("user32.dll", EntryPoint = "SendMessageA", CharSet = CharSet.Ansi, SetLastError = false)]
		internal static extern Int32 SendMessage(IntPtr hWnd, Int32 msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		internal static extern uint MapVirtualKey(uint uCode, uint uMapType);

		[DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "MapVirtualKeyExW", ExactSpelling = true)]
		internal static extern uint MapVirtualKeyEx(uint uCode, uint uMapType, IntPtr dwhkl);

		[DllImport("user32", SetLastError = true, CharSet = CharSet.Unicode)]
		internal static extern int GetKeyNameText(uint lParam, StringBuilder lpString, int nSize);

		[DllImport("user32.dll")]
		internal static extern IntPtr GetKeyboardLayout(uint idThread);

		[DllImport("user32.dll", EntryPoint = "SendMessageA", CharSet = CharSet.Ansi, SetLastError = false)]
		internal static extern Int32 SendMessage(IntPtr hWnd, Int32 msg, Int32 wParam, IntPtr lParam);


		#endregion

		#region Fields

		private const int SRCCOPY = 0x00CC0020;
		private const int CAPTUREBLT = 1073741824;
		internal const int ECM_FIRST = 0x1500;
		private const int GWL_STYLE = -16;
		private const ulong WS_VISIBLE = 0x10000000L;
		private const ulong WS_BORDER = 0x00800000L;
		private const ulong TARGETWINDOW = WS_BORDER | WS_VISIBLE;


		internal const Int32 WM_USER = 0x0400;
		internal const Int32 HKM_GETHOTKEY = (WM_USER + 2);
		internal const Int32 HOTKEYF_SHIFT = 0x01;
		internal const Int32 HOTKEYF_CONTROL = 0x02;
		internal const Int32 HOTKEYF_ALT = 0x04;
		internal const Int32 HOTKEYF_EXT = 0x08;
		internal const String HOTKEY_CLASS = "msctls_hotkey32";

		internal const Int32 MAPVK_VK_TO_VSC = 0;
		internal const Int32 MAPVK_VSC_TO_VK = 1;
		internal const Int32 MAPVK_VK_TO_CHAR = 2;
		internal const Int32 MAPVK_VSC_TO_VK_EX = 3;
		internal const uint KLF_NOTELLSHELL = 0x00000080;

		#endregion

		#region Structures

		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;


			public RECT(int left, int top, int right, int bottom) {
				Left = left;
				Top = top;
				Right = right;
				Bottom = bottom;
			}

			public Rectangle ToRectangle() {
				return new Rectangle(Left, Top, Right - Left, Bottom - Top);
			}
		}


		[StructLayout(LayoutKind.Sequential)]
		internal struct POINT
		{
			public int X;
			public int Y;

			public POINT(int x, int y) {
				X = x;
				Y = y;
			}

			public static explicit operator POINT(Point pt) {
				return new POINT(pt.X, pt.Y);
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Gets a segment of the desktop as an image.
		/// </summary>
		/// <returns>A <see cref="System.Drawing.Image"/> containg an image of the full desktop.</returns>
		/*internal static Image GetDesktopBitmap() {
			return GetDesktopBitmap(FindWindow(null, "Program Manager"));
		}*/

		/// <summary>
		/// Gets a segment of the desktop as an image.
		/// </summary>
		/// <returns>A <see cref="System.Drawing.Image"/> containg an image of the full desktop.</returns>
		/*internal static Image GetDesktopBitmap(IntPtr hWnd) {
			return GetDesktopBitmap(hWnd, false, Color.Empty);
		}*/

		/// <summary>
		/// Gets a segment of the desktop as an image.
		/// </summary>
		/// <returns>A <see cref="System.Drawing.Image"/> containg an image of the full desktop.</returns>
		/*internal static Image GetDesktopBitmap(IntPtr hWnd, bool colorNonFormArea, Color backgroundColor) {
			Image capture = null;

			try {
				RECT rect = new RECT();
				GetWindowRect(hWnd, ref rect);
				capture = GetDesktopBitmap(rect.ToRectangle());

				if( colorNonFormArea )
					return ColorNonRegionFormArea(hWnd, capture, backgroundColor);
				else
					return capture;
			} finally {
				if( capture != null && colorNonFormArea )
					capture.Dispose();
			}
		}*/

		/// <summary>
		/// Gets a segment of the desktop as an image.
		/// </summary>
		/// <param name="rectangle">The rectangular area to capture.</param>
		/// <returns>A <see cref="System.Drawing.Image"/> containg an image of the desktop 
		/// at the specified coordinates</returns>
		/*internal static Image GetDesktopBitmap(Rectangle rectangle) {
			return GetDesktopBitmap(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
		}*/

		[DllImport("Gdi32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DeleteObject(	IntPtr hObject );

		/// <summary>
		/// Retrieves an image of the specified part of your screen.
		/// </summary>
		/// <param name="x">The X coordinate of the requested area</param> 
		/// <param name="y">The Y coordinate of the requested area</param> 
		/// <param name="width">The width of the requested area</param> 
		/// <param name="height">The height of the requested area</param> 
		/// <returns>A <see cref="System.Drawing.Image"/> of the desktop at 
		/// the specified coordinates.</returns> 
		internal static Image GetDesktopBitmap(int x, int y, int width, int height, Bitmap dest) {
			//Create the image and graphics to capture the portion of the desktop.
			Image destinationImage = dest;
			Graphics destinationGraphics = Graphics.FromImage(destinationImage);
			//Console.WriteLine(destinationGraphics.ClipBounds.Location);

			IntPtr destinationGraphicsHandle = IntPtr.Zero;
			IntPtr windowDC = GetDC(IntPtr.Zero);
			try {
				//Pointers for window handles
				destinationGraphicsHandle = destinationGraphics.GetHdc();

				//Get the screencapture
				int dwRop = SRCCOPY;
				//if( Configuration.Current.HideFormDuringCapture )
				//	dwRop |= CAPTUREBLT;

				BitBlt(destinationGraphicsHandle, 0, 0, width, height, windowDC, x, y, dwRop);
			} finally {
				destinationGraphics.ReleaseHdc(destinationGraphicsHandle);
				DeleteObject(windowDC);
			}
			destinationGraphics.Dispose();

			// Don't forget to dispose this image
			return destinationImage;
		}

		private static Region GetRegionByHWnd(IntPtr hWnd) {
			IntPtr windowRegion = CreateRectRgn(0, 0, 0, 0);
			GetWindowRgn(hWnd, windowRegion);
			return Region.FromHrgn(windowRegion);
		}

		private static Bitmap ColorNonRegionFormArea(IntPtr hWnd, Image capture, Color color) {
			Bitmap finalCapture;

			using( Region region = GetRegionByHWnd(hWnd) )
			using( Graphics drawGraphics = Graphics.FromImage(capture) )
			using( SolidBrush brush = new SolidBrush(color) ) {
				RectangleF bounds = region.GetBounds(drawGraphics);
				if( bounds == RectangleF.Empty ) {
					GraphicsUnit unit = GraphicsUnit.Pixel;
					bounds = capture.GetBounds(ref unit);

					if( (GetWindowLongA(hWnd, GWL_STYLE) & TARGETWINDOW) == TARGETWINDOW ) {
						IntPtr windowRegion = CreateRoundRectRgn(0, 0, (int)bounds.Width + 1, (int)bounds.Height + 1, 9, 9);
						Region r = Region.FromHrgn(windowRegion);

						r.Complement(bounds);
						drawGraphics.FillRegion(brush, r);
					}
				} else {
					region.Complement(bounds);
					drawGraphics.FillRegion(brush, region);
				}

				finalCapture = new Bitmap((int)bounds.Width, (int)bounds.Height);
				using( Graphics finalGraphics = Graphics.FromImage(finalCapture) ) {
					finalGraphics.SmoothingMode = SmoothingMode.AntiAlias;
					finalGraphics.DrawImage(capture, new RectangleF(new PointF(0, 0), finalCapture.Size), bounds, GraphicsUnit.Pixel);
				}
			}
			return finalCapture;
		}

		#endregion
	}
}