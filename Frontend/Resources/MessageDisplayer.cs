using Frontend.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Frontend.Resources
{
    internal static class MessageDisplayer
    {
        public static void DisplayMessage(string message)
        {
            MessageBox.Show(message);
            SoundManager.PlaySound(SoundManager.SoundEffect.Click);
        }

        public static void DisplayError(string message)
        {
            SoundManager.PlaySound(SoundManager.SoundEffect.Error);
            MessageBox.Show(message);
            SoundManager.PlaySound(SoundManager.SoundEffect.Click);
        }
    }
}
