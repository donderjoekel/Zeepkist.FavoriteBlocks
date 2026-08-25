using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using ZeepSDK.Messaging;
using ZeepSDK.Utilities;

namespace Zeepkist.FavoriteBlocks;

public static class Favorites
{
    private static readonly Dictionary<int, BlockProperties> _blockProperties = new();
    private static ConfigEntry<string> _favoritesConfig;
    private static List<int> _blockIds = [];
    
    public static IReadOnlyDictionary<int, BlockProperties> BlockProperties => _blockProperties;
    public static IReadOnlyList<int> BlockIds => _blockIds;

    public static event Action FavoritesUpdated;
    
    public static void Init(ConfigEntry<string> favorites)
    {
        _favoritesConfig = favorites;
        _blockIds.Clear();
        
        string[] values = _favoritesConfig.Value.Split(',');
        foreach (string favorite in values)
        {
            if (int.TryParse(favorite, out int blockId))
            {
                _blockIds.Add(blockId);
            }
        }
    }

    public static void CacheBlockProperties()
    {
        if (_blockProperties.Count > 0)
            return;
        
        BlockList blockList = ComponentCache.Get<LEV_LevelEditorCentral>().inspector.globalBlockList;
        
        foreach (BlockProperties block in blockList.blocks)
        {
            _blockProperties.Add(block.blockID, block);
        }
    }

    public static void ToggleFavorite(long blockId)
    {
        if (!_blockProperties.TryGetValue((int)blockId, out BlockProperties block))
        {
            MessengerApi.LogWarning("[Favorites] Block not found");
            return;
        }

        if (_blockIds.Contains(block.blockID))
        {
            RemoveFavorite(block.blockID);
        }
        else
        {
            AddFavorite(block.blockID);
        }
    }

    private static void AddFavorite(int blockId)
    {
        _blockIds.Add(blockId);
        UpdateFavorites();
        MessengerApi.LogSuccess("[Favorites] Added to favorites");
    }
    
    private static void RemoveFavorite(int blockId)
    {
        _blockIds.Remove(blockId);
        UpdateFavorites();
        MessengerApi.LogSuccess("[Favorites] Removed from favorites");
    }

    private static void UpdateFavorites()
    {
        _blockIds.Sort();
        _favoritesConfig.Value = string.Join(",", _blockIds);
        FavoritesUpdated?.Invoke();
    }
}
