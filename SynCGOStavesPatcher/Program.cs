using System;
using System.Linq;
using System.Threading.Tasks;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.FormKeys.SkyrimSE;

namespace SynPatcher
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance.AddRunnabilityCheck(Runable).AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch).Run(args, new RunPreferences() {
               ActionsForEmptyArgs = new RunDefaultPatcher()
               {
                   IdentifyingModKey = "SynCGOStaves.esp",
                   TargetRelease = GameRelease.SkyrimSE
               } 
            });
        }

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
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
        }
        public static async Task Runable(IRunnabilityState state) {
            state.LoadOrder.AssertHasMod(ModKey.FromNameAndExtension("DSerCombatGameplayOverhaul.esp"));
        }
    }
}
