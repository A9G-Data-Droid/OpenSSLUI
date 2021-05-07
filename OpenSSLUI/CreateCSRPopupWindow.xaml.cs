using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections;
using System.IO;
using OpenSSLUI.codebase;

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

        private void _PopUpCreateCSRCancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void _PopUpCreateCSROKBtn_Click(object sender, RoutedEventArgs e)
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
                    String _ErrorX = (String)_IEnumerator.Current;
                    if (_ErrorX != null && !_ErrorX.Equals(string.Empty))
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
                if (!String.IsNullOrEmpty(_PopUpCreateCSREmailTF.Text))
                {

                     _ValidEmailFormat = OpenSSLFieldValidator.ValidateFormat(_PopUpCreateCSREmailTF.Text, "Email", "Email Address ");
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
                        String _ConfigFolderName = "csrFolder";
                        String _ConfigFileName = "OpenSSLCreateCSRConfig.txt";
                        bool _ConfigFolderExist = Directory.Exists(_OpenSSLUIPATHEnvVar + "\\" + _ConfigFolderName);
                        if (!_ConfigFolderExist)
                        {
                            //create folder first
                            Directory.CreateDirectory(_OpenSSLUIPATHEnvVar + "\\" + _ConfigFolderName);
                        }
                        //Create config file
                        bool _ConfigFileExist = File.Exists(_OpenSSLUIPATHEnvVar + "\\" + _ConfigFolderName + "\\" + _ConfigFileName);
                        if (_ConfigFileExist)
                        {
                            File.Delete(_OpenSSLUIPATHEnvVar + "\\" + _ConfigFolderName + "\\" + _ConfigFileName);
                        }

                        String _CountryName = "countryName=" + _PopUpCreateCSRCounrtyNameTF.Text;
                        String _StateProviceName = "stateOrProvinceName=" + _PopUpCreateCSRStateTF.Text;
                        String _LocationCity = "localityName=" + _PopUpCreateCSRLocationTF.Text;
                        String _OrganizationName = "organizationName=" + _PopUpCreateCSROrgNameTF.Text;
                        String _OrganizationUnitName = "organizationalUnitName=" + _PopUpCreateCSROrgUnitTF.Text;
                        String _CommonName = "commonName=" + _PopUpCreateCSRCommonNameTF.Text;
                        String _EmailAddress = "emailAddress=" + _PopUpCreateCSREmailTF.Text;

                       
                        if (String.IsNullOrEmpty(_PopUpCreateCSRStateTF.Text))
                        {
                            _StateProviceName = "";
                        }
                        if (String.IsNullOrEmpty(_PopUpCreateCSRLocationTF.Text))
                        {
                            _LocationCity = "";
                        }
                        if (String.IsNullOrEmpty(_PopUpCreateCSROrgNameTF.Text))
                        {
                            _OrganizationName = "";
                        }
                        if (String.IsNullOrEmpty(_PopUpCreateCSROrgUnitTF.Text))
                        {
                            _OrganizationUnitName = "";
                        }
                        if (String.IsNullOrEmpty(_PopUpCreateCSREmailTF.Text))
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
