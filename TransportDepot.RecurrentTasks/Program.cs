using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransportDepot.RecurrentTasks
{
  class Program
  {
    static void Main(string[] args)
    {
      var type = typeof(ICommand);
      var types = AppDomain.CurrentDomain.GetAssemblies()
          .SelectMany(s => s.GetTypes())
          .Where(p => type.IsAssignableFrom(p)).Where(t=> t.BaseType != null).ToList();
          

      foreach (Type t in types)
      {
        var o = (ICommand) Activator.CreateInstance(t);
        o.Execute();
      }
    }

  }
}
