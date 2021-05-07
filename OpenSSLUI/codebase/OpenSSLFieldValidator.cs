using System;
using System.Windows.Controls;
using System.Collections;

namespace OpenSSLUI.codebase
{
    class OpenSSLFieldValidator
    {
        private static ArrayList _ErrorMessageCollection = new ArrayList();


        public static Boolean ValidateTextField(TextBox _TextBox,String _FieldName) 
        {
            String _Value = _TextBox.Text;
            if (_Value == null || _Value.Equals(string.Empty))
            {
                _ErrorMessageCollection.Add(_FieldName + " value is compulsory");
                return false;
            }
            else 
            {
                return true;
            }
        }

        public static ArrayList GetErrorList() 
        {
            return _ErrorMessageCollection;
        }

        public static void ClearErrorList()
        {
            _ErrorMessageCollection.Clear();
        }

        public static Boolean ValidateTextFields(Hashtable _TextFileds)
        {
            bool status = true;
            IDictionaryEnumerator _IDictionaryEnumerator = _TextFileds.GetEnumerator();
            while (_IDictionaryEnumerator.MoveNext()) 
            {
                String _Key = (String)_IDictionaryEnumerator.Key;
                if (!_Key.Contains("Password"))
                {
                    TextBox _TextBox = (TextBox)_IDictionaryEnumerator.Value;
                    String _Text = _TextBox.Text;
                    if (_Text == null || _Text.Equals(string.Empty))
                    {
                        _ErrorMessageCollection.Add(_Key + " value is compulsory");
                        status = false;
                    }
                }
                else 
                {
                    PasswordBox _PasswordBox = (PasswordBox)_IDictionaryEnumerator.Value;
                    String _Text = _PasswordBox.Password;
                    if (_Text == null || _Text.Equals(string.Empty))
                    {
                        _ErrorMessageCollection.Add(_Key + " value is compulsory");
                        status = false;
                    }
                }
            }
            return status;
        }

        public static bool ValidateFormat(String _Value, String _ItemType,String _FieldDisplayName)
        {
            bool _ItemValidationSuccess = false;
            if (!String.IsNullOrEmpty(_Value) && !String.IsNullOrEmpty(_ItemType)) 
            {
                if (_ItemType.Equals("Email", StringComparison.CurrentCultureIgnoreCase)) 
                {
                    bool _PositioningOk = false;
                    bool _AtContains = _Value.Contains("@");
                    bool _DotContains = _Value.Contains(".");

                    int _IndexOfAt = _Value.IndexOf("@");
                    int _IndexOfDot = _Value.IndexOf(".");
                    if (_IndexOfDot >= (_IndexOfAt + 1))
                    {
                         _PositioningOk = true;
                    }

                    if (_AtContains && _DotContains && _PositioningOk) 
                    {
                        _ItemValidationSuccess = true;
                    }
                }
            }
            if (!_ItemValidationSuccess) 
            {
                _ErrorMessageCollection.Add(_FieldDisplayName + " format is incorrect, please enter valid format!");
            }
            return _ItemValidationSuccess;
        }

    }
}
