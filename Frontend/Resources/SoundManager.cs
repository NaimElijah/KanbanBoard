using System;
using System.Collections.Generic;
using System.Media;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Frontend.Utilities
{
    public static class SoundManager
    {
        public enum SoundEffect
        {
            Startup,
            Login,
            Logout,
            Welcome,
            Click,
            Error,
            Notification,
        }

        private static Dictionary<SoundEffect, SoundPlayer> sounds;

        static SoundManager()
        {
            sounds = new Dictionary<SoundEffect, SoundPlayer>();
            InitializeSounds();
        }

        private static void InitializeSounds()
        {
            AddSound(SoundEffect.Startup, @"C:\Windows\Media\Windows Shutdown.wav");
            AddSound(SoundEffect.Login, @"C:\Windows\Media\Speech On.wav");
            AddSound(SoundEffect.Logout, @"C:\Windows\Media\Speech Off.wav");
            AddSound(SoundEffect.Welcome, @"C:\Windows\Media\Windows Notify Calendar.wav");
            AddSound(SoundEffect.Error, @"C:\Windows\Media\Windows Critical Stop.wav");
            AddSound(SoundEffect.Notification, @"C:\Windows\Media\Windows Message Nudge.wav");
            AddSound(SoundEffect.Click, @"C:\Windows\Media\Windows Navigation Start.wav");
        }

        private static void AddSound(SoundEffect soundEffect, string soundPath)
        {
            try
            {
                var player = new SoundPlayer(soundPath);
                player.LoadAsync();
                sounds[soundEffect] = player;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading sound {soundEffect}: {ex.Message}");
            }
        }

        public static void PlaySound(SoundEffect soundEffect)
        {
            try
            {
                if (sounds.TryGetValue(soundEffect, out SoundPlayer player) && player.IsLoadCompleted)
                {
                    player.Play();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error playing sound {soundEffect}: {ex.Message}");
            }
        }

        public static void Dispose()
        {
            foreach (var player in sounds.Values)
            {
                player.Dispose();
            }
            sounds.Clear();
        }
    }
}