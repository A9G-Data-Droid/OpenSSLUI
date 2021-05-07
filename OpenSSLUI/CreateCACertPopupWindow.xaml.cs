using System;
using System.Windows;
using OpenSSLUI.codebase;
using System.Collections;
using System.IO;

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

        private void _PopUpCreateCACertCancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void _PopUpCreateCACertOKBtn_Click(object sender, RoutedEventArgs e)
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
                    String _ErrorX = (String)_IEnumerator.Current;
                    if (_ErrorX != null && !_ErrorX.Equals(string.Empty))
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
                if (!String.IsNullOrEmpty(_PopUpCreateCACertEmailTF.Text)) 
                {
                    _ValidEmailFormat = OpenSSLFieldValidator.ValidateFormat(_PopUpCreateCACertEmailTF.Text, "Email", "Email Address ");
                }
                
                if (!_ValidEmailFormat)
                {
                    ArrayList _ErrorList = OpenSSLFieldValidator.GetErrorList();
                    IEnumerator _IEnumerator = _ErrorList.GetEnumerator();
                    while (_IEnumerator.MoveNext())
                    {
                        String _ErrorX = (String)_IEnumerator.Current;
                        if (_ErrorX != null && !_ErrorX.Equals(string.Empty))
                        {
                            MessageBox.Show(_ErrorX, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else // Proceed the creation of config file
                {                  
                    // Check ENV VAR 
                    String _OpenSSLUIPATHEnvVar = OpenSSLENVVarProvider.GetOPENSSLUIPATHEnvVar();
                    if (String.IsNullOrEmpty(_OpenSSLUIPATHEnvVar))
                    {
                        MessageBox.Show("OPENSSL_UI_PATH is not set, please set the path before continue!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else 
                    {
                        String _ConfigFolderName = "configFolder";
                        String _ConfigFileName = "OpenSSLCreateCACertConfig.txt";
                        bool _ConfigFolderExist = Directory.Exists(_OpenSSLUIPATHEnvVar + "\\" + _ConfigFolderName);
                        if (!_ConfigFolderExist) 
                        {
                            // Create folder first
                            Directory.CreateDirectory(_OpenSSLUIPATHEnvVar + "\\" + _ConfigFolderName);
                        }
                        // Create config file
                        bool _ConfigFileExist = File.Exists(_OpenSSLUIPATHEnvVar + "\\" + _ConfigFolderName + "\\" + _ConfigFileName);
                        if (_ConfigFileExist) 
                        {
                            File.Delete(_OpenSSLUIPATHEnvVar + "\\" + _ConfigFolderName + "\\" + _ConfigFileName);
                        } 

                        String _CountryName = "countryName="+_PopUpCreateCACertCounrtyNameTF.Text;
                        String _StateProviceName = "stateOrProvinceName="+_PopUpCreateCACertStateTF.Text;
                        String _LocationCity = "localityName="+_PopUpCreateCACertLocationTF.Text;
                        String _OrganizationName = "organizationName="+_PopUpCreateCACertOrgNameTF.Text;
                        String _OrganizationUnitName = "organizationalUnitName="+_PopUpCreateCACertOrgUnitTF.Text;
                        String _CommonName = "commonName="+_PopUpCreateCACertCommonNameTF.Text;
                        String _EmailAddress = "emailAddress="+_PopUpCreateCACertEmailTF.Text;

                      
                        if (String.IsNullOrEmpty(_PopUpCreateCACertStateTF.Text)) 
                        {
                            _StateProviceName = "";
                        }
                        if (String.IsNullOrEmpty(_PopUpCreateCACertLocationTF.Text))
                        {
                            _LocationCity = "";
                        }
                        if (String.IsNullOrEmpty(_PopUpCreateCACertOrgNameTF.Text))
                        {
                            _OrganizationName = "";
                        }
                        if (String.IsNullOrEmpty(_PopUpCreateCACertOrgUnitTF.Text))
                        {
                            _OrganizationUnitName = "";
                        }
                        if (String.IsNullOrEmpty(_PopUpCreateCACertEmailTF.Text))
                        {
                            _EmailAddress = "";
                        }

                        String[] _CreateCACertInfo = {"[ req ]","prompt=no","distinguished_name = req_distinguished_name",
                                                     "","[req_distinguished_name ]",_OrganizationName,_OrganizationUnitName,
                                                     _EmailAddress,_LocationCity,_StateProviceName,_CountryName,_CommonName};
                        //crete the file again
                        File.WriteAllLines(_OpenSSLUIPATHEnvVar + "\\" + _ConfigFolderName + "\\" + _ConfigFileName, _CreateCACertInfo);
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
