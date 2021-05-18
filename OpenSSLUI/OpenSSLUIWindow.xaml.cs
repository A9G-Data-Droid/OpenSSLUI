using OpenSSLUI.codebase;
using System;
using System.Collections;
using System.IO;
using System.Windows;
using System.Windows.Forms;

namespace OpenSSLUI
{
    /// <summary>
    /// Interaction logic for OpenSSLUIWindow.xaml
    /// </summary>
    public partial class OpenSSLUIWindow : Window
    {
        private string _SELECTED_FOLDER_LOCATION = string.Empty;
        private string _SELECTED_CSR_FOLDER_LOCATION = string.Empty;
        private string _SELECTED_CA_KEY_FOLDER_LOCATION = string.Empty;
        private string _SELECTED_SIGNED_CSR_FOLDER_LOCATION = string.Empty;

        private const string COMMAND_GENRSA = "genrsa";

        public OpenSSLUIWindow()
        {
            InitializeComponent();
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            DoCleanExit();
        }

        private void CreateCAKeyResetBtn_Click(object sender, RoutedEventArgs e)
        {
            _CaKeyNameTF.Clear();
            _KeyLocationTF.Clear();
            PasswordTF.Password = string.Empty;
            PasswordRetypeTF.Password = string.Empty;
        }

        private void KeyLocationTriggerBtn_Click(object sender, RoutedEventArgs e)
        {
            _SELECTED_FOLDER_LOCATION = string.Empty;
            using FolderBrowserDialog _FodlerBrowserDialogue = new();
            _FodlerBrowserDialogue.SelectedPath = OpenSSLEnvVarProvider.GetCurrentDirPath();
            _ = _FodlerBrowserDialogue.ShowDialog();
            _SELECTED_FOLDER_LOCATION = _FodlerBrowserDialogue.SelectedPath;
            _KeyLocationTF.Text = _SELECTED_FOLDER_LOCATION;
        }

        private void GenerateKeyBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateCreateCAKey())
            {
                return;
            }

            if (!MatchRetypePassword())
            {
                return;
            }

            // genrsa -des3 -passout pass:yourpassword -out /path/to/your/key_file 1024
            // Process user inputs to create CA key
            string _OpenSSLUIPATHEnvVar = OpenSSLEnvVarProvider.GetOPENSSLUIPATHEnvVar();
            if (string.IsNullOrEmpty(_OpenSSLUIPATHEnvVar))
            {
                System.Windows.MessageBox.Show("OPENSSL_UI_PATH is not set, Please set the path before continue!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                // Variable is available, Run OpenSSL
                string _KeyName = _CaKeyNameTF.Text;
                string _KeyLocation = _KeyLocationTF.Text;
                string _BitLength = _BitLengthCmb.Text;
                string _Password = PasswordRetypeTF.Password;
                if (string.IsNullOrEmpty(_BitLength))
                {
                    _BitLength = OpenSSLConfig.DEFAULT_BIT_LENGTH;
                }

                string _InvocationParameters;

                // Flag to distinguise whether user has keyed in password fields
                if (PassPhraseProvided())
                {
                    // Execute openssl command to create a key with passphrase
                    _InvocationParameters = COMMAND_GENRSA + " -aes128 -passout pass:" + _Password + " -out \"" + Path.Combine(_KeyLocation, _KeyName + " " + _BitLength) + "\"";
                }
                else
                {
                    // Create a key without a passphase
                    _InvocationParameters = COMMAND_GENRSA + " -out \"" + Path.Combine(_KeyLocation, _KeyName + " " + _BitLength) + "\"";
                }

                // Start an OpenSSL process with descriptive error if not found
                OpenSSL.TryOpenSSL(_InvocationParameters);
            }
        }

        private void CSRLocationTriggerBtn_Click(object sender, RoutedEventArgs e)
        {
            _SELECTED_CSR_FOLDER_LOCATION = string.Empty;
            using OpenFileDialog _FileDialogue = new();
            _FileDialogue.InitialDirectory = OpenSSLEnvVarProvider.GetCurrentDirPath();
            _ = _FileDialogue.ShowDialog();
            string _FileName = _FileDialogue.FileName;
            if (!string.IsNullOrEmpty(_FileName))
            {
                _SELECTED_CSR_FOLDER_LOCATION = _FileName;
                _CSRLocationTF.Text = _SELECTED_CSR_FOLDER_LOCATION;
            }
        }

        private void SelectCAKeyTriggerBtn_Click(object sender, RoutedEventArgs e)
        {
            _SELECTED_CA_KEY_FOLDER_LOCATION = string.Empty;
            using OpenFileDialog _CAFileDialogue = new();
            _CAFileDialogue.InitialDirectory = OpenSSLEnvVarProvider.GetCurrentDirPath();
            _ = _CAFileDialogue.ShowDialog();
            string _CAKeyFileName = _CAFileDialogue.FileName;
            if (!string.IsNullOrEmpty(_CAKeyFileName))
            {
                _SELECTED_CA_KEY_FOLDER_LOCATION = _CAKeyFileName;
                _SelectCAKeyLocationTF.Text = _SELECTED_CA_KEY_FOLDER_LOCATION;
            }
        }

        private bool ValidateCreateCAKey()
        {

            Hashtable _FieldList = new()
            {
                { "Key Name", _CaKeyNameTF },
                { "Save Location ", _KeyLocationTF }
            };

            string _Password = PasswordTF.Password;
            string _RetypePassword = PasswordRetypeTF.Password;
            if (!string.IsNullOrEmpty(_Password) || !string.IsNullOrEmpty(_RetypePassword))
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
                    string _ErrorX = (string)_IEnumerator.Current;
                    if (!string.IsNullOrEmpty(_ErrorX))
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
            string _Password = PasswordTF.Password;
            string _RetypePassword = PasswordRetypeTF.Password;
            if (!string.IsNullOrEmpty(_Password) && !string.IsNullOrEmpty(_RetypePassword) && !_RetypePassword.Equals(_Password))
            {
                System.Windows.MessageBox.Show("Password fields do not match!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private bool PassPhraseProvided()
        {
            bool passphraseProvided = true;
            string _Password = PasswordTF.Password;
            string _RetypePassword = PasswordRetypeTF.Password;
            if (string.IsNullOrEmpty(_Password) && string.IsNullOrEmpty(_RetypePassword))
            {
                passphraseProvided = false;
            }

            return passphraseProvided;
        }

        private void SignedCertLocationTriggerBtn_Click(object sender, RoutedEventArgs e)
        {
            _SELECTED_SIGNED_CSR_FOLDER_LOCATION = string.Empty;
            using FolderBrowserDialog _FodlerBrowserDialogue = new();
            _FodlerBrowserDialogue.SelectedPath = OpenSSLEnvVarProvider.GetCurrentDirPath();
            _ = _FodlerBrowserDialogue.ShowDialog();
            _SELECTED_SIGNED_CSR_FOLDER_LOCATION = _FodlerBrowserDialogue.SelectedPath;
            _SignedCertLocationTF.Text = _SELECTED_SIGNED_CSR_FOLDER_LOCATION;
        }

        private void ExitCSRBtn_Click(object sender, RoutedEventArgs e)
        {
            DoCleanExit();
        }

        private static void DoCleanExit()
        {
            if (System.Windows.Forms.Application.MessageLoop)
            {
                System.Windows.Forms.Application.Exit();
            }
            else
            {
                Environment.Exit(1);
            }
        }

        private void SignBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateSignCSR())
            {
                return;
            }

            string _CsrFile = _CSRLocationTF.Text;
            string _KeyFile = _SelectCAKeyLocationTF.Text;
            string _CACertificate = _SignCSRCertificateLocationTF.Text;
            string _SignedCSRCertName = _SignedCSRCertNameTF.Text;
            string _SignedCertiLocation = _SignedCertLocationTF.Text;
            string _ValidityPeriod = _ValidityPeriodCmb.Text;
            string _CACertPassword = _SignCSRCACertPasswordTF.Password;
            if (string.IsNullOrEmpty(_ValidityPeriod))
            {
                _ValidityPeriod = OpenSSLConfig.DEFAULT_VALIDITY_PERIOD;
            }

            if (!File.Exists(_CsrFile))
            {
                System.Windows.MessageBox.Show("CSR File selected does not exists!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (!File.Exists(_KeyFile))
            {
                System.Windows.MessageBox.Show("Key File selected does not exists!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string _OpenSSLUIPATHEnvVar = OpenSSLEnvVarProvider.GetOPENSSLUIPATHEnvVar();
            if (string.IsNullOrEmpty(_OpenSSLUIPATHEnvVar))
            {
                System.Windows.MessageBox.Show("OPENSSL_UI_PATH is not set , Please set the path before continue!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Sign the CSR
            // openssl ca -config server.cfg -in client.csr -cert server.crt -keyfile server.key -out client.crt -days 1825
            string _OpenSSLCertType = _SignCSRCertTypeCmb.Text;
            string _OpenSSLConfigFileName;
            if (!string.IsNullOrEmpty(_OpenSSLCertType) && _OpenSSLCertType.Equals("Client", StringComparison.CurrentCultureIgnoreCase))
            {
                _OpenSSLConfigFileName = OpenSSLEnvVarProvider.GetOpenSSLConfEnvVar();
            }
            else
            {
                _OpenSSLConfigFileName = OpenSSLConfig.CAConfigFileFullPath;
            }

            string _InvocationParameters;
            if (!string.IsNullOrEmpty(_CACertPassword))
            {
                //RSA key has a password
                _InvocationParameters = " ca " + " -passin pass:" + _CACertPassword + " -config \"" + _OpenSSLConfigFileName + "\" -in \"" + _CsrFile + "\" -cert \"" + _CACertificate + "\"";
            }
            else
            {
                //RSA key does not have a password
                _InvocationParameters = " ca  -config \"" + _OpenSSLConfigFileName + "\" -in \"" + _CsrFile + "\" -cert \"" + _CACertificate + "\"";
            }

            // Add Common Parameters
            _InvocationParameters = _InvocationParameters + " -keyfile \"" + _KeyFile + "\" -out \"" + Path.Combine(_SignedCertiLocation, _SignedCSRCertName) + "\"" +
                            " -days " + _ValidityPeriod + " -batch";

            //Before run this command, clear the openssl database file
            string caDBfullPath = Path.Combine(_OpenSSLUIPATHEnvVar, "ca.db.index");
            File.Delete(caDBfullPath);
            using FileStream _FS = File.Create(caDBfullPath);
            _FS.Close();

            // Start an OpenSSL process with descriptive error if not found
            OpenSSL.TryOpenSSL(_InvocationParameters);
        }

        private bool ValidateSignCSR()
        {

            Hashtable _FieldList = new()
            {
                {
                    "CSR ",
                    _CSRLocationTF
                },
                {
                    "CA Key location ",
                    _SelectCAKeyLocationTF
                },
                {
                    "Signed CSR Name ",
                    _SignedCSRCertNameTF
                },
                {
                    "CA Certificate Location ",
                    _SignCSRCertificateLocationTF
                },
                {
                    "Signed CSR Location ",
                    _SignedCertLocationTF
                }
            };

            //_FieldList.Add("CA Cert Password ", _SignCSRCACertPasswordTF);

            string _Password = PasswordTF.Password;
            string _RetypePassword = PasswordRetypeTF.Password;
            if (!string.IsNullOrEmpty(_Password) || !string.IsNullOrEmpty(_RetypePassword))
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
                    string _ErrorX = (string)_IEnumerator.Current;
                    if (!string.IsNullOrEmpty(_ErrorX))
                    {
                        System.Windows.MessageBox.Show(_ErrorX, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }

            OpenSSLFieldValidator.ClearErrorList();
            return _Status;
        }

        private void CreateCACertGenerateCertBtn_Click(object sender, RoutedEventArgs e)
        {

            // Validate if all required infomation is keyed in in the parent window
            if (!ValidateCreateCACertificate())
            {
                return;
            }

            //openssl req -config openssl.conf -new -x509 -days 1001 -key keys/ca.key -out certs/ca.cer
            CreateCACertPopupWindow _CreateCACertPopuWindow = new();
            _CreateCACertPopuWindow.ShowDialog();

            string _OpenSSLUIPATHEnvVar = OpenSSLEnvVarProvider.GetOPENSSLUIPATHEnvVar();
            if (string.IsNullOrEmpty(_OpenSSLUIPATHEnvVar))
            {
                System.Windows.MessageBox.Show("OPENSSL_UI_PATH is not set , Please set the path before continue!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (File.Exists(OpenSSLConfig.CAConfigFileFullPath))
            {
                // Process command to creat a certificate
                /*req -passin pass:welcome -config 
                    *C:\work\office\temp\20090623\openssl\configFolder\OpenSSLCreateCACertConfig.txt 
                    *-new -x509 -days 1001 -key keys/OpenSSLUITest.key -out certs/OpenSSLUICert.crt
                    */
                const string COMMAND_REQ = "req";
                string _CAKeyPass = _CreateCACertCAKeyPasswordTF.Password;
                string _ValidityPeriod;
                if (string.IsNullOrEmpty(_CreateCACertDaysCmb.Text))
                {
                    _ValidityPeriod = OpenSSLConfig.DEFAULT_CA_VALIDITY_PERIOD;
                }
                else
                {
                    _ValidityPeriod = _CreateCACertDaysCmb.Text;
                }

                // Self signed certificate is created in this case,
                // no Algorithm is specified. Infact there is only one option, -x509
                //String _Algorithm = _CreateCACertAlgorithmCmb.Text;
                //if (String.IsNullOrEmpty(_Algorithm))
                //{
                //    string _Algorithm = _DEFAULT_CA_ALOGORITHM;
                //}

                string _CreateCACertKeyLocation = _CreateCAKeyKeyLocationTB.Text;
                string _CreateCACertLocation = Path.Combine(_CreateCACertLocationTF.Text, _CreateCACertNameTF.Text);
                string _InvocationParameters;
                if (string.IsNullOrEmpty(_CreateCACertCAKeyPasswordTF.Password))
                {
                    // No RSA key password is provided
                    _InvocationParameters = COMMAND_REQ + "  -config \"" + OpenSSLConfig.CAConfigFileFullPath + "\""
                        + " -new  -" + OpenSSLConfig.DEFAULT_CA_ALOGORITHM + " -days " + _ValidityPeriod + " -key \""
                        + _CreateCACertKeyLocation + "\" -out \"" + _CreateCACertLocation + "\"";
                }
                else
                {
                    // RSA key password is provided
                    _InvocationParameters = COMMAND_REQ + " -passin pass:" + _CAKeyPass + " -config \"" + OpenSSLConfig.CAConfigFileFullPath + "\""
                    + " -new  -" + OpenSSLConfig.DEFAULT_CA_ALOGORITHM + " -days " + _ValidityPeriod + " -key \""
                    + _CreateCACertKeyLocation + "\" -out \"" + _CreateCACertLocation + "\"";
                }

                // Start an OpenSSL process with descriptive error if not found
                OpenSSL.TryOpenSSL(_InvocationParameters);
            }
            else
            {
                System.Windows.MessageBox.Show("OpenSSLCreateCACertConfig.txt file not found in " + OpenSSLConfig.CAConfigFileFullPath, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateCACertKeyLocationTriggerBtn_Click(object sender, RoutedEventArgs e)
        {
            using OpenFileDialog _CAFileDialogue = new();
            _CAFileDialogue.InitialDirectory = OpenSSLEnvVarProvider.GetCurrentDirPath();
            _ = _CAFileDialogue.ShowDialog();
            string _CAKeyFileName = _CAFileDialogue.FileName;
            if (!string.IsNullOrEmpty(_CAKeyFileName))
            {
                _CreateCAKeyKeyLocationTB.Text = _CAKeyFileName;
            }
        }

        private void CreateCACertLocationTriggerBtn_Click(object sender, RoutedEventArgs e)
        {
            using FolderBrowserDialog _FodlerBrowserDialogue = new();
            _FodlerBrowserDialogue.SelectedPath = OpenSSLEnvVarProvider.GetCurrentDirPath();
            _ = _FodlerBrowserDialogue.ShowDialog();
            _CreateCACertLocationTF.Text = _FodlerBrowserDialogue.SelectedPath;
        }

        private bool ValidateCreateCACertificate()
        {
            Hashtable _FieldList = new()
            {
                {
                    "CA Key File ",
                    _CreateCAKeyKeyLocationTB
                },
                {
                    "Save Location ",
                    _CreateCACertLocationTF
                },
                {
                    "Certificate Name ",
                    _CreateCACertNameTF
                }
            };

            if (!string.IsNullOrEmpty(_CreateCACertCAKeyPasswordTF.Password))
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
                    string _ErrorX = (string)_IEnumerator.Current;
                    if (!string.IsNullOrEmpty(_ErrorX))
                    {
                        System.Windows.MessageBox.Show(_ErrorX, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }

            OpenSSLFieldValidator.ClearErrorList();
            return _Status;
        }

        private void CreateCACertExitBtn_Click(object sender, RoutedEventArgs e)
        {
            DoCleanExit();
        }

        private void SignCSRCACertLocationTriggerBtn_Click(object sender, RoutedEventArgs e)
        {
            using OpenFileDialog _CAFileDialogue = new();
            _CAFileDialogue.InitialDirectory = OpenSSLEnvVarProvider.GetCurrentDirPath();
            _ = _CAFileDialogue.ShowDialog();
            string _CACertLocation = _CAFileDialogue.FileName;
            if (!string.IsNullOrEmpty(_CACertLocation))
            {
                _SignCSRCertificateLocationTF.Text = _CACertLocation;
            }
        }

        private void ResetCSRBtn_Click(object sender, RoutedEventArgs e)
        {
            _CSRLocationTF.Text = string.Empty;
            _SelectCAKeyLocationTF.Text = string.Empty;
            _SignedCSRCertNameTF.Text = string.Empty;
            _SignCSRCertificateLocationTF.Text = string.Empty;
            _SignedCertLocationTF.Text = string.Empty;
            _SignCSRCACertPasswordTF.Password = string.Empty;
        }

        private void CreateCACertResetBtn_Click(object sender, RoutedEventArgs e)
        {
            _CreateCAKeyKeyLocationTB.Text = string.Empty;
            _CreateCACertLocationTF.Text = string.Empty;
            _CreateCACertNameTF.Text = string.Empty;
            _CreateCACertCAKeyPasswordTF.Password = string.Empty;
        }

        private void CreateCSRCSRLocationTriggerBtn_Click(object sender, RoutedEventArgs e)
        {
            using FolderBrowserDialog _FodlerBrowserDialogue = new();
            _FodlerBrowserDialogue.SelectedPath = OpenSSLEnvVarProvider.GetCurrentDirPath();
            _ = _FodlerBrowserDialogue.ShowDialog();
            _CreateCSRCSRLocationTF.Text = _FodlerBrowserDialogue.SelectedPath;
        }

        private void CreateCSRPrivateKeyLocationTriggerBtn_Click(object sender, RoutedEventArgs e)
        {
            using OpenFileDialog _CreateCSRFileDialogue = new();
            _CreateCSRFileDialogue.InitialDirectory = OpenSSLEnvVarProvider.GetCurrentDirPath();
            _ = _CreateCSRFileDialogue.ShowDialog();
            string _CSRPrivateKeyLocation = _CreateCSRFileDialogue.FileName;
            if (!string.IsNullOrEmpty(_CSRPrivateKeyLocation))
            {
                _CreateCSRPrivateKeyLocationTF.Text = _CSRPrivateKeyLocation;
            }
        }

        private void CreateCSRExitBtn_Click(object sender, RoutedEventArgs e)
        {
            DoCleanExit();
        }

        private void CreateCSRGenerateBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateCreateCSR())
            {
                return;
            }

            CreateCSRPopupWindow _CreateCSRPopuWindow = new();
            _CreateCSRPopuWindow.ShowDialog();

            string _OpenSSLUIPATHEnvVar = OpenSSLEnvVarProvider.GetOPENSSLUIPATHEnvVar();
            if (string.IsNullOrEmpty(_OpenSSLUIPATHEnvVar))
            {
                System.Windows.MessageBox.Show("OPENSSL_UI_PATH is not set , Please set the path before continue!", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (File.Exists(OpenSSLConfig.CSRConfigFileFullPath))
            {
                // Process command to creat a certificate
                /*
                    * OpenSSL> req -passin pass:welcome -new -key C:\work\personal\temp\key\PrivateKey.key -out client.csr 
                    * -config C:\work\office\temp\20090623\openssl\csrFolder\OpenSSLCreateCSRConfig.txt
                    */

                const string COMMAND_REQ = "req";
                string _CreateCSRName = _CreateCSRCSRNameTF.Text;
                string _CreateCSRLocation = _CreateCSRCSRLocationTF.Text;
                string _CreateCSRAbsoluteName = Path.Combine(_CreateCSRLocation, _CreateCSRName);
                string _CSRPrivateKeyLocation = _CreateCSRPrivateKeyLocationTF.Text;
                string _CSRPrivateKeyPass = _CreateCSRCSRPasswordTF.Password;

                string _InvocationParameters;
                if (!string.IsNullOrEmpty(_CSRPrivateKeyPass))
                {
                    // RSA key has a password 
                    _InvocationParameters = COMMAND_REQ + " -passin pass:" + _CSRPrivateKeyPass + " -new -key \"" +
                        _CSRPrivateKeyLocation + "\" -out \"" + _CreateCSRAbsoluteName + "\" -config \"" + OpenSSLConfig.CSRConfigFileFullPath + "\"";
                }
                else
                {
                    // RSA key does not have a password
                    _InvocationParameters = COMMAND_REQ + " -new -key \"" + _CSRPrivateKeyLocation + "\" -out \"" +
                        _CreateCSRAbsoluteName + "\" -config \"" + OpenSSLConfig.CSRConfigFileFullPath + "\"";
                }

                // Start an OpenSSL process with descriptive error if not found
                OpenSSL.TryOpenSSL(_InvocationParameters);
            }
            else
            {
                System.Windows.MessageBox.Show("OpenSSLCreateCACertConfig.txt file not found in " + OpenSSLConfig.CSRConfigFileFullPath, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateCreateCSR()
        {
            Hashtable _FieldList = new()
            {
                {
                    "CSR Name ",
                    _CreateCSRCSRNameTF
                },
                {
                    "CSR Location ",
                    _CreateCSRCSRLocationTF
                },
                {
                    "CSR Private Key ",
                    _CreateCSRPrivateKeyLocationTF
                }
            };

            //_FieldList.Add("Private Key Password ", _CreateCSRCSRPasswordTF);

            bool _Status = OpenSSLFieldValidator.ValidateTextFields(_FieldList);
            if (!_Status)
            {
                ArrayList _ErrorList = OpenSSLFieldValidator.GetErrorList();
                IEnumerator _IEnumerator = _ErrorList.GetEnumerator();
                while (_IEnumerator.MoveNext())
                {
                    string _ErrorX = (string)_IEnumerator.Current;
                    if (!string.IsNullOrEmpty(_ErrorX))
                    {
                        System.Windows.MessageBox.Show(_ErrorX, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }

            OpenSSLFieldValidator.ClearErrorList();
            return _Status;
        }

        private void CreateCSRResetBtn_Click(object sender, RoutedEventArgs e)
        {
            _CreateCSRCSRNameTF.Text = string.Empty;
            _CreateCSRCSRLocationTF.Text = string.Empty;
            _CreateCSRPrivateKeyLocationTF.Text = string.Empty;
            _CreateCSRCSRPasswordTF.Password = string.Empty;
        }

        private void UtilCreatePKCS12ClientCertificateLocationTriggerBtn_Click(object sender, RoutedEventArgs e)
        {
            using OpenFileDialog _UtilCreatePKCS12FileDialogue = new();
            _UtilCreatePKCS12FileDialogue.InitialDirectory = OpenSSLEnvVarProvider.GetCurrentDirPath();
            _ = _UtilCreatePKCS12FileDialogue.ShowDialog();
            string _UtilCreatePKCS12ClientCertLocation = _UtilCreatePKCS12FileDialogue.FileName;
            if (!string.IsNullOrEmpty(_UtilCreatePKCS12ClientCertLocation))
            {
                _UtilCreatePKCS12ClientCertificateLocationTb.Text = _UtilCreatePKCS12ClientCertLocation;
            }
        }

        private void UtilCreatePKCS12ClientPrivateKeyLocationTriggerBtn_Click(object sender, RoutedEventArgs e)
        {
            using OpenFileDialog _UtilCreatePKCS12FileDialogue = new();
            _UtilCreatePKCS12FileDialogue.InitialDirectory = OpenSSLEnvVarProvider.GetCurrentDirPath();
            _ = _UtilCreatePKCS12FileDialogue.ShowDialog();
            string _UtilCreatePKCS12ClientPrivateKeyLocation = _UtilCreatePKCS12FileDialogue.FileName;
            if (!string.IsNullOrEmpty(_UtilCreatePKCS12ClientPrivateKeyLocation))
            {
                _UtilCreatePKCS12ClientPrivateKeyLocationTb.Text = _UtilCreatePKCS12ClientPrivateKeyLocation;
            }
        }

        private void UtilCreatePKCS12SaveInTriggerBtn_Click(object sender, RoutedEventArgs e)
        {
            using FolderBrowserDialog _FodlerBrowserDialogue = new();
            _FodlerBrowserDialogue.SelectedPath = OpenSSLEnvVarProvider.GetCurrentDirPath();
            _ = _FodlerBrowserDialogue.ShowDialog();
            _UtilCreatePKCS12SaveInTb.Text = _FodlerBrowserDialogue.SelectedPath;
        }

        private void UtilCreatePKCS12ExportBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateExportToPKCS12())
            {
                return;
            }

            string _OpenSSLUIPATHEnvVar = OpenSSLEnvVarProvider.GetOPENSSLUIPATHEnvVar();
            if (string.IsNullOrEmpty(_OpenSSLUIPATHEnvVar))
            {
                System.Windows.MessageBox.Show("OPENSSL_UI_PATH is not set , Please set the path before continue!", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            const string _PKCS_EXTENSION = ".p12";
            const string _COM_PKCS12 = "pkcs12";
            const string _COM_EXPORT = "export";

            /*
                * pkcs12 -passin pass:welcome -export -in C:\\work\\personal\\temp\\certs
                * \\ranil-lptp-cert.crt -inkey C:\\work\\personal\\temp\\key\\ranil-lptp -out C:\\
                * work\\personal\\temp\\certs\\ranil-lptp-234.p12 -password pass:welcome
                */

            string _ClientCert = _UtilCreatePKCS12ClientCertificateLocationTb.Text;
            string _ClientPrivateKey = _UtilCreatePKCS12ClientPrivateKeyLocationTb.Text;
            string _PKCS12SaveLocation = _UtilCreatePKCS12SaveInTb.Text;
            string _PKCS12FileName = _UtilCreatePKCS12FileNameTb.Text;
            string _PKCS12SaveFullPath = Path.Combine(_PKCS12SaveLocation, _PKCS12FileName + _PKCS_EXTENSION);
            string _ClientKeyPass = _UtilCreatePKCS12ClientKeyPasswordTb.Password;
            string _PKCS12Password = _UtilCreatePKCS12PasswordTb.Password;

            // Add dynamic parameters
            string _InvocationParameters;
            if (!string.IsNullOrEmpty(_ClientKeyPass) && !string.IsNullOrEmpty(_PKCS12Password))
            {
                // client RSA key has a password and PKCS12 certificate chain also has a password
                _InvocationParameters = _COM_PKCS12 + " -passin pass:" + _ClientKeyPass + " -" + _COM_EXPORT;
            }
            else if (string.IsNullOrEmpty(_ClientKeyPass) && !string.IsNullOrEmpty(_PKCS12Password))
            {
                // client RSA key does not have a password and PKCS12 certificate chain also has a password
                _InvocationParameters = _COM_PKCS12 + " -" + _COM_EXPORT;
            }
            else if (!string.IsNullOrEmpty(_ClientKeyPass)) // && String.IsNullOrEmpty(_PKCS12Password) is implicit at this point
            {
                // client RSA key has a password and PKCS12 certificate chain does not have a password
                _InvocationParameters = _COM_PKCS12 + " -passin pass:" + _ClientKeyPass + " -" + _COM_EXPORT;
            }
            else
            {
                // client RSA key does not have a password and PKCS12 certificate chain does not have a password
                _InvocationParameters = _COM_PKCS12 + " -" + _COM_EXPORT;
            }

            // Add parameters common to each scenario
            _InvocationParameters = _InvocationParameters + " -in \"" + _ClientCert + "\" -inkey \"" + _ClientPrivateKey + "\"" +
                " -out \"" + _PKCS12SaveFullPath + "\" -passout pass:" + _PKCS12Password;

            // Start an OpenSSL process with descriptive error if not found
            OpenSSL.TryOpenSSL(_InvocationParameters);
        }

        private bool ValidateExportToPKCS12()
        {
            OpenSSLFieldValidator.ClearErrorList();
            Hashtable _FieldList = new()
            {
                {
                    "Client Certificate Location ",
                    _UtilCreatePKCS12ClientCertificateLocationTb
                },
                {
                    "Client Private Key Location ",
                    _UtilCreatePKCS12ClientPrivateKeyLocationTb
                },
                {
                    "Save PKCS12 In Folder ",
                    _UtilCreatePKCS12SaveInTb
                },
                {
                    "PKCS12 File Name ",
                    _UtilCreatePKCS12FileNameTb
                }
            };

            //_FieldList.Add("Client Key Password  ", _UtilCreatePKCS12ClientKeyPasswordTb);
            //_FieldList.Add("PKCS12 Password  ", _UtilCreatePKCS12PasswordTb);

            bool _Status = OpenSSLFieldValidator.ValidateTextFields(_FieldList);
            if (!_Status)
            {
                ArrayList _ErrorList = OpenSSLFieldValidator.GetErrorList();
                IEnumerator _IEnumerator = _ErrorList.GetEnumerator();
                while (_IEnumerator.MoveNext())
                {
                    string _ErrorX = (string)_IEnumerator.Current;
                    if (!string.IsNullOrEmpty(_ErrorX))
                    {
                        System.Windows.MessageBox.Show(_ErrorX, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }

            OpenSSLFieldValidator.ClearErrorList();
            return _Status;
        }

        private void UtilCreatePKCS12ResetBtn_Click(object sender, RoutedEventArgs e)
        {
            _UtilCreatePKCS12ClientCertificateLocationTb.Text = string.Empty;
            _UtilCreatePKCS12ClientPrivateKeyLocationTb.Text = string.Empty;
            _UtilCreatePKCS12SaveInTb.Text = string.Empty;
            _UtilCreatePKCS12FileNameTb.Text = string.Empty;
            _UtilCreatePKCS12ClientKeyPasswordTb.Password = string.Empty;
            _UtilCreatePKCS12PasswordTb.Password = string.Empty;

        }

        private void UtilCreatePKCS12ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            DoCleanExit();
        }
    }
}
