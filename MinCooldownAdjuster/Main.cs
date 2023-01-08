using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using R2API.Utils;

namespace MinCooldownAdjuster
{
    [BepInPlugin(ModGuid, ModName, ModVer)]
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    
    //[R2APISubmoduleDependency(nameof(ItemAPI), nameof(LanguageAPI), nameof(EliteAPI), nameof(RecalculateStatsAPI), nameof(PrefabAPI), nameof(DotAPI), nameof(LegacyResourcesAPI))]

    public class MinCooldownAdjusterPlugin : BaseUnityPlugin
    {
        public const string ModGuid = "com.Zenithrium.MinCooldownAdjuster";
        public const string ModName = "MinCooldownAdjuster";
        public const string ModVer = "1.0.2";

        //Provides a direct access to this plugin's logger for use in any of your other classes.
        public static BepInEx.Logging.ManualLogSource ModLogger;

        public static ConfigEntry<float> minCooldown;

        private void Awake()
        {
            minCooldown = Config.Bind<float>("General", "Minimum Cooldown", 0f, "Adjust the minimum cooldown between skill activations. You probably want this to be 0, but you can make it any other number if you're in to that. For reference, the base game value is .5 seconds.");

            ModLogger = Logger; //probably don't need this but maybe nice to have
            float cooldownAdjusted = Mathf.Min(0, minCooldown.Value); // feeling lazy and i don't want to allow negative cooldowns because i don't know what that'll even do
            //On.RoR2.GenericSkill.CalculateFinalRechargeInterval += ((On.RoR2.GenericSkill.orig_CalculateFinalRechargeInterval original, RoR2.GenericSkill self) => Mathf.Min(self.baseRechargeInterval, Mathf.Max(cooldownAdjusted, self.baseRechargeInterval * self.cooldownScale - self.flatCooldownReduction)));

            On.RoR2.GenericSkill.CalculateFinalRechargeInterval += (orig, self) => {
                float num = orig.Invoke(self);
                if (self) {
                    num = Mathf.Max(cooldownAdjusted, self.baseRechargeInterval * self.cooldownScale - self.flatCooldownReduction);
                }
                return num;
            };
        }
    }
    
}
