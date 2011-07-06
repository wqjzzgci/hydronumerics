using System;
using System.IO;
using HydroNumerics.MikeSheTools.PFS.SheFile;

namespace HydroNumerics.MikeSheTools.Core
{
	/// <summary>
	/// This class provides different filenames from the .she-file
	/// </summary>
	public class FileNames
	{
		private string _fileNameWithPath;
    private string _fileNameWithoutPath;
    private string _resultsPath;
    private InputFile _input;


    internal FileNames(InputFile Input)
    {
      _input = Input;
      _fileNameWithPath = Input.FileName;
      Initialize();
    }


    private void Initialize()
    {
      //Gets the filename without path and extension
      _fileNameWithoutPath = Path.GetFileNameWithoutExtension(_fileNameWithPath);

      //Todo read from SheFile when necessary
      _resultsPath = Path.Combine(Path.GetDirectoryName(_fileNameWithPath), _fileNameWithoutPath + ".she - Result Files");

    }
    
    public FileNames(string MSheFileName)
		{
			CheckFiles(Path.GetFullPath(MSheFileName));
			_fileNameWithPath = MSheFileName;
      _input = new InputFile(MSheFileName);
      Initialize();
		}

    public FileNames()
    {

    }

		/// <summary>
    /// Gets the Fif-filename
		/// </summary>
		/// <returns></returns>
		public string FifFileName
		{
      get
      {
        string FName = Path.Combine(_resultsPath , Path.ChangeExtension (_fileNameWithoutPath, ".fif"));
        CheckFiles(FName);
        return FName;
      }
		}

    /// <summary>
    /// Gets the .wel Filename
    /// </summary>
    public string WelFileName
    {
      get
      {
        return _input.MIKESHE_FLOWMODEL.SaturatedZone.Well.Filename; 
      }
    }

    /// <summary>
    /// Gets the .sim 11 filename
    /// </summary>
    public string Sim11FileName
    {
      get
      {
        return _input.MIKESHE_FLOWMODEL.River.Filename;
      }
    }



		//Returns the .dfs0 file with MikeShe to Mike11 flow.
		public string MShe2RiverFileName(int Code)
		{
			string extension="";
			if (Code==1)
				extension="_Dr2River.dfs0";
			else if (Code==2)
				extension="_OL2River.dfs0";
			else if (Code==3)
				extension="_SZ2River.dfs0";
			else if (Code==4)
				extension="_Total2River.dfs0";
      return getFile(extension);
		}

    public string SZ2DFileName
    {
      get
      {
        return getFile("_2DSZ.dfs2");
      }
    }

		public string SZ3DFileName
		{
      get
      {
        return getFile("_3DSZ.dfs3");
      }
		}
/// <summary>
/// Returns the FileName for the flow-file;
/// </summary>
/// <returns></returns>
    public string SZ3DFlowFileName
    {
      get
      {
        return getFile("_3DSZflow.dfs3");
      }
    }

    public string PreProcessed2D
    {
      get
      {
        return getFile("_PreProcessed.DFS2");
      }
    }

    public string PreProcessedSZ3D
    {
      get
      {
        return getFile("_PreProcessed_3DSZ.dfs3");
      }
    }

    public string DetailedTimeSeriesSZ
    {
      get
      {
        return getFile("DetailedTS_SZ.dfs0");
      }
    }

    public string DetailedTimeSeriesM11
    {
      get
      {
        return getFile("DetailedTS_M11.dfs0");
      }
    }

    /// <summary>
    /// Gets a string with the name and path of the .she-file
    /// </summary>
    public string SheFile
    {
      get
      {
        return this._fileNameWithPath;
      }
    }

    /// <summary>
    /// Gets the ouput directory
    /// </summary>
    public string ResultsDirectory
    {
      get
      {
        return _resultsPath;
      }
    }

		//Throws an exception if the file does not exist
		public void CheckFiles(params string[] FileNames)
		{
			foreach (string file in FileNames)
			{
				if (!File.Exists(file))
					throw new FileNotFoundException (file + " ikke fundet!");
			}
		}

    private string getFile(string extension)
    {
      string FileName = Path.Combine(_resultsPath, _fileNameWithoutPath + extension);
      //CheckFiles(FileName);
      return FileName;
    }

	}
}
