using System.IO;
using Microsoft.Extensions.Logging;
using UnityEngine;

namespace EasyCards.Services;

public class SpriteLoader : ISpriteLoader
{
    public SpriteLoader(ILogger<SpriteLoader> logger)
    {
        Logger = logger;
    }

    private ILogger<SpriteLoader> Logger { get; }

    private Texture2D LoadPNGIntoTexture(string filePath)
    {
        Texture2D tex = null;

        if (File.Exists(filePath))
        {
            var fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2, TextureFormat.RGBA32, true, false);
            ImageConversion.LoadImage(tex, fileData);

            if (tex != null)
            {
                // Set FilterMode so the images don't end up blurry
                tex.filterMode = FilterMode.Point;
            }
            else
            {
                Logger.LogWarning($"Texture couldn't be loaded: {filePath}");
            }
        }

        return tex;
    }

    public Sprite LoadSprite(string filePath)
    {
        var tex = LoadPNGIntoTexture(filePath);
        return Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
    }
}