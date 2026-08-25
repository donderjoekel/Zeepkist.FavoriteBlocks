using System.Collections.Generic;
using UnityEngine;
using ZeepSDK.Messaging;
using ZeepSDK.Utilities;

namespace Zeepkist.FavoriteBlocks;

public class FavoritesBlocksFolder : BlocksFolder
{
    private readonly Dictionary<int, BlockProperties> _blockProperties = new();
    private readonly List<int> _blockIds = [];

    private void Awake()
    {
        name = "Favorites";
        localizedName = "Favorites";
        useLocalizedFolderName = false;
        folderThumb = PlayerManager.Instance.youTriedMedal;
        hasParent = true;
        folderID = "1";
        
        _blockIds.Clear();

        string[] values = Settings.Favorites.Split(',');
        foreach (string favorite in values)
        {
            if (int.TryParse(favorite, out int blockId))
            {
                _blockIds.Add(blockId);
            }
        }
        
        BlockList blockList = ComponentCache.Get<LEV_LevelEditorCentral>().inspector.globalBlockList;
        
        foreach (BlockProperties block in blockList.blocks)
        {
            _blockProperties.Add(block.blockID, block);
        }
        
        AddBlocksToFolder();
    }

    private void Update()
    {
        if (Plugin.HoveredBlockId < 0)
            return;

        if (Input.GetKeyDown(Settings.FavoritesButton))
        {
            ToggleFavorite(Plugin.HoveredBlockId);
        }
    }

    public void ToggleFavorite(long blockId)
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

    private void AddFavorite(int blockId)
    {
        _blockIds.Add(blockId);
        UpdateFavorites();
        MessengerApi.LogSuccess("[Favorites] Added to favorites");
    }
    
    private void RemoveFavorite(int blockId)
    {
        _blockIds.Remove(blockId);
        UpdateFavorites();
        MessengerApi.LogSuccess("[Favorites] Removed from favorites");
    }

    private void UpdateFavorites()
    {
        _blockIds.Sort();
        Settings.Favorites = string.Join(",", _blockIds);
        
        AddBlocksToFolder();

        if (ComponentCache.Get<LEV_LevelEditorCentral>().inspector.currentBlocksFolder == this)
        {
            ComponentCache.Get<LEV_LevelEditorCentral>().inspector.GoNewFolder(this, 0);
        }
    }

    private void AddBlocksToFolder()
    {
        blocks.Clear();
        foreach (int blockId in _blockIds)
        {
            if (_blockProperties.TryGetValue(blockId, out BlockProperties block))
            {
                blocks.Add(block);
            }
        }
    }
}
