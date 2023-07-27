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
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Security.Permissions;

namespace TNx.aXiom.WMPointer
{
    // Base class for multi-touch aware form.
    // Receives touch notifications through Windows messages and converts them
    // to touch events Touchdown, Touchup and Touchmove.
    public class WMPointerForm : Form
    {
        ///////////////////////////////////////////////////////////////////////
        // Public interface

        // Constructor
        [SecurityPermission(SecurityAction.Demand)]
        public WMPointerForm()
        {
            // Setup handlers
            Load += new System.EventHandler(this.OnLoadHandler);
        }

        ///////////////////////////////////////////////////////////////////////
        // Protected members, for derived classes.

        // Touch event handlers
        protected event EventHandler<WMPointerEventArgs> TargetUpdate;   // touch down event handler

        // EventArgs passed to Touch handlers
        protected class WMPointerEventArgs : System.EventArgs
        {
            // Private data members
            public Target Target { get; }

            // Constructor
            public WMPointerEventArgs(Target target)
            {
                Target = target;
            }
        }

        ///////////////////////////////////////////////////////////////////////
        /// 



        ///////////////////////////////////////////////////////////////////////
        // Private methods

        // OnLoad window event handler: Registers the form for multi-touch input.
        // in:
        //      sender      object that has sent the event
        //      e           event arguments
        private void OnLoadHandler(Object sender, EventArgs e)
        {
            try
            {
                // Registering the window for multi-touch, using the default settings.
                // p/invoking into user32.dll
                if (!WmPointer.RegisterTouchWindow(this.Handle, 0))
                {
                    Debug.Print("ERROR: Could not register window for multi-touch");
                }
            }
            catch (Exception exception)
            {
                Debug.Print("ERROR: RegisterTouchWindow API not available");
                Debug.Print(exception.ToString());
                MessageBox.Show("RegisterTouchWindow API not available", "MTScratchpadWMTouch ERROR",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, 0);
            }
        }

        WmPointer.POINTER_TOUCH_INFO pti = new WmPointer.POINTER_TOUCH_INFO();
        Target target = new Target();

        // Window procedure. Receives WM_ messages.
        // Translates WM_TOUCH window messages to touch events.
        // Normally, touch events are sufficient for a derived class,
        // but the window procedure can be overriden, if needed.
        // in:
        //      m       message
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        protected override void WndProc(ref Message m)
        {
            try
            {

                switch (m.Msg)
                {
                    case WmPointer.WM_POINTERENTER:
                        m = ExtractTarget(m, PointerEvents.Enter);
                        break;
                    case WmPointer.WM_POINTERLEAVE:
                        m = ExtractTarget(m, PointerEvents.Leave);
                        break;
                    case WmPointer.WM_POINTERUPDATE:
                        m = ExtractTarget(m, PointerEvents.Update);
                        break;
                    case WmPointer.WM_POINTERDOWN:
                        m = ExtractTarget(m, PointerEvents.Down);
                        break;
                    case WmPointer.WM_POINTERUP:
                        m = ExtractTarget(m, PointerEvents.Up);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            // Call parent WndProc for default message processing.
            base.WndProc(ref m);
        }

        private Message ExtractTarget(Message m, PointerEvents pointerEvents)
        {
            WmPointer.GetPointerTouchInfo((uint)LoWord(m.WParam.ToInt32()), ref pti);
            Point pt = PointToClient(new Point(pti.rcContact.x, pti.rcContact.y));

            // Proximity = 128
            // Hover = 129 -> 255 (higher value means contact is closer to the screen)
            // Touch = 0 -> 127 (higher value denotes more pressure being applied)
            int z = (int)(pti.pressure - 1) / 4;
            target = new Target(pti.pointerInfo.pointerId, pt.X, pt.Y, z, pointerEvents);

            TargetUpdate?.Invoke(this, new WMPointerEventArgs(target));

            return m;
        }

        // Extracts lower 16-bit word from an 32-bit int.
        // in:
        //      number      int
        // returns:
        //      lower word
        private static int LoWord(int number)
        {
            return (number & 0xffff);
        }
    }
}
