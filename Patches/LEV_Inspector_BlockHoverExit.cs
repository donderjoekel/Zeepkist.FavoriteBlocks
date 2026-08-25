using HarmonyLib;
using JetBrains.Annotations;

namespace Zeepkist.FavoriteBlocks.Patches;

[HarmonyPatch(typeof(LEV_Inspector), nameof(LEV_Inspector.BlockHoverExit))]
public class LEV_Inspector_BlockHoverExit
{
    [UsedImplicitly]
    public static void Postfix()
    {
        Plugin.HoveredBlockId = -1;
    }
}
