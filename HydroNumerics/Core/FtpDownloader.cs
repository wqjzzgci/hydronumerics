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

    public bool CheckConnection()
    {
      try
      {
        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(UriString);
        request.KeepAlive = false;
        request.Credentials = new NetworkCredential(UserName, PassWord);
        request.GetResponse();
      }
      catch (WebException ex)
      {
        return false;
      }
      return true;      
    }


    public bool TryPutFile(string FileName, string FileContent)
    {
      bool succes = false;
      try
      {
        string file = UriString + "/" + FileName;
        // Get the object used to communicate with the server.
        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(file);
        request.Method = WebRequestMethods.Ftp.UploadFile;
        request.KeepAlive = false;

        if (string.IsNullOrEmpty(UserName))
          UserName = "anonymous";
        request.Credentials = new NetworkCredential(UserName, PassWord);

        Stream requestStream = request.GetRequestStream();
        var fileContents = Encoding.UTF8.GetBytes(FileContent);
        requestStream.Write(fileContents, 0, fileContents.Length);
        requestStream.Close();

        FtpWebResponse response = (FtpWebResponse)request.GetResponse();

        if(response.StatusCode == FtpStatusCode.ClosingData)
          succes = true;
       
      }
      catch (Exception e)
      {
 
      }

      return succes;
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
        request.KeepAlive = false;

        request.Credentials = new NetworkCredential(UserName, PassWord);

        FtpWebResponse response = (FtpWebResponse)request.GetResponse();

        Stream responseStream = response.GetResponseStream();
        StreamReader reader = new StreamReader(responseStream, Encoding.Default);

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
        request.KeepAlive = false;

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
      request.KeepAlive = false;
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
