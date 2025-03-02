using System.Text.RegularExpressions;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using TerrariaAchievementLib.Tools;

namespace TerrariaAchievementLib.Systems
{
    /// <summary>
    /// Filters which rare creatures are detected
    /// </summary>
    public enum RareCreatureFilter
    { 
        /// <summary>
        /// Detect no creatures
        /// </summary>
        None,
        
        /// <summary>
        /// Detect only enemies
        /// </summary>
        Enemies,

        /// <summary>
        /// Detect enemies and critters
        /// </summary>
        Critters,

        /// <summary>
        /// Detect all creatures
        /// </summary>
        NPCs
    }
    
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
        /// Type of creatures to detect
        /// </summary>
        private static RareCreatureFilter _filter;

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
        /// Enables rare creature notifications
        /// </summary>
        public static void SetFilter(RareCreatureFilter filter) => _filter = filter;

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

            switch (_filter)
            {
                case RareCreatureFilter.None:
                    return;

                case RareCreatureFilter.Enemies:
                    if (npc.CountsAsACritter || npc.isLikeATownNPC)
                        return;
                    break;

                case RareCreatureFilter.Critters:
                    if (npc.isLikeATownNPC)
                        return;
                    break;
            }

            if (npc.FullName != _recentRareCreature)
            {
                LogTool.ChatLog($"Rare creature detected nearby: {npc.FullName}", sound: SoundID.ResearchComplete);
                _recentRareCreature = npc.FullName;
            }
        }
    }
}
