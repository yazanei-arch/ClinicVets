using System;
using System.Windows.Forms;

namespace ClinicVets.VisitsMedicines
{
    class Program
    {
        static void Main(string[] args)
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware); // Adds crispness
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new VisitForm());
        }
    }
}