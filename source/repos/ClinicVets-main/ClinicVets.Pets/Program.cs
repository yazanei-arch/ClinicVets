using ClinicVets.UI;
using System;
using System.Windows.Forms;

namespace ClinicVets.Pets
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new PetManagementForm());
        }
    }
}