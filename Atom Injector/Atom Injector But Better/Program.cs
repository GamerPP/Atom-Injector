using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Atom_Injector_But_Better
{
	internal static class Program
	{
		[STAThread]
		private static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new AtomInjector());
		}
	}
}