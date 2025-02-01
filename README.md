# TerrariaAchievementLib

Terraria tModLoader mod library to add new achievements to the in-game list.

## How to Use

1. Add this repository as a submodule to your Terraria tModLoader mod.

2. Define a class that inherits `AchievementSystem`.

3. Define the abstract `Identifier` property. The library uses this to identify achievements that are unique to your mod, which ensures there are no potential naming clashes with vanilla achievements. Example for a mod making completionist achievements: `protected override string Identifier { get => "COMPLETIONIST"; }`.

4. Define the abstract `TexturePaths` property. Achievement textures can only hold 120 achievements. Example for a mod with 121 achievements: `protected override List<string> TexturePaths { get => ["CompletionistAchievements/Assets/Achievements-1", "CompletionistAchievements/Assets/Achievements-2"]; }`.

5. Override the virtual `RegisterAchievements` function. All achievements are registered here, using conditions that are defined and handled in this library:

```
protected override void RegisterAchievements()
{
    ConditionReqs reqs = new(PlayerDiff.Classic, WorldDiff.Expert, SpecialSeed.None);

    // Add achievement for the Minecart upgrade
    string name = "MINECART_UPGRADE";
    AchCondition cond = ItemCraftCondition.Craft(reqs, ItemID.MinecartPowerup);
    RegisterAchievement(name, cond, AchievementCategory.Collector);
}
```

Alternatively, you can define your own conditions that inherit `AchCondition`.
