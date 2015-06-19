using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace HydroNumerics.Core
{
  public class BufferedFileSender
  {
    private Timer t;
    private FtpDownloader FTPServer;


    private Dictionary<string, string> _FileBuffer;
    public Dictionary<string, string> FileBuffer
    {
      get {
        if (_FileBuffer == null)
          _FileBuffer = new Dictionary<string, string>();
        return _FileBuffer; 
      }
    }
    
    
    
    Dictionary<string, string> OldFiles = new Dictionary<string, string>();

    public BufferedFileSender(FtpDownloader FTPServer, TimeSpan ResendInterval)
    {
      this.FTPServer = FTPServer;
      t = new Timer(ResendInterval.TotalMilliseconds);
      t.Elapsed += t_Elapsed;
      t.AutoReset = true;
      t.Start();
    }

    void t_Elapsed(object sender, ElapsedEventArgs e)
    {
      t.Stop();
      List<string> SucceedFiles = new List<string>();
      foreach (var item in OldFiles)
      {
        if (FTPServer.TryPutFile(item.Key, item.Value))
        {
          SucceedFiles.Add(item.Key);
          LastSendTime = DateTime.UtcNow;
        }

      }
      foreach (var item in SucceedFiles)
        OldFiles.Remove(item);

      t.Start();
    }



    public void SendFile(string filename, string FileContent)
    {
      if (!FTPServer.TryPutFile(filename, FileContent))
        OldFiles.Add(filename, FileContent);
      else
        LastSendTime = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets the time of the last succeded file transfer
    /// </summary>
    public DateTime LastSendTime { get; set; }


  }
}
