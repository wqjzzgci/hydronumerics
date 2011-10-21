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
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace DataConnectorUI2
{
    /// <summary>
    /// LoadStyle
    ///     helper class for storing style initializations
    /// </summary>
    public class LoadStyle
    {

        private string _stroke = string.Empty;
        private string _fill = string.Empty;
        private double _opacity = 0.0;

        public LoadStyle(string stroke, string fill, double opacity)
        {
            _stroke = stroke;
            _fill = fill;
            _opacity = opacity;
        }

        public string stroke
        {
            get { return _stroke; }
            set { _stroke = value; }
        }

        public string fill
        {
            get { return _fill; }
            set { _fill = value; }
        }

        public double opacity
        {
            get { return _opacity; }
            set { _opacity = value; }
        }
    }
}
