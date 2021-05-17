using OpenSSLUI.codebase;
using System;
using System.Collections;
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
            Hashtable _FieldList = new Hashtable
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
            bool isValid = OpenSSLFieldValidator.ValidateTextFields(_FieldList);
            if (!isValid)
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
            }
            else
            {
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
                }
                else // Proceed the creation of config file
                {
                    // Check ENV VAR 
                    string _OpenSSLUIPATHEnvVar = OpenSSLENVVarProvider.GetOPENSSLUIPATHEnvVar();
                    if (string.IsNullOrEmpty(_OpenSSLUIPATHEnvVar))
                    {
                        MessageBox.Show("OPENSSL_UI_PATH is not set, please set the path before continue!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        string _ConfigFolderName = "csrFolder";
                        string _ConfigFileName = "OpenSSLCreateCSRConfig.txt";
                        bool _ConfigFolderExist = Directory.Exists(Path.Combine(_OpenSSLUIPATHEnvVar, _ConfigFolderName));
                        if (!_ConfigFolderExist)
                        {
                            //create folder first
                            Directory.CreateDirectory(Path.Combine(_OpenSSLUIPATHEnvVar, _ConfigFolderName));
                        }
                        //Create config file
                        string configFileFullPath = Path.Combine(_OpenSSLUIPATHEnvVar, _ConfigFolderName, _ConfigFileName);
                        if (File.Exists(configFileFullPath))
                        {
                            File.Delete(configFileFullPath);
                        }

                        string _CountryName = "countryName=" + _PopUpCreateCSRCounrtyNameTF.Text;
                        string _StateProviceName = "stateOrProvinceName=" + _PopUpCreateCSRStateTF.Text;
                        string _LocationCity = "localityName=" + _PopUpCreateCSRLocationTF.Text;
                        string _OrganizationName = "organizationName=" + _PopUpCreateCSROrgNameTF.Text;
                        string _OrganizationUnitName = "organizationalUnitName=" + _PopUpCreateCSROrgUnitTF.Text;
                        string _CommonName = "commonName=" + _PopUpCreateCSRCommonNameTF.Text;
                        string _EmailAddress = "emailAddress=" + _PopUpCreateCSREmailTF.Text;

                       
                        if (string.IsNullOrEmpty(_PopUpCreateCSRStateTF.Text))
                        {
                            _StateProviceName = "";
                        }

                        if (string.IsNullOrEmpty(_PopUpCreateCSRLocationTF.Text))
                        {
                            _LocationCity = "";
                        }

                        if (string.IsNullOrEmpty(_PopUpCreateCSROrgNameTF.Text))
                        {
                            _OrganizationName = "";
                        }

                        if (string.IsNullOrEmpty(_PopUpCreateCSROrgUnitTF.Text))
                        {
                            _OrganizationUnitName = "";
                        }

                        if (string.IsNullOrEmpty(_PopUpCreateCSREmailTF.Text))
                        {
                            _EmailAddress = "";
                        }

                        string[] _CreateCACertInfo = {"[ req ]","prompt=no","distinguished_name = req_distinguished_name",
                                                     "","[req_distinguished_name ]",_OrganizationName,_OrganizationUnitName,
                                                     _EmailAddress,_LocationCity,_StateProviceName,_CountryName,_CommonName};
                        //crete the file again
                        File.WriteAllLines(configFileFullPath, _CreateCACertInfo);
                        MessageBoxResult _MessageBoxResult = MessageBox.Show("Certificate Infomation captured successfully!", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);

                        if (_MessageBoxResult.ToString().Equals("OK", StringComparison.CurrentCultureIgnoreCase))
                        {
                            Close();
                        }
                    }
                }
            }
        }
    }
}
