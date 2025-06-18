using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    private static List<Texture2D> createdTextures = new List<Texture2D>();
    private static List<Sprite> createdSprites = new List<Sprite>();

    public static Sprite CreateSprite(int width, int height, System.Action<Texture2D> fillAction)
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
        fillAction(texture);
        texture.Apply();

        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, width, height), Vector2.one * 0.5f);

        createdTextures.Add(texture);
        createdSprites.Add(sprite);

        return sprite;
    }

    public static void Cleanup()
    {
        foreach (Sprite sprite in createdSprites)
        {
            if (sprite != null) Destroy(sprite);
        }

        foreach (Texture2D texture in createdTextures)
        {
            if (texture != null) Destroy(texture);
        }

        createdSprites.Clear();
        createdTextures.Clear();
    }
}
