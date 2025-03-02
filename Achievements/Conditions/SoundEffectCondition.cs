using System.Collections.Generic;
using Terraria;
using Terraria.Audio;

namespace TerrariaAchievementLib.Achievements.Conditions
{
    /// <summary>
    /// Helper to create a condition that listens for sounds(s) to be played
    /// </summary>
    public class SoundEffectCondition : CustomAchievementCondition
    {
        /// <summary>
        /// Base condition identifier (used for saving to achievements.dat)
        /// </summary>
        private const string CustomName = "CUSTOM_SOUND_EFFECT";


        /// <summary>
        /// True if the class has been hooked to the helper event
        /// </summary>
        private static bool _isHooked;

        /// <summary>
        /// Sounds and the conditions that are listening for them to be triggered
        /// </summary>
        private static readonly Dictionary<SoundStyle, List<SoundEffectCondition>> _listeners = [];

        /// <summary>
        /// Sounds that need to be triggered to satisfy the condition
        /// </summary>
        private readonly SoundStyle[] Sounds;

        /// <summary>
        /// Sound variant
        /// </summary>
        private int _variant;


        /// <summary>
        /// Creates a condition that listens for a sound to be played
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="variant">Sound variant</param>
        /// <param name="sound">Sound to listen for</param>
        private SoundEffectCondition(ConditionReqs reqs, int variant, SoundStyle sound) : base($"{CustomName}_{reqs.Identifier}-{sound.SoundPath}{variant}", reqs)
        {
            Sounds = [sound];
            Listen(this, variant);
        }

        /// <summary>
        /// Creates a condition that listens for any of the sounds to be played
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="variant">Sound variant</param>
        /// <param name="sounds">Sound to listen for</param>
        private SoundEffectCondition(ConditionReqs reqs, int variant, SoundStyle[] sounds) : base($"{CustomName}_{string.Join(",", variant)}", reqs)
        {
            Sounds = sounds;
            Listen(this, variant);
        }

        /// <summary>
        /// Helper to create a condition that listens for a sound to be played
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="variant">Sound variant</param>
        /// <param name="sound">Sound to listen for</param>
        /// <returns>Sound effect achievement condition</returns>
        public static CustomAchievementCondition Hear(ConditionReqs reqs, int variant, SoundStyle sound) => new SoundEffectCondition(reqs, variant, sound);

        /// <summary>
        /// Helper to create a condition that listens for any of the sounds to be played
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="variant">Sound variant</param>
        /// <param name="sounds">Sound to listen for</param>
        /// <returns>Sound effect achievement condition</returns>
        public static CustomAchievementCondition HearAny(ConditionReqs reqs, int variant, params SoundStyle[] sounds) => new SoundEffectCondition(reqs, variant, sounds);

        /// <summary>
        /// Helper to create a condition that listens for all of the sounds to be played
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="variant">Sound variant</param>
        /// <param name="sounds">Sound to listen for</param>
        /// <returns>Sound effect achievement conditions</returns>
        public static List<CustomAchievementCondition> HearAll(ConditionReqs reqs, int variant, params SoundStyle[] sounds)
        {
            List<CustomAchievementCondition> conditions = [];
            foreach (var sound in sounds)
                conditions.Add(new SoundEffectCondition(reqs, variant, sound));
            return conditions;
        }

        /// <summary>
        /// Helper to check if any conditions are listening for the sound
        /// </summary>
        private static bool IsListeningForSound(SoundStyle sound, Dictionary<SoundStyle, List<SoundEffectCondition>> listeners, out List<SoundEffectCondition> conditions) => listeners.TryGetValue(sound, out conditions);

        /// <summary>
        /// Registers a condition to listen to a list of sounds
        /// </summary>
        private static void ListenForSounds(SoundEffectCondition condition, SoundStyle[] sounds, Dictionary<SoundStyle, List<SoundEffectCondition>> listeners)
        {
            // Loop through all IDs in the condition
            foreach (SoundStyle sound in sounds)
            {
                // Create empty list of listeners for the ID if there are none
                if (!IsListeningForSound(sound, listeners, out var conditions))
                {
                    conditions = [];
                    listeners.Add(sound, conditions);
                }

                // Add the current condition to the listeners for this ID
                conditions.Add(condition);
            }
        }

        /// <summary>
        /// Hook that is called when a sound effect is played
        /// </summary>
        /// <param name="player">Player that heard the sound</param>
        /// <param name="variant">Sound effect variant</param>
        /// <param name="sound">Sound effect</param>
        private static void CustomAchievementsHelper_OnSoundEffect(Player player, int variant, SoundStyle sound)
        {
            if (!IsListeningForSound(sound, _listeners, out var conditions))
                return;

            foreach (var condition in conditions)
            {
                // Check sound variant when applicable
                if (condition._variant != 0 && variant != condition._variant)
                    continue;

                if (condition.Reqs.Pass(player))
                    condition.Complete();
            }
        }

        /// <summary>
        /// Listen for events so the condition can be completed
        /// </summary>
        /// <param name="condition">Achievement condition</param>
        /// <param name="variant">Sound variant</param>
        private static void Listen(SoundEffectCondition condition, int variant)
        {
            if (!_isHooked)
            {
                CustomAchievementsHelper.OnSoundEffect += CustomAchievementsHelper_OnSoundEffect;
                _isHooked = true;
            }

            ListenForSounds(condition, condition.Sounds, _listeners);
            condition._variant = variant;
        }
    }
}
