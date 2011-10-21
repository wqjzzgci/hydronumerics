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


using System;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Maps.MapControl;

namespace DataConnectorUI2.controls
{
    /// <summary>
    /// Custom Pushpin class
    ///     implements powerscaling
    /// </summary>
    public partial class Pin : UserControl
    {
        public Pin()
        {
            InitializeComponent();
        }

        private bool _powerscale;
        public bool powerscale
        {
            get { return _powerscale; }
            set { _powerscale = value; }
        }

        private Map _map;
        public Map MapInstance
        {
            get
            {
                return _map;
            }
            set
            {
                _map = value;
                _map.ViewChangeOnFrame += MapViewChangeOnFrame;
                if (_powerscale) ApplyPowerLawScaling(_map.ZoomLevel);
            }
        }

        public ImageSource ImageSource
        {
            get { return PinImage.Source; }
            set { PinImage.Source = value; }
        }

        void MapViewChangeOnFrame(object sender, MapEventArgs e)
        {
            if (_powerscale) ApplyPowerLawScaling(MapInstance.ZoomLevel);
        }

        private void ApplyPowerLawScaling(double currentZoomLevel)
        {
            double scale = Math.Pow(0.05 * (currentZoomLevel + 1), 2) + 0.01;
            if (scale > 1) scale = 1;
            if (scale < 0.125) scale = 0.125;
            PinScaleTransform.ScaleX = scale;
            PinScaleTransform.ScaleY = scale;
        }
    }
}
