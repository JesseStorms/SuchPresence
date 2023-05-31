using Discord;
using UnityEngine;
using System;
using BepInEx.Logging;
using UnityEngine.SceneManagement;
using AS;

namespace RichPresence
{
    public class DiscordController : MonoBehaviour
    {
        public Discord.Discord discord;
        public Discord.ActivityManager activityManager;
        public Discord.Activity activity;
        public DateTime dateTime = new DateTime();
        public TimeSpan elapsed = new TimeSpan();
        private TimeSpan updateInterval = new TimeSpan(0, 0, 0, 0, 5000);
        private static SceneState sceneState = SceneState.None;
        private static string statusString = "Existing...";
        private static string detailsString = "A figment of imagination!";
        private static string smallImage = ImageKey.DefaultSmall;
        private static string bigImage = ImageKey.Default;
        private static string smallImageText = "SuchPresence Plugin by @JesseStorms";
        private static string bigImageText = "A figment of imagination!";
        private static int _money = 0;
        private static int _fame = 0;
        private static string _name = "playa";
        private static string _gamemode = "uuh";


        private void Awake()
        {
            this.discord = new Discord.Discord(1113438294311714857, 0UL);
            this.activityManager = this.discord.GetActivityManager();
            this.activity.Details = "lol testing stuff";
            this.activityManager.RegisterSteam(1293180);
        }

        private void Update()
        {
            this.elapsed += TimeSpan.FromSeconds(Time.deltaTime); 
            if (this.elapsed >= this.updateInterval) 
            {
                this.elapsed -= this.updateInterval; 
                this.UpdateState();
            }
            this.discord.RunCallbacks();
        }

        private Discord.Activity UpdateState()
        {
            this.setDetails();
            this.setState();
            this.activity = new Discord.Activity
            {
                Details = DiscordController.statusString,
                State = DiscordController.detailsString,
                Assets =
                {
                    LargeImage = DiscordController.bigImage,
                    LargeText = DiscordController.bigImageText,
                    SmallImage = DiscordController.smallImage,
                    SmallText = DiscordController.smallImageText,
                }

            };
            this.UpdateActivity(this.activity);
            return this.activity;
        }
        private void setState()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            string sceneName = activeScene.name;
            if(sceneName.ToLower().Contains("mainmenu")){
                DiscordController.sceneState = SceneState.Menu;
                DiscordController.statusString = "In the Main Menu";
                DiscordController.bigImage = ImageKey.Default;
                DiscordController.bigImageText = "Main Menu";
            }
            else if (sceneName.ToLower().Contains("studio")){
                DiscordController.sceneState = SceneState.Studio;
                DiscordController.statusString = "Painting In their Studio";
                DiscordController.bigImage = ImageKey.Studio;
                DiscordController.bigImageText = "Studio";
            }
            else if (sceneName.ToLower().Contains("gallery")){
                DiscordController.sceneState = SceneState.Gallery;
                DiscordController.statusString = "Checking the Gallery";
                DiscordController.bigImage = ImageKey.Gallery;
                DiscordController.bigImageText = "Gallery";
            }
            else{
                DiscordController.sceneState = SceneState.InGame;
                DiscordController.statusString = "Existing somewhere?";
                DiscordController.bigImage = ImageKey.Default;
                DiscordController.bigImageText = "Playing the game";
            }
            
            if (DateTime.Now.Second < 20)
            {
                DiscordController.smallImageText = "SuchPresence beta1.0.0";
                DiscordController.smallImage = ImageKey.Github;
            }
            if (DateTime.Now.Second < 40)
            {
                DiscordController.smallImageText = "SuchPresence Plugin by @JesseStorms";
                DiscordController.smallImage = ImageKey.Github;
            }
        }
        private void setDetails()
        {
            GameObject sceneManagers = GameObject.Find("SceneManagers");
            if (sceneManagers != null)
            {
                AS.PlayerStatus playerStatus = sceneManagers.GetComponent<AS.PlayerStatus>();
                if( playerStatus?.Money != null ){
                    DiscordController._money = playerStatus.Money;
                }
                if (playerStatus?.Fame != null)
                {
                    DiscordController._fame = playerStatus.Fame;
                }
                if (playerStatus?.ArtistName != null)
                {
                    DiscordController._name = playerStatus.ArtistName;
                }
                DiscordController.detailsString = "Credits: " + DiscordController._money.ToString() + " | Fame: " + DiscordController._fame.ToString();
                switch(playerStatus?.GameMode)
                {
                    case EGameMode.STORY:
                        DiscordController._gamemode = "Story Mode";
                        DiscordController.smallImageText = "Story mode";
                        DiscordController.smallImage = ImageKey.DefaultSmall;
                        break;
                    case EGameMode.FAST_FORWARD:
                        DiscordController._gamemode = "Fast Mode";
                        DiscordController.detailsString = "Fast mode";
                        DiscordController.smallImage = ImageKey.DefaultSmall;
                        break;
                    case EGameMode.CREATIVE:
                        DiscordController._gamemode = "Creative Mode";
                        DiscordController.smallImageText = "Creative mode";
                        DiscordController.detailsString = "Being extremely creative!";
                        DiscordController.smallImage = ImageKey.DefaultSmall;
                        break;
                    default:
                        DiscordController._gamemode = "Idle";
                        DiscordController.smallImageText = "Idle";
                        break;
                }
            }else{
                DiscordController.detailsString = "A figment of imagination!";
                DiscordController.bigImageText = "not initted :(";
                DiscordController.smallImageText = "SuchPresence by @JesseStorms";
            }

        }

        private void UpdateActivity(Activity activity)
        {
            Plugin.Log.LogDebug("Updating Discord RPC...");
            this.activityManager.UpdateActivity(activity, (res) =>
            {
                if (res == Discord.Result.Ok)
                {
                    Plugin.Log.LogDebug("Discord RPC updated successfully!");
                }
                else
                {
                    Plugin.Log.LogError("Discord RPC failed to update!");
                }
            });   
        }
        private void OnApplicationQuit() => this.discord.Dispose();
    }

}