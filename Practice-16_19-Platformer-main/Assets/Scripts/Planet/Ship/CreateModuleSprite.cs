using UnityEngine;

public static class ModuleSpriteCreator
{
    public static Sprite CreateWeaponSprite()
    {
        return CreateSprite(64, 64, (texture) =>
        {
            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    if (x > 20 && x < 40 && y > 10 && y < 30)
                    {
                        texture.SetPixel(x, y, Color.red);
                    }
                    else if (Mathf.Abs(x - 30) < 5 && y < 40)
                    {
                        texture.SetPixel(x, y, Color.gray);
                    }
                    else
                    {
                        texture.SetPixel(x, y, Color.clear);
                    }
                }
            }
        });
    }

    public static Sprite CreateEngineSprite()
    {
        return CreateSprite(64, 64, (texture) =>
        {
            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    if (x > 25 && x < 35 && y > 20 && y < 40)
                    {
                        texture.SetPixel(x, y, Color.blue);
                    }
                    else if (x >= 20 && x < 40 && y >= 10 && y < 20)
                    {
                        texture.SetPixel(x, y, Color.cyan);
                    }
                    else
                    {
                        texture.SetPixel(x, y, Color.clear);
                    }
                }
            }
        });
    }

    private static Sprite CreateSprite(int width, int height, System.Action<Texture2D> fillAction)
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
        fillAction(texture);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
    }
}