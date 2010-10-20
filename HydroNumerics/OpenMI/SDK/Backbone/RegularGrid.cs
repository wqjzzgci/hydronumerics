
using System;
using OpenMI.Standard;

namespace HydroNumerics.OpenMI.Sdk.Backbone
{
	/// <summary>

	/// </summary>
	public class RegularGrid : IElementSet
	{
        private int    numberOfCells;		 // Number of cells in the grid
        private double[] cellCenterXCoords;  // X-coordinates for all grid cells, starting from buttom left and going up column by column
        private double[] cellCenterYCoords;  // Y-coordinates for all grid cells, starting from buttom left and going up column by column
        private double cellSize;             // The size of each individual kvadratic grid cell (all cells are the same size) [meters]
        private double gridAngle;            // rotation angle of the grid in deg.
        private int nx;
        private int ny;

        	
        public RegularGrid(double origX, double origY, double cellSize, int nx, int ny, double gridAngle)
        {
            this.nx = nx;
            this.ny = ny;
            this.numberOfCells = nx * ny;
            this.cellCenterXCoords = new double[nx * ny];
            this.cellCenterYCoords = new double[nx * ny];
            this.cellSize = cellSize;
            this.gridAngle = gridAngle;

            int index = 0;
            for (int n = 0; n < ny; n++)
            {
                for (int i = 0; i < nx; i++)
                {
                    cellCenterXCoords[index] = origX + i * cellSize + 0.5 * cellSize;
                    index++;
                }
            }

            index = 0;
            //for (int n = 0; n < ny; n++)
            for (int n = ny - 1; n >= 0; n--)
            {
                for (int i = 0; i < nx; i++)
                {
                    cellCenterYCoords[index] = origY + n * cellSize + 0.5 * cellSize;
                    index++;
                }
            }
        }

		
		public int Version
		{
			get
			{
				return 0;
			}
		}

   
		/// <summary>
		/// Return elementID
		/// </summary>
		/// <param name="elementIndex"></param>
		/// <returns>Index in the ElementSet</returns>
		public string GetElementID(int elementIndex)
		{
			return(cellCenterXCoords[elementIndex].ToString()+"_"+cellCenterYCoords[elementIndex].ToString());
		}

		/// <summary>
		/// ElementSet description
		/// </summary>
		public string Description
		{
			get
			{
				return "Regular Grid";
			}
		}

		/// <summary>
		/// ElementSet ID
		/// </summary>
		public string ID
		{
			get
			{
				return "RegularGrid";
			}
		}

		/// <summary>
		/// ElementType. Always ElementType.XYPolygon
		/// </summary>
		public ElementType ElementType
		{
			get
			{ 
				return ElementType.XYPolygon;
			}
		}

		/// <summary>
		/// Returns x-coordinates for a requested vertex in the ElementSet
		/// </summary>
		/// <param name="elementIndex">Element index</param>
		/// <param name="vertexIndex">Vertex index</param>
		/// <returns>x-coordinate</returns>
		public double GetXCoordinate(int elementIndex, int vertexIndex)
		{
            double angle = Math.PI * gridAngle / 180.0;
            double x = GetNonRotatedXCoordinate(elementIndex,vertexIndex);

			if (gridAngle > 0)
            {
                //double x0 = cellCenterXCoords[0] - cellSize * 0.5;
                //double y0 = cellCenterYCoords[0] - cellSize * 0.5;
                double x0 = cellCenterXCoords[nx*(ny-1)] - cellSize * 0.5;
                double y0 = cellCenterYCoords[nx * (ny - 1)] - cellSize * 0.5;
                double y = GetNonRotatedYCoordinate(elementIndex,vertexIndex);

                double alpha;
                
                if ((x - x0) == 0)
                {
                    alpha = Math.PI / 2.0;
                }
                else 
                {
                    alpha = Math.Atan((y-y0)/(x-x0));
                }

                x = Math.Sqrt((x - x0)*(x - x0) + (y - y0)*(y-y0)) * Math.Cos(angle + alpha) + x0;

            }

            return x;
		}
		
		/// <summary>
		/// Returns the y-coordinate for the requested vertex in the ElementSet
		/// </summary>
		/// <param name="elementIndex">element index</param>
		/// <param name="vertexIndex">vertex index</param>
		/// <returns>y-coordinate</returns>
		public double GetYCoordinate(int elementIndex, int vertexIndex)
		{
            double angle = Math.PI * gridAngle / 180.0;
            double y = GetNonRotatedYCoordinate(elementIndex,vertexIndex);

            if (gridAngle > 0)
            {
                double y0 = cellCenterYCoords[nx * (ny - 1)] - cellSize * 0.5;
                double x0 = cellCenterXCoords[nx * (ny - 1)] - cellSize * 0.5;
                double x = GetNonRotatedXCoordinate(elementIndex,vertexIndex);

                double alpha;
                
                if ((x - x0) == 0)
                {
                    alpha = Math.PI / 2.0;
                }
                else 
                {
                   alpha = Math.Atan((y-y0)/(x-x0));
                }

                y = Math.Sqrt((x - x0)*(x - x0) + (y - y0)*(y-y0)) * Math.Sin(angle + alpha) + y0;
            }

            return y;
		}

       

		/// <summary>
		/// This elementset is only defined in two dimensions. Calling this method will 
		/// give an exception
		/// </summary>
		/// <param name="elementIndex">ElementSet index</param>
		/// <param name="vertexIndex">Vertex index</param>
		/// <returns>Always throws an exception</returns>
		public double GetZCoordinate(int elementIndex, int vertexIndex)
		{
			throw new System.Exception("Z coordinate is not defined for an XYPolygon");
		}

		/// <summary>
		/// ElementCount. The number of elements in the ElementSet
		/// </summary>
		public int ElementCount
		{
			get
			{
				return numberOfCells;
			}
		}

		/// <summary>
		/// Return the number of vertex in the requested element in the ElementSet.
		/// In this implementation this method always return 4.
		/// </summary>
		/// <param name="elementIndex">Element index</param>
		/// <returns>returns always 4</returns>
		public int GetVertexCount(int elementIndex)
		{
			return 4;
		}

		/// <summary>
		/// No spatial reference is defined for this class.
		/// </summary>
		public ISpatialReference SpatialReference
		{
			get
			{
				return new SpatialReference("no reference");
			}
		}
        
		/// <summary>
		/// Returns the index number of an element in the ElementSet
		/// </summary>
		/// <param name="elementID">Element ID</param>
		/// <returns>index</returns>
		public int GetElementIndex(string elementID)
		{
			string xID = "-999";
			string yID = "-999";
			char [] spilt = new char[]{'_'};
			int ctr = 0;
			foreach(string subString in elementID.Split(spilt))
			{
				if(ctr == 0) xID = subString;
				if(ctr == 1) yID = subString;
				ctr ++;
			}

			return (GetElementIndex(double.Parse(xID),double.Parse(yID)));
		}

		/// <summary>
		/// Returns the ElementIndex for a specified location (x,y)
		/// </summary>
		/// <param name="x">x-coordinate</param>
		/// <param name="y">y-coordinate</param>
		/// <returns>Element index</returns>
		private int GetElementIndex(double x,double y)
		{
			for (int iref = 0; iref<numberOfCells; iref++)
			{
				if((Math.Abs(cellCenterXCoords[iref]-x)<double.Epsilon) && (Math.Abs(cellCenterYCoords[iref]-y) < double.Epsilon))
				{
					return iref;
				}
			}

			return -999;
		}


		/// <summary>
		/// This ElementSet i two-dimensional, so this method will always return null
		/// </summary>
		/// <param name="elementIndex">Element index</param>
		/// <param name="faceIndex">face index</param>
		/// <returns>always returns null</returns>
		public int[] GetFaceVertexIndices(int elementIndex, int faceIndex)
		{
		      return null;
		}

		/// <summary>
		/// This elementSet is two-dimensional, so this method always returns zero
		/// </summary>
		/// <param name="elementIndex">Element index</param>
		/// <returns>always return zero</returns>
		public int GetFaceCount(int elementIndex)
		{
			return 0;
		}

        private double GetNonRotatedXCoordinate(int elementIndex, int vertexIndex)
        {
            if(vertexIndex==0 || vertexIndex==3 )
            {
                return (cellCenterXCoords[elementIndex]- cellSize/2);
            }
            else if(vertexIndex==1 || vertexIndex==2)
            {
                return (cellCenterXCoords[elementIndex]+ cellSize/2);
            }
            else
            {
                throw new System.Exception("Vertex index outside range [0;4]. Vertex index was:"+vertexIndex.ToString());
            }
        }
		
        
        private double GetNonRotatedYCoordinate(int elementIndex, int vertexIndex)
        {
            if(vertexIndex==0 || vertexIndex==1 )
            {
                return (cellCenterYCoords[elementIndex]-cellSize/2);
            }
            else if(vertexIndex==2 || vertexIndex==3)
            {
                return (cellCenterYCoords[elementIndex]+cellSize/2);
            }
            else
            {
                throw new System.Exception("Vertex index outside range [0;4]. Vertex index was:"+vertexIndex.ToString());
            }
        }

        /// <summary>
        /// Number of celles in the finite volume grid including boundary cells
        /// </summary>
        public int NumberOfCells
        {
            get
            {
                return numberOfCells;
            }
        }

        /// <summary>
        /// The size of each individual kvadratic grid cell (all cells are the same size) [meters]
        /// </summary>
        public double CellSize
        {
            get
            {
                return cellSize;
            }
        }

        /// <summary>
        /// X-coordinates for all grid cells, starting from buttom left and going up column by column
        /// </summary>
        public double[] CellCenterXCoords
        {
            get
            {
                return cellCenterXCoords;
            }
        }

        /// <summary>
        /// Y-coordinates for all grid cells, starting from buttom left and going up column by column
        /// </summary>
        public double[] CellCenterYCoords
        {
            get
            {
                return cellCenterYCoords;
            }
        }

        public double GridAngle
        {
            get
            {
                return gridAngle;
            }
        }

	}
}