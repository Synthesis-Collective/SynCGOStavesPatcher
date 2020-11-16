using System;
using System.Linq;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.FormKeys.SkyrimSE;

namespace SynPatcher
{
    public class Program
    {
        public static int Main(string[] args)
        {
            return SynthesisPipeline.Instance.Patch<ISkyrimMod, ISkyrimModGetter>(
                args: args,
                patcher: RunPatch,
                userPreferences: new UserPreferences()
                {
                    ActionsForEmptyArgs = new RunDefaultPatcher()
                    {
                        IdentifyingModKey = "SynCGOStaves.esp",
                        TargetRelease = GameRelease.SkyrimSE,
                        BlockAutomaticExit = true,
                    }
                });
        }

        public static void RunPatch(SynthesisState<ISkyrimMod, ISkyrimModGetter> state)
        {
            var mk = ModKey.FromNameAndExtension("DSerCombatGameplayOverhaul.esp");
            if(state.LoadOrder.ContainsKey(mk)) {
                foreach(var weap in state.LoadOrder.PriorityOrder.OnlyEnabled().Weapon().WinningOverrides()) {
                    if(weap.Keywords?.Contains(Skyrim.Keyword.WeapTypeStaff)??false) {
                        if(!weap.FormKey.ModKey.FileName.Contains("CGOStaves")) {
                            Console.WriteLine($"Patching staff {weap.Name}");
                            var newweap = state.PatchMod.Weapons.GetOrAddAsOverride(weap);
                            newweap.BlockBashImpact = Skyrim.ImpactDataSet.WPNBashBowImpactSet;
                            newweap.AlternateBlockMaterial = Skyrim.MaterialType.MaterialBlockBowsStaves;
                            newweap.ImpactDataSet = Skyrim.ImpactDataSet.WPNzBluntImpactSet;
                            newweap.AttackFailSound = Skyrim.SoundDescriptor.WPNSwing2Hand;
                        }
                    }
                }
            } else {
                Console.WriteLine("DSerCombatGameplayOverhaul.esp not found in load order, doing nothing");
            }
        }
    }
}
