using BepInEx;
using HarmonyLib;
using UnityEngine;
using ZeepSDK.LevelEditor;
using ZeepSDK.Utilities;

namespace Zeepkist.FavoriteBlocks;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, "Block Favorites", MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("ZeepSDK")]
public class Plugin : BaseUnityPlugin
{
    public static long HoveredBlockId { get; set; }
    
    private Harmony _harmony;
    private FavoritesBlocksFolder _favoritesFolder;

    private void Awake()
    {
        _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        _harmony.PatchAll();

        Settings.Initialize(this);

        // Plugin startup logic
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        LevelEditorApi.EnteredLevelEditor += OnEnteredLevelEditor;
    }

    private void OnEnteredLevelEditor()
    {
        Favorites.CacheBlockProperties();
        BlocksFolder folder = GetOrCreateFavoritesFolder();
        BlockList blockList = ComponentCache.Get<LEV_LevelEditorCentral>().inspector.globalBlockList;
        if (!blockList.globalBlocksFolder.folders.Contains(folder))
        {
            blockList.globalBlocksFolder.folders.Insert(0, folder);
        }
    }

    private BlocksFolder GetOrCreateFavoritesFolder()
    {
        if (_favoritesFolder != null)
            return _favoritesFolder;
        _favoritesFolder = transform.GetComponentInChildren<FavoritesBlocksFolder>();
        if (_favoritesFolder != null)
            return _favoritesFolder;

        BlockList blockList = ComponentCache.Get<LEV_LevelEditorCentral>().inspector.globalBlockList;
        GameObject favoritesGameObject = new("Favorites");
        favoritesGameObject.transform.SetParent(transform);
        FavoritesBlocksFolder favoritesFolder = favoritesGameObject.AddComponent<FavoritesBlocksFolder>();
        favoritesFolder.parent = blockList.globalBlocksFolder;
        blockList.globalBlocksFolder.folders.Insert(0, favoritesFolder);
        _favoritesFolder = favoritesFolder;
        return _favoritesFolder;
    }

    private void OnDestroy()
    {
        LevelEditorApi.EnteredLevelEditor -= OnEnteredLevelEditor;
        _harmony?.UnpatchSelf();
        _harmony = null;
    }
}
