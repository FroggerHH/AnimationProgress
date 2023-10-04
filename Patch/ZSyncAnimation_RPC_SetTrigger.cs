namespace AnimationProgress.Patch;

[HarmonyPatch(typeof(ZSyncAnimation), nameof(ZSyncAnimation.RPC_SetTrigger))]
internal static class Patch_ZSyncAnimation_RPC_SetTrigger
{
    private static Player player;

    [HarmonyPrefix]
    private static void Prefix(ZSyncAnimation __instance, string name)
    {
        player = __instance.GetComponent<Player>();
        if (!player) return;
        if (!__instance) return;
        var rightItem = player.GetRightItem();
        if (rightItem == null) return;

        var skill = player.GetSkills().GetCustomSkill(greatSwordSkillConfig.Value);
        if (skill == null) return;
        var level = skill.m_level;
        var levelMatch = level > greatSwordSkillLevelNeededConfig.Value;
        var animationMatch = rightItem.m_shared.m_animationState == Greatsword;
        
        // Debug($"skill '{skill.LocalizeSkill()}', "
        //       + $"level is '{level}', animationMatch '{animationMatch}, "
        //       + $"levelMatch is '{levelMatch}'");
        
        if (animationMatch && levelMatch)
            FastReplaceRAC(player, ControllerType.GreatSword);
        else
            FastReplaceRAC(player, ControllerType.Original);
    }
}