using HarmonyLib;
using NeosModLoader;
using FrooxEngine;
using BaseX;
using System;
using CloudX.Shared;

namespace Cheese4All
{
    public class Cheese4All : NeosMod
    {
        public override string Name => "Cheese4All";
        public override string Author => "eia485";
        public override string Version => "1.2.0";
        public override string Link => "https://github.com/EIA485/NeosCheese4All/";
        private static ModConfiguration config;

        static ModConfigurationKey<bool> AccurateCheese = new("AccurateCheese", "use unadulterated cheese", () => false);

        public override void OnEngineInit()
        {
            config = GetConfiguration();
            Harmony harmony = new Harmony("net.eia485.Cheese4All");
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(PointerInteractionController), "BeforeInputUpdate")]
        class Cheese4AllPatch
        {
            public static void Prefix(ref bool ____cheeseActivated, ref UnlitMaterial ____pointerMaterial, PointerInteractionController __instance)
            {
                if (__instance.World == Userspace.UserspaceWorld&&!____cheeseActivated)
                {
                    ____cheeseActivated = true;
                    ____pointerMaterial.TintColor.Value = new color(1,1,1,1);
                    //implement the ultra top secret (so secret froox doesn't even know about it) potato mode
                    if (__instance.Cloud.CurrentUser?.Id == "U-ProbablePrime")
                        ____pointerMaterial.Texture.Target = (IAssetProvider<ITexture2D>)__instance.Slot.AttachTexture(NeosAssets.Graphics.Badges.potato);
                    else//enable cheese for the masses
                        ____pointerMaterial.Texture.Target = (IAssetProvider<ITexture2D>)__instance.Slot.AttachTexture(NeosAssets.Graphics.Badges.Cheese);

                    if (!config.GetValue(AccurateCheese))
                    {
                        StaticTexture2D tex = ____pointerMaterial.Texture.Target as StaticTexture2D;
                        tex.FilterMode.Value = TextureFilterMode.Anisotropic;
                        tex.Uncompressed.Value = true;
                        tex.WrapModeU.Value = TextureWrapMode.Clamp;
                        tex.WrapModeV.Value = TextureWrapMode.Clamp;
                        tex.PowerOfTwoAlignThreshold.Value = 1;
                        tex.MipMapFilter.Value = CodeX.Filtering.Bilinear;
                    }

                }
            }
        }
    }
}