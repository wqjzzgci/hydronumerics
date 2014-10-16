using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.IO;

namespace HydroNumerics.Core
{
  public class FtpDownloader
  {
    public string UriString { get; set; }
    public string UserName { get; set; }
    public string PassWord { get; set; }

    public FtpDownloader(string UriString, string UserName, string password)
    {
      this.UriString = UriString;
      this.UserName = UserName;
      this.PassWord = password;
    }


    /// <summary>
    /// Tries to get the content of the file from the server
    /// </summary>
    /// <param name="FileName"></param>
    /// <param name="FileContent"></param>
    /// <returns></returns>
    public bool TryGetFile(string FileName, out string FileContent)
    {
      try
      {
        string file = UriString + "/" + FileName; 

        // Get the object used to communicate with the server.
        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(file);
        request.Method = WebRequestMethods.Ftp.DownloadFile;
        
        request.Credentials = new NetworkCredential(UserName, PassWord);

        FtpWebResponse response = (FtpWebResponse)request.GetResponse();

        Stream responseStream = response.GetResponseStream();
        StreamReader reader = new StreamReader(responseStream);

        FileContent = reader.ReadToEnd();

        reader.Close();
        response.Close();
        return true;
      }
      catch (Exception e)
      {
        FileContent = "";
        return false;
      }
    }

    /// <summary>
    /// Returns the time of the last update for date file
    /// </summary>
    /// <param name="FileName"></param>
    /// <returns></returns>
    public DateTime? GetFileDate(string FileName)
    {
      try
      {
        string file = UriString + "/" + FileName;

        // Get the object used to communicate with the server.
        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(file);
        request.Method = WebRequestMethods.Ftp.GetDateTimestamp;
        request.Credentials = new NetworkCredential(UserName, PassWord);

        using (FtpWebResponse resp = (FtpWebResponse)request.GetResponse())
        {
          return resp.LastModified;
        }

      }
      catch(Exception)
      {

      }
      return null;

    }


    /// <summary>
    /// Gets the list of files on the server
    /// </summary>
    /// <returns></returns>
    public List<string> GetFileList()
    {
      List<string> files = new List<string>();
      // Get the object used to communicate with the server.
      FtpWebRequest request = (FtpWebRequest)WebRequest.Create(UriString);
      request.Method = WebRequestMethods.Ftp.ListDirectory;
      request.Credentials = new NetworkCredential(UserName, PassWord);
      FtpWebResponse response = (FtpWebResponse)request.GetResponse();

      Stream responseStream = response.GetResponseStream();
      StreamReader reader = new StreamReader(responseStream);

      while (!reader.EndOfStream)
      {
        files.Add(reader.ReadLine());
      }
      reader.Close();
      response.Close();

      return files;
    }
  }
}
