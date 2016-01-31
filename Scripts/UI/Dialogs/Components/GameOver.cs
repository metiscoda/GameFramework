﻿//----------------------------------------------
// Flip Web Apps: Game Framework
// Copyright © 2016 Flip Web Apps / Mark Hewitt
//----------------------------------------------

using System;
using System.Collections;
using FlipWebApps.GameFramework.Scripts.GameObjects;
using FlipWebApps.GameFramework.Scripts.GameObjects.Components;
using FlipWebApps.GameFramework.Scripts.GameStructure;
using FlipWebApps.GameFramework.Scripts.GameStructure.Levels;
using FlipWebApps.GameFramework.Scripts.GameStructure.Levels.ObjectModel;
using FlipWebApps.GameFramework.Scripts.Localisation;
using FlipWebApps.GameFramework.Scripts.Social;
using FlipWebApps.GameFramework.Scripts.UI.Other;
using FlipWebApps.GameFramework.Scripts.UI.Other.Components;
using UnityEngine;
using UnityEngine.Assertions;

#if FACEBOOK_SDK
using FlipWebApps.GameFramework.Scripts.Facebook.Components;
#endif
#if UNITY_ANALYTICS
using System.Collections.Generic;
using UnityEngine.Analytics;
#endif

namespace FlipWebApps.GameFramework.Scripts.UI.Dialogs.Components
{
    /// <summary>
    /// Base class for a game over dialog.
    /// </summary>
    [RequireComponent(typeof(DialogInstance))]
    public class GameOver : Singleton<GameOver>
    {
        [Header("General")]
        public string LocalisationBase = "GameOver";
        public int TimesPlayedBeforeRatingPrompt = -1;
        public bool ShowStars = true;
        public bool ShowTime = true;
        public bool ShowCoins = true;
        public bool ShowScore = true;
        [Header("Tuning")]
        public float PeriodicUpdateDelay = 1f;

        protected DialogInstance DialogInstance;

        protected override void GameSetup()
        {
            DialogInstance = GetComponent<DialogInstance>();

            Assert.IsNotNull(DialogInstance.DialogGameObject, "Ensure that you have set the script execution order of dialog instance in settings (see help for details.");
        }

        public virtual void Show(bool isWon)
        {
            Level currentLevel = GameManager.Instance.Levels.Selected;

            // show won / lost game objects as appropriate
            GameObjectHelper.SafeSetActive(GameObjectHelper.GetChildNamedGameObject(DialogInstance.gameObject, "Won", true), isWon);
            GameObjectHelper.SafeSetActive(GameObjectHelper.GetChildNamedGameObject(DialogInstance.gameObject, "Lost", true), !isWon);

            // set some text based upon the result
            UIHelper.SetTextOnChildGameObject(DialogInstance.gameObject, "AchievementText", LocaliseText.Format(LocalisationBase + ".Achievement", currentLevel.Score, currentLevel.Name));

            // setup stars
            var starsGameObject = GameObjectHelper.GetChildNamedGameObject(DialogInstance.gameObject, "Stars", true);
            GameObjectHelper.SafeSetActive(starsGameObject, ShowStars);
            if (ShowStars)
            {
                Assert.IsNotNull(starsGameObject, "GameOver->ShowStars is enabled, but could not find a 'Stars' gameobject. Disable the option or fix the structure.");
                starsGameObject.SetActive(ShowStars);
                int newStarsWon = GetNewStarsWon();
                currentLevel.StarsWon |= newStarsWon;
                GameObject star1WonGameObject = GameObjectHelper.GetChildNamedGameObject(starsGameObject, "Star1", true);
                GameObject star2WonGameObject = GameObjectHelper.GetChildNamedGameObject(starsGameObject, "Star2", true);
                GameObject star3WonGameObject = GameObjectHelper.GetChildNamedGameObject(starsGameObject, "Star3", true);
                StarWon(currentLevel.StarsWon, newStarsWon, star1WonGameObject, 1);
                StarWon(currentLevel.StarsWon, newStarsWon, star2WonGameObject, 2);
                StarWon(currentLevel.StarsWon, newStarsWon, star3WonGameObject, 4);
                GameObjectHelper.SafeSetActive(
                    GameObjectHelper.GetChildNamedGameObject(starsGameObject, "StarWon", true),
                    newStarsWon != 0);
            }

            // set time
            TimeSpan difference = TimeSpan.Zero;
            var timeGameObject = GameObjectHelper.GetChildNamedGameObject(DialogInstance.gameObject, "Time", true);
            GameObjectHelper.SafeSetActive(timeGameObject, ShowTime);
            if (ShowTime)
            {
                Assert.IsTrue(LevelManager.IsActive, "Ensure that you have a LevelManager component attached to your scene.");
                Assert.IsNotNull(timeGameObject,
                    "GameOver->ShowTime is enabled, but could not find a 'Time' gameobject. Disable the option or fix the structure.");
                difference = DateTime.Now - LevelManager.Instance.StartTime;
                UIHelper.SetTextOnChildGameObject(timeGameObject, "TimeResult",
                    difference.Minutes.ToString("D2") + "." + difference.Seconds.ToString("D2"), true);
            }

            // set coins
            var coinsGameObject = GameObjectHelper.GetChildNamedGameObject(DialogInstance.gameObject, "Coins", true);
            GameObjectHelper.SafeSetActive(coinsGameObject, ShowCoins);
            if (ShowCoins)
            {
                Assert.IsNotNull(coinsGameObject,
                    "GameOver->ShowCoins is enabled, but could not find a 'Coins' gameobject. Disable the option or fix the structure.");
                UIHelper.SetTextOnChildGameObject(coinsGameObject, "CoinsResult",
                    currentLevel.Coins.ToString(), true);
            }

            // set score
            var scoreGameObject = GameObjectHelper.GetChildNamedGameObject(DialogInstance.gameObject, "Score", true);
            GameObjectHelper.SafeSetActive(scoreGameObject, ShowScore);
            if (ShowScore)
            {
                Assert.IsNotNull(scoreGameObject, "GameOver->ShowScore is enabled, but could not find a 'Score' gameobject. Disable the option or fix the structure.");
                var distanceText = LocaliseText.Format(LocalisationBase + ".ScoreResult", currentLevel.Score.ToString());
                if (currentLevel.HighScore > currentLevel.OldHighScore)
                    distanceText += "\n" + LocaliseText.Get(LocalisationBase + ".NewHighScore");
                UIHelper.SetTextOnChildGameObject(scoreGameObject, "ScoreResult", distanceText, true);
            }

            UpdateNeededCoins();

            // save game state.
            GameManager.Instance.Player.UpdatePlayerPrefs();
            currentLevel.UpdatePlayerPrefs();
            PlayerPrefs.Save();

            //show dialog
            DialogInstance.Show();

            //TODO bug - as we increase TimesPlayedForRatingPrompt on both game start (GameManager) and level finish we can miss this comparison.
            if (GameManager.Instance.TimesPlayedForRatingPrompt == TimesPlayedBeforeRatingPrompt)
            {
                GameFeedback gameFeedback = new GameFeedback();
                gameFeedback.GameFeedbackAssumeTheyLikeOptional();
            }

#if UNITY_ANALYTICS
            // record some analytics on the level played
            var values = new Dictionary<string, object>
                {
                    { "score", currentLevel.Score },
                    { "Coins", currentLevel.Coins },
                    { "time", difference },
                    { "level", currentLevel.Number }
                };
            if (ShowTime) values.Add("time", difference);   // difference is set above (only) if ShowTime is set

            Analytics.CustomEvent("GameOver", values);
#endif

            // co routine to periodic updates of display (don't need to do this every frame)
            if (!Mathf.Approximately(PeriodicUpdateDelay, 0))
                StartCoroutine(PeriodicUpdate());
        }

        void StarWon(int starsWon, int newStarsWon, GameObject starGameObject, int bitMask)
        {
            // default state
            GameObjectHelper.GetChildNamedGameObject(starGameObject, "NotWon", true).SetActive((starsWon & bitMask) != bitMask);
            GameObjectHelper.GetChildNamedGameObject(starGameObject, "Won", true).SetActive((starsWon & bitMask) == bitMask);

            // if just won then animate
            if ((newStarsWon & bitMask) == bitMask)
            {
                UnityEngine.Animation animation = starGameObject.GetComponent<UnityEngine.Animation>();
                if (animation != null)
                    animation.Play();
            }
        }

        public virtual IEnumerator PeriodicUpdate()
        {
            while (true)
            {
                UpdateNeededCoins();

                yield return new WaitForSeconds(PeriodicUpdateDelay);
            }
        }


        public virtual int GetNewStarsWon()
        {
            return 0;
        }


        public void UpdateNeededCoins()
        {
            int minimumCoins = GameManager.Instance.Levels.ExtraValueNeededToUnlock(GameManager.Instance.Player.Coins);
            if (minimumCoins == 0)
                UIHelper.SetTextOnChildGameObject(DialogInstance.gameObject, "TargetCoins", LocaliseText.Format(LocalisationBase + ".TargetCoinsGot", minimumCoins), true);
            else
                UIHelper.SetTextOnChildGameObject(DialogInstance.gameObject, "TargetCoins", LocaliseText.Format(LocalisationBase + ".TargetCoins", minimumCoins), true);
        }

        public void FacebookShare()
        {
#if FACEBOOK_SDK
            FacebookManager.Instance.PostAndLoginIfNeeded();
#endif
        }

        public void Continue()
        {
            FadeLevelManager.Instance.LoadScene(GameManager.GetIdentifierScene("Menu"));
        }

        public void Retry()
        {
            FadeLevelManager.Instance.LoadScene(GameManager.GetIdentifierScene("Game"));
        }
    }
}