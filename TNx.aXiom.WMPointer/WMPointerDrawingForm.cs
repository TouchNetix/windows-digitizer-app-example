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

// MTScratchpadWMPointer application.
// Description:
//  Inside the application window, user can draw using multiple fingers
//  at the same time. The trace of each finger is drawn using different
//  color. If the connected aXiom has hover enabled ellipses will be
//  drawn in the window, the size of which increases the further the target
//  is from the screen.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TNx.aXiom.WMPointer
{
    // Main app, WMTouchForm-derived, multi-touch aware form.
    public partial class WMPointerDrawingForm : WMPointerForm
    {
        // Constructor
        public WMPointerDrawingForm()
        {
            InitializeComponent();

            // Setup handlers
            TargetUpdate += MTScratchpadWMPointerForm_TargetUpdate;

            // Set window properties
            this.BackColor = SystemColors.Window;
            this.WindowState = FormWindowState.Maximized;
            this.Text = "TNx Drawing Example";
        }

        private void MTScratchpadWMPointerForm_TargetUpdate(object sender, WMPointerEventArgs e)
        {
            if (e.Target.PointerEvent == PointerEvents.Enter)
            {
                _previousTargets.Add(e.Target.Id, e.Target);
                return;
            }
            else if (e.Target.PointerEvent == PointerEvents.Leave)
            {
                _previousTargets.Remove(e.Target.Id);
                return;
            }
            else if (e.Target.PointerEvent == PointerEvents.Update)
            {
                if (!_previousTargets.TryGetValue(e.Target.Id, out Target previousTarget))
                    return;

                _previousTargets[e.Target.Id] = e.Target;

                Graphics g = this.CreateGraphics();

                if (e.Target.IsProx)
                {
                    Console.WriteLine("Proximity");
                }
                else if (e.Target.IsHover)
                {
                    int hoverSize = Math.Abs(e.Target.Z - 250);
                    g.DrawEllipse(Pens.Red, e.Target.X, e.Target.Y, hoverSize, hoverSize);
                }
                else if (e.Target.IsTouch && previousTarget.IsTouch)
                {
                    _touchPen.Width = (e.Target.Z);
                    g.DrawLine(_touchPen, e.Target.X, e.Target.Y, previousTarget.X, previousTarget.Y);
                }
            }
        }

        private Dictionary<uint, Target> _previousTargets = new Dictionary<uint, Target>();
        private Pen _touchPen = new Pen(Color.Black);
    }
}