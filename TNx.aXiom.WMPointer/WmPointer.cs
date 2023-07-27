/*******************************************************************************
*                                    NOTICE
*
* Copyright (c) 2023 TouchNetix
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in all
* copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
* SOFTWARE.
*
*******************************************************************************/

using System;
using System.Runtime.InteropServices;

namespace TNx.aXiom.WMPointer
{
    public static class WmPointer
    {
        ///////////////////////////////////////////////////////////////////////
        // Private class definitions, structures, attributes and native
        // functions

        // Multitouch/Touch glue (from winuser.h file)
        // Since the managed layer between C# and WinAPI functions does not 
        // exist at the moment for multi-touch related functions this part of 
        // the code is required to replicate definitions from winuser.h file.

        // Touch event window message constants [winuser.h]
        //private const int WM_TOUCH = 0x0240;
        //private const int WM_POINTERDEVICEINRANGE = 0x239;
        public const int WM_POINTERENTER = 0x0249;
        public const int WM_POINTERLEAVE = 0x024A;
        public const int WM_POINTERDOWN = 0x0246;
        public const int WM_POINTERUP = 0x0247;
        public const int WM_POINTERUPDATE = 0x0245;


        /// <summary>
        /// https://learn.microsoft.com/en-us/windows/win32/api/winuser/ne-winuser-tagpointer_input_type
        /// </summary>
        public enum POINTER_INPUT_TYPE
        {
            PT_POINTER,
            PT_TOUCH,
            PT_PEN,
            PT_MOUSE,
            PT_TOUCHPAD
        };

        /// <summary>
        /// https://learn.microsoft.com/en-us/windows/win32/api/windef/ns-windef-point
        /// </summary>
        public struct POINT
        {
            public int x;
            public int y;
        }

        /// <summary>
        /// https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-pointer_info
        /// </summary>
        public struct POINTER_INFO
        {
            public POINTER_INPUT_TYPE pointerType;
            public uint pointerId;
            public uint frameId;
            public uint pointerFlags;
            public IntPtr sourceDevice;
            public IntPtr hwndTarget;
            public POINT ptPixelLocation;
            public POINT ptHimetricLocation;
            public POINT ptPixelLocationRaw;
            public POINT ptHimetricLocationRaw;
            public uint dwTime;
            public uint historyCount;
            public int InputData;
            public uint dwKeyStates;
            public ulong PerformanceCount;
            public uint ButtonChangeType;
        }

        [System.ComponentModel.TypeConverter(typeof(System.Windows.RectConverter))]
        public struct RECT : IFormattable
        {
            public int w;
            public int h;
            public int x;
            public int y;
            public string ToString(string format, IFormatProvider formatProvider)
            {
                return "";
            }
        }

        public struct POINTER_TOUCH_INFO
        {
            public POINTER_INFO pointerInfo;
            public uint touchFlags;
            public uint touchMask;
            public RECT rcContact;
            public RECT rcContactRaw;
            public uint orientation;
            public uint pressure;
        }


        [StructLayout(LayoutKind.Sequential)]
        private struct POINTS
        {
            public short x;
            public short y;
        }

        // Currently touch/multitouch access is done through unmanaged code
        // We must p/invoke into user32 [winuser.h]
        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RegisterTouchWindow(System.IntPtr hWnd, uint ulFlags);

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetPointerTouchInfo(uint pointerID, ref POINTER_TOUCH_INFO pti);
    }
}
