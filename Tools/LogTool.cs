using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
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
    /// Type of message to log to the mod's console/client.log
    /// </summary>
    public enum ModLogType
    {
        Info,
        Warn,
        Error,
        Fatal,
        Debug
    };


    /// <summary>
    /// Tool to display messages to the user
    /// </summary>
    public class LogTool
    {
        /// <summary>
        /// Stylized message header
        /// </summary>
        private static string _chatHeader = "";

        /// <summary>
        /// Mod to log messages for
        /// </summary>
        private static Mod _mod;


        /// <summary>
        /// Set the logger defaults
        /// </summary>
        public static void SetDefaults(Mod mod)
        {
            _mod ??= mod;

            if (string.IsNullOrEmpty(_chatHeader))
                _chatHeader = $"[c/FFF014:{_mod}][c/FFFFFF::] ";
        }

        /// <summary>
        /// Set a custom chat log header
        /// </summary>
        /// <param name="customHeader">Custom header</param>
        public static void SetCustomChatHeader(string customHeader) => _chatHeader = customHeader;

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
                    Main.NewText($"{_chatHeader} {msg}");
                    break;

                case ChatLogType.Success:
                    Main.NewText($"{_chatHeader} {msg}", Color.Green);
                    break;

                case ChatLogType.Warn:
                    Main.NewText($"{_chatHeader} {msg}", Color.Yellow);
                    break;

                case ChatLogType.Error:
                    Main.NewText($"{_chatHeader} {msg}", Color.Red);
                    break;
            }

            if (sound != default)
                SoundEngine.PlaySound(sound);
        }

        /// <summary>
        /// Log a message to the mod's console/client.log
        /// </summary>
        /// <param name="msg">Message to log</param>
        /// <param name="type">Type of message</param>
        public static void ModLog(string msg, ModLogType type = ModLogType.Info)
        {
            if (_mod == null)
                return;
            
            switch (type)
            {
                case ModLogType.Info:
                    _mod.Logger.Info(msg);
                    break;

                case ModLogType.Warn:
                    _mod.Logger.Warn(msg);
                    break;

                case ModLogType.Error:
                    _mod.Logger.Error(msg);
                    break;

                case ModLogType.Fatal:
                    _mod.Logger.Fatal(msg);
                    break;

                case ModLogType.Debug:
                    _mod.Logger.Debug(msg);
                    break;
            }
        }

        /// <summary>
        /// Log a message to Terraria's in-game chat and to the mod's console/client.log
        /// </summary>
        /// <param name="msg">Message to log</param>
        /// <param name="ctype">Type of chat message</param>
        /// <param name="csound">Sound to play with chat message</param>
        /// <param name="mtype">Type of mod message</param>
        public static void Log(string msg, ChatLogType ctype = ChatLogType.Info, SoundStyle csound = default, ModLogType mtype = ModLogType.Info)
        {
            ChatLog(msg, ctype, csound);
            ModLog(msg, mtype);
        }

        /// <summary>
        /// Display usage after typing in a chat command wrong
        /// </summary>
        /// <param name="usage">Chat command usage</param>
        public static void DisplayUsage(string usage) => ChatLog(usage, ChatLogType.Error, SoundID.PlayerHit);
    }
}
