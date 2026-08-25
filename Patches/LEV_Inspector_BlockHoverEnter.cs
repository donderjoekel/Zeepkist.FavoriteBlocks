using HarmonyLib;
using JetBrains.Annotations;

namespace Zeepkist.FavoriteBlocks.Patches;

[HarmonyPatch(typeof(LEV_Inspector), nameof(LEV_Inspector.BlockHoverEnter))]
public class LEV_Inspector_BlockHoverEnter
{
    [UsedImplicitly]
    public static void Postfix(long blockID)
    {
        Plugin.HoveredBlockId = blockID;
    }
}
