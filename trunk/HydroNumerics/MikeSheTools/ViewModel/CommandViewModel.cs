using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace HydroNumerics.MikeSheTools.ViewModel
{
  public class CommandViewModel:BaseViewModel
  {
    public CommandViewModel(string displayName, ICommand command )
    {
      if (command==null)
        throw new ArgumentNullException("command");

      DisplayName = displayName;
      Command = command;
    }

    public ICommand Command {get;private set;}

  }
}
