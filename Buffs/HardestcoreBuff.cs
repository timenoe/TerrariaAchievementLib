using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TerrariaAchievementLib.Buffs
{
    /// <summary>
    /// Buff to apply to the player when Hardestcore is enabled
    /// </summary>
    public class HardestcoreBuff : ModBuff
    {
        // Add the mod's name to the path since this is used as a submodule
        public override string Texture => (Mod.Name + "." + GetType().Namespace + "." + Name).Replace('.', '/');

        public override void SetStaticDefaults()
        {
            // Don't save buff
            Main.buffNoSave[Type] = true;

            // Make buff duration infinite
            Main.buffNoTimeDisplay[Type] = true;
            BuffID.Sets.TimeLeftDoesNotDecrease[Type] = true;
        }

        // Ignore right-clicks to cancel
        public override bool RightClick(int buffIndex) => false;
    }
}