using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Achievements;
using Terraria.GameContent.Achievements;
using Terraria.Localization;
using TerrariaAchievementLib.Achievements;
using TerrariaAchievementLib.Systems;

namespace TerrariaAchievementLib.Tools
{
    public class AchievementTool
    {
        /// <summary>
        /// Checks if an achievement was added by a mod
        /// </summary>
        /// <param name="ach">Achievement to check</param>
        /// <returns>True if the achievement was added by a mod</returns>
        public static bool IsModdedAchievement(Achievement ach) => ach.ModAchievement != null;

        /// <summary>
        /// Get the internal name of an achievement using its localized name
        /// </summary>
        /// <param name="localizedName">Localized achievement name</param>
        /// <returns>Internal achievement name</returns>
        public static string GetInternalAchievementName(string localizedName)
        {
            Dictionary<string, Achievement> achs = (Dictionary<string, Achievement>)typeof(AchievementManager).GetField("_achievements", AchievementSystem.ReflectionFlags)?.GetValue(Main.Achievements);
            if (achs == null)
                return "";

            foreach (string name in achs.Keys)
            {
                if (string.Equals(Language.GetText("Achievements." + name + "_Name").Value, localizedName, StringComparison.OrdinalIgnoreCase))
                    return name;
            }

            return "";
        }

        /// <summary>
        /// Get missing elements for a tracked achievement using its localized name
        /// </summary>
        /// <param name="localizedName">Localized achievement name</param>
        /// <param name="result">Missing elements on success; failure otherwise</param>
        /// <returns>True on success</returns>
        public static bool GetMissingElementsLocalized(string localizedName, out string result)
        {
            string internalName = GetInternalAchievementName(localizedName);
            if (string.IsNullOrEmpty(internalName))
            {
                result = $"The localized achievement name \"{localizedName}\" is not recognized";
                return false;
            }

            return GetMissingElementsInternal(internalName, out result);
        }

        /// <summary>
        /// Get missing elements for a tracked achievement using its internal name
        /// </summary>
        /// <param name="internalName">Internal achievement name</param>
        /// <param name="result">Missing elements on success; failure otherwise</param>
        /// <returns>True on success</returns>
        public static bool GetMissingElementsInternal(string internalName, out string result)
        {
            Dictionary<string, Achievement> achs = (Dictionary<string, Achievement>)typeof(AchievementManager).GetField("_achievements", AchievementSystem.ReflectionFlags)?.GetValue(Main.Achievements);
            if (achs == null)
            {
                result = "Could not get registered achievements using reflection";
                return false;
            }

            if (!achs.TryGetValue(internalName, out Achievement ach))
            {
                result = $"The internal achievement name \"{internalName}\" is not registered";
                return false;
            }

            IAchievementTracker tracker = (IAchievementTracker)typeof(Achievement).GetField("_tracker", AchievementSystem.ReflectionFlags)?.GetValue(ach);
            if (tracker == null || tracker is not ConditionsCompletedTracker)
            {
                result = $"[a:{internalName}] is not an achievement that tracks individual elements";
                return false;
            }

            Dictionary<string, AchievementCondition> conditions = (Dictionary<string, AchievementCondition>)typeof(Achievement).GetField("_conditions", AchievementSystem.ReflectionFlags)?.GetValue(ach);
            if (conditions == null)
            {
                result = $"[a:{internalName}] has no conditions to track";
                return false;
            }

            if (ach.IsCompleted)
            {
                result = $"[a:{internalName}] is already completed";
                return true;
            }

            List<string> missingElements = [];
            foreach (AchievementCondition condition in conditions.Values)
            {
                if (condition.IsCompleted)
                    continue;

                // Only vanilla condition type that is tracked per condition
                if (condition is NPCKilledCondition)
                {
                    short[] ids = (short[])typeof(NPCKilledCondition).GetField("_npcIds", AchievementSystem.ReflectionFlags)?.GetValue(condition);
                    if (ids == null)
                        continue;

                    string npcName = "";
                    foreach (int id in ids)
                    {
                        // Get unique names for group of 'any of' IDs
                        if (string.IsNullOrEmpty(npcName))
                        {
                            npcName = Lang.GetNPCName(id).Value;
                        }
                        else
                        {
                            string newNpcName = Lang.GetNPCName(id).Value;
                            if (!string.IsNullOrEmpty(newNpcName) && !npcName.Contains(newNpcName))
                                npcName += $" & {newNpcName}";
                        }

                    }

                    // Add NPC if it exists and append IDs to the end of the element for clarity
                    if (!missingElements.Contains(npcName))
                        missingElements.Add($"{npcName} ({string.Join(",", ids)})");
                }

                // All custom conditions that are tracked per condition
                else if (condition.GetType().BaseType.Name == "AchIdCondition")
                {
                    int[] ids = (int[])condition.GetType().GetField("Ids", AchievementSystem.ReflectionFlags)?.GetValue(condition);
                    if (ids == null)
                        continue;

                    string elementName = "";
                    foreach (int id in ids)
                    {
                        // Only get name once for group of 'any of' IDs
                        if (string.IsNullOrEmpty(elementName))
                        {
                            if (AchIdCondition.BuffConditions.Contains(condition.GetType().Name))
                                elementName = Lang.GetBuffName(id);

                            else if (AchIdCondition.ItemConditions.Contains(condition.GetType().Name))
                                elementName = Lang.GetItemName(id).Value;

                            else if (AchIdCondition.NpcConditions.Contains(condition.GetType().Name))
                                elementName = Lang.GetNPCName(id).Value;
                        }
                        else
                        {
                            string newElementName = "";

                            if (AchIdCondition.BuffConditions.Contains(condition.GetType().Name))
                                newElementName = Lang.GetBuffName(id);

                            else if (AchIdCondition.ItemConditions.Contains(condition.GetType().Name))
                                newElementName = Lang.GetItemName(id).Value;

                            else if (AchIdCondition.NpcConditions.Contains(condition.GetType().Name))
                                newElementName = Lang.GetNPCName(id).Value;

                            if (!string.IsNullOrEmpty(newElementName) && !elementName.Contains(newElementName))
                                elementName += $" & {newElementName}";
                        }
                    }

                    // Add element if it exists and append IDs to the end of the element for clarity
                    if (!string.IsNullOrEmpty(elementName) && !missingElements.Contains(elementName))
                        missingElements.Add($"{elementName} ({string.Join(",", ids)})");
                }
            }

            if (missingElements.Count > 0)
                result = $"You are missing {missingElements.Count} element(s) for [a:{internalName}]: {string.Join(", ", missingElements)}";
            else
                result = $"You are not missing any elements for [a:{internalName}]";

            return true;
        }

        /// <summary>
        /// Reset progress for an achievement using its localized name
        /// </summary>
        /// <param name="localizedName">Localized achievement name</param>
        /// <param name="result">Confirmation on success; failure otherwise</param>
        /// <returns>True on success</returns>
        public static bool ResetAchievementLocalized(string localizedName, out string result)
        {
            string internalName = GetInternalAchievementName(localizedName);
            if (string.IsNullOrEmpty(internalName))
            {
                result = $"The localized achievement name \"{localizedName}\" is not recognized";
                return false;
            }

            return ResetAchievementInternal(internalName, out result);
        }

        /// <summary>
        /// Reset progress for an achievement using its internal name
        /// </summary>
        /// <param name="internalName">Internal achievement name</param>
        /// <param name="result">Confirmation on success; failure otherwise</param>
        /// <returns>True on success</returns>
        public static bool ResetAchievementInternal(string internalName, out string result)
        {
            Dictionary<string, Achievement> achs = (Dictionary<string, Achievement>)typeof(AchievementManager).GetField("_achievements", AchievementSystem.ReflectionFlags)?.GetValue(Main.Achievements);
            if (achs == null)
            {
                result = "Could not get registered achievements using reflection";
                return false;
            }

            if (!achs.TryGetValue(internalName, out Achievement value))
            {
                result = $"The internal achievement name \"{internalName}\" is not registered";
                return false;
            }

            value.ClearProgress();
            Main.Achievements.Save();
            result = $"Successfully reset local progress for [a:{internalName}]!";
            return true;
        }

        /// <summary>
        /// Unlock an achievement using its localized name
        /// </summary>
        /// <param name="localizedName">Localized achievement name</param>
        /// <param name="failure">Failure if applicable</param>
        /// <returns>True on success</returns>
        public static bool UnlockAchievementLocalized(string localizedName, out string failure)
        {
            string internalName = GetInternalAchievementName(localizedName);
            if (string.IsNullOrEmpty(internalName))
            {
                failure = $"The localized achievement name \"{localizedName}\" is not recognized";
                return false;
            }

            return UnlockAchievementInternal(internalName, out failure);
        }

        /// <summary>
        /// Unlock an achievement using its internal name
        /// </summary>
        /// <param name="internalName">Internal achievement name</param>
        /// <param name="failure">Failure if applicabl</param>
        /// <returns>True on success</returns>
        public static bool UnlockAchievementInternal(string internalName, out string failure)
        {
            failure = "";

            Dictionary<string, Achievement> achs = (Dictionary<string, Achievement>)typeof(AchievementManager).GetField("_achievements", AchievementSystem.ReflectionFlags)?.GetValue(Main.Achievements);
            if (achs == null)
            {
                failure = "Could not get registered achievements using reflection";
                return false;
            }

            if (!achs.TryGetValue(internalName, out Achievement ach))
            {
                failure = $"The internal achievement name \"{internalName}\" is not registered";
                return false;
            }

            Dictionary<string, AchievementCondition> conds = (Dictionary<string, AchievementCondition>)typeof(Achievement).GetField("_conditions", AchievementSystem.ReflectionFlags)?.GetValue(ach);
            if (conds == null)
            {
                failure = $"[a:{internalName}] has no conditions to unlock";
                return false;
            }

            if (ach.IsCompleted)
            {
                failure = $"[a:{internalName}] is already completed";
                return true;
            }

            ach.ClearProgress();
            foreach (KeyValuePair<string, AchievementCondition> cond in conds)
                cond.Value.Complete();

            Main.Achievements.Save();
            return true;
        }
    }
}
