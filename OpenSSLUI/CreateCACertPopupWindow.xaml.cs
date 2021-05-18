using OpenSSLUI.codebase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace OpenSSLUI
{
    /// <summary>
    /// Interaction logic for CreateCACertPopupWindow.xaml
    /// </summary>
    public partial class CreateCACertPopupWindow : Window
    {
        public CreateCACertPopupWindow()
        {
            InitializeComponent();
        }

        private void PopUpCreateCACertCancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Create a config file using the infomation that user has keyed in
        private void PopUpCreateCACertOKBtn_Click(object sender, RoutedEventArgs e)
        {
            // List of fields to validate if the user has keyed in required infomation
            Hashtable _FieldList = new()
            // Only the common name is made mandatory...
            {
                { "Country Name", _PopUpCreateCACertCounrtyNameTF },
                //{ "Full State or Province ", _PopUpCreateCACertStateTF },
                //{ "Location (City) ", _PopUpCreateCACertLocationTF },
                //{ "Organization name (Company) ", _PopUpCreateCACertOrgNameTF },
                //{ "Organizational Unit ", _PopUpCreateCACertOrgUnitTF },
                //{ "Email Address ", _PopUpCreateCACertEmailTF },
                { "Common Name ", _PopUpCreateCACertCommonNameTF }
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

            // Validate email address format
            OpenSSLFieldValidator.ClearErrorList();
            bool _ValidEmailFormat = true;
            if (!string.IsNullOrEmpty(_PopUpCreateCACertEmailTF.Text))
            {
                _ValidEmailFormat = OpenSSLFieldValidator.ValidateFormat(_PopUpCreateCACertEmailTF.Text, "Email", "Email Address ");
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
            string _ConfigFolderPath = OpenSSLConfig.ConfigFolderName;
            if (!Directory.Exists(_ConfigFolderPath))
            {
                //create folder first
                Directory.CreateDirectory(_ConfigFolderPath);
            }

            // Create config file
            if (File.Exists(OpenSSLConfig.CAConfigFileFullPath))
            {
                File.Delete(OpenSSLConfig.CAConfigFileFullPath);
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

            configParameters.Add("countryName   =   " + _PopUpCreateCACertCounrtyNameTF.Text);

            if (!string.IsNullOrEmpty(_PopUpCreateCACertStateTF.Text))
            {
                configParameters.Add("stateOrProvinceName   =   " + _PopUpCreateCACertStateTF.Text);
            }

            if (!string.IsNullOrEmpty(_PopUpCreateCACertLocationTF.Text))
            {
                configParameters.Add("localityName  =   " + _PopUpCreateCACertLocationTF.Text);
            }

            if (!string.IsNullOrEmpty(_PopUpCreateCACertOrgNameTF.Text))
            {
                configParameters.Add("organizationName  =   " + _PopUpCreateCACertOrgNameTF.Text);
            }

            if (!string.IsNullOrEmpty(_PopUpCreateCACertOrgUnitTF.Text))
            {
                configParameters.Add("organizationalUnitName    =   " + _PopUpCreateCACertOrgUnitTF.Text);
            }

            // Required
            configParameters.Add("commonName    =   " + _PopUpCreateCACertCommonNameTF.Text);

            if (!string.IsNullOrEmpty(_PopUpCreateCACertEmailTF.Text))
            {
                configParameters.Add("emailAddress  =   " + _PopUpCreateCACertEmailTF.Text);
            }

            // Crete the file
            File.WriteAllLines(OpenSSLConfig.CAConfigFileFullPath, configParameters);
            MessageBoxResult _MessageBoxResult = MessageBox.Show("Certificate Infomation captured successfully!", "SUCCESS", MessageBoxButton.OKCancel, MessageBoxImage.Information);

            if (_MessageBoxResult.ToString().Equals("OK", StringComparison.CurrentCultureIgnoreCase))
            {
                Close();
            }
        }
    }
}
