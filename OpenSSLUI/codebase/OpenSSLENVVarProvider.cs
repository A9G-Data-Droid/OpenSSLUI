using System;

namespace OpenSSLUI.codebase
{
    class OpenSSLENVVarProvider
    {
        public static String GetCurrentDirPath() 
        {
            // Get the path to the running process
            String _CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            return _CurrentDirectory;
        }

        public static String GetOPENSSLUIPATHEnvVar() 
        {
            // Environmental variable "OPENSSL_UI_PATH"
            String _OpenSSLUIPath = Environment.GetEnvironmentVariable("OPENSSL_UI_PATH");
            if (string.IsNullOrEmpty(_OpenSSLUIPath))
            {
                _OpenSSLUIPath = GetCurrentDirPath();
                //Environment.SetEnvironmentVariable("OPENSSL_UI_PATH", _OpenSSLUIPath);
            }
            return _OpenSSLUIPath;
        }
    }
}
