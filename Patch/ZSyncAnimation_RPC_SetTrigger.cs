namespace AnimationProgress.Patch;

[HarmonyPatch(typeof(ZSyncAnimation), nameof(ZSyncAnimation.RPC_SetTrigger))]
internal static class Patch_ZSyncAnimation_RPC_SetTrigger
{
    private static Player player;

    [HarmonyPrefix]
    private static void Prefix(ZSyncAnimation __instance, string name)
    {
        if (!__instance) return;
        Debug($"Patch_ZSyncAnimation_RPC_SetTrigger, name '{name}");
        player = __instance.GetComponent<Player>();
        if (!player) return;
        Debug("ApplyAnim 0");

        var rightItem = player.GetRightItem();
        Debug($"ApplyAnim 1 rightItem '{rightItem}'");
        if (rightItem == null) return;
        Debug($"ApplyAnim 2 animationState '{rightItem.m_shared.m_animationState}'");
        if (rightItem.m_shared.m_animationState == Greatsword)
            FastReplaceRAC(player, "GreatSword");
        else FastReplaceRAC(player, "Original");
    }

    private static bool FastReplaceRAC(Player player, string toReplace)
    {
        Debug($"FastReplaceRAC 0 replaceTo '{toReplace}'");
        CustomRuntimeControllers.TryGetValue(toReplace, out var replace);
        Debug($"FastReplaceRAC 1 replace '{replace}'");
        if (!replace || player.m_animator.runtimeAnimatorController == replace) return false;
        Debug("FastReplaceRAC 2'");

        player.m_animator.runtimeAnimatorController = replace;
        player.m_animator.Update(Time.deltaTime);
        Debug("FastReplaceRAC 3'");
        return true;
    }
}