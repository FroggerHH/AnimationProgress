using System.Diagnostics.CodeAnalysis;

namespace AnimationProgress.Patch;

[HarmonyPatch]
public static class PatchPlayerStart
{
    [HarmonyPatch(typeof(Player), nameof(Player.Start))]
    [HarmonyWrapSafe] [HarmonyPostfix]
    [HarmonyAfter("org.bepinex.plugins.dualwield")]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private static void Patch(Player __instance)
    {
        if (CustomRuntimeControllers.Count == 0 && Player.m_localPlayer is not null)
        {
            var original = MakeAnimatorOverrideController(new(),
                __instance.m_animator.runtimeAnimatorController);
            original.name = "Original";
            CustomRuntimeControllers["Original"] = original;
            var greatSword = MakeAnimatorOverrideController(replacementMap["GreatSword"],
                __instance.m_animator.runtimeAnimatorController);
            greatSword.name = "GreatSword";
            CustomRuntimeControllers["GreatSword"] = greatSword;

            //ExternalAnimations["Attack3External"] = __instance.m_animator.runtimeAnimatorController.
        }
    }
}