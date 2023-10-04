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
            var original = MakeAnimatorOverrideController(new Dictionary<string, string>(),
                __instance.m_animator.runtimeAnimatorController);
            original.name = ControllerType.Original.ToString();
            CustomRuntimeControllers[ControllerType.Original] = original;
            var greatSword = MakeAnimatorOverrideController(replacementMap[ControllerType.GreatSword],
                __instance.m_animator.runtimeAnimatorController);
            greatSword.name = ControllerType.GreatSword.ToString();
            CustomRuntimeControllers[ControllerType.GreatSword] = greatSword;
        }
    }
}