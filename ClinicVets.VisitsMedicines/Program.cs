using System;
using System.Windows.Forms;

namespace ClinicVets.VisitsMedicines
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args != null && args.Length > 0)
            {
                string mode = (args[0] ?? string.Empty).Trim();
                if (mode.Equals("medicines", StringComparison.OrdinalIgnoreCase))
                {
                    Application.Run(new Form1());
                    return;
                }

                if (mode.Length > 0)
                {
                    Application.Run(new VisitForm(mode));
                    return;
                }
            }

            Application.Run(new VisitForm());
        }
    }
}
