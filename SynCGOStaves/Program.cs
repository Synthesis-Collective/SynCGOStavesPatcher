using System;
using System.Linq;
using System.Threading.Tasks;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.FormKeys.SkyrimSE;
using Noggog;


namespace SynCGOStaves
{
    internal class Program
    {
        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .AddRunnabilityCheck(Runable)
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetTypicalOpen(GameRelease.SkyrimSE, "SynCGOStaves.esp")
                .Run(args);
        }

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            state.LoadOrder.PriorityOrder.OnlyEnabled().Weapon().WinningOverrides().ForEach(weap =>
            {
                {
                    if (weap.HasKeyword(Skyrim.Keyword.WeapTypeStaff))
                    {
                        if (!weap.FormKey.ModKey.FileName.Contains("CGOStaves"))
                        {
                            Console.WriteLine($"Patching staff {weap.Name}");
                            var newweap = state.PatchMod.Weapons.GetOrAddAsOverride(weap);
                            newweap.BlockBashImpact.SetTo(Skyrim.ImpactDataSet.WPNBashBowImpactSet);
                            newweap.AlternateBlockMaterial.SetTo(Skyrim.MaterialType.MaterialBlockBowsStaves);
                            newweap.ImpactDataSet.SetTo(Skyrim.ImpactDataSet.WPNzBluntImpactSet);
                            newweap.AttackFailSound.SetTo(Skyrim.SoundDescriptor.WPNSwing2Hand);
                        }
                    }
                }
            });
        }
        public static void Runable(IRunnabilityState state)
        {
            state.LoadOrder.AssertHasMod(ModKey.FromNameAndExtension("DSerCombatGameplayOverhaul.esp"));
        }
    }
}
