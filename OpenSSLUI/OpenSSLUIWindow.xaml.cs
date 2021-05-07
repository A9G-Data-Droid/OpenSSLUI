using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using OpenSSLUI.codebase;
using System.Collections;
using System.Diagnostics;
using System.IO;

namespace OpenSSLUI
{
    /// <summary>
    /// Interaction logic for OpenSSLUIWindow.xaml
    /// </summary>
    public partial class OpenSSLUIWindow : Window
    {
        private String _SELECTED_FOLDER_LOCATION = string.Empty;
        private String _SELECTED_CSR_FOLDER_LOCATION = string.Empty;
        private String _SELECTED_CA_KEY_FOLDER_LOCATION = string.Empty;
        private String _SELECTED_SIGNED_CSR_FOLDER_LOCATION = string.Empty;

        private String _DEFAULT_BIT_LENGTH = "2048";
        private String _DEFAULT_VALIDITY_PERIOD = "365";
        private String _DEFAULT_CA_VALIDITY_PERIOD = "1000";
        private String _DEFAULT_CA_ALOGORITHM = "x509";

        public OpenSSLUIWindow()
        {
            InitializeComponent();
        }

        private void _ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void _CreateCAKeyResetBtn_Click(object sender, RoutedEventArgs e)
        {
            _CaKeyNameTF.Clear();
            _KeyLocationTF.Clear();
            PasswordTF.Password = string.Empty;
            PasswordRetypeTF.Password = string.Empty;
        }

        private void _KeyLocationTriggerBtn_Click(object sender, RoutedEventArgs e)
        {
            _SELECTED_FOLDER_LOCATION = string.Empty;
            FolderBrowserDialog _FodlerBrowserDialogue = new FolderBrowserDialog();
            _FodlerBrowserDialogue.SelectedPath = OpenSSLENVVarProvider.GetCurrentDirPath();
            DialogResult _DialogueResult = _FodlerBrowserDialogue.ShowDialog();
            _SELECTED_FOLDER_LOCATION = _FodlerBrowserDialogue.SelectedPath;
            _KeyLocationTF.Text = _SELECTED_FOLDER_LOCATION;
            _FodlerBrowserDialogue.Dispose();
        }

        private void _KeyLocationTF_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void CaKeyNameTF_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void _GenerateKeyBtn_Click(object sender, RoutedEventArgs e)
        {
            String COMMAND_GENRSA = "genrsa"; 
            bool status = ValidateCreateCAKey();
            if (status)
            {
                // Match passwords 
                bool matched = MatchRetypePassword();
                // Flag to distinguise whether user has keyed in password fields
                bool passphraseNeeded = PassPhraseProvided();
                if (matched)
                {
                    // genrsa -des3 -passout pass:yourpassword -out /path/to/your/key_file 1024
                    // Process user inputs to create CA key
                    String _OpenSSLExecutableFileName = "openssl.exe";
                    String _OpenSSLUIPATHEnvVar = OpenSSLENVVarProvider.GetOPENSSLUIPATHEnvVar();
                    if (String.IsNullOrEmpty(_OpenSSLUIPATHEnvVar))
                    {
                        System.Windows.MessageBox.Show("OPENSSL_UI_PATH is not set, Please set the path before continue!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        // Variable is available, Run OpenSSL
                        String _KeyName = _CaKeyNameTF.Text;
                        String _KeyLocation = _KeyLocationTF.Text;
                        String _BitLength = _BitLengthCmb.Text;
                        String _Password = PasswordRetypeTF.Password;
                        String _InvocationParameters = string.Empty;
                        if (String.IsNullOrEmpty(_BitLength))
                        {
                            _BitLength = _DEFAULT_BIT_LENGTH;
                        }
                        String _OpenSSLExecutableFullPath = _OpenSSLUIPATHEnvVar + "\\" + _OpenSSLExecutableFileName;

                        // Execute openssl command to create a key with passphrase
                        if (passphraseNeeded)
                        {
                            _InvocationParameters = COMMAND_GENRSA + " -aes128 -passout pass:" + _Password + " -out " + _KeyLocation + "\\" + _KeyName + " " + _BitLength;
                        }
                        else 
                        {
                            // Create a key without a passphase
                            _InvocationParameters = COMMAND_GENRSA + " -out " + _KeyLocation + "\\" + _KeyName + " " + _BitLength;
                        }

                        // Start an OpenSSL process with descriptive error if not found
                        _tryOpenSSL(_OpenSSLExecutableFullPath, _InvocationParameters);                 
                    }
                }
            }
        }

        private void _tryOpenSSL(string _OpenSSLExecutableFullPath, string _InvocationParameters)
        {
            try
            {
                Process.Start(_OpenSSLExecutableFullPath, _InvocationParameters);
                System.Windows.MessageBox.Show("OpenSSL ran: " + Environment.NewLine + _InvocationParameters, "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                string _ErrorX = ex.Message + Environment.NewLine + _OpenSSLExecutableFullPath;
                System.Windows.MessageBox.Show(_ErrorX, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void _CSRLocationTriggerBtn_Click(object sender, RoutedEventArgs e)
        {
            _SELECTED_CSR_FOLDER_LOCATION = string.Empty;
            OpenFileDialog _FileDialogue = new OpenFileDialog();
            _FileDialogue.InitialDirectory = OpenSSLENVVarProvider.GetCurrentDirPath();
            DialogResult _DialogueResult = _FileDialogue.ShowDialog();
            String _FileName = _FileDialogue.FileName;
            if (!String.IsNullOrEmpty(_FileName))
            {
                _SELECTED_CSR_FOLDER_LOCATION = _FileName;
                _CSRLocationTF.Text = _SELECTED_CSR_FOLDER_LOCATION;
            }
            _FileDialogue.Dispose();

        }

        private void _CSRLocationTF_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void _SelectCAKeyLocationTF_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void _SelectCAKeyTriggerBtn_Click(object sender, RoutedEventArgs e)
        {
            _SELECTED_CA_KEY_FOLDER_LOCATION = string.Empty;
            OpenFileDialog _CAFileDialogue = new OpenFileDialog();
            _CAFileDialogue.InitialDirectory = OpenSSLENVVarProvider.GetCurrentDirPath();
            DialogResult _DialogueResult = _CAFileDialogue.ShowDialog();
            String _CAKeyFileName = _CAFileDialogue.FileName;
            if (!String.IsNullOrEmpty(_CAKeyFileName))
            {
                _SELECTED_CA_KEY_FOLDER_LOCATION = _CAKeyFileName;
                _SelectCAKeyLocationTF.Text = _SELECTED_CA_KEY_FOLDER_LOCATION;
            }
            _CAFileDialogue.Dispose();
        }

        private Boolean ValidateCreateCAKey() 
        {

            Hashtable _FieldList = new Hashtable();
            _FieldList.Add("Key Name", _CaKeyNameTF);
            _FieldList.Add("Save Location ", _KeyLocationTF);
            String _Password = PasswordTF.Password;
            String _RetypePassword = PasswordRetypeTF.Password;
            if (!String.IsNullOrEmpty(_Password) || !String.IsNullOrEmpty(_RetypePassword))
            {
                _FieldList.Add("RSA key Password ", PasswordTF);
                _FieldList.Add("Retype RSA key Password ", PasswordRetypeTF);
            }

            bool _Status = OpenSSLFieldValidator.ValidateTextFields(_FieldList);
          if (!_Status) 
          {
              ArrayList _ErrorList = OpenSSLFieldValidator.GetErrorList();
              IEnumerator _IEnumerator = _ErrorList.GetEnumerator();
              while (_IEnumerator.MoveNext())
              {
                  String _ErrorX = (String)_IEnumerator.Current;
                  if (_ErrorX != null && !_ErrorX.Equals(string.Empty))
                  {
                      System.Windows.MessageBox.Show(_ErrorX, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                  }
              }
              
          }
          OpenSSLFieldValidator.ClearErrorList();
          return _Status;
        }

        private bool MatchRetypePassword() 
        {
            String _Password = PasswordTF.Password;
            String _RetypePassword = PasswordRetypeTF.Password;
            if (!String.IsNullOrEmpty(_Password) && !String.IsNullOrEmpty(_RetypePassword)) 
            {
                if (!_RetypePassword.Equals(_Password)) 
                {
                    System.Windows.MessageBox.Show("Password fields do not match!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            return true;
        }

        private bool PassPhraseProvided() 
        {
            bool passphraseProvided = true;
            String _Password = PasswordTF.Password;
            String _RetypePassword = PasswordRetypeTF.Password;
            if (String.IsNullOrEmpty(_Password) && String.IsNullOrEmpty(_RetypePassword)) 
            {
                passphraseProvided = false;
            }
            return passphraseProvided;
        
        }

        private void _SignedCertLocationTriggerBtn_Click(object sender, RoutedEventArgs e)
        {
            _SELECTED_SIGNED_CSR_FOLDER_LOCATION = string.Empty;
            FolderBrowserDialog _FodlerBrowserDialogue = new FolderBrowserDialog();
            _FodlerBrowserDialogue.SelectedPath = OpenSSLENVVarProvider.GetCurrentDirPath();
            DialogResult _DialogueResult = _FodlerBrowserDialogue.ShowDialog();
            _SELECTED_SIGNED_CSR_FOLDER_LOCATION = _FodlerBrowserDialogue.SelectedPath;
            _SignedCertLocationTF.Text = _SELECTED_SIGNED_CSR_FOLDER_LOCATION;
            _FodlerBrowserDialogue.Dispose();
        }

        private void _ExitCSRBtn_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void _SignBtn_Click(object sender, RoutedEventArgs e)
        {
            bool csrvalidinput = ValidateSignCSR();
            if (csrvalidinput) 
            {
                String _CsrFile = _CSRLocationTF.Text;
                String _KeyFile = _SelectCAKeyLocationTF.Text;
                String _CACertificate = _SignCSRCertificateLocationTF.Text;
                String _SignedCSRCertName = _SignedCSRCertNameTF.Text;
                String _SignedCertiLocation = _SignedCertLocationTF.Text;
                String _ValidityPeriod = _ValidityPeriodCmb.Text;
                String _CACertPassword = _SignCSRCACertPasswordTF.Password;
                if (String.IsNullOrEmpty(_ValidityPeriod)) 
                {
                    _ValidityPeriod = _DEFAULT_VALIDITY_PERIOD;
                }
               
                // Sign the CSR
                bool _CSRFileExist = File.Exists(_CsrFile);
                if (!_CSRFileExist)
                {
                    System.Windows.MessageBox.Show("CSR File selected does not exists!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else 
                {
                    bool _CSRKeyFileExist = File.Exists(_KeyFile);
                    if (!_CSRKeyFileExist)
                    {
                        System.Windows.MessageBox.Show("Key File selected does not exists!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else 
                    {
                        // Sign the file
                        // openssl ca -config server.cfg -in client.csr -cert server.crt -keyfile server.key -out client.crt -days 1825
                        String _OpenSSLExecutableFileName = "openssl.exe";
                        String _OpenSSLUIPATHEnvVar = OpenSSLENVVarProvider.GetOPENSSLUIPATHEnvVar();
                        if (String.IsNullOrEmpty(_OpenSSLUIPATHEnvVar))
                        {
                            System.Windows.MessageBox.Show("OPENSSL_UI_PATH is not set , Please set the path before continue!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else 
                        {
                            String _OpenSSLConfigFileName_Client = "openssl_cl.cnd";
                            String _OpenSSLConfigFileName_Server = "openssl.cnd";
                            String _OpenSSLConfigFileName = string.Empty;
                            String _OpenSSLCertType = _SignCSRCertTypeCmb.Text;
                            if (!String.IsNullOrEmpty(_OpenSSLCertType) && _OpenSSLCertType.Equals("Client",StringComparison.CurrentCultureIgnoreCase))
                            {
                                _OpenSSLConfigFileName = _OpenSSLConfigFileName_Client;
                            }
                            else 
                            {
                                _OpenSSLConfigFileName = _OpenSSLConfigFileName_Server;
                            }
                            String _OpenSSLExecutableFullPath = _OpenSSLUIPATHEnvVar + "\\" + _OpenSSLExecutableFileName;
                            String _OpenSSLConfigPath = _OpenSSLUIPATHEnvVar + "\\" + _OpenSSLConfigFileName;

                            String _InvocationParameters = string.Empty;

                            if (!String.IsNullOrEmpty(_CACertPassword))
                            {
                                //RSA key has a password
                                _InvocationParameters = " ca " + " -passin pass:" + _CACertPassword + " -config " + _OpenSSLConfigPath + " -in " + _CsrFile + " -cert " + _CACertificate +
                                            " -keyfile " + _KeyFile + " -out " + _SignedCertiLocation + "\\" + _SignedCSRCertName +
                                            " -days " + _ValidityPeriod + " -batch";
                            }
                            else 
                            {
                                //RSA key does not have a password
                                _InvocationParameters = " ca  -config " + _OpenSSLConfigPath + " -in " + _CsrFile + " -cert " + _CACertificate +
                                            " -keyfile " + _KeyFile + " -out " + _SignedCertiLocation + "\\" + _SignedCSRCertName +
                                            " -days " + _ValidityPeriod + " -batch";
                            }

                            //Before run this command, clear the openssl database file
                            File.Delete(_OpenSSLUIPATHEnvVar+"\\"+"ca.db.index");
                            FileStream _FS = File.Create(_OpenSSLUIPATHEnvVar + "\\" + "ca.db.index");
                            _FS.Close();

                            // Start an OpenSSL process with descriptive error if not found
                            _tryOpenSSL(_OpenSSLExecutableFullPath, _InvocationParameters);
                        }

                    }
                }

            }
        }

        private Boolean ValidateSignCSR()
        {

            Hashtable _FieldList = new Hashtable();
            _FieldList.Add("CSR ", _CSRLocationTF);
            _FieldList.Add("CA Key location ", _SelectCAKeyLocationTF);
            _FieldList.Add("Signed CSR Name ", _SignedCSRCertNameTF);
            _FieldList.Add("CA Certificate Location ", _SignCSRCertificateLocationTF);
            _FieldList.Add("Signed CSR Location ", _SignedCertLocationTF);
            //_FieldList.Add("CA Cert Password ", _SignCSRCACertPasswordTF);
            
            String _Password = PasswordTF.Password;
            String _RetypePassword = PasswordRetypeTF.Password;
            if (!String.IsNullOrEmpty(_Password) || !String.IsNullOrEmpty(_RetypePassword))
            {
                _FieldList.Add("PEM Password ", PasswordTF);
                _FieldList.Add("Retype PEM Password ", PasswordRetypeTF);
            }

            bool _Status = OpenSSLFieldValidator.ValidateTextFields(_FieldList);
            if (!_Status)
            {
                ArrayList _ErrorList = OpenSSLFieldValidator.GetErrorList();
                IEnumerator _IEnumerator = _ErrorList.GetEnumerator();
                while (_IEnumerator.MoveNext())
                {
                    String _ErrorX = (String)_IEnumerator.Current;
                    if (_ErrorX != null && !_ErrorX.Equals(string.Empty))
                    {
                        System.Windows.MessageBox.Show(_ErrorX, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

            }
            OpenSSLFieldValidator.ClearErrorList();
            return _Status;
        }

        private void _CreateCACertGenerateCertBtn_Click(object sender, RoutedEventArgs e)
        {

            // Validate if all required infomation is keyed in in the parent window
            bool validInfoProvided = ValidateCreateCACertificate();
            if (validInfoProvided)
            {

                //openssl req -config openssl.conf -new -x509 -days 1001 -key keys/ca.key -out certs/ca.cer
                CreateCACertPopupWindow _CreateCACertPopuWindow = new CreateCACertPopupWindow();
                _CreateCACertPopuWindow.ShowDialog();

                String _OpenSSLUIPATHEnvVar = OpenSSLENVVarProvider.GetOPENSSLUIPATHEnvVar();
                if (String.IsNullOrEmpty(_OpenSSLUIPATHEnvVar))
                {
                    System.Windows.MessageBox.Show("OPENSSL_UI_PATH is not set , Please set the path before continue!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    String _ConfigFolderName = "configFolder";
                    String _ConfigFileName = "OpenSSLCreateCACertConfig.txt";
                    bool _ConfigFileExist = File.Exists(_OpenSSLUIPATHEnvVar + "\\" + _ConfigFolderName + "\\" + _ConfigFileName);
                    if (_ConfigFileExist)
                    {
                        // Process command to creat a certificate
                        /*req -passin pass:welcome -config 
                         *C:\work\office\temp\20090623\openssl\configFolder\OpenSSLCreateCACertConfig.txt 
                         *-new -x509 -days 1001 -key keys/OpenSSLUITest.key -out certs/OpenSSLUICert.crt
                         */
                        String _OpenSSLExecutableFileName = "openssl.exe";

                        String COMMAND_REQ = "req";
                        String _CAKeyPass = _CreateCACertCAKeyPasswordTF.Password;
                        String _CreateCAConfigPath = _OpenSSLUIPATHEnvVar + "\\" + _ConfigFolderName + "\\" + _ConfigFileName;
                        String _ValidityPeriod = _CreateCACertDaysCmb.Text;
                        if (String.IsNullOrEmpty(_ValidityPeriod))
                        {
                            _ValidityPeriod = _DEFAULT_CA_VALIDITY_PERIOD;
                        }

                        // Self signed certificate is created in this case,
                        // no Algorithm is specified. Infact there is only one option, -x509
                        //String _Algorithm = _CreateCACertAlgorithmCmb.Text;
                        //if (String.IsNullOrEmpty(_Algorithm))
                        //{
                        String _Algorithm = _DEFAULT_CA_ALOGORITHM;
                        //}

                        String _CreateCACertKeyLocation = _CreateCAKeyKeyLocationTB.Text;
                        String _CreateCACertLocation = _CreateCACertLocationTF.Text+"\\"+_CreateCACertNameTF.Text;
                        String _InvocationParameters = string.Empty;
                        if (String.IsNullOrEmpty(_CreateCACertCAKeyPasswordTF.Password))
                        {
                            // No RSA key password is provided
                            _InvocationParameters = COMMAND_REQ + "  -config " + _CreateCAConfigPath
                                + " -new  -" + _DEFAULT_CA_ALOGORITHM + " -days " + _DEFAULT_CA_VALIDITY_PERIOD + " -key "
                                + _CreateCACertKeyLocation + " -out " + _CreateCACertLocation;
                        }
                        else 
                        {
                            // RSA key password is provided
                            _InvocationParameters = COMMAND_REQ + " -passin pass:" + _CAKeyPass + " -config " + _CreateCAConfigPath
                            + " -new  -" + _DEFAULT_CA_ALOGORITHM + " -days " + _DEFAULT_CA_VALIDITY_PERIOD + " -key "
                            + _CreateCACertKeyLocation + " -out " + _CreateCACertLocation;
                        }

                        String _OpenSSLExecutableFullPath = _OpenSSLUIPATHEnvVar + "\\" + _OpenSSLExecutableFileName;

                        // Start an OpenSSL process with descriptive error if not found
                        _tryOpenSSL(_OpenSSLExecutableFullPath, _InvocationParameters);
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("OpenSSLCreateCACertConfig.txt file not found in \"" + _OpenSSLUIPATHEnvVar + "\\" + _ConfigFolderName + "\"", "Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }

        }

        private void _CreateCACertKeyLocationTriggerBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog _CAFileDialogue = new OpenFileDialog();
            _CAFileDialogue.InitialDirectory = OpenSSLENVVarProvider.GetCurrentDirPath();
            DialogResult _DialogueResult = _CAFileDialogue.ShowDialog();
            String _CAKeyFileName = _CAFileDialogue.FileName;
            if (!String.IsNullOrEmpty(_CAKeyFileName))
            {
                _CreateCAKeyKeyLocationTB.Text = _CAKeyFileName;
            }
            _CAFileDialogue.Dispose();
        }

        private void _CreateCACertLocationTF_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void _CreateCACertLocationTriggerBtn_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog _FodlerBrowserDialogue = new FolderBrowserDialog();
            _FodlerBrowserDialogue.SelectedPath = OpenSSLENVVarProvider.GetCurrentDirPath();
            DialogResult _DialogueResult = _FodlerBrowserDialogue.ShowDialog();
            _CreateCACertLocationTF.Text = _FodlerBrowserDialogue.SelectedPath;
            _FodlerBrowserDialogue.Dispose();

        }

        private bool ValidateCreateCACertificate() 
        {
            Hashtable _FieldList = new Hashtable();
            _FieldList.Add("CA Key File ", _CreateCAKeyKeyLocationTB);
            _FieldList.Add("Save Location ", _CreateCACertLocationTF);
            _FieldList.Add("Certificate Name ", _CreateCACertNameTF);
            if (!String.IsNullOrEmpty(_CreateCACertCAKeyPasswordTF.Password))
            {
                _FieldList.Add("RSA Key Password ", _CreateCACertCAKeyPasswordTF);
            }

            bool _Status = OpenSSLFieldValidator.ValidateTextFields(_FieldList);
            if (!_Status)
            {
                ArrayList _ErrorList = OpenSSLFieldValidator.GetErrorList();
                IEnumerator _IEnumerator = _ErrorList.GetEnumerator();
                while (_IEnumerator.MoveNext())
                {
                    String _ErrorX = (String)_IEnumerator.Current;
                    if (_ErrorX != null && !_ErrorX.Equals(string.Empty))
                    {
                        System.Windows.MessageBox.Show(_ErrorX, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

            }
            OpenSSLFieldValidator.ClearErrorList();
            return _Status;
        }

        private void _CreateCACertExitBtn_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void _SignCSRCACertLocationTriggerBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog _CAFileDialogue = new OpenFileDialog();
            _CAFileDialogue.InitialDirectory = OpenSSLENVVarProvider.GetCurrentDirPath();
            DialogResult _DialogueResult = _CAFileDialogue.ShowDialog();
            String _CACertLocation = _CAFileDialogue.FileName;
            if (!String.IsNullOrEmpty(_CACertLocation))
            {
                _SignCSRCertificateLocationTF.Text = _CACertLocation;
            }
            _CAFileDialogue.Dispose();
        }

        private void _ResetCSRBtn_Click(object sender, RoutedEventArgs e)
        {
            _CSRLocationTF.Text = string.Empty;
            _SelectCAKeyLocationTF.Text = string.Empty;
            _SignedCSRCertNameTF.Text = string.Empty;
            _SignCSRCertificateLocationTF.Text = string.Empty;
            _SignedCertLocationTF.Text = string.Empty;
            _SignCSRCACertPasswordTF.Password = string.Empty;
            
        }

        private void PasswordTF_PasswordChanged(object sender, RoutedEventArgs e)
        {

        }

        private void PasswordRetypeTF_PasswordChanged(object sender, RoutedEventArgs e)
        {

        }

        private void _CreateCACertResetBtn_Click(object sender, RoutedEventArgs e)
        {
            _CreateCAKeyKeyLocationTB.Text = string.Empty;
            _CreateCACertLocationTF.Text = string.Empty;
            _CreateCACertNameTF.Text = string.Empty;
            _CreateCACertCAKeyPasswordTF.Password = string.Empty;
        }

        private void _CreateCSRCSRLocationTriggerBtn_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog _FodlerBrowserDialogue = new FolderBrowserDialog();
            _FodlerBrowserDialogue.SelectedPath = OpenSSLENVVarProvider.GetCurrentDirPath();
            DialogResult _DialogueResult = _FodlerBrowserDialogue.ShowDialog();
            _CreateCSRCSRLocationTF.Text = _FodlerBrowserDialogue.SelectedPath;
            _FodlerBrowserDialogue.Dispose();
        }

        private void _CreateCSRCSRLocationTF_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void _CreateCSRPrivateKeyLocationTriggerBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog _CreateCSRFileDialogue = new OpenFileDialog();
            _CreateCSRFileDialogue.InitialDirectory = OpenSSLENVVarProvider.GetCurrentDirPath();
            DialogResult _DialogueResult = _CreateCSRFileDialogue.ShowDialog();
            String _CSRPrivateKeyLocation = _CreateCSRFileDialogue.FileName;
            if (!String.IsNullOrEmpty(_CSRPrivateKeyLocation))
            {
                _CreateCSRPrivateKeyLocationTF.Text = _CSRPrivateKeyLocation;
            }
            _CreateCSRFileDialogue.Dispose();
        }

        private void _CreateCSRPrivateKeyLocationTF_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void _CreateCSRExitBtn_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void _CreateCSRGenerateBtn_Click(object sender, RoutedEventArgs e)
        {

            bool validInfoProvided = ValidateCreateCSR();
            if (validInfoProvided)
            {

                CreateCSRPopupWindow _CreateCSRPopuWindow = new CreateCSRPopupWindow();
                _CreateCSRPopuWindow.ShowDialog();

                String _OpenSSLUIPATHEnvVar = OpenSSLENVVarProvider.GetOPENSSLUIPATHEnvVar();
                if (String.IsNullOrEmpty(_OpenSSLUIPATHEnvVar))
                {
                    System.Windows.MessageBox.Show("OPENSSL_UI_PATH is not set , Please set the path before continue!", "Error",
                      MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    String _ConfigFolderName = "csrFolder";
                    String _ConfigFileName = "OpenSSLCreateCSRConfig.txt";
                    bool _ConfigFileExist = File.Exists(_OpenSSLUIPATHEnvVar + "\\" + _ConfigFolderName + "\\" + _ConfigFileName);
                    if (_ConfigFileExist)
                    {
                        // Process command to creat a certificate
                        /*
                         * OpenSSL> req -passin pass:welcome -new -key C:\work\personal\temp\key\PrivateKey.key -out client.csr 
                         * -config C:\work\office\temp\20090623\openssl\csrFolder\OpenSSLCreateCSRConfig.txt
                         */
                        String _OpenSSLExecutableFileName = "openssl.exe";

                        String COMMAND_REQ = "req";
                        String _CreateCSRName = _CreateCSRCSRNameTF.Text;
                        String _CreateCSRLocation = _CreateCSRCSRLocationTF.Text;
                        String _CreateCSRAbsoluteName = _CreateCSRLocation + "\\" +_CreateCSRName;
                        String _CSRPrivateKeyLocation = _CreateCSRPrivateKeyLocationTF.Text;
                        String _CSRPrivateKeyPass = _CreateCSRCSRPasswordTF.Password;
                        String _CreateCSRConfigPath = _OpenSSLUIPATHEnvVar + "\\" + _ConfigFolderName + "\\" + _ConfigFileName;
                        
                        String _InvocationParameters = string.Empty;
                        if (!String.IsNullOrEmpty(_CSRPrivateKeyPass))
                        {
                            // RSA key has a password 
                            _InvocationParameters = COMMAND_REQ + " -passin pass:" + _CSRPrivateKeyPass + " -new -key " +
                                _CSRPrivateKeyLocation + " -out " + _CreateCSRAbsoluteName + " -config " + _CreateCSRConfigPath;
                        }
                        else 
                        {
                            // RSA key does not have a password
                            _InvocationParameters = COMMAND_REQ + " -new -key " +
                                   _CSRPrivateKeyLocation + " -out " + _CreateCSRAbsoluteName + " -config " + _CreateCSRConfigPath;
                        }

                        String _OpenSSLExecutableFullPath = _OpenSSLUIPATHEnvVar + "\\" + _OpenSSLExecutableFileName;

                        // Start an OpenSSL process with descriptive error if not found
                        _tryOpenSSL(_OpenSSLExecutableFullPath, _InvocationParameters);
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("OpenSSLCreateCACertConfig.txt file not found in \"" + _OpenSSLUIPATHEnvVar + "\\" + _ConfigFolderName + "\"", "Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

            }

        }

        private bool ValidateCreateCSR()
        {
            Hashtable _FieldList = new Hashtable();
            _FieldList.Add("CSR Name ", _CreateCSRCSRNameTF);
            _FieldList.Add("CSR Location ", _CreateCSRCSRLocationTF);
            _FieldList.Add("CSR Private Key ", _CreateCSRPrivateKeyLocationTF);
            //_FieldList.Add("Private Key Password ", _CreateCSRCSRPasswordTF);

            bool _Status = OpenSSLFieldValidator.ValidateTextFields(_FieldList);
            if (!_Status)
            {
                ArrayList _ErrorList = OpenSSLFieldValidator.GetErrorList();
                IEnumerator _IEnumerator = _ErrorList.GetEnumerator();
                while (_IEnumerator.MoveNext())
                {
                    String _ErrorX = (String)_IEnumerator.Current;
                    if (_ErrorX != null && !_ErrorX.Equals(string.Empty))
                    {
                        System.Windows.MessageBox.Show(_ErrorX, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            OpenSSLFieldValidator.ClearErrorList();
            return _Status;
        }

        private void _CreateCSRResetBtn_Click(object sender, RoutedEventArgs e)
        {
            _CreateCSRCSRNameTF.Text = string.Empty;
            _CreateCSRCSRLocationTF.Text = string.Empty;
            _CreateCSRPrivateKeyLocationTF.Text = string.Empty;
            _CreateCSRCSRPasswordTF.Password = string.Empty;
        }

        private void _UtilCreatePKCS12ClientCertificateLocationTriggerBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog _UtilCreatePKCS12FileDialogue = new OpenFileDialog();
            _UtilCreatePKCS12FileDialogue.InitialDirectory = OpenSSLENVVarProvider.GetCurrentDirPath();
            DialogResult _DialogueResult = _UtilCreatePKCS12FileDialogue.ShowDialog();
            String _UtilCreatePKCS12ClientCertLocation = _UtilCreatePKCS12FileDialogue.FileName;
            if (!String.IsNullOrEmpty(_UtilCreatePKCS12ClientCertLocation))
            {
                _UtilCreatePKCS12ClientCertificateLocationTb.Text = _UtilCreatePKCS12ClientCertLocation;
            }
            _UtilCreatePKCS12FileDialogue.Dispose();
        }

        private void _UtilCreatePKCS12ClientPrivateKeyLocationTriggerBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog _UtilCreatePKCS12FileDialogue = new OpenFileDialog();
            _UtilCreatePKCS12FileDialogue.InitialDirectory = OpenSSLENVVarProvider.GetCurrentDirPath();
            DialogResult _DialogueResult = _UtilCreatePKCS12FileDialogue.ShowDialog();
            String _UtilCreatePKCS12ClientPrivateKeyLocation = _UtilCreatePKCS12FileDialogue.FileName;
            if (!String.IsNullOrEmpty(_UtilCreatePKCS12ClientPrivateKeyLocation))
            {
                _UtilCreatePKCS12ClientPrivateKeyLocationTb.Text = _UtilCreatePKCS12ClientPrivateKeyLocation;
            }
            _UtilCreatePKCS12FileDialogue.Dispose();
        }

        private void _UtilCreatePKCS12SaveInTriggerBtn_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog _FodlerBrowserDialogue = new FolderBrowserDialog();
            _FodlerBrowserDialogue.SelectedPath = OpenSSLENVVarProvider.GetCurrentDirPath();
            DialogResult _DialogueResult = _FodlerBrowserDialogue.ShowDialog();
            _UtilCreatePKCS12SaveInTb.Text = _FodlerBrowserDialogue.SelectedPath;
            _FodlerBrowserDialogue.Dispose();
        }

        private void _UtilCreatePKCS12ExportBtn_Click(object sender, RoutedEventArgs e)
        {
            bool validInfoProvided = ValidateExportToPKCS12();
            if (validInfoProvided) 
            {
                String _PKCS_EXTENSION = ".p12";
                String _COM_PKCS12 = "pkcs12";
                String _COM_EXPORT = "export";

                /*
                 * pkcs12 -passin pass:welcome -export -in C:\\work\\personal\\temp\\certs
                 * \\ranil-lptp-cert.crt -inkey C:\\work\\personal\\temp\\key\\ranil-lptp -out C:\\
                 * work\\personal\\temp\\certs\\ranil-lptp-234.p12 -password pass:welcome
                 */

                String _ClientCert = _UtilCreatePKCS12ClientCertificateLocationTb.Text;
                String _ClientPrivateKey = _UtilCreatePKCS12ClientPrivateKeyLocationTb.Text;
                String _PKCS12SaveLocation = _UtilCreatePKCS12SaveInTb.Text;
                String _PKCS12FileName = _UtilCreatePKCS12FileNameTb.Text;
                String _PKCS12SaveFullPath = _PKCS12SaveLocation + "\\" + _PKCS12FileName + _PKCS_EXTENSION;
                String _ClientKeyPass = _UtilCreatePKCS12ClientKeyPasswordTb.Password;
                String _PKCS12Password = _UtilCreatePKCS12PasswordTb.Password;
                
                String _OpenSSLUIPATHEnvVar = OpenSSLENVVarProvider.GetOPENSSLUIPATHEnvVar();
                if (String.IsNullOrEmpty(_OpenSSLUIPATHEnvVar))
                {
                    System.Windows.MessageBox.Show("OPENSSL_UI_PATH is not set , Please set the path before continue!", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else 
                {
                    String _OpenSSLExecutableFileName = "openssl.exe";
                    String _InvocationParameters = string.Empty;
                    if (!String.IsNullOrEmpty(_ClientKeyPass) && !String.IsNullOrEmpty(_PKCS12Password)) 
                    {
                        // client RSA key has a password and PKCS12 certificate chain also has a password
                         _InvocationParameters = _COM_PKCS12 + " -passin pass:" + _ClientKeyPass + " -" + _COM_EXPORT + " -in " + _ClientCert + " -inkey " + _ClientPrivateKey
                            + " -out " + _PKCS12SaveFullPath + " -passout pass:" + _PKCS12Password;
                    }
                    else if (String.IsNullOrEmpty(_ClientKeyPass) && !String.IsNullOrEmpty(_PKCS12Password)) 
                    {
                        // client RSA key does not have a password and PKCS12 certificate chain also has a password
                        _InvocationParameters = _COM_PKCS12 + " -" + _COM_EXPORT + " -in " + _ClientCert + " -inkey " + _ClientPrivateKey
                            + " -out " + _PKCS12SaveFullPath + " -passout pass:" + _PKCS12Password;
                    }
                    else if (!String.IsNullOrEmpty(_ClientKeyPass) && String.IsNullOrEmpty(_PKCS12Password))
                    {
                        // client RSA key has a password and PKCS12 certificate chain does not have a password
                        _InvocationParameters = _COM_PKCS12 + " -passin pass:" + _ClientKeyPass + " -" + _COM_EXPORT + " -in " + _ClientCert + " -inkey " + _ClientPrivateKey
                            + " -out " + _PKCS12SaveFullPath +" -passout pass:" + _PKCS12Password;
                    }
                    else 
                    {
                        // client RSA key does not have a password and PKCS12 certificate chain does not have a password
                        _InvocationParameters = _COM_PKCS12 + " -" + _COM_EXPORT + " -in " + _ClientCert + " -inkey " + _ClientPrivateKey
                            + " -out " + _PKCS12SaveFullPath + " -passout pass:" + _PKCS12Password;
                    
                    }

                    String _OpenSSLExecutableFullPath = _OpenSSLUIPATHEnvVar + "\\" + _OpenSSLExecutableFileName;

                    // Start an OpenSSL process with descriptive error if not found
                    _tryOpenSSL(_OpenSSLExecutableFullPath, _InvocationParameters);                    
                }

            }
        }

        private bool ValidateExportToPKCS12()
        {
            OpenSSLFieldValidator.ClearErrorList();
            Hashtable _FieldList = new Hashtable();
            _FieldList.Add("Client Certificate Location ", _UtilCreatePKCS12ClientCertificateLocationTb);
            _FieldList.Add("Client Private Key Location ", _UtilCreatePKCS12ClientPrivateKeyLocationTb);
            _FieldList.Add("Save PKCS12 In Folder ", _UtilCreatePKCS12SaveInTb);
            _FieldList.Add("PKCS12 File Name ", _UtilCreatePKCS12FileNameTb);
            //_FieldList.Add("Client Key Password  ", _UtilCreatePKCS12ClientKeyPasswordTb);
            //_FieldList.Add("PKCS12 Password  ", _UtilCreatePKCS12PasswordTb);

            bool _Status = OpenSSLFieldValidator.ValidateTextFields(_FieldList);
            if (!_Status)
            {
                ArrayList _ErrorList = OpenSSLFieldValidator.GetErrorList();
                IEnumerator _IEnumerator = _ErrorList.GetEnumerator();
                while (_IEnumerator.MoveNext())
                {
                    String _ErrorX = (String)_IEnumerator.Current;
                    if (_ErrorX != null && !_ErrorX.Equals(string.Empty))
                    {
                        System.Windows.MessageBox.Show(_ErrorX, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

            }
            OpenSSLFieldValidator.ClearErrorList();
            return _Status;
        }

        private void _UtilCreatePKCS12ResetBtn_Click(object sender, RoutedEventArgs e)
        {
            _UtilCreatePKCS12ClientCertificateLocationTb.Text = string.Empty;
            _UtilCreatePKCS12ClientPrivateKeyLocationTb.Text = string.Empty;
            _UtilCreatePKCS12SaveInTb.Text = string.Empty;
            _UtilCreatePKCS12FileNameTb.Text = string.Empty;
            _UtilCreatePKCS12ClientKeyPasswordTb.Password = string.Empty;
            _UtilCreatePKCS12PasswordTb.Password = string.Empty;

        }

        private void _UtilCreatePKCS12ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void _SignCSRCertTypeCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
