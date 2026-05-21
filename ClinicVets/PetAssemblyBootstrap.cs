using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace ClinicVets
{
    /// <summary>Loads ClinicVets.Pets at runtime without a circular project reference.</summary>
    internal static class PetAssemblyBootstrap
    {
        private static bool _resolveHandlerAttached;

        internal static void TryRegister()
        {
            try
            {
                EnsureAssemblyResolveHandler();

                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string petsPath = Path.Combine(baseDir, "ClinicVets.Pets.exe");
                if (!File.Exists(petsPath))
                {
                    return;
                }

                Assembly petsAssembly = Assembly.LoadFrom(petsPath);
                Type bootstrapType = petsAssembly.GetType("ClinicVets.Pets.PetFlowBootstrap", throwOnError: false);
                if (bootstrapType == null)
                {
                    return;
                }

                MethodInfo register = bootstrapType.GetMethod(
                    "Register",
                    BindingFlags.Public | BindingFlags.Static);
                if (register == null)
                {
                    return;
                }

                register.Invoke(null, null);
            }
            catch (TargetInvocationException ex)
            {
                ShowLoadError(ex.InnerException ?? ex);
            }
            catch (Exception ex)
            {
                ShowLoadError(ex);
            }
        }

        private static void EnsureAssemblyResolveHandler()
        {
            if (_resolveHandlerAttached)
            {
                return;
            }

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            _resolveHandlerAttached = true;
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            try
            {
                string simpleName = new AssemblyName(args.Name).Name;
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string candidate = Path.Combine(baseDir, simpleName + ".dll");
                if (File.Exists(candidate))
                {
                    return Assembly.LoadFrom(candidate);
                }
            }
            catch
            {
            }

            return null;
        }

        private static void ShowLoadError(Exception ex)
        {
            MessageBox.Show(
                "Pet module could not be loaded." + Environment.NewLine + Environment.NewLine
                + (ex?.Message ?? "Unknown error."),
                "ClinicVets",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }
}
