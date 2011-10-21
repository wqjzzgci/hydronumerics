/* ******************************************************************************
 * 
 * Copyright 2010 Microsoft Corporation
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not 
 * use this file except in compliance with the License. You may obtain a copy of 
 * the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY 
 * KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED
 * WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE,
 * MERCHANTABLITY OR NON-INFRINGEMENT. 
 *  
 * See the Apache 2 License for the specific language governing permissions and
 * limitations under the License.
 * 
 ******************************************************************************* */


using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DataConnectorUI2.controls
{
    /// <summary>
    /// Provides a Draggable control. 
    /// Must be nested inside a Canvas.
    /// </summary>
    public class DragDropPanel : Canvas
    {
        Point beginP;
        Point currentP;
        bool dragOn = false;
        public DragDropPanel() : base()
        {
            this.MouseLeftButtonDown += new MouseButtonEventHandler(DragDropPanel_MouseLeftButtonDown);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(DragDropPanel_MouseLeftButtonUp);
            this.MouseMove += new MouseEventHandler(DragDropPanel_MouseMove);
            this.Cursor = Cursors.Hand;
        }

        void DragDropPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragOn)
            {
                currentP = e.GetPosition(null);
                double x0 = System.Convert.ToDouble(this.GetValue(Canvas.LeftProperty));
                double y0 = System.Convert.ToDouble(this.GetValue(Canvas.TopProperty));
                this.SetValue(Canvas.LeftProperty, x0 + currentP.X - beginP.X);
                this.SetValue(Canvas.TopProperty, y0 + currentP.Y - beginP.Y);
                beginP = currentP;
            }
        }

        void DragDropPanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (dragOn)
            {
                this.Opacity *= 2;
                this.ReleaseMouseCapture();
                dragOn = false;
            }
        }

        void DragDropPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.FrameworkElement c = sender as System.Windows.FrameworkElement;
            dragOn = true;
            beginP = e.GetPosition(null);
            c.Opacity *= 0.5;
            c.CaptureMouse();
        }
    }
}
