using OpenSSLUI.codebase;
using System.IO;

namespace OpenSSLUI
{
#pragma warning disable S101 // Types should be named in PascalCase
    internal static class OpenSSLConfig
#pragma warning restore S101 // Types should be named in PascalCase
    {
        public static string DEFAULT_BIT_LENGTH => Properties.Settings.Default.DEFAULT_BIT_LENGTH;
        public static string DEFAULT_VALIDITY_PERIOD => Properties.Settings.Default.DEFAULT_VALIDITY_PERIOD;
        public static string DEFAULT_CA_VALIDITY_PERIOD => Properties.Settings.Default.DEFAULT_CA_VALIDITY_PERIOD;
        public static string DEFAULT_CA_ALOGORITHM => Properties.Settings.Default.DEFAULT_CA_ALOGORITHM;

        public static string OpenSSLExecutableFileName => Path.Combine(OpenSSLEnvVarProvider.GetOPENSSLUIPATHEnvVar(), Properties.Settings.Default.OpenSSLExecutableFileName);

        public static string ConfigFolderName => Properties.Settings.Default.ConfigFolderName;

        public static string CSRConfigFileFullPath => Path.Combine(OpenSSLEnvVarProvider.GetOPENSSLUIPATHEnvVar(), ConfigFolderName, Properties.Settings.Default.CSRConfigFile);

        public static string CAConfigFileFullPath => Path.Combine(OpenSSLEnvVarProvider.GetOPENSSLUIPATHEnvVar(), ConfigFolderName, Properties.Settings.Default.CAConfigFile);
    }
}
