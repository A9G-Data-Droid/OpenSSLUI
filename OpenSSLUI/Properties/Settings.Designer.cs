﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OpenSSLUI.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.8.1.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("config")]
        public string ConfigFolderName {
            get {
                return ((string)(this["ConfigFolderName"]));
            }
            set {
                this["ConfigFolderName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("OpenSSLCreateCSRConfig.txt")]
        public string CSRConfigFile {
            get {
                return ((string)(this["CSRConfigFile"]));
            }
            set {
                this["CSRConfigFile"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("OpenSSLCreateCACertConfig.txt")]
        public string CAConfigFile {
            get {
                return ((string)(this["CAConfigFile"]));
            }
            set {
                this["CAConfigFile"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("openssl.exe")]
        public string OpenSSLExecutableFileName {
            get {
                return ((string)(this["OpenSSLExecutableFileName"]));
            }
            set {
                this["OpenSSLExecutableFileName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("2048")]
        public string DEFAULT_BIT_LENGTH {
            get {
                return ((string)(this["DEFAULT_BIT_LENGTH"]));
            }
            set {
                this["DEFAULT_BIT_LENGTH"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("365")]
        public string DEFAULT_VALIDITY_PERIOD {
            get {
                return ((string)(this["DEFAULT_VALIDITY_PERIOD"]));
            }
            set {
                this["DEFAULT_VALIDITY_PERIOD"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1000")]
        public string DEFAULT_CA_VALIDITY_PERIOD {
            get {
                return ((string)(this["DEFAULT_CA_VALIDITY_PERIOD"]));
            }
            set {
                this["DEFAULT_CA_VALIDITY_PERIOD"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("x509")]
        public string DEFAULT_CA_ALOGORITHM {
            get {
                return ((string)(this["DEFAULT_CA_ALOGORITHM"]));
            }
            set {
                this["DEFAULT_CA_ALOGORITHM"] = value;
            }
        }
    }
}
