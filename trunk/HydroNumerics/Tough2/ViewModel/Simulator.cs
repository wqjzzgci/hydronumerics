
using System;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;


namespace HydroNumerics.Tough2.ViewModel
{

  public delegate void SimulationFinishedHandler(object sender, SimulationInfo SimInfo);

  public class Simulator
  {
    private Model _tough2;
    private Process p;
    private StringBuilder totalOutput;

    public event SimulationFinishedHandler SimulationFinished;

    /// <summary>
    /// Gets and sets the executable, I.e. T2voc.exe, t2eco2m.exe ...
    /// </summary>
    public string Executable { get; set; }

    /// <summary>
    /// Gets the total output;
    /// </summary>
    public string TotalOutput
    {
      get
      {
        return totalOutput.ToString();
      }
    }

    public Simulator(Model M)
    {
      _tough2 = M;
      Executable = @"C:\Jacob\Tough2\T2VOC\T2VOC\Debug\T2voc.exe";
    }


    /// <summary>
    /// Runs a simulation using the selected executable
    /// </summary>
    public void Run(bool Async)
    {
      _killed = false;
      _tough2.Results.Clear();
      totalOutput = new StringBuilder();

      Directory.SetCurrentDirectory(_tough2.ModelDirectory);
      // Start the child process.
      p = new Process();
      // Redirect the output stream of the child process.
      p.StartInfo.UseShellExecute = false;
      p.StartInfo.RedirectStandardOutput = true;
      p.StartInfo.RedirectStandardInput = true;
      p.StartInfo.FileName = Executable;

      p.StartInfo.CreateNoWindow = true;

      p.Start();

      p.OutputDataReceived += new DataReceivedEventHandler(p_OutputDataReceived);

      if (Async)
        p.Exited += new EventHandler(p_Exited);
      p.EnableRaisingEvents = true;

      p.BeginOutputReadLine();

      p.StandardInput.Write(_tough2.FileContent);

      if (!Async)
      {
        p.WaitForExit();
        //To avoid a race condition
        p_Exited(this, null);
      }
    }

    /// <summary>
    /// Stops the simulation
    /// </summary>
    public void Stop()
    {
      if (p != null & !p.HasExited)
      {
        p.Kill();
        _killed = true;
      }
    }

    private bool _killed = false;

    public SimulationInfo si { get; private set; }

    /// <summary>
    /// This method is called on exit. Both synchrone and asynchrone
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void p_Exited(object sender, EventArgs e)
    {
        si = new SimulationInfo();
        si.TotalRuntime = p.TotalProcessorTime;
        if (_killed)
          si.ExitInfo = CauseOfStop.StoppedByUser;
        else
        {
          if (p.ExitCode != 0)
            si.ExitInfo = CauseOfStop.ConvergenceFailure;
          else
            si.ExitInfo = CauseOfStop.TotalTimeReached;

        }
        if (SimulationFinished != null)
        {
          SimulationFinished(this, si);
      }
    }

    //Do not put too much in this thread as it will hold up the simulation
    private void p_OutputDataReceived(object sender, DataReceivedEventArgs e)
    {
      if (!String.IsNullOrEmpty(e.Data))
      {
        totalOutput.AppendLine(e.Data);
        _tough2.Results.ReadOutputLine(e.Data);
      }
    }
  }
}
