using OpenSSLUI.codebase;
using System;
using System.Collections;
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

        private void PopUpCreateCACertOKBtn_Click(object sender, RoutedEventArgs e)
        {
            // Create a config file using the infomation that user has keyed in

            // List of fields to validate if the user has keyed in required infomation
            Hashtable _FieldList = new Hashtable
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
                        string _ConfigFolderName = "configFolder";
                        string _ConfigFileName = "OpenSSLCreateCACertConfig.txt";
                        bool _ConfigFolderExist = Directory.Exists(Path.Combine(_OpenSSLUIPATHEnvVar, _ConfigFolderName));
                        if (!_ConfigFolderExist) 
                        {
                            // Create folder first
                            Directory.CreateDirectory(Path.Combine(_OpenSSLUIPATHEnvVar, _ConfigFolderName));
                        }

                        // Create config file
                        string configFileFullPath = Path.Combine(_OpenSSLUIPATHEnvVar, _ConfigFolderName, _ConfigFileName);
                        if (File.Exists(configFileFullPath)) 
                        {
                            File.Delete(configFileFullPath);
                        }

                        string _CountryName = "countryName="+_PopUpCreateCACertCounrtyNameTF.Text;
                        string _StateProviceName = "stateOrProvinceName="+_PopUpCreateCACertStateTF.Text;
                        string _LocationCity = "localityName="+_PopUpCreateCACertLocationTF.Text;
                        string _OrganizationName = "organizationName="+_PopUpCreateCACertOrgNameTF.Text;
                        string _OrganizationUnitName = "organizationalUnitName="+_PopUpCreateCACertOrgUnitTF.Text;
                        string _CommonName = "commonName="+_PopUpCreateCACertCommonNameTF.Text;
                        string _EmailAddress = "emailAddress="+_PopUpCreateCACertEmailTF.Text;

                      
                        if (string.IsNullOrEmpty(_PopUpCreateCACertStateTF.Text)) 
                        {
                            _StateProviceName = "";
                        }

                        if (string.IsNullOrEmpty(_PopUpCreateCACertLocationTF.Text))
                        {
                            _LocationCity = "";
                        }

                        if (string.IsNullOrEmpty(_PopUpCreateCACertOrgNameTF.Text))
                        {
                            _OrganizationName = "";
                        }

                        if (string.IsNullOrEmpty(_PopUpCreateCACertOrgUnitTF.Text))
                        {
                            _OrganizationUnitName = "";
                        }

                        if (string.IsNullOrEmpty(_PopUpCreateCACertEmailTF.Text))
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
