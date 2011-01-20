#region Copyright
/*
* Copyright (c) HydroInform ApS & Jacob Gudbjerg
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*     * Redistributions of source code must retain the above copyright
*       notice, this list of conditions and the following disclaimer.
*     * Redistributions in binary form must reproduce the above copyright
*       notice, this list of conditions and the following disclaimer in the
*       documentation and/or other materials provided with the distribution.
*     * Neither the name of the HydroInform ApS & Jacob Gudbjerg nor the
*       names of its contributors may be used to endorse or promote products
*       derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY "HydroInform ApS & Jacob Gudbjerg" ``AS IS'' AND ANY
* EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL "HydroInform ApS & Jacob Gudbjerg" BE LIABLE FOR ANY
* DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
* (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
* ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
* (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
* SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#endregion
﻿using System;
using HydroNumerics.OpenMI.Sdk.Backbone;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace HydroNumerics.OpenMI.Sdk.Spatial
{
    public class SpatialAdaptedOutputFactory : IAdaptedOutputFactory
    {
        private string _caption = String.Empty;
        private string _description = String.Empty;
        private readonly string _id;

        public SpatialAdaptedOutputFactory(string id)
        {
            _id = id;
        }

        public IIdentifiable[] GetAvailableAdaptedOutputIds(IBaseOutput adaptee, IBaseInput target)
        {
          ITimeSpaceOutput tsadaptee = adaptee as ITimeSpaceOutput;
          ITimeSpaceInput tstarget = target as ITimeSpaceInput;

          // This inly works with time space items, and target can not be null
          if (tsadaptee == null || tstarget == null)
            return new IIdentifiable[0];

          // This only works with element sets
          if (!(tsadaptee.SpatialDefinition is IElementSet && tstarget.SpatialDefinition is IElementSet))
            return new IIdentifiable[0];

          return ElementMapper.GetAvailableMethods(tsadaptee.ElementSet().ElementType, tstarget.ElementSet().ElementType);
        }

        public IBaseAdaptedOutput CreateAdaptedOutput(IIdentifiable adaptedOutputId, IBaseOutput adaptee, IBaseInput target)
        {
            IBaseAdaptedOutput adaptedOutput = new ElementMapperAdaptedOutput(adaptedOutputId, (ITimeSpaceOutput)adaptee, target.ElementSet());

            // Connect adaptor and adaptee
            if (!adaptee.AdaptedOutputs.Contains(adaptedOutput))
            {
                adaptee.AddAdaptedOutput(adaptedOutput);
            }

            return adaptedOutput;
        }

        public IDescribable GetAdaptedOutputDescription(IIdentifiable id)
        {
            return ElementMapper.GetAdaptedOutputDescription(id);
        }

        public string Id
        {
            get { return _id; }
        }

        public string Caption
        {
            get { return _caption; }
            set { _caption = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        internal class AdaptedOutputIdentifier : IIdentifiable
        {
            private readonly string _adaptedOutputId;

            public AdaptedOutputIdentifier(string adaptedOutputId)
            {
                _adaptedOutputId = adaptedOutputId;
            }

            public string Id
            {
                get
                {
                    return _adaptedOutputId;
                }
            }

            public string Caption { get; set; }
            public string Description { get; set; }
        }    
    }
}
