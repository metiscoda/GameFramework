﻿//----------------------------------------------
// Flip Web Apps: Game Framework
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

using FlipWebApps.GameFramework.Scripts.FreePrize.Components;
using FlipWebApps.GameFramework.Scripts.GameStructure;
using UnityEditor;
using UnityEngine;

namespace FlipWebApps.GameFramework.Scripts.Debugging.Components {

    /// <summary>
    /// Component for allowing various cheat functions to be called such as increasing score, resetting prefs etc..
    /// </summary>
    public class CheatFunctionsWindow : EditorWindow
    {
        // Add menu item
        [MenuItem("Window/Flip Web Apps/Cheat Functions Windows")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            EditorWindow window = EditorWindow.GetWindow(typeof(CheatFunctionsWindow));
            window.titleContent.text = "Cheat Functions";
        }

        void OnGUI()
        {
            PreferencesMenuOptions();
            PlayerMenuOptions();
            WorldMenuOptions();
            LevelMenuOptions();
            FreePrizeMenuOptions();
        }

        private static void PreferencesMenuOptions()
        {
            // preferences
            GUILayout.Label("Preferences", new GUIStyle() { fontStyle = FontStyle.Bold, padding = new RectOffset(5, 5, 5, 5) });
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Reset", GUILayout.Width(100)))
            {
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();
                Debug.Log("Player prefs deleted. Note: Some gameobjects might hold values and write these out after this call!");
            }
            GUILayout.EndHorizontal();
        }

        private static void PlayerMenuOptions()
        {
            // player
            GUILayout.Label("Player", new GUIStyle() { fontStyle = FontStyle.Bold, padding = new RectOffset(5, 5, 5, 5) });
            // lives
            GUILayout.BeginHorizontal();
            var playerLives = GameManager.IsActive ? " (" + GameManager.Instance.Player.Lives + ")" : "";
            GUILayout.Label("Lives" + playerLives, GUILayout.Width(100));
            if (GUILayout.Button("-1", GUILayout.Width(50)))
            {
                if (Application.isPlaying && GameManager.IsActive)
                {
                    GameManager.Instance.Player.Lives -= 1;
                }
                else
                {
                    Debug.LogWarning("This only works in play mode. You also need to add a GameManager.");
                }
            }
            if (GUILayout.Button("+1", GUILayout.Width(50)))
            {
                if (Application.isPlaying && GameManager.IsActive)
                {
                    GameManager.Instance.Player.Lives += 1;
                }
                else
                {
                    Debug.LogWarning("This only works in play mode. You also need to add a GameManager.");
                }
            }
            GUILayout.EndHorizontal();
            // player score
            GUILayout.BeginHorizontal();
            string playerScore = GameManager.IsActive ? " (" + GameManager.Instance.Player.Score + ")" : "";
            GUILayout.Label("Score" + playerScore, GUILayout.Width(100));
            if (GUILayout.Button("-100", GUILayout.Width(50)))
            {
                UpdatePlayerScore(-100);
            }
            if (GUILayout.Button("-10", GUILayout.Width(50)))
            {
                UpdatePlayerScore(-10);
            }
            if (GUILayout.Button("0", GUILayout.Width(50)))
            {
                UpdatePlayerScore(int.MinValue);
            }
            if (GUILayout.Button("+10", GUILayout.Width(50)))
            {
                UpdatePlayerScore(10);
            }
            if (GUILayout.Button("+100", GUILayout.Width(50)))
            {
                UpdatePlayerScore(100);
            }
            GUILayout.EndHorizontal();

            // player coins
            GUILayout.BeginHorizontal();
            string playerCoins = GameManager.IsActive ? " (" + GameManager.Instance.Player.Coins + ")" : "";
            GUILayout.Label("Coins" + playerCoins, GUILayout.Width(100));
            if (GUILayout.Button("-100", GUILayout.Width(50)))
            {
                UpdatePlayerCoins(-100);
            }
            if (GUILayout.Button("-10", GUILayout.Width(50)))
            {
                UpdatePlayerCoins(-10);
            }
            if (GUILayout.Button("0", GUILayout.Width(50)))
            {
                UpdatePlayerCoins(int.MinValue);
            }
            if (GUILayout.Button("+10", GUILayout.Width(50)))
            {
                UpdatePlayerCoins(10);
            }
            if (GUILayout.Button("+100", GUILayout.Width(50)))
            {
                UpdatePlayerCoins(100);
            }
            GUILayout.EndHorizontal();
        }

        private static void WorldMenuOptions()
        {
            GUILayout.Label("World", new GUIStyle() { fontStyle = FontStyle.Bold, padding = new RectOffset(5, 5, 5, 5) });
            // general
            GUILayout.BeginHorizontal();
            GUILayout.Label("Current Worlds", GUILayout.Width(100));
            if (GUILayout.Button("Unlock All", GUILayout.Width(100)))
            {
                if (Application.isPlaying && GameManager.IsActive && GameManager.Instance.Worlds != null)
                {
                    foreach (var world in GameManager.Instance.Worlds.Items)
                    {
                        world.IsUnlocked = true;
                        world.IsUnlockedAnimationShown = true;
                        world.UpdatePlayerPrefs();
                    }
                    PlayerPrefs.Save();
                }
                else
                {
                    Debug.LogWarning("This only works in play mode. You also need to add a GameManager and have worlds setup.");
                }
            }
            if (GUILayout.Button("Lock All", GUILayout.Width(100)))
            {
                if (Application.isPlaying && GameManager.IsActive && GameManager.Instance.Worlds != null)
                {
                    foreach (var world in GameManager.Instance.Worlds.Items)
                    {
                        world.IsUnlocked = false;
                        world.IsUnlockedAnimationShown = false;
                        world.UpdatePlayerPrefs();
                    }
                    PlayerPrefs.Save();
                }
                else
                {
                    Debug.LogWarning("This only works in play mode. You also need to add a GameManager and have worlds setup.");
                }
            }
            GUILayout.EndHorizontal();
        }

        private static void LevelMenuOptions()
        {
            GUILayout.Label("Level", new GUIStyle() { fontStyle = FontStyle.Bold, padding = new RectOffset(5, 5, 5, 5) });
            // general
            GUILayout.BeginHorizontal();
            GUILayout.Label("Current Levels", GUILayout.Width(100));
            if (GUILayout.Button("Unlock All", GUILayout.Width(100)))
            {
                if (Application.isPlaying && GameManager.IsActive && GameManager.Instance.Levels != null)
                {
                    foreach (var level in GameManager.Instance.Levels.Items)
                    {
                        level.IsUnlocked = true;
                        level.IsUnlockedAnimationShown = true;
                        level.UpdatePlayerPrefs();
                    }
                    PlayerPrefs.Save();
                }
                else
                {
                    Debug.LogWarning("This only works in play mode. You also need to add a GameManager and have levels setup.");
                }
            }
            if (GUILayout.Button("Lock All", GUILayout.Width(100)))
            {
                if (Application.isPlaying && GameManager.IsActive && GameManager.Instance.Levels != null)
                {
                    foreach (var level in GameManager.Instance.Levels.Items)
                    {
                        level.IsUnlocked = false;
                        level.IsUnlockedAnimationShown = false;
                        level.UpdatePlayerPrefs();
                    }
                    PlayerPrefs.Save();
                }
                else
                {
                    Debug.LogWarning("This only works in play mode. You also need to add a GameManager and have levels setup.");
                }
            }
            GUILayout.EndHorizontal();

            // level score
            GUILayout.BeginHorizontal();
            string levelScore = (GameManager.IsActive && GameManager.Instance.Levels != null) ? " (" + GameManager.Instance.Levels.Selected.Score + ")" : "";
            GUILayout.Label("Score" + levelScore, GUILayout.Width(100));
            if (GUILayout.Button("-100", GUILayout.Width(50)))
            {
                UpdateLevelScore(-100);
            }
            if (GUILayout.Button("-10", GUILayout.Width(50)))
            {
                UpdateLevelScore(-10);
            }
            if (GUILayout.Button("0", GUILayout.Width(50)))
            {
                UpdateLevelScore(int.MinValue);
            }
            if (GUILayout.Button("+10", GUILayout.Width(50)))
            {
                UpdateLevelScore(10);
            }
            if (GUILayout.Button("+100", GUILayout.Width(50)))
            {
                UpdateLevelScore(100);
            }
            GUILayout.EndHorizontal();

            // player coins
            GUILayout.BeginHorizontal();
            string levelCoins = (GameManager.IsActive && GameManager.Instance.Levels != null) ? " (" + GameManager.Instance.Levels.Selected.Coins + ")" : "";
            GUILayout.Label("Coins" + levelCoins, GUILayout.Width(100));
            if (GUILayout.Button("-100", GUILayout.Width(50)))
            {
                UpdateLevelCoins(-100);
            }
            if (GUILayout.Button("-10", GUILayout.Width(50)))
            {
                UpdateLevelCoins(-10);
            }
            if (GUILayout.Button("0", GUILayout.Width(50)))
            {
                UpdateLevelCoins(int.MinValue);
            }
            if (GUILayout.Button("+10", GUILayout.Width(50)))
            {
                UpdateLevelCoins(10);
            }
            if (GUILayout.Button("+100", GUILayout.Width(50)))
            {
                UpdateLevelCoins(100);
            }
            GUILayout.EndHorizontal();
        }

        static void FreePrizeMenuOptions()
        {
            GUI.enabled = Application.isPlaying && FreePrizeManager.IsActive;

            GUILayout.BeginHorizontal();
            if (Application.isPlaying && FreePrizeManager.IsActive)
                GUILayout.Label("Free Prize (prize in " + FreePrizeManager.Instance.GetTimeToPrize() + ")",
                    EditorStyles.boldLabel);
            else if (Application.isPlaying && !FreePrizeManager.IsActive)
                GUILayout.Label("Free Prize (no FreePrizeMaanger detected)", EditorStyles.boldLabel);
            else
                GUILayout.Label("Free Prize", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Make Prize Available", GUILayout.Width(150)))
            {
                FreePrizeManager.Instance.MakePrizeAvailable();
            }
            if (GUILayout.Button("Reset Counter", GUILayout.Width(150)))
            {
                FreePrizeManager.Instance.StartNewCountdown();
            }
            GUILayout.EndHorizontal();
        }

        private static void UpdatePlayerScore(int amount)
        {
            if (Application.isPlaying)
            {
                if (GameManager.IsActive)
                {
                    if (amount > 0)
                        GameManager.Instance.GetPlayer().AddPoints(amount);
                    else
                        GameManager.Instance.GetPlayer().RemovePoints(-amount);
                }
                else
                {
                    Debug.LogWarning("You need to add a GameManager to your scene to use this function.");
                }
            }
            else
            {
                Debug.LogWarning("THis only works in play mode.");
            }
        }

        private static void UpdatePlayerCoins(int amount)
        {
            if (Application.isPlaying)
            {
                if (GameManager.IsActive)
                {
                    if (amount > 0)
                        GameManager.Instance.GetPlayer().AddCoins(amount);
                    else
                        GameManager.Instance.GetPlayer().RemoveCoins(-amount);
                }
                else
                {
                    Debug.LogWarning("You need to add a GameManager to your scene to use this function.");
                }
            }
            else
            {
                Debug.LogWarning("THis only works in play mode.");
            }
        }

        private static void UpdateLevelScore(int amount)
        {
            if (Application.isPlaying)
            {
                if (GameManager.IsActive)
                {
                    if (GameManager.Instance.Levels != null)
                    {
                        if (amount > 0)
                            GameManager.Instance.Levels.Selected.AddPoints(amount);
                        else
                            GameManager.Instance.Levels.Selected.RemovePoints(-amount);
                    }
                    else
                    {
                        Debug.LogWarning("You need to have levels setup to use this function.");
                    }
                }
                else
                {
                    Debug.LogWarning("You need to add a GameManager to your scene to use this function.");
                }
            }
            else
            {
                Debug.LogWarning("THis only works in play mode.");
            }
        }

        private static void UpdateLevelCoins(int amount)
        {
            if (Application.isPlaying)
            {
                if (GameManager.IsActive)
                {
                    if (GameManager.Instance.Levels != null)
                    {
                        if (amount > 0)
                            GameManager.Instance.Levels.Selected.AddCoins(amount);
                        else
                            GameManager.Instance.Levels.Selected.RemoveCoins(-amount);
                    }
                    else
                    {
                        Debug.LogWarning("You need to have levels setup to use this function.");
                    }
                }
                else
                {
                    Debug.LogWarning("You need to add a GameManager to your scene to use this function.");
                }
            }
            else
            {
                Debug.LogWarning("THis only works in play mode.");
            }
        }
    }
}