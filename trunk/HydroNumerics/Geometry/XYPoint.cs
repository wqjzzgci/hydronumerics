#region Copyright
/*
* Copyright (c) 2005,2006,2007, OpenMI Association
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*     * Redistributions of source code must retain the above copyright
*       notice, this list of conditions and the following disclaimer.
*     * Redistributions in binary form must reproduce the above copyright
*       notice, this list of conditions and the following disclaimer in the
*       documentation and/or other materials provided with the distribution.
*     * Neither the name of the OpenMI Association nor the
*       names of its contributors may be used to endorse or promote products
*       derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY "OpenMI Association" ``AS IS'' AND ANY
* EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL "OpenMI Association" BE LIABLE FOR ANY
* DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
* (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
* ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
* (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
* SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#endregion 
using System;
using System.Runtime.Serialization;

using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;


namespace HydroNumerics.Geometry
{


  /// <summary>
  /// XYPoint is simply a x and a y coordinate.
  /// </summary>
  [DataContract]
  public class XYPoint: HydroNumerics.Geometry.IXYPoint
  {
	  private double _x;
    private double _y;


    /// <summary>
    /// Gets and set the coordinatsystem that the easting and northing is in. Changing here will not change the coordinate values
    /// </summary>
    public ICoordinateSystem UTMSystem { get; private set; }


    private double? latitude;
    private double? longitude;

    /// <summary>
    /// Gets the latitude. Do not use the setter.
    /// </summary>
    [DataMember]
    public double Latitude
    {
      get
      {
        if (!latitude.HasValue)
          SetLatLong();
        return latitude.Value;
      }
      set
      {
        latitude = value;
      }
    }
    
    /// <summary>
    /// Gets the longitude. Do not use the setter
    /// </summary>
    [DataMember]
    public double Longitude
    {
      get
      {
        if (!longitude.HasValue)
          SetLatLong();
        return longitude.Value;
      }
      set
      {
        longitude = value;
      }
    }

    private void SetLatLong()
    {

      UTMSystem = ProjNet.CoordinateSystems.ProjectedCoordinateSystem.WGS84_UTM(32, true);
      ICoordinateTransformation trans = new CoordinateTransformationFactory().CreateFromCoordinateSystems(UTMSystem, ProjNet.CoordinateSystems.GeographicCoordinateSystem.WGS84);

      double[] p1 = trans.MathTransform.Transform(new double[] { X, Y });
      Latitude = p1[1];
      Longitude = p1[0];
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <returns>None</returns>
    public XYPoint()
	  {
   	  _x = -9999;
		  _y = -9999;
	  }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <returns>None</returns>
    public XYPoint(double x, double y)
	  {
		 _x = x;
		 _y = y;
	  }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <returns>None</returns>
    public XYPoint(XYPoint xypoint)
	  {
		  _x = xypoint.X;
		  _y = xypoint.Y;
	  }

    /// <summary>
    /// Read/Write property describing the x-coordinate of the point.
    /// </summary>
    [DataMember]
    public double X
	  {
		  get
		  {
		  	return _x;
		  }
		  set
		  {
			  _x = value;
		  }
	  }

    /// <summary>
    /// Read/Write property describing the y-coordinate of the point.
    /// </summary>
    [DataMember]
    public double Y
	  {
		  get
		  {
			  return _y;
		  }
		  set
		  {
			  _y = value;
		  }
	  }

    /// <summary>
    /// Returns the distance
    /// </summary>
    /// <param name="Other"></param>
    /// <returns></returns>
    public double GetDistance(IXYPoint Other)
    {
      double diff_N = this.Y - Other.Y;
      double diff_E = this.X - Other.X;

      return Math.Sqrt(Math.Pow(diff_N, 2.0) + Math.Pow(diff_E, 2.0));
    }

    
    /// <summary>
    /// Compares the object type and the coordinates of the object and the 
    /// object passed as parameter.
    /// </summary>
    /// <returns>True if object type is XYPolyline and the coordinates are 
    /// equal to to the coordinates of the current object. False otherwise.</returns>
    public override bool Equals(Object obj) 
    {
      if (obj == null || GetType() != obj.GetType()) 
        return false;
      else
        return ((XYPoint) obj).X == this.X && ((XYPoint) obj).Y == this.Y;
    }

    /// <summary>
    /// Get Hash Code.
    /// </summary>
    /// <returns>Hash Code for the current instance.</returns>
    public override int GetHashCode()
    {
      return _x.GetHashCode() ^ _y.GetHashCode();
    }
  }
}
