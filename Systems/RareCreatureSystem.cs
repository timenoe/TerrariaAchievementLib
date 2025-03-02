using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using TerrariaAchievementLib.Tools;

namespace TerrariaAchievementLib.Systems
{
    /// <summary>
    /// Used to display rare creature notifications when the Lifeform Analyzer is equipped
    /// </summary>
    public class RareCreatureSystem : ModSystem
    {
        /// <summary>
        /// Recently detected rare creature
        /// </summary>
        private static string _recentRareCreature = "";

        /// <summary>
        /// True if enemy notifications are enabled
        /// </summary>
        private static bool _enabledEnemies;

        /// <summary>
        /// True if critter notifications are enabled
        /// </summary>
        private static bool _enabledCritters;

        /// <summary>
        /// True if NPC notifications are enabled
        /// </summary>
        private static bool _enabledNpcs;

        public override void OnModLoad()
        {
            if (Main.dedServ)
                return;

            On_Main.DrawInfoAccs_AdjustInfoTextColorsForNPC += On_Main_DrawInfoAccs_AdjustInfoTextColorsForNPC;
            On_Language.GetTextValue_string += On_Language_GetTextValue_string;
            
        }

        public override void OnModUnload()
        {
            if (Main.dedServ)
                return;

            On_Main.DrawInfoAccs_AdjustInfoTextColorsForNPC -= On_Main_DrawInfoAccs_AdjustInfoTextColorsForNPC;
            On_Language.GetTextValue_string -= On_Language_GetTextValue_string;
        }

        /// <summary>
        /// Enables specific rare creature notifications
        /// </summary>
        public static void SetEnabled(bool enabledEnemies, bool enabledCritters, bool enabledNpcs)
        { 
            _enabledEnemies = enabledEnemies;
            _enabledCritters = enabledCritters;
            _enabledNpcs = enabledNpcs;
        }

        /// <summary>
        /// Detour to reset the recent rare creature
        /// </summary>
        /// <param name="orig">Original GetTextValue method</param>
        /// <param name="key">Key of the text to get</param>
        /// <returns></returns>
        private string On_Language_GetTextValue_string(On_Language.orig_GetTextValue_string orig, string key)
        {
            if (key == "GameUI.NoRareCreatures")
                _recentRareCreature = "";

            return orig.Invoke(key);
        }

        /// <summary>
        /// Detour to display when a new rare creature is detect
        /// </summary>
        /// <param name="orig">Original DrawInfoAccs_AdjustInfoTextColorsForNPC method</param>
        /// <param name="self">Main instance</param>
        /// <param name="npc">NPC that was detected</param>
        /// <param name="infoTextColor">Text color</param>
        /// <param name="infoTextShadowColor">Shadow color</param>
        private void On_Main_DrawInfoAccs_AdjustInfoTextColorsForNPC(On_Main.orig_DrawInfoAccs_AdjustInfoTextColorsForNPC orig, Main self, NPC npc, ref Microsoft.Xna.Framework.Color infoTextColor, ref Microsoft.Xna.Framework.Color infoTextShadowColor)
        {
            orig.Invoke(self, npc, ref infoTextColor, ref infoTextShadowColor);

            bool isCritter = npc.CountsAsACritter;
            bool isNpc = npc.isLikeATownNPC;
            bool isEnemy = !isCritter && !isNpc;

            if (_enabledEnemies && isEnemy || _enabledCritters && isCritter || _enabledNpcs && isNpc)
            {
                if (npc.FullName != _recentRareCreature)
                {
                    LogTool.ChatLog($"Rare creature detected nearby: {npc.FullName}", sound: SoundID.ResearchComplete);
                    _recentRareCreature = npc.FullName;
                }
            }
        }
    }
}
