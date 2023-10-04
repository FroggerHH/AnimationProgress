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
        mod.LoadAssetBundle("dwanimations");
        greatSwordSkillConfig = mod.config("General", "GreatSwordSkill", Swords.ToString(), "");
        greatSwordSkillLevelNeededConfig = mod.config("General", "GreatSwordSkillLevelNeeded", 50, "");

        ExternalAnimations["Attack1External"] = mod.bundle.LoadAsset<AnimationClip>("Attack1");
        ExternalAnimations["Attack2External"] = mod.bundle.LoadAsset<AnimationClip>("Attack2");
        ExternalAnimations["Attack3External"] = mod.bundle.LoadAsset<AnimationClip>("Attack3");
        ExternalAnimations["Attack4External"] = mod.bundle.LoadAsset<AnimationClip>("DWspecial");
        // ExternalAnimations["Attack3External"] = asset.LoadAsset<AnimationClip>("Attack3");
        mod.bundle.Unload(false);

        Dictionary<string, string> replacement = new();
        replacementMap[ControllerType.GreatSword] = new Dictionary<string, string>
        {
            { "Greatsword BaseAttack (1)", "Attack1External" },
            { "Greatsword BaseAttack (2)", "Attack2External" },
            { "Greatsword BaseAttack (3)", "Attack3External" },
            { "Greatsword Secondary Attack", "Attack4External" }
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
        Debug($"anims in AnimatorController '{original}' is \n {aoc.animationClips.Select(x => x.name).GetString()}");
        Debug($"replacement anims is \n {replacement.Select(x => $"{x.Key}.{x.Value}").GetString()}");

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