using System;
using System.Windows.Forms;

namespace MouseHighlighter
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new HighlighterForm());
        }
    }
}
