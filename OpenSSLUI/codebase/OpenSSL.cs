using System;
using System.Diagnostics;
using System.Windows;

namespace OpenSSLUI.codebase
{
    internal static class OpenSSL
    {
        public static void TryOpenSSL(string _InvocationParameters)
        {
            Debug.WriteLine("Default config file: " + OpenSSLEnvVarProvider.GetOpenSSLConfEnvVar());

            try
            {
                // Open a command window and keep it open to show the results
                var openSSL = new ProcessStartInfo
                {
                    FileName = OpenSSLConfig.OpenSSLExecutableFileName,
                    Arguments = _InvocationParameters,
                    UseShellExecute = false,
                    ErrorDialog = true
                };

                Debug.WriteLine(openSSL.FileName + " " + openSSL.Arguments);

                Process.Start(openSSL).WaitForExit();
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                string _ErrorX = ex.Message + Environment.NewLine + OpenSSLConfig.OpenSSLExecutableFileName;
                MessageBox.Show(_ErrorX, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
