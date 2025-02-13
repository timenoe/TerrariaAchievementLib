using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace TerrariaAchievementLib.Tools
{
    /// <summary>
    /// Type of message to log to the in-game chat<br/>
    /// Info = White<br/>
    /// Success = Green<br/>
    /// Warn = Yellow<br/>
    /// Error = Red
    /// </summary>
    public enum ChatLogType
    {
        Info,
        Success,
        Warn,
        Error
    };


    /// <summary>
    /// Tool to display messages to the user
    /// </summary>
    public class MessageTool
    {
        /// <summary>
        /// Stylized message header
        /// </summary>
        private static string _msgHeader = "[c/FFF014:TerrariaAchievementLib][c/FFFFFF::] ";


        /// <summary>
        /// Set the message header to be unique to the mod
        /// </summary>
        /// <param name="mod">Mod to display in the message header</param>
        public static void SetModMsgHeader(Mod mod) => _msgHeader = $"[c/FFF014:{mod.Name}][c/FFFFFF::] ";

        /// <summary>
        /// Log a message to Terraria's in-game chat
        /// </summary>
        /// <param name="msg">Message to log</param>
        /// <param name="type">Type of message</param>
        /// <param name="sound">Sound to play with message</param>
        public static void ChatLog(string msg, ChatLogType type = ChatLogType.Info, SoundStyle sound = default)
        {
            switch (type)
            {
                case ChatLogType.Info:
                    Main.NewText($"{_msgHeader} {msg}");
                    break;

                case ChatLogType.Success:
                    Main.NewText($"{_msgHeader} {msg}", Color.Green);
                    break;

                case ChatLogType.Warn:
                    Main.NewText($"{_msgHeader} {msg}", Color.Yellow);
                    break;

                case ChatLogType.Error:
                    Main.NewText($"{_msgHeader} {msg}", Color.Red);
                    break;
            }

            if (sound != default)
                SoundEngine.PlaySound(sound);
        }
    }
}
