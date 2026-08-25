using BepInEx.Configuration;
using UnityEngine;

namespace Zeepkist.FavoriteBlocks;

public static class Settings
{
    private static ConfigEntry<KeyCode> _favoritesButton;
    private static ConfigEntry<string> _favorites;

    public static KeyCode FavoritesButton => _favoritesButton.Value;

    public static string Favorites
    {
        get => _favorites.Value;
        set => _favorites.Value = value;
    }

    public static void Initialize(Plugin plugin)
    {
        _favorites = plugin.Config.Bind(
            "Editor Favorites - General",
            "Favorites",
            string.Empty,
            "A comma separated list of block IDs to make favorites");

        _favoritesButton = plugin.Config.Bind(
            "Editor Favorites - Keys",
            "Toggle Favorite",
            KeyCode.F,
            "The key to toggle a favorite when hovering over a block");
    }
}
