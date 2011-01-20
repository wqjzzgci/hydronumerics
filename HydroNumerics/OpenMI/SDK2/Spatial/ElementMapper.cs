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

using System;
using System.Collections;
using System.Collections.Generic;
using HydroNumerics.OpenMI.Sdk.Backbone;
using HydroNumerics.OpenMI.Sdk.Backbone.Generic;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace HydroNumerics.OpenMI.Sdk.Spatial
{
    public enum ElementMapperMethod
    {
        Nearest,
        Inverse,
        Mean,
        Sum,
        WeightedMean,
        WeightedSum,
        Distribute,
        Value
    }

    /// <summary>
    /// The ElementMapper converts one ValueSet (inputValues) associated one ElementSet (fromElements)
    /// to a new ValuesSet (return value of MapValue) that corresponds to another ElementSet 
    /// (toElements). The conversion is a two step procedure where the first step (Initialize) is 
    /// executed at initialisation time only, whereas the MapValues is executed during time stepping.
    /// 
    /// <p>The Initialize method will create a conversion matrix with the same number of rows as the
    /// number of elements in the ElementSet associated to the accepting component (i.e. the toElements) 
    /// and the same number of columns as the number of elements in the ElementSet associated to the 
    /// providing component (i.e. the fromElements).</p>
    /// 
    /// <p>Mapping is possible for any zero-, one- and two-dimensional elemets. Zero dimensional 
    /// elements will always be points, one-dimensional elements will allways be polylines and two-
    /// dimensional elements will allways be polygons.</p>
    /// 
    /// <p>The ElementMapper contains a number of methods for mapping between the different element types.
    /// As an example polyline to polygon mapping may be done either as Weighted Mean or as Weighted Sum.
    /// Typically the method choice will depend on the quantity mapped. Such that state variables such as 
    /// water level will be mapped using Weighted Mean whereas flux variables such as seepage from river 
    /// to groundwater will be mapped using Weighted Sum. The list of available methods for a given 
    /// combination of from and to element types is obtained using the GetAvailableMethods method.</p>
    /// </summary>
    /// 
    public class ElementMapper
    {
        private const string _ElementMapperPrefix = "ElementMapper";
        private const int _NumberOfAvailableMethods = 16;

        private static readonly SMethod[] _AvailableMethods;
        private bool _isInitialised;

        private IMatrix<double> _mappingMatrix; // the mapping matrix
        private ElementMapperMethod? _method;
        private int _numberOfColumns;
        private int _numberOfRows;

        static ElementMapper()
        {
            _AvailableMethods = new SMethod[_NumberOfAvailableMethods];

            _AvailableMethods[0].FromElementsShapeType = ElementType.Point;
            _AvailableMethods[0].ToElementsShapeType = ElementType.Point;
            _AvailableMethods[0].ElementMapperMethod = ElementMapperMethod.Nearest;
            _AvailableMethods[0].Description = "Nearest";
            _AvailableMethods[0].Id = _ElementMapperPrefix + "100";

            _AvailableMethods[1].FromElementsShapeType = ElementType.Point;
            _AvailableMethods[1].ToElementsShapeType = ElementType.Point;
            _AvailableMethods[1].ElementMapperMethod = ElementMapperMethod.Inverse;
            _AvailableMethods[1].Description = "Inverse";
            _AvailableMethods[1].Id = _ElementMapperPrefix + "101";

            _AvailableMethods[2].FromElementsShapeType = ElementType.Point;
            _AvailableMethods[2].ToElementsShapeType = ElementType.PolyLine;
            _AvailableMethods[2].ElementMapperMethod = ElementMapperMethod.Nearest;
            _AvailableMethods[2].Description = "Nearest";
            _AvailableMethods[2].Id = _ElementMapperPrefix + "200";

            _AvailableMethods[3].FromElementsShapeType = ElementType.Point;
            _AvailableMethods[3].ToElementsShapeType = ElementType.PolyLine;
            _AvailableMethods[3].ElementMapperMethod = ElementMapperMethod.Inverse;
            _AvailableMethods[3].Description = "Inverse";
            _AvailableMethods[3].Id = _ElementMapperPrefix + "201";

            _AvailableMethods[4].FromElementsShapeType = ElementType.Point;
            _AvailableMethods[4].ToElementsShapeType = ElementType.Polygon;
            _AvailableMethods[4].ElementMapperMethod = ElementMapperMethod.Mean;
            _AvailableMethods[4].Description = "Mean";
            _AvailableMethods[4].Id = _ElementMapperPrefix + "300";

            _AvailableMethods[5].FromElementsShapeType = ElementType.Point;
            _AvailableMethods[5].ToElementsShapeType = ElementType.Polygon;
            _AvailableMethods[5].ElementMapperMethod = ElementMapperMethod.Sum;
            _AvailableMethods[5].Description = "Sum";
            _AvailableMethods[5].Id = _ElementMapperPrefix + "301";

            _AvailableMethods[6].FromElementsShapeType = ElementType.PolyLine;
            _AvailableMethods[6].ToElementsShapeType = ElementType.Point;
            _AvailableMethods[6].ElementMapperMethod = ElementMapperMethod.Nearest;
            _AvailableMethods[6].Description = "Nearest";
            _AvailableMethods[6].Id = _ElementMapperPrefix + "400";

            _AvailableMethods[7].FromElementsShapeType = ElementType.PolyLine;
            _AvailableMethods[7].ToElementsShapeType = ElementType.Point;
            _AvailableMethods[7].ElementMapperMethod = ElementMapperMethod.Inverse;
            _AvailableMethods[7].Description = "Inverse";
            _AvailableMethods[7].Id = _ElementMapperPrefix + "401";

            _AvailableMethods[8].FromElementsShapeType = ElementType.PolyLine;
            _AvailableMethods[8].ToElementsShapeType = ElementType.Polygon;
            _AvailableMethods[8].ElementMapperMethod = ElementMapperMethod.WeightedMean;
            _AvailableMethods[8].Description = "Weighted Mean";
            _AvailableMethods[8].Id = _ElementMapperPrefix + "500";

            _AvailableMethods[9].FromElementsShapeType = ElementType.PolyLine;
            _AvailableMethods[9].ToElementsShapeType = ElementType.Polygon;
            _AvailableMethods[9].ElementMapperMethod = ElementMapperMethod.WeightedSum;
            _AvailableMethods[9].Description = "Weighted Sum";
            _AvailableMethods[9].Id = _ElementMapperPrefix + "501";

            _AvailableMethods[10].FromElementsShapeType = ElementType.Polygon;
            _AvailableMethods[10].ToElementsShapeType = ElementType.Point;
            _AvailableMethods[10].ElementMapperMethod = ElementMapperMethod.Value;
            _AvailableMethods[10].Description = "Value";
            _AvailableMethods[10].Id = _ElementMapperPrefix + "600";

            _AvailableMethods[11].FromElementsShapeType = ElementType.Polygon;
            _AvailableMethods[11].ToElementsShapeType = ElementType.PolyLine;
            _AvailableMethods[11].ElementMapperMethod = ElementMapperMethod.WeightedMean;
            _AvailableMethods[11].Description = "Weighted Mean";
            _AvailableMethods[11].Id = _ElementMapperPrefix + "700";

            _AvailableMethods[12].FromElementsShapeType = ElementType.Polygon;
            _AvailableMethods[12].ToElementsShapeType = ElementType.PolyLine;
            _AvailableMethods[12].ElementMapperMethod = ElementMapperMethod.WeightedSum;
            _AvailableMethods[12].Description = "Weighted Sum";
            _AvailableMethods[12].Id = _ElementMapperPrefix + "701";

            _AvailableMethods[13].FromElementsShapeType = ElementType.Polygon;
            _AvailableMethods[13].ToElementsShapeType = ElementType.Polygon;
            _AvailableMethods[13].ElementMapperMethod = ElementMapperMethod.WeightedMean;
            _AvailableMethods[13].Description = "Weighted Mean";
            _AvailableMethods[13].Id = _ElementMapperPrefix + "800";

            _AvailableMethods[14].FromElementsShapeType = ElementType.Polygon;
            _AvailableMethods[14].ToElementsShapeType = ElementType.Polygon;
            _AvailableMethods[14].ElementMapperMethod = ElementMapperMethod.WeightedSum;
            _AvailableMethods[14].Description = "Weighted Sum";
            _AvailableMethods[14].Id = _ElementMapperPrefix + "801";

            _AvailableMethods[15].FromElementsShapeType = ElementType.Polygon;
            _AvailableMethods[15].ToElementsShapeType = ElementType.Polygon;
            _AvailableMethods[15].ElementMapperMethod = ElementMapperMethod.Distribute;
            _AvailableMethods[15].Description = "Distribute";
            _AvailableMethods[15].Id = _ElementMapperPrefix + "802";
        
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ElementMapper()
        {
            _numberOfRows = 0;
            _numberOfColumns = 0;
            _isInitialised = false;
        }

        public IMatrix<double> MappingMatrix
        {
            get { return _mappingMatrix; }
        }

        public bool IsInitialized
        {
            get { return _isInitialised; }
        }

        /// <summary>
        /// Initialises the ElementMapper. The initialisation includes setting the _isInitialised
        /// flag and calls UpdateMappingMatrix for claculation of the mapping matrix.
        /// </summary>
        ///
        /// <param name="method">String description of mapping method</param> 
        /// <param name="fromElements">The IElementSet to map from.</param>
        /// <param name="toElements">The IElementSet to map to</param>
        /// 
        /// <returns>
        /// The method has no return value.
        /// </returns>
        public void Initialise(IIdentifiable method, IElementSet fromElements, IElementSet toElements)
        {
            UpdateMappingMatrix(method, fromElements, toElements);
            _isInitialised = true;
        }

        /// <summary>
        /// MapValues calculates for each set of timestep data 
        /// a resulting IValueSet through multiplication of an inputValues IValueSet
        /// vector with the mapping maprix. 
        /// </summary>
        /// <param name="inputValues">IValueSet of values to be mapped.</param>
        /// <returns>
        /// A IValueSet found by mapping of the inputValues on to the toElementSet.
        /// </returns>
        public TimeSpaceValueSet<double> MapValues(ITimeSpaceValueSet inputValues)
        {
            if (!_isInitialised)
            {
                throw new Exception(
                    "ElementMapper objects needs to be initialised before the MapValue method can be used");
            }
            if (!ValueSet.GetElementCount(inputValues).Equals(_numberOfColumns))
            {
                throw new Exception("Dimension mismatch between inputValues and mapping matrix");
            }

            // Make a time-space value set of the correct size
            TimeSpaceValueSet<double> result = CreateResultValueSet(inputValues.TimesCount(), _numberOfRows);

            MapValues(result, inputValues);
            
            return result;
        }

        /// <summary>
        /// Creates a result value set of the size specified
        /// </summary>
        /// <returns>A Value set of the correct size.</returns>
        public static TimeSpaceValueSet<double> CreateResultValueSet(int numtimes, int numElements)
        {
            ListArray<double> outValues = new ListArray<double>(numtimes);
            for (int i = 0; i < numtimes; i++)
            {
                outValues.Add(new double[numElements]);
            }
            return new TimeSpaceValueSet<double>(outValues);
        }

        /// <summary>
        /// MapValues calculates for each set of timestep data 
        /// a resulting IValueSet through multiplication of an inputValues IValueSet
        /// vector with the mapping maprix. 
        /// <para>
        /// This version can be used if the output value set is to be reused (performance or for
        /// adding up)
        /// </para>
        /// </summary>
        /// <param name="outputValues">IValueset of mapped values, of the correct size</param>
        /// <param name="inputValues">IValueSet of values to be mapped.</param>
        public void MapValues(ITimeSpaceValueSet<double> outputValues, ITimeSpaceValueSet inputValues)
        {
            for (int i = 0; i < inputValues.Values2D.Count; i++)
            {
                _mappingMatrix.Product(outputValues.Values2D[i], inputValues.GetElementValuesForTime<double>(i));
            }
        }

        
        /// <summary>
        /// Calculates the mapping matrix between fromElements and toElements. The mapping method 
        /// is decided from the combination of methodDescription, fromElements.ElementType and 
        /// toElements.ElementType. 
        /// The valid values for methodDescription is obtained through use of the 
        /// GetAvailableMethods method.
        /// </summary>
        /// 
        /// <remarks>
        /// UpdateMappingMatrix is called during initialisation. UpdateMappingMatrix must be called prior
        /// to Mapvalues.
        /// </remarks>
        /// 
        /// <param name="methodIdentifier">String identification of mapping method</param> 
        /// <param name="fromElements">The IElementset to map from.</param>
        /// <param name="toElements">The IElementset to map to</param>
        ///
        /// <returns>
        /// The method has no return value.
        /// </returns>
        private void UpdateMappingMatrix(IIdentifiable methodIdentifier, IElementSet fromElements, IElementSet toElements)
        {
            try
            {
                ElementSetChecker.CheckElementSet(fromElements);
                ElementSetChecker.CheckElementSet(toElements);

                _method = GetMethod(methodIdentifier);
                _numberOfRows = toElements.ElementCount;
                _numberOfColumns = fromElements.ElementCount;
                _mappingMatrix = new DoubleSparseMatrix(_numberOfRows,_numberOfColumns);

                if (fromElements.ElementType == ElementType.Point && toElements.ElementType == ElementType.Point)
                {
                    #region

                    try
                    {
                        for (int i = 0; i < _numberOfRows; i++)
                        {
                            XYPoint toPoint = CreateXYPoint(toElements, i);
                            for (int j = 0; j < _numberOfColumns; j++)
                            {
                                XYPoint fromPoint = CreateXYPoint(fromElements, j);
                                _mappingMatrix[i, j] = XYGeometryTools.CalculatePointToPointDistance(toPoint, fromPoint);
                            }
                        }

                        if (_method == ElementMapperMethod.Nearest)
                        {
                            for (int i = 0; i < _numberOfRows; i++)
                            {
                                double minDist = _mappingMatrix[i, 0];
                                for (int j = 1; j < _numberOfColumns; j++)
                                {
                                    if (_mappingMatrix[i, j] < minDist)
                                    {
                                        minDist = _mappingMatrix[i, j];
                                    }
                                }
                                int denominator = 0;
                                for (int j = 0; j < _numberOfColumns; j++)
                                {
                                    if (_mappingMatrix[i, j] == minDist)
                                    {
                                        _mappingMatrix[i, j] = 1;
                                        denominator++;
                                    }
                                    else
                                    {
                                        _mappingMatrix[i, j] = 0;
                                    }
                                }
                                for (int j = 0; j < _numberOfColumns; j++)
                                {
                                    _mappingMatrix[i, j] = _mappingMatrix[i, j]/denominator;
                                }
                            }
                        }
                        else if (_method == ElementMapperMethod.Inverse)
                        {
                            for (int i = 0; i < _numberOfRows; i++)
                            {
                                double minDist = _mappingMatrix[i, 0];
                                for (int j = 1; j < _numberOfColumns; j++)
                                {
                                    if (_mappingMatrix[i, j] < minDist)
                                    {
                                        minDist = _mappingMatrix[i, j];
                                    }
                                }
                                if (minDist == 0)
                                {
                                    int denominator = 0;
                                    for (int j = 0; j < _numberOfColumns; j++)
                                    {
                                        if (_mappingMatrix[i, j] == minDist)
                                        {
                                            _mappingMatrix[i, j] = 1;
                                            denominator++;
                                        }
                                        else
                                        {
                                            _mappingMatrix[i, j] = 0;
                                        }
                                    }
                                    for (int j = 0; j < _numberOfColumns; j++)
                                    {
                                        _mappingMatrix[i, j] = _mappingMatrix[i, j]/denominator;
                                    }
                                }
                                else
                                {
                                    double denominator = 0;
                                    for (int j = 0; j < _numberOfColumns; j++)
                                    {
                                        _mappingMatrix[i, j] = 1/_mappingMatrix[i, j];
                                        denominator = denominator + _mappingMatrix[i, j];
                                    }
                                    for (int j = 0; j < _numberOfColumns; j++)
                                    {
                                        _mappingMatrix[i, j] = _mappingMatrix[i, j]/denominator;
                                    }
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("methodDescription unknown for point point mapping");
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Point to point mapping failed", e);
                    }

                    #endregion
                }
                else if (fromElements.ElementType == ElementType.Point && toElements.ElementType == ElementType.PolyLine)
                {
                    #region

                    try
                    {
                        for (int i = 0; i < _numberOfRows; i++)
                        {
                            XYPolyline toPolyLine = CreateXYPolyline(toElements, i);
                            for (int j = 0; j < _numberOfColumns; j++)
                            {
                                XYPoint fromPoint = CreateXYPoint(fromElements, j);
                                _mappingMatrix[i, j] = XYGeometryTools.CalculatePolylineToPointDistance(toPolyLine,
                                                                                                        fromPoint);
                            }
                        }

                        if (_method == ElementMapperMethod.Nearest)
                        {
                            for (int i = 0; i < _numberOfRows; i++)
                            {
                                double minDist = _mappingMatrix[i, 0];
                                for (int j = 1; j < _numberOfColumns; j++)
                                {
                                    if (_mappingMatrix[i, j] < minDist)
                                    {
                                        minDist = _mappingMatrix[i, j];
                                    }
                                }
                                int denominator = 0;
                                for (int j = 0; j < _numberOfColumns; j++)
                                {
                                    if (_mappingMatrix[i, j] == minDist)
                                    {
                                        _mappingMatrix[i, j] = 1;
                                        denominator++;
                                    }
                                    else
                                    {
                                        _mappingMatrix[i, j] = 0;
                                    }
                                }
                                for (int j = 0; j < _numberOfColumns; j++)
                                {
                                    _mappingMatrix[i, j] = _mappingMatrix[i, j]/denominator;
                                }
                            }
                        }
                        else if (_method == ElementMapperMethod.Inverse)
                        {
                            for (int i = 0; i < _numberOfRows; i++)
                            {
                                double minDist = _mappingMatrix[i, 0];
                                for (int j = 1; j < _numberOfColumns; j++)
                                {
                                    if (_mappingMatrix[i, j] < minDist)
                                    {
                                        minDist = _mappingMatrix[i, j];
                                    }
                                }
                                if (minDist == 0)
                                {
                                    int denominator = 0;
                                    for (int j = 0; j < _numberOfColumns; j++)
                                    {
                                        if (_mappingMatrix[i, j] == minDist)
                                        {
                                            _mappingMatrix[i, j] = 1;
                                            denominator++;
                                        }
                                        else
                                        {
                                            _mappingMatrix[i, j] = 0;
                                        }
                                    }
                                    for (int j = 0; j < _numberOfColumns; j++)
                                    {
                                        _mappingMatrix[i, j] = _mappingMatrix[i, j]/denominator;
                                    }
                                }
                                else
                                {
                                    double denominator = 0;
                                    for (int j = 0; j < _numberOfColumns; j++)
                                    {
                                        _mappingMatrix[i, j] = 1/_mappingMatrix[i, j];
                                        denominator = denominator + _mappingMatrix[i, j];
                                    }
                                    for (int j = 0; j < _numberOfColumns; j++)
                                    {
                                        _mappingMatrix[i, j] = _mappingMatrix[i, j]/denominator;
                                    }
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("methodDescription unknown for point to polyline mapping");
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Point to polyline mapping failed", e);
                    }

                    #endregion
                }
                else if (fromElements.ElementType == ElementType.Point &&
                         toElements.ElementType == ElementType.Polygon)
                {
                    #region

                    try
                    {
                        for (int i = 0; i < _numberOfRows; i++)
                        {
                            XYPolygon polygon = CreateXYPolygon(toElements, i);
                            int count = 0;
                            XYPoint point;
                            for (int n = 0; n < _numberOfColumns; n++)
                            {
                                point = CreateXYPoint(fromElements, n);
                                if (XYGeometryTools.IsPointInPolygon(point, polygon))
                                {
                                    if (_method == ElementMapperMethod.Mean)
                                    {
                                        count = count + 1;
                                    }
                                    else if (_method == ElementMapperMethod.Sum)
                                    {
                                        count = 1;
                                    }
                                    else
                                    {
                                        throw new Exception(
                                            "methodDescription unknown for point to polygon mapping");
                                    }
                                }
                            }
                            for (int n = 0; n < _numberOfColumns; n++)
                            {
                                point = CreateXYPoint(fromElements, n);

                                if (XYGeometryTools.IsPointInPolygon(point, polygon))
                                {
                                    _mappingMatrix[i, n] = 1.0/count;
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Point to polygon mapping failed", e);
                    }

                    #endregion
                }
                else if (fromElements.ElementType == ElementType.PolyLine &&
                         toElements.ElementType == ElementType.Point)
                {
                    #region

                    try
                    {
                        for (int i = 0; i < _numberOfRows; i++)
                        {
                            XYPoint toPoint = CreateXYPoint(toElements, i);
                            for (int j = 0; j < _numberOfColumns; j++)
                            {
                                XYPolyline fromPolyLine = CreateXYPolyline(fromElements, j);
                                _mappingMatrix[i, j] =
                                    XYGeometryTools.CalculatePolylineToPointDistance(fromPolyLine, toPoint);
                            }
                        }

                        if (_method == ElementMapperMethod.Nearest)
                        {
                            for (int i = 0; i < _numberOfRows; i++)
                            {
                                double minDist = _mappingMatrix[i, 0];
                                for (int j = 1; j < _numberOfColumns; j++)
                                {
                                    if (_mappingMatrix[i, j] < minDist)
                                    {
                                        minDist = _mappingMatrix[i, j];
                                    }
                                }
                                int denominator = 0;
                                for (int j = 0; j < _numberOfColumns; j++)
                                {
                                    if (_mappingMatrix[i, j] == minDist)
                                    {
                                        _mappingMatrix[i, j] = 1;
                                        denominator++;
                                    }
                                    else
                                    {
                                        _mappingMatrix[i, j] = 0;
                                    }
                                }
                                for (int j = 0; j < _numberOfColumns; j++)
                                {
                                    _mappingMatrix[i, j] = _mappingMatrix[i, j]/denominator;
                                }
                            }
                        }
                        else if (_method == ElementMapperMethod.Inverse)
                        {
                            for (int i = 0; i < _numberOfRows; i++)
                            {
                                double minDist = _mappingMatrix[i, 0];
                                for (int j = 1; j < _numberOfColumns; j++)
                                {
                                    if (_mappingMatrix[i, j] < minDist)
                                    {
                                        minDist = _mappingMatrix[i, j];
                                    }
                                }
                                if (minDist == 0)
                                {
                                    int denominator = 0;
                                    for (int j = 0; j < _numberOfColumns; j++)
                                    {
                                        if (_mappingMatrix[i, j] == minDist)
                                        {
                                            _mappingMatrix[i, j] = 1;
                                            denominator++;
                                        }
                                        else
                                        {
                                            _mappingMatrix[i, j] = 0;
                                        }
                                    }
                                    for (int j = 0; j < _numberOfColumns; j++)
                                    {
                                        _mappingMatrix[i, j] = _mappingMatrix[i, j]/denominator;
                                    }
                                }
                                else
                                {
                                    double denominator = 0;
                                    for (int j = 0; j < _numberOfColumns; j++)
                                    {
                                        _mappingMatrix[i, j] = 1/_mappingMatrix[i, j];
                                        denominator = denominator + _mappingMatrix[i, j];
                                    }
                                    for (int j = 0; j < _numberOfColumns; j++)
                                    {
                                        _mappingMatrix[i, j] = _mappingMatrix[i, j]/denominator;
                                    }
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("methodDescription unknown for polyline to point mapping");
                        }
                    }
                    catch (Exception e) // Catch for all of the Point to Polyline part 
                    {
                        throw new Exception("Polyline to point mapping failed", e);
                    }

                    #endregion
                }
                else if (fromElements.ElementType == ElementType.PolyLine &&
                         toElements.ElementType == ElementType.Polygon)
                {
                    #region

                    try
                    {
                        // For each polygon in target
                        for (int i = 0; i < _numberOfRows; i++)
                        {
                            XYPolygon polygon = CreateXYPolygon(toElements, i);

                            if (_method == ElementMapperMethod.WeightedMean)
                            {
                                double totalLineLengthInPolygon = 0;
                                for (int n = 0; n < _numberOfColumns; n++)
                                {
                                    XYPolyline polyline = CreateXYPolyline(fromElements, n);
                                    _mappingMatrix[i, n] =
                                        XYGeometryTools.CalculateLengthOfPolylineInsidePolygon(
                                            polyline, polygon);
                                    totalLineLengthInPolygon += _mappingMatrix[i, n];
                                }
                                if (totalLineLengthInPolygon > 0)
                                {
                                    for (int n = 0; n < _numberOfColumns; n++)
                                    {
                                        _mappingMatrix[i, n] = _mappingMatrix[i, n]/
                                                               totalLineLengthInPolygon;
                                    }
                                }
                            }
                            else if (_method == ElementMapperMethod.WeightedSum)
                            {
                                // For each line segment in PolyLine
                                for (int n = 0; n < _numberOfColumns; n++)
                                {
                                    XYPolyline polyline = CreateXYPolyline(fromElements, n);
                                    _mappingMatrix[i, n] =
                                        XYGeometryTools.CalculateLengthOfPolylineInsidePolygon(
                                            polyline, polygon)/polyline.GetLength();
                                }
                            }
                            else
                            {
                                throw new Exception(
                                    "methodDescription unknown for polyline to polygon mapping");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Polyline to polygon mapping failed", e);
                    }

                    #endregion
                }
                else if (fromElements.ElementType == ElementType.Polygon &&
                         toElements.ElementType == ElementType.Point)
                {
                    #region

                    try
                    {
                        for (int n = 0; n < _numberOfRows; n++)
                        {
                            XYPoint point = CreateXYPoint(toElements, n);
                            for (int i = 0; i < _numberOfColumns; i++)
                            {
                                XYPolygon polygon = CreateXYPolygon(fromElements, i);
                                if (XYGeometryTools.IsPointInPolygon(point, polygon))
                                {
                                    if (_method == ElementMapperMethod.Value)
                                    {
                                        _mappingMatrix[n, i] = 1.0;
                                    }
                                    else
                                    {
                                        throw new Exception(
                                            "methodDescription unknown for polygon to point mapping");
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Polygon to point mapping failed", e);
                    }

                    #endregion
                }
                else if (fromElements.ElementType == ElementType.Polygon &&
                         toElements.ElementType == ElementType.PolyLine)
                    // Polygon to PolyLine
                {
                    #region

                    try
                    {
                        for (int i = 0; i < _numberOfRows; i++)
                        {
                            XYPolyline polyline = CreateXYPolyline(toElements, i);

                            if (_method == ElementMapperMethod.WeightedMean)
                            {
                                for (int n = 0; n < _numberOfColumns; n++)
                                {
                                    XYPolygon polygon = CreateXYPolygon(fromElements, n);
                                    _mappingMatrix[i, n] =
                                        XYGeometryTools.CalculateLengthOfPolylineInsidePolygon(
                                            polyline, polygon)/polyline.GetLength();
                                }
                                double sum = 0;
                                for (int n = 0; n < _numberOfColumns; n++)
                                {
                                    sum += _mappingMatrix[i, n];
                                }
                                for (int n = 0; n < _numberOfColumns; n++)
                                {
                                    _mappingMatrix[i, n] = _mappingMatrix[i, n]/sum;
                                }
                            }
                            else if (_method == ElementMapperMethod.WeightedSum)
                            {
                                for (int n = 0; n < _numberOfColumns; n++)
                                {
                                    XYPolygon polygon = CreateXYPolygon(fromElements, n);
                                    _mappingMatrix[i, n] =
                                        XYGeometryTools.CalculateLengthOfPolylineInsidePolygon(
                                            polyline, polygon)/polyline.GetLength();
                                }
                            }
                            else
                            {
                                throw new Exception(
                                    "methodDescription unknown for polygon to polyline mapping");
                            }
                        }
                    }
                    catch (Exception e) // catch for all of Polygon to PolyLine
                    {
                        throw new Exception("Polygon to polyline mapping failed", e);
                    }

                    #endregion
                }
                else if (fromElements.ElementType == ElementType.Polygon &&
                         toElements.ElementType == ElementType.Polygon)
                    // Polygon to Polygon
                {
                    #region

                    try
                    {
                        for (int i = 0; i < _numberOfRows; i++)
                        {
                            XYPolygon toPolygon = CreateXYPolygon(toElements, i);

                            for (int j = 0; j < _numberOfColumns; j++)
                            {
                                XYPolygon fromPolygon = CreateXYPolygon(fromElements, j);
                                _mappingMatrix[i, j] = XYGeometryTools.CalculateSharedArea(
                                    toPolygon, fromPolygon);
                                if (_method == ElementMapperMethod.Distribute)
                                {
                                    _mappingMatrix[i, j] /= fromPolygon.GetArea();
                                }
                            }

                            if (_method == ElementMapperMethod.WeightedMean)
                            {
                                double denominator = 0;
                                for (int j = 0; j < _numberOfColumns; j++)
                                {
                                    denominator = denominator + _mappingMatrix[i, j];
                                }
                                for (int j = 0; j < _numberOfColumns; j++)
                                {
                                    if (denominator != 0)
                                    {
                                        _mappingMatrix[i, j] = _mappingMatrix[i, j]/denominator;
                                    }
                                }
                            }
                            else if (_method == ElementMapperMethod.WeightedSum)
                            {
                                for (int j = 0; j < _numberOfColumns; j++)
                                {
                                    _mappingMatrix[i, j] = _mappingMatrix[i, j]/toPolygon.GetArea();
                                }
                            }
                            else if (_method != ElementMapperMethod.Distribute)
                            {
                                throw new Exception(
                                    "methodDescription unknown for polygon to polygon mapping");
                            }
                        }
                    }
                    catch (Exception e) // catch for all of Polygon to Polygon
                    {
                        throw new Exception("Polygon to polygon mapping failed", e);
                    }

                    #endregion
                }
                else // if the fromElementType, toElementType combination is no implemented
                {
                    throw new Exception(
                        "Mapping of specified ElementTypes not included in ElementMapper");
                }
            }
            catch (Exception e)
            {
                throw new Exception("UpdateMappingMatrix failed to update mapping matrix", e);
            }
        }

        /// <summary>
        /// Extracts the (row, column) element from the MappingMatrix.
        /// </summary>
        /// 
        /// <param name="row">Zero based row index</param>
        /// <param name="column">Zero based column index</param>
        /// <returns>
        /// Element(row, column) from the mapping matrix.
        /// </returns>
        public double GetValueFromMappingMatrix(int row, int column)
        {
            try
            {
                ValidateIndicies(row, column);
            }
            catch (Exception e)
            {
                throw new Exception("GetValueFromMappingMatrix failed.", e);
            }
            return _mappingMatrix[row, column];
        }

        /// <summary>
        /// Sets individual the (row, column) element in the MappingMatrix.
        /// </summary>
        /// 
        /// <param name="value">Element value to set</param>
        /// <param name="row">Zero based row index</param>
        /// <param name="column">Zero based column index</param>
        /// <returns>
        /// No value is returned.
        /// </returns>
        public void SetValueInMappingMatrix(double value, int row, int column)
        {
            try
            {
                ValidateIndicies(row, column);
            }
            catch (Exception e)
            {
                throw new Exception("SetValueInMappingMatrix failed.", e);
            }
            _mappingMatrix[row, column] = value;
        }

        private void ValidateIndicies(int row, int column)
        {
            if (row < 0)
            {
                throw new Exception("Negative row index not allowed. GetValueFromMappingMatrix failed.");
            }
            if (row >= _numberOfRows)
            {
                throw new Exception(
                    "Row index exceeds mapping matrix dimension. GetValueFromMappingMatrix failed.");
            }
            if (column < 0)
            {
                throw new Exception("Negative column index not allowed. GetValueFromMappingMatrix failed.");
            }
            if (column >= _numberOfColumns)
            {
                throw new Exception(
                    "Column index exceeds mapping matrix dimension. GetValueFromMappingMatrix failed.");
            }
        }

        /// <summary>
        /// Gives a list of descriptions (strings) for available mapping methods 
        /// given the combination of fromElementType and toElementType
        /// </summary>
        /// 
        /// <param name="sourceElementType">Element type of the elements in
        /// the fromElementset</param>
        /// <param name="targetElementType">Element type of the elements in
        /// the toElementset</param>
        /// 
        /// <returns>
        ///	<p>ArrayList of method descriptions</p>
        public static IIdentifiable[] GetAvailableMethods(ElementType sourceElementType, ElementType targetElementType)
        {
            var methods = new List<IIdentifiable>();

            for (int i = 0; i < _AvailableMethods.Length; i++)
            {
                if (sourceElementType == _AvailableMethods[i].FromElementsShapeType)
                {
                    if (targetElementType == _AvailableMethods[i].ToElementsShapeType)
                    {
                        methods.Add(new Identifier(_AvailableMethods[i].Id){ Description = _AvailableMethods[i].Description});
                    }
                }
            }
            return methods.ToArray();
        }

        /// <summary>
        /// This method will return an ArrayList of <see cref="ITimeSpaceAdaptedOutput"/> that the ElementMapper 
        /// provides when mapping from the ElementType specified in the method argument. 
        /// </summary>
        /// <remarks>
        ///  Each <see cref="ITimeSpaceAdaptedOutput"/> object will contain 3 IArguments:
        ///  <p> [Key]              [Value]                      [ReadOnly]    [Description]----------------- </p>
        ///  <p> ["Type"]           ["SpatialMapping"]           [true]        ["Using the ElementMapper"] </p>
        ///  <p> ["Caption"]        [The Operation Caption]      [true]        ["Internal ElementMapper adaptedOutput Caption"] </p>
        ///  <p> ["Description"]    [The Operation Description]  [true]        ["Using the ElementMapper"] </p>
        ///  <p> ["ToElementType"]  [ElementType]                [true]        ["Valid To-Element Types"]  </p>
        /// </remarks>
        /// <returns>
        ///  ArrayList which contains the available <see cref="ITimeSpaceAdaptedOutput"/>.
        /// </returns>
        public static List<IArgument> GetAdaptedOutputArguments(IIdentifiable methodIdentifier)
        {
            if (!(methodIdentifier.Id.StartsWith(_ElementMapperPrefix)))
            {
                throw new Exception("Unknown method identifier: " + methodIdentifier);
            }

            for (int i = 0; i < _AvailableMethods.Length; i++)
            {
                if (String.Compare(_AvailableMethods[i].Id, methodIdentifier.Id)==0)
                {
                    var arguments = new List<IArgument>();
                    arguments.Add(new ArgumentString("Caption", _AvailableMethods[i].Id,
                                                     "Internal ElementMapper AdaptedOutput Caption"));
                    arguments.Add(new ArgumentString("Description", _AvailableMethods[i].Description,
                                                     "Operation description"));
                    arguments.Add(new ArgumentString("Type", "SpatialMapping", "Using the ElementMapper"));
                    arguments.Add(new ArgumentString("FromElementType",
                                                     _AvailableMethods[i].FromElementsShapeType.ToString(),
                                                     "Valid From-Element Types"));
                    arguments.Add(new ArgumentString("ToElementType",
                                                     _AvailableMethods[i].ToElementsShapeType.ToString(),
                                                     "Valid To-Element Types"));
                    return arguments;
                }
            }
            throw new Exception("Unknown methodID: " + methodIdentifier.Id);
        }

        private static XYPoint CreateXYPoint(IElementSet elementSet, int index)
        {
            if (elementSet.ElementType != ElementType.Point)
            {
                throw new ArgumentOutOfRangeException("elementSet",
                                                      "Cannot create XYPoint, the element type of the element set is not XYPoint");
            }

            return new XYPoint(elementSet.GetVertexXCoordinate(index, 0), elementSet.GetVertexYCoordinate(index, 0));
        }

        private static XYPolyline CreateXYPolyline(IElementSet elementSet, int index)
        {
            if (!(elementSet.ElementType == ElementType.PolyLine))
            {
                throw new Exception("Cannot create XYPolyline");
            }

            var xyPolyline = new XYPolyline();
            for (int i = 0; i < elementSet.GetVertexCount(index); i++)
            {
                xyPolyline.Points.Add(new XYPoint(elementSet.GetVertexXCoordinate(index, i),
                                                  elementSet.GetVertexYCoordinate(index, i)));
            }

            return xyPolyline;
        }

        private static XYPolygon CreateXYPolygon(IElementSet elementSet, int index)
        {
            if (elementSet.ElementType != ElementType.Polygon)
            {
                throw new Exception("Cannot create XYPolyline");
            }

            var xyPolygon = new XYPolygon();

            for (int i = 0; i < elementSet.GetVertexCount(index); i++)
            {
                xyPolygon.Points.Add(new XYPoint(elementSet.GetVertexXCoordinate(index, i),
                                                 elementSet.GetVertexYCoordinate(index, i)));
            }

            return xyPolygon;
        }

        public ElementType GetTargetElementType()
        {
            throw new NotImplementedException();
        }

        public static IDescribable GetAdaptedOutputDescription(IIdentifiable identifiable)
        {
            SMethod sMethod = FindSMethod(identifiable);
            return new Describable(sMethod.Id, sMethod.Description);
        }

        public static ElementMapperMethod GetMethod(IIdentifiable identifiable)
        {
          return FindSMethod(identifiable).ElementMapperMethod;
        }

        public static ElementType GetToElementType(IIdentifiable identifiable)
        {
          return FindSMethod(identifiable).ToElementsShapeType;
        }


        private static SMethod FindSMethod(IIdentifiable identifiable)
        {
            foreach (SMethod availableMethod in _AvailableMethods)
            {
                if (String.Compare(availableMethod.Id, identifiable.Id) == 0)
                {
                    return availableMethod;
                }
            }
            throw new Exception("Invalid method indentifier in Update Mapping Matrix funtion " + identifiable.Id);
        }

        #region Nested type: SMethod

        private struct SMethod
        {
            public string Description;
            public ElementMapperMethod ElementMapperMethod;
            public ElementType FromElementsShapeType;
            public string Id;
            public ElementType ToElementsShapeType;
        }

        #endregion
    }
}