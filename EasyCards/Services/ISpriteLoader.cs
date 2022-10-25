using UnityEngine;

namespace EasyCards.Services;

public interface ISpriteLoader
{
    Sprite LoadSprite(string filePath);
}