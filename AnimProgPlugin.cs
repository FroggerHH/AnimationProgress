using BepInEx;
using BepInEx.Configuration;

namespace AnimationProgress;

[BepInPlugin(ModGuid, ModName, ModVersion)]
// ReSharper disable once IdentifierTypo
[BepInDependency("org.bepinex.plugins.dualwield", DependencyFlags.SoftDependency)]
public class AnimProgPlugin : BaseUnityPlugin
{
    internal const string
        ModAuthor = "Frogger",
        ModName = "AnimationProgress",
        ModVersion = "1.0.0",
        ModGuid = $"com.{ModAuthor}.{ModName}";

    internal static ConfigEntry<string> greatSwordSkillConfig;
    internal static ConfigEntry<int> greatSwordSkillLevelNeededConfig;


    internal static readonly Dictionary<ControllerType, Dictionary<string, string>> replacementMap = new();
    internal static readonly Dictionary<string, AnimationClip> ExternalAnimations = new();
    internal static readonly Dictionary<ControllerType, RuntimeAnimatorController> CustomRuntimeControllers = new();

    private void Awake()
    {
        CreateMod(this, ModName, ModAuthor, ModVersion);
        mod.LoadAssetBundle("animationprogress");
        greatSwordSkillConfig = mod.config("General", "GreatSwordSkill", Swords.ToString(), "");
        greatSwordSkillLevelNeededConfig = mod.config("General", "GreatSwordSkillLevelNeeded", 50, "");

        ExternalAnimations["1"] = mod.bundle.LoadAsset<AnimationClip>("1_(Longs_Attack_p_RD)");
        ExternalAnimations["2"] = mod.bundle.LoadAsset<AnimationClip>("2_(Longs_Attack_p_LD)");
        ExternalAnimations["3"] = mod.bundle.LoadAsset<AnimationClip>("3_(Longs_Attack_p_LU)");
        ExternalAnimations["special"] = mod.bundle.LoadAsset<AnimationClip>("special_(Longs_Attack_D)");
        mod.bundle.Unload(false);

        Dictionary<string, string> replacement = new();
        replacementMap[ControllerType.GreatSword] = new Dictionary<string, string>
        {
            { "Greatsword BaseAttack (1)", "1" },
            { "Greatsword BaseAttack (2)", "2" },
            { "Greatsword BaseAttack (3)", "3" },
            { "Greatsword Secondary Attack", "special" }
        };
    }


    internal static RuntimeAnimatorController MakeAnimatorOverrideController(
        Dictionary<string, string> replacement,
        RuntimeAnimatorController original)
    {
        if (!original)
        {
            DebugError("[AnimationProgress.MakeAnimatorOverrideController] original is null");
            return null;
        }

        AnimatorOverrideController aoc = new(original);
        List<KeyValuePair<AnimationClip, AnimationClip>> anims = new();

        foreach (var clip in aoc.animationClips)
        {
            var animName = clip.name;
            if (replacement.TryGetValue(animName, out var value))
            {
                var newClip = Instantiate(ExternalAnimations[value]);
                newClip.name = animName;
                anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, newClip));
            } else
            {
                anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, clip));
            }
        }

        aoc.ApplyOverrides(anims);
        return aoc;
    }


    internal static bool FastReplaceRAC(Player player, ControllerType replaceTo)
    {
        if (CustomRuntimeControllers.TryGetValue(replaceTo, out var replace))
        {
            if (player.m_animator.runtimeAnimatorController == replace) return false;

            player.m_animator.runtimeAnimatorController = replace;
            player.m_animator.Update(Time.deltaTime);
            return true;
        }

        DebugError($"[AnimationProgress.FastReplaceRAC] CustomRuntimeControllers does not contain '{replaceTo}'");
        return false;
    }
}