using System;
using System.IO;

namespace OpenSSLUI.codebase
{
#pragma warning disable S101 // Types should be named in PascalCase
    internal static class OpenSSLEnvVarProvider
#pragma warning restore S101 // Types should be named in PascalCase
    {
        private const string OPENSSL_UI_PATH = "OPENSSL_UI_PATH";
        private const string OPENSSL_CONF = "OPENSSL_CONF";

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
                Environment.SetEnvironmentVariable(OPENSSL_UI_PATH, _OpenSSLUIPath);
            }

            return _OpenSSLUIPath;
        }

        public static string GetOpenSSLConfEnvVar()
        {
            string _OpenSSLConfPath = Environment.GetEnvironmentVariable(OPENSSL_CONF);
            if (string.IsNullOrEmpty(_OpenSSLConfPath))
            {
                _OpenSSLConfPath = Path.Combine(GetOPENSSLUIPATHEnvVar(), "openssl.cfg");
                Environment.SetEnvironmentVariable(OPENSSL_CONF, _OpenSSLConfPath);
            }

            return _OpenSSLConfPath;
        }
    }
}
