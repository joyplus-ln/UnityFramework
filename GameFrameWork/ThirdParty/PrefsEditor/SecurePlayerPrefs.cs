//----------------------------------------------
// Flip Web Apps: Prefs Editor
// Copyright © 2016-2017 Flip Web Apps / Mark Hewitt
//
// Please direct any bugs/comments/suggestions to http://www.flipwebapps.com
// 
// The copyright owner grants to the end user a non-exclusive, worldwide, and perpetual license to this Asset
// to integrate only as incorporated and embedded components of electronic games and interactive media and 
// distribute such electronic game and interactive media. End user may modify Assets. End user may otherwise 
// not reproduce, distribute, sublicense, rent, lease or lend the Assets. It is emphasized that the end 
// user shall not be entitled to distribute or transfer in any way (including, without, limitation by way of 
// sublicense) the Assets in any other way than as integrated components of electronic games and interactive media. 

// The above copyright notice and this permission notice must not be removed from any files.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//----------------------------------------------

using System;
using System.Globalization;
//using System.IO;
//using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace PrefsEditor
{
    /// <summary>
    /// A secured alternative to the <a href="http://docs.unity3d.com/Documentation/ScriptReference/PlayerPrefs.html">PlayerPrefs</a> class.
    /// </summary>
    /// This class saves preferences values in an encrypted format.
    public static class SecurePlayerPrefs
    {
        static string DefaultPassPhrase = "FWAhT4k8.Mgv";

        /// <summary>
        /// A dummy string that can be used for encrypted entries to see if they are valid or not
        /// </summary>
        public const string NotFoundString = "|[<not found>]|";

        /// <summary>
        /// Enum for the different types of preferences an individual item can be.
        /// </summary>
        public enum ItemType : byte { None, Int, Float, String } //, Bool, Vector2, Vector3 }

        /// <summary>
        /// Mode of encryption for the specified item.
        /// </summary>
        /// TODO: AES is not currently supported.
        public enum ItemMode : byte { XOR, AES }

        /// <summary>
        /// Flag indicating whether to use secure prefs.
        /// </summary>
        /// Note: at the current time all prefs used through this interface must adhere to this flag. The only way to mix 
        /// secure and standard prefs is to mix calls with standard PlayerPrefs calls.
        public static bool UseSecurePrefs { get; set; }

        /// <summary>
        /// Flag indicating whether to use automatically convert unsecure prefs.
        /// </summary>
        /// If this is set then secure prefs that aren't found will automatically fall back and try to locate an 
        /// unsecured version and then replace that with the secured alternative. This can be useful for existing
        /// games that make use of the standard player prefs.
        public static bool AutoConvertUnsecurePrefs { get; set; }

        /// <summary>
        /// The pass phrase that should be used. Be sure to override this with your own value.
        /// </summary>
        public static string PassPhrase
        {
            get { return _passPhrase; }
            set
            {
                _passPhrase = string.IsNullOrEmpty(value) ? DefaultPassPhrase : value;
                IsPassPhraseSet = true;
            }
        }
        static string _passPhrase = DefaultPassPhrase;

        /// <summary>
        /// Returns whether the pass phrase has been set. You should not rely on the default pass phrase value!
        /// </summary>
        public static bool IsPassPhraseSet { get; set; }

        #region General Management Functions
        /// <summary>
        /// Wrapper for the same method in PlayerPrefs but works with encrypted player prefs.
        /// </summary>
        public static bool HasKey(string key, bool? useSecurePrefs = null)
        {
            // if not using secure prefs, or we provide an override then just fall through to standard call.
            if ((!UseSecurePrefs && !useSecurePrefs.HasValue) || (useSecurePrefs.HasValue && useSecurePrefs.Value == false))
                return PlayerPrefs.HasKey(key);

            var hasEncryptedKey = PlayerPrefs.HasKey(EncryptKey(key));
            if (hasEncryptedKey || !AutoConvertUnsecurePrefs)
                return hasEncryptedKey;

            return PlayerPrefs.HasKey(key);
        }

        /// <summary>
        /// Encrypt the key using simple Xor symmetric algorithm
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string EncryptKey(string key)
        {           
            return XorEncrypt(key, PassPhrase);
        }

        /// <summary>
        /// Decrypt the key using simple Xor symmetric algorithm
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string DecryptKey(string key)
        {
            return XorDecrypt(key, PassPhrase);
        }

        /// <summary>
        /// For a secured entry, returns the type that it represents. Note that key should be the version saved in Player Prefs
        /// (Encrypted for secured prefs).
        /// </summary>
        /// <param name="encryptedString"></param>
        /// <returns></returns>
        public static ItemType GetItemType(string encryptedString)
        {
            try
            {
                var combinedBytes = Convert.FromBase64String(encryptedString);
                return (ItemType)combinedBytes[0];
            }
            catch (Exception)
            {
                //Debug.LogWarning("Error Decrypting Prefs Item:" + e.Message);
                return ItemType.None;
            }
        }

        /// <summary>
        /// Wrapper for the same method in PlayerPrefs but works with encrypted player prefs.
        /// </summary>
        public static void Save()
        {
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Wrapper for the same method in PlayerPrefs but works with encrypted player prefs.
        /// </summary>
        public static void DeleteAll()
        {
            PlayerPrefs.DeleteAll();
        }

        /// <summary>
        /// Wrapper for the same method in PlayerPrefs but works with encrypted player prefs.
        /// </summary>
        public static void DeleteKey(string key, bool? useSecurePrefs = null)
        {
            // if not using secure prefs, or we provide an override then just fall through to standard call.
            if ((!UseSecurePrefs && !useSecurePrefs.HasValue) || (useSecurePrefs.HasValue && useSecurePrefs.Value == false))
            {
                PlayerPrefs.DeleteKey(key);
                return;
            }

            PlayerPrefs.DeleteKey(EncryptKey(key));
            if (AutoConvertUnsecurePrefs)
                PlayerPrefs.DeleteKey(key);
        }
        #endregion General Management Functions

        #region Float
        /// <summary>
        /// Wrapper for the same method in PlayerPrefs but works with encrypted player prefs.
        /// </summary>
        public static float GetFloat(string key, float defaultValue = 0.0f, bool? useSecurePrefs = null)
        {
            // if not using secure prefs, or we provide an override then just fall through to standard call.
            if ((!UseSecurePrefs && !useSecurePrefs.HasValue) || (useSecurePrefs.HasValue && useSecurePrefs.Value == false))
                return PlayerPrefs.GetFloat(key, defaultValue);

            // using secure prefs.
            var prefsEntryBytes = GetDecryptedPrefsEntry(key, ItemType.Float);
            if (prefsEntryBytes != null)
            {
                return BitConverter.ToSingle(prefsEntryBytes, 0);
            }

            // if the prefs entry was not found and we are auto converting then try and get and replace any unencrypted value.
            if (AutoConvertUnsecurePrefs && PlayerPrefs.HasKey(key))
            {
                var value = PlayerPrefs.GetFloat(key, defaultValue);
                SetFloat(key, value);
                PlayerPrefs.DeleteKey(key);
                return value;
            }

            // nothing found so return the default value
            return defaultValue;
        }

        /// <summary>
        /// Wrapper for the same method in PlayerPrefs but works with encrypted player prefs.
        /// </summary>
        public static void SetFloat(string key, float value, bool? useSecurePrefs = null)
        {
            // if not using secure prefs, or we provide an override then just fall through to standard call.
            if ((!UseSecurePrefs && !useSecurePrefs.HasValue) || (useSecurePrefs.HasValue && useSecurePrefs.Value == false))
            {
                PlayerPrefs.SetFloat(key, value);
                return;
            }

            var valueBytes = BitConverter.GetBytes(value);
            SetEncryptedPrefsEntry(key, valueBytes, ItemType.Float);
        }
        #endregion Float

        #region Int
        /// <summary>
        /// Wrapper for the same method in PlayerPrefs but works with encrypted player prefs.
        /// </summary>
        public static int GetInt(string key, int defaultValue = 0, bool? useSecurePrefs = null)
        {
            // if not using secure prefs, or we provide an override then just fall through to standard call.
            if ((!UseSecurePrefs && !useSecurePrefs.HasValue) || (useSecurePrefs.HasValue && useSecurePrefs.Value == false))
                return PlayerPrefs.GetInt(key, defaultValue);

            // using secure prefs.
            var prefsEntryBytes = GetDecryptedPrefsEntry(key, ItemType.Int);
            if (prefsEntryBytes != null)
            {
                return BitConverter.ToInt32(prefsEntryBytes, 0);
            }

            // if the prefs entry was not found and we are auto converting then try and get and replace any unencrypted value.
            if (AutoConvertUnsecurePrefs && PlayerPrefs.HasKey(key))
            {
                var value = PlayerPrefs.GetInt(key, defaultValue);
                SetInt(key, value);
                PlayerPrefs.DeleteKey(key);
                return value;
            }

            // nothing found so return the default value
            return defaultValue;
        }


        /// <summary>
        /// Wrapper for the same method in PlayerPrefs but works with encrypted player prefs.
        /// </summary>
        public static void SetInt(string key, int value, bool? useSecurePrefs = null)
        {
            // if not using secure prefs, or we provide an override then just fall through to standard call.
            if ((!UseSecurePrefs && !useSecurePrefs.HasValue) || (useSecurePrefs.HasValue && useSecurePrefs.Value == false))
            {
                PlayerPrefs.SetInt(key, value);
                return;
            }

            var valueBytes = BitConverter.GetBytes(value);
            SetEncryptedPrefsEntry(key, valueBytes, ItemType.Int);
        }
        #endregion Int

        #region String
        /// <summary>
        /// Wrapper for the same method in PlayerPrefs but works with encrypted player prefs.
        /// </summary>
        public static string GetString(string key, string defaultValue = "", bool? useSecurePrefs = null, ItemType itemType = ItemType.String)
        {
            // if not using secure prefs, or we provide an override then just fall through to standard call.
            if ((!UseSecurePrefs && !useSecurePrefs.HasValue) || (useSecurePrefs.HasValue && useSecurePrefs.Value == false))
                return PlayerPrefs.GetString(key, defaultValue);

            // using secure prefs.
            var prefsEntryBytes = GetDecryptedPrefsEntry(key, itemType);
            if (prefsEntryBytes != null)
            {
                return Encoding.UTF8.GetString(prefsEntryBytes, 0, prefsEntryBytes.Length);
            }

            // if the prefs entry was not found and we are auto converting then try and get and replace any unencrypted value.
            if (AutoConvertUnsecurePrefs && PlayerPrefs.HasKey(key))
            {
                var value = PlayerPrefs.GetString(key, defaultValue);
                SetString(key, value);
                PlayerPrefs.DeleteKey(key);
                return value;
            }

            // nothing found so return the default value
            return defaultValue;
        }

        /// <summary>
        /// Wrapper for the same method in PlayerPrefs but works with encrypted player prefs.
        /// </summary>
        public static void SetString(string key, string value, bool? useSecurePrefs = null, ItemType itemType = ItemType.String)
        {
            // if not using secure prefs, or we provide an override then just fall through to standard call.
            if ((!UseSecurePrefs && !useSecurePrefs.HasValue) || (useSecurePrefs.HasValue && useSecurePrefs.Value == false))
            {
                PlayerPrefs.SetString(key, value);
                return;
            }

            var valueBytes = Encoding.UTF8.GetBytes(value);
            SetEncryptedPrefsEntry(key, valueBytes, itemType);
        }
        #endregion String

        #region Boolean Extension

        /// <summary>
        /// Get a boolean value from PlayerPrefs.
        /// </summary>
        public static bool GetBool(string key, bool defaultValue = false, bool? useSecurePrefs = null)
        {
            return GetInt(key, defaultValue ? 1 : 0, useSecurePrefs) == 1;
        }


        /// <summary>
        /// Set a boolean value in PlayerPrefs.
        /// </summary>
        public static void SetBool(string key, bool value, bool? useSecurePrefs = null)
        {
            SetInt(key, value ? 1 : 0, useSecurePrefs);
        }
        #endregion Boolean Extension

        #region Vector2 Extension

        /// <summary>
        /// Get a Vector2 value from PlayerPrefs.
        /// </summary>
        public static Vector2? GetVector2(string key, Vector2? defaultValue = null, bool? useSecurePrefs = null)
        {
            var result = GetString(key, null, useSecurePrefs); //, ItemType.Vector2);
            if (result == null)
                return defaultValue;
            var parts = result.Split(':');
            if (parts.Length != 2)
            {
                Debug.LogWarning(
                    "GetVector2 found an invalid value and will return the default. Please check you are accessing the correct prefs item.");
                return defaultValue;
            }
            float x, y;
            if (float.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out x) && 
                float.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out y))
                return new Vector2(x, y);
            Debug.LogWarning(
                "GetVector2 found an invalid number value and will return the default. Please check you are accessing the correct prefs item.");
            return defaultValue;
        }


        /// <summary>
        /// Set a Vector2 value in PlayerPrefs.
        /// </summary>
        public static void SetVector2(string key, Vector2 value, bool? useSecurePrefs = null)
        {
            SetString(key, value.x.ToString(CultureInfo.InvariantCulture) + ":" + value.y.ToString(CultureInfo.InvariantCulture), useSecurePrefs); //, ItemType.Vector2);
        }
        #endregion Vector2 Extension

        #region Vector3 Extension

        /// <summary>
        /// Get a Vector3 value from PlayerPrefs.
        /// </summary>
        public static Vector3? GetVector3(string key, Vector3? defaultValue = null, bool? useSecurePrefs = null)
        {
            var result = GetString(key, null, useSecurePrefs); //, ItemType.Vector3);
            if (result == null)
                return defaultValue;
            var parts = result.Split(':');
            if (parts.Length != 3)
            {
                Debug.LogWarning(
                    "GetVector3 found an invalid value and will return the default. Please check you are accessing the correct prefs item.");
                return defaultValue;
            }
            float x, y, z;
            if (float.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out x) &&
                float.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out y) &&
                float.TryParse(parts[2], NumberStyles.Any, CultureInfo.InvariantCulture, out z))
                return new Vector3(x, y, z);
            Debug.LogWarning(
                "GetVector3 found an invalid number value and will return the default. Please check you are accessing the correct prefs item.");
            return defaultValue;
        }

        /// <summary>
        /// Set a Vector3 value in PlayerPrefs.
        /// </summary>
        public static void SetVector3(string key, Vector3 value, bool? useSecurePrefs = null)
        {
            SetString(key, value.x.ToString(CultureInfo.InvariantCulture) + ":" + value.y.ToString(CultureInfo.InvariantCulture) + ":" + value.z.ToString(CultureInfo.InvariantCulture), useSecurePrefs); //, ItemType.Vector3);
        }

        #endregion Vector3 Extension

        #region Color Extension

        /// <summary>
        /// Get a Color value from PlayerPrefs.
        /// </summary>
        public static Color? GetColor(string key, Color? defaultValue = null, bool? useSecurePrefs = null)
        {
            var result = GetString(key, null, useSecurePrefs); //, ItemType.Vector3);
            if (result == null)
                return defaultValue;
            var parts = result.Split(':');
            if (parts.Length != 4)
            {
                Debug.LogWarning(
                    "Color found an invalid value and will return the default. Please check you are accessing the correct prefs item.");
                return defaultValue;
            }
            byte r, g, b, a;
            if (byte.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out r) &&
                byte.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out g) &&
                byte.TryParse(parts[2], NumberStyles.Any, CultureInfo.InvariantCulture, out b) &&
                byte.TryParse(parts[3], NumberStyles.Any, CultureInfo.InvariantCulture, out a))
                return new Color(r, g, b, a);
            Debug.LogWarning(
                "Color found an invalid number value and will return the default. Please check you are accessing the correct prefs item.");
            return defaultValue;
        }

        /// <summary>
        /// Set a Color value in PlayerPrefs.
        /// </summary>
        public static void SetColor(string key, Color value, bool? useSecurePrefs = null)
        {
            SetString(key, value.r.ToString(CultureInfo.InvariantCulture) + ":" + value.g.ToString(CultureInfo.InvariantCulture) + ":" + value.b.ToString(CultureInfo.InvariantCulture) + ":" + value.a.ToString(CultureInfo.InvariantCulture), useSecurePrefs);
        }

        #endregion Color Extension

        #region Encryption

        /// <summary>
        /// Set an encrypted prefs entry based upon the passed byte array and type
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        public static void SetEncryptedPrefsEntry(string key, byte[] value, ItemType type)
        {
            PlayerPrefs.SetString(EncryptKey(key), EncryptValue(value, type));
        }

        /// <summary>
        /// Generate an encrypted string based upon the passed byte array and type
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string EncryptValue(byte[] value, ItemType type)
        {
            var encryptedBytes = XorEncryptDecrypt(value, PassPhrase);
            var combinedBytes = new byte[encryptedBytes.Length + 2];
            combinedBytes[0] = (byte)type;
            combinedBytes[1] = (byte)ItemMode.XOR;  // TODO add support for other types.
            Buffer.BlockCopy(encryptedBytes, 0, combinedBytes, 2, encryptedBytes.Length);

            return Convert.ToBase64String(combinedBytes);
        }

        /// <summary>
        /// Get the raw encrypted prefs value for the given key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetRawEncryptedPrefsEntry(string key)
        {
            return PlayerPrefs.GetString(EncryptKey(key), null);
        }

        /// <summary>
        /// Get and decrypt an encrypted prefs entry into a byte array.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <returns>byte array, or null if there was an error.</returns>
        public static byte[] GetDecryptedPrefsEntry(string key, ItemType type)
        {
            var value = PlayerPrefs.GetString(EncryptKey(key), null);
            return DecryptValue(value, type);
        }

        /// <summary>
        /// Check an encrypted string is in the correct format and decrypt the contained byte array
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns>byte array, or null if there was an error.</returns>
        public static byte[] DecryptValue(string value, ItemType type)
        {
            if (value == null) return null;
            try
            {
                var combinedBytes = Convert.FromBase64String(value);
                if (combinedBytes[0] != (byte) type) return null;

                return XorEncryptDecrypt(combinedBytes, PassPhrase, 2);
            }
            catch (Exception)
            {
                //Debug.LogWarning("Error Decrypting Prefs String:" + e.Message);
                return null;
            }
        }

        #endregion Encryption

        #region XOR Encryption
        /// <summary>
		/// Simple XOR based symmetric encryption of a value uses the passed key.
		/// </summary>
		public static string XorEncrypt(string value, string encryptionKey)
        {
            if (string.IsNullOrEmpty(value)) return null;

            var valueBytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(XorEncryptDecrypt(valueBytes, encryptionKey));
        }

        /// <summary>
		/// Simple XOR based symmetric decryption of a value uses the passed key.
		/// </summary>
		public static string XorDecrypt(string value, string encryptionKey)
        {
            if (string.IsNullOrEmpty(value)) return null;

            try
            {
                var valueBytes = Convert.FromBase64String(value);
                var decryptedBytes = XorEncryptDecrypt(valueBytes, encryptionKey);
                return Encoding.UTF8.GetString(decryptedBytes, 0, decryptedBytes.Length);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
		/// Simple XOR based symmetric encryption and decryption of a value using the passed key.
		/// </summary>
		public static byte[] XorEncryptDecrypt(byte[] value, string encryptionKey, int startIndex = 0)
        {
            var encryptionKeyLength = encryptionKey.Length;
            var result = new byte[value.Length - startIndex];

            for (var i = 0; i < value.Length - startIndex; i++)
            {
                result[i] = (byte)(value[i + startIndex] ^ encryptionKey[i % encryptionKeyLength]);
            }

            return result;
        }
        #endregion XOR Encryption
/*
        #region AES Encryption
        // This constant is used to determine the keysize of the encryption algorithm in bits.
        // We divide this by 8 within the code below to get the equivalent number of bytes.
        private const int Keysize = 256;

        //static string _salt = "FWA1789t.v";
        static string _initialisationVector = "FWfhT4e8.7gvgyk4";  // IV must be keysize / 8. A 16 char string = 32 bytes 

        // This constant determines the number of iterations for the password bytes generation function.
        //private const int DerivationIterations = 1000;

        /// <summary>
        /// Encrypt a string using the given pass phrase
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="passPhrase"></param>
        /// <returns></returns>
        public static string Encrypt(string plainText, string passPhrase)
        {
            var ivStringBytes = Encoding.UTF8.GetBytes(_initialisationVector);

            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            //var passwordDeriveBytes = new PasswordDeriveBytes(passPhrase, null);
            var keyBytes = new byte[0];//passwordDeriveBytes.GetBytes(Keysize / 8);

            using (var symmetricKey = new RijndaelManaged())
            {
                symmetricKey.Mode = CipherMode.CBC;

                using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                            cryptoStream.FlushFinalBlock();
                            return Convert.ToBase64String(memoryStream.ToArray());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Decrypt a string using the given pass phrase.
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="passPhrase"></param>
        /// <returns></returns>
        public static string Decrypt(string cipherText, string passPhrase)
        {
            try {
                var ivStringBytes = Encoding.UTF8.GetBytes(_initialisationVector);
                var cipherTextBytes = Convert.FromBase64String(cipherText);

                //var passwordDeriveBytes = new PasswordDeriveBytes(passPhrase, null);
                var keyBytes = new byte[0]; //passwordDeriveBytes.GetBytes(Keysize / 8);

                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.Mode = CipherMode.CBC;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                var plainTextBytes = new byte[cipherTextBytes.Length];
                                var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.ToString());
                return null;
            }
        }

        #endregion AES Encryption
    */
    }
}

