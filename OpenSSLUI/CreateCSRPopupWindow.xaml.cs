using OpenSSLUI.codebase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace OpenSSLUI
{
    /// <summary>
    /// Interaction logic for CreateCSRPopupWindow.xaml
    /// </summary>
    public partial class CreateCSRPopupWindow : Window
    {
        public CreateCSRPopupWindow()
        {
            InitializeComponent();
        }

        private void PopUpCreateCSRCancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void PopUpCreateCSROKBtn_Click(object sender, RoutedEventArgs e)
        {
            // Create a config file using the infomation that user has keyed in

            // List of fields to validate if the user has keyed in required infomation
            Hashtable _FieldList = new()
            // Only the common name is made mandatory...
            {
                { "Country Name", _PopUpCreateCSRCounrtyNameTF },
                //{ "Full State or Province ", _PopUpCreateCSRStateTF },
                //{ "Location (City) ", _PopUpCreateCSRLocationTF },
                //{ "Organization name (Company) ", _PopUpCreateCSROrgNameTF },
                //{ "Organizational Unit ", _PopUpCreateCSROrgUnitTF },
                //{ "Email Address ", _PopUpCreateCSREmailTF },
                { "Common Name ", _PopUpCreateCSRCommonNameTF }
            };

            OpenSSLFieldValidator.ClearErrorList();
            if (!OpenSSLFieldValidator.ValidateTextFields(_FieldList))
            {
                ArrayList _ErrorList = OpenSSLFieldValidator.GetErrorList();
                IEnumerator _IEnumerator = _ErrorList.GetEnumerator();
                while (_IEnumerator.MoveNext())
                {
                    string _ErrorX = (string)_IEnumerator.Current;
                    if (!string.IsNullOrEmpty(_ErrorX))
                    {
                        MessageBox.Show(_ErrorX, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                return;
            }

            //validate email address format
            OpenSSLFieldValidator.ClearErrorList();

            bool _ValidEmailFormat = true;
            if (!string.IsNullOrEmpty(_PopUpCreateCSREmailTF.Text))
            {
                _ValidEmailFormat = OpenSSLFieldValidator.ValidateFormat(_PopUpCreateCSREmailTF.Text, "Email", "Email Address ");
            }

            if (!_ValidEmailFormat)
            {
                ArrayList _ErrorList = OpenSSLFieldValidator.GetErrorList();
                IEnumerator _IEnumerator = _ErrorList.GetEnumerator();
                while (_IEnumerator.MoveNext())
                {
                    string _ErrorX = (string)_IEnumerator.Current;
                    if (!string.IsNullOrEmpty(_ErrorX))
                    {
                        MessageBox.Show(_ErrorX, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                return;
            }

            // Check ENV VAR 
            string _OpenSSLUIPATHEnvVar = OpenSSLEnvVarProvider.GetOPENSSLUIPATHEnvVar();
            if (string.IsNullOrEmpty(_OpenSSLUIPATHEnvVar))
            {
                MessageBox.Show("OPENSSL_UI_PATH is not set, please set the path before continue!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Proceed the creation of config file
            if (!Directory.Exists(OpenSSLConfig.ConfigFolderName))
            {
                // Create folder first
                Directory.CreateDirectory(OpenSSLConfig.ConfigFolderName);
            }

            // Create config file                        
            if (File.Exists(OpenSSLConfig.CSRConfigFileFullPath))
            {
                File.Delete(OpenSSLConfig.CSRConfigFileFullPath);
            }

            List<string> configParameters = new()
            {
                "HOME   =    .",
                "",
                "[ ca ]",
                "default_ca  =   CA_default",
                "",
                "[ CA_default ]",
                "dir = ./ ",
                "",
                "[ req ]",
                "prompt =   no",
                "distinguished_name =   req_distinguished_name",
                "",
                "[ req_distinguished_name ]"
            };

            configParameters.Add("countryName   =   " + _PopUpCreateCSRCounrtyNameTF.Text);

            if (!string.IsNullOrEmpty(_PopUpCreateCSRStateTF.Text))
            {
                configParameters.Add("stateOrProvinceName   =   " + _PopUpCreateCSRStateTF.Text);
            }

            if (!string.IsNullOrEmpty(_PopUpCreateCSRLocationTF.Text))
            {
                configParameters.Add("localityName  =   " + _PopUpCreateCSRLocationTF.Text);
            }

            if (!string.IsNullOrEmpty(_PopUpCreateCSROrgNameTF.Text))
            {
                configParameters.Add("organizationName  =   " + _PopUpCreateCSROrgNameTF.Text);
            }

            if (!string.IsNullOrEmpty(_PopUpCreateCSROrgUnitTF.Text))
            {
                configParameters.Add("organizationalUnitName    =   " + _PopUpCreateCSROrgUnitTF.Text);
            }

            // Required
            configParameters.Add("commonName    =   " + _PopUpCreateCSREmailTF.Text);

            if (!string.IsNullOrEmpty(_PopUpCreateCSREmailTF.Text))
            {
                configParameters.Add("emailAddress  =   " + _PopUpCreateCSREmailTF.Text);
            }

            // Crete the file
            File.WriteAllLines(OpenSSLConfig.CSRConfigFileFullPath, configParameters);
            MessageBoxResult _MessageBoxResult = MessageBox.Show("Certificate Infomation captured successfully!", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);

            if (_MessageBoxResult.ToString().Equals("OK", StringComparison.CurrentCultureIgnoreCase))
            {
                Close();
            }
        }
    }
}
