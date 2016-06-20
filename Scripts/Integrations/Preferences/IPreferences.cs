﻿//----------------------------------------------
// Flip Web Apps: Prefs Editor
// Copyright © 2016 Flip Web Apps / Mark Hewitt
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

using UnityEngine;

namespace FlipWebApps.GameFramework.Scripte.Integrations.Preferences
{
    /// <summary>
    /// Abstracts away the underlying preferences clases allowing us to extend and use other assets.
    /// </summary>
    public  interface IPreferences
    {
        /// <summary>
        /// Flag indicating whether the current factory implementation supports secure prefs.
        /// </summary>
        bool SupportsSecurePrefs { get; }

        /// <summary>
        /// Flag indicating whether to use secure prefs.
        /// 
        /// Note: at the current time all prefs used through this interface must adhere to this flag. The only way to mix 
        /// secure and standard prefs is to mix calls with standard PlayerPrefs calls.
        /// </summary>
        bool UseSecurePrefs { get; set; }

        /// <summary>
        /// For the similar method in PlayerPrefs.
        /// </summary>
        void DeleteAll();

        /// <summary>
        /// For the similar method in PlayerPrefs.
        /// </summary>
        void DeleteKey(string key);

        /// <summary>
        /// For the similar method in PlayerPrefs.
        /// </summary>
        float GetFloat(string key, float defaultValue = 0.0f, bool? useSecurePrefs = null);

        /// <summary>
        /// For the similar method in PlayerPrefs.
        /// </summary>
        int GetInt(string key, int defaultValue = 0, bool? useSecurePrefs = null);

        /// <summary>
        /// For the similar method in PlayerPrefs.
        /// </summary>
        string GetString(string key, string defaultValue = "", bool? useSecurePrefs = null);

        /// <summary>
        /// For the similar method in PlayerPrefs.
        /// </summary>
        bool HasKey(string key);

        /// <summary>
        /// For the similar method in PlayerPrefs.
        /// </summary>
        void Save();

        /// <summary>
        /// For the similar method in PlayerPrefs.
        /// </summary>
        void SetFloat(string key, float value, bool? useSecurePrefs = null);

        /// <summary>
        /// For the similar method in PlayerPrefs.
        /// </summary>
        void SetInt(string key, int value, bool? useSecurePrefs = null);

        /// <summary>
        /// For the similar method in PlayerPrefs.
        /// </summary>
        void SetString(string key, string value, bool? useSecurePrefs = null);
    }
}
