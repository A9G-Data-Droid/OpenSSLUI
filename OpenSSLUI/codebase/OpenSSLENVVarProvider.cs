using System;

namespace OpenSSLUI.codebase
{
    internal static class OpenSSLENVVarProvider
    {
        private const string OPENSSL_UI_PATH = "OPENSSL_UI_PATH";

        public static string GetCurrentDirPath()
        {
            // Get the path to the running process
            string _CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            return _CurrentDirectory;
        }

        public static string GetOPENSSLUIPATHEnvVar()
        {
            string _OpenSSLUIPath = Environment.GetEnvironmentVariable(OPENSSL_UI_PATH);
            if (string.IsNullOrEmpty(_OpenSSLUIPath))
            {
                _OpenSSLUIPath = GetCurrentDirPath();
                SetOPENSSLUIPATHEnvVar(_OpenSSLUIPath);
            }

            return _OpenSSLUIPath;
        }

        public static void SetOPENSSLUIPATHEnvVar(string newPath)
        {
            if (!string.IsNullOrEmpty(newPath))
            {
                Environment.SetEnvironmentVariable(OPENSSL_UI_PATH, newPath);
            }
        }
    }
}
