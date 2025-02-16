using Terraria;
using Terraria.GameContent.Achievements;
using Terraria.ModLoader;
using TerrariaAchievementLib.Achievements;

namespace TerrariaAchievementLib.Systems
{
    /// <summary>
    /// Hooks vanilla events to custom ones<br/>
    /// Ensures vanilla events are properly unsubscribed to
    /// </summary>
    public class VanillaEventSystem : ModSystem
    {
        public override void OnModLoad()
        {
            if (Main.dedServ)
                return;
            
            AchievementsHelper.OnItemCraft += CustomAchievementsHelper.NotifyItemCraft;
            AchievementsHelper.OnItemPickup += CustomAchievementsHelper.NotifyItemGrab;
            AchievementsHelper.OnProgressionEvent += CustomAchievementsHelper.NotifyFlagProgression;
            AchievementsHelper.OnNPCKilled += CustomAchievementsHelper.NotifyNpcKill;
            AchievementsHelper.OnTileDestroyed += CustomAchievementsHelper.NotifyTileDestroy;
        }

        public override void OnModUnload()
        {
            if (Main.dedServ)
                return;

            AchievementsHelper.OnItemCraft -= CustomAchievementsHelper.NotifyItemCraft;
            AchievementsHelper.OnItemPickup -= CustomAchievementsHelper.NotifyItemGrab;
            AchievementsHelper.OnProgressionEvent -= CustomAchievementsHelper.NotifyFlagProgression;
            AchievementsHelper.OnNPCKilled -= CustomAchievementsHelper.NotifyNpcKill;
            AchievementsHelper.OnTileDestroyed -= CustomAchievementsHelper.NotifyTileDestroy;
        }
    }
}
