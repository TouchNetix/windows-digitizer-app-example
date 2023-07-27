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

namespace TNx.aXiom.WMPointer
{
    public struct Target
    {
        public uint Id;
        public int X;
        public int Y;
        public int Z;

        // Proximity = 128
        // Hover = 129 -> 255 (higher value means contact is closer to the screen)
        // Touch = 0 -> 127 (higher value denotes more pressure being applied)
        public bool IsTouch => (Z >= 0 && Z <= 127);
        public bool IsHover => (Z >= 129 && Z <= 255);
        public bool IsProx => (Z == 128);
        public PointerEvents PointerEvent;

        public Target(uint id, int x, int y, int z, PointerEvents pointerEvent)
        {
            Id = id;

            X = x;
            Y = y;
            Z = z;

            PointerEvent = pointerEvent;
        }
    }
}
