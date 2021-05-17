using System;
using System.Collections;
using System.Windows.Controls;

namespace OpenSSLUI.codebase
{
    internal static class OpenSSLFieldValidator
    {
        private static readonly ArrayList _ErrorMessageCollection = new();


        public static bool ValidateTextField(TextBox _TextBox, string _FieldName)
        {
            string _Value = _TextBox.Text;
            if (string.IsNullOrEmpty(_Value))
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

        public static bool ValidateTextFields(Hashtable _TextFileds)
        {
            bool status = true;
            IDictionaryEnumerator _IDictionaryEnumerator = _TextFileds.GetEnumerator();
            while (_IDictionaryEnumerator.MoveNext())
            {
                string _Key = (string)_IDictionaryEnumerator.Key;
                if (!_Key.Contains("Password"))
                {
                    TextBox _TextBox = (TextBox)_IDictionaryEnumerator.Value;
                    string _Text = _TextBox.Text;
                    if (string.IsNullOrEmpty(_Text))
                    {
                        _ErrorMessageCollection.Add(_Key + " value is compulsory");
                        status = false;
                    }
                }
                else
                {
                    PasswordBox _PasswordBox = (PasswordBox)_IDictionaryEnumerator.Value;
                    string _Text = _PasswordBox.Password;
                    if (string.IsNullOrEmpty(_Text))
                    {
                        _ErrorMessageCollection.Add(_Key + " value is compulsory");
                        status = false;
                    }
                }
            }
            return status;
        }

        public static bool ValidateFormat(string _Value, string _ItemType, string _FieldDisplayName)
        {
            bool _ItemValidationSuccess = false;
            if (!string.IsNullOrEmpty(_Value) && !string.IsNullOrEmpty(_ItemType) && _ItemType.Equals("Email", StringComparison.CurrentCultureIgnoreCase))
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
            if (!_ItemValidationSuccess)
            {
                _ErrorMessageCollection.Add(_FieldDisplayName + " format is incorrect, please enter valid format!");
            }
            return _ItemValidationSuccess;
        }

    }
}
