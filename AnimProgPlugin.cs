using System.IO;
using System.Reflection;
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


    internal static readonly Dictionary<string, Dictionary<string, string>> replacementMap = new();
    private static readonly Dictionary<string, int> attackMap = new();
    internal static readonly Dictionary<string, AnimationClip> ExternalAnimations = new();
    internal static readonly Dictionary<string, RuntimeAnimatorController> CustomRuntimeControllers = new();

    private void Awake()
    {
        CreateMod(this, ModName, ModAuthor, ModVersion);
        mod.LoadAssetBundle("dwanimations");

        ExternalAnimations["Attack1External"] = mod.bundle.LoadAsset<AnimationClip>("Attack1");
        ExternalAnimations["Attack2External"] = mod.bundle.LoadAsset<AnimationClip>("Attack2");
        // ExternalAnimations["Attack3External"] = asset.LoadAsset<AnimationClip>("Attack3");
        mod.bundle.Unload(false);

        attackMap["greatsword"] = 1;
        attackMap["greatsword_secondary"] = 2;

        Dictionary<string, string> replacement = new();
        foreach (var (attackToReplace, replacementAnimID) in attackMap.Select(x => (x.Key, x.Value)))
            replacement[attackToReplace] = $"Attack{replacementAnimID}External";
        replacementMap["GreatSword"] = replacement;
    }


    internal static RuntimeAnimatorController MakeAnimatorOverrideController(
        Dictionary<string, string> replacement,
        RuntimeAnimatorController original)
    {
        if (!original)
        {
            DebugError($"[AnimationProgress.MakeAnimatorOverrideController] original is null");
            return null;
        }
        AnimatorOverrideController aoc = new(original);
        List<KeyValuePair<AnimationClip, AnimationClip>> anims = new();
        foreach (var clip in aoc.animationClips)
        {
            var animName = clip.name;
            if (replacement.TryGetValue(animName, out var value))
            {
                AnimationClip newClip = Instantiate(ExternalAnimations[value]);
                newClip.name = animName;
                anims.Add(new(clip, newClip));
            } else anims.Add(new(clip, clip));
        }

        aoc.ApplyOverrides(anims);
        return aoc;
    }
}