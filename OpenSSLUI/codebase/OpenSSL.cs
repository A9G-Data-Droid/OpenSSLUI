using System;
using System.Diagnostics;
using System.Windows;

namespace OpenSSLUI.codebase
{
#pragma warning disable S101 // Types should be named in PascalCase
    internal static class OpenSSL
#pragma warning restore S101 // Types should be named in PascalCase
    {
        public static void TryOpenSSL(string _InvocationParameters)
        {
            Console.WriteLine("Default config file: " + OpenSSLEnvVarProvider.GetOpenSSLConfEnvVar());

            try
            {
                // Open a command window and keep it open to show the results
                using Process openSSL = new();
                openSSL.StartInfo.FileName = "CMD.EXE";
                openSSL.StartInfo.Arguments = "/K \"\"" + OpenSSLConfig.OpenSSLExecutableFileName + "\" " + _InvocationParameters + "\"";
                openSSL.StartInfo.UseShellExecute = false;
                openSSL.StartInfo.ErrorDialog = true;

                Console.WriteLine(openSSL.StartInfo.FileName + " " + openSSL.StartInfo.Arguments);

                openSSL.Start();

                if (!openSSL.HasExited)
                {
                    openSSL.WaitForExit();
                }
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                string _ErrorX = ex.Message + Environment.NewLine + OpenSSLConfig.OpenSSLExecutableFileName;
                MessageBox.Show(_ErrorX, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
