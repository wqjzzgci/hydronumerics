using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;


namespace PostInstallation
{
  [RunInstaller(true)]
  public partial class LaunchVideo : System.Configuration.Install.Installer
  {
    public LaunchVideo()
    {
      InitializeComponent();
    }

    [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
    public override void Install(IDictionary stateSaver)
    {
      base.Install(stateSaver);
    }

    [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
    public override void Commit(IDictionary savedState)
    {
      base.Commit(savedState);
      System.Diagnostics.Process.Start(@"..\instructionvideos\wellviewer1 på dansk.wmv");
    }

    [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
    public override void Rollback(IDictionary savedState)
    {
      base.Rollback(savedState);
    }

    [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
    public override void Uninstall(IDictionary savedState)
    {
      base.Uninstall(savedState);
    }


  }
}
