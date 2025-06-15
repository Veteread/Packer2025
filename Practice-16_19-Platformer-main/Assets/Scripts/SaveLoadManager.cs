using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

[System.Serializable]
public class BlockData
{
    public int id;
    public int count;

    public void DecreaseCount(int amount = 1)
    {
    	count = Mathf.Max(0, count - amount);
    }
}

[System.Serializable]
public class BlockDataWrapper
{
    public List<BlockData> blocks;
}

public class SaveLoadManager
{
    private const string BLOCKS_FILE_NAME = "blocks.json";
    private static readonly string SAVE_FOLDER = @"C:\save\";

    public static void SaveBlocks(List<BlockData> blocks)
    {
        try
        {
            // Создаем директорию, если её нет
            if (!Directory.Exists(SAVE_FOLDER))
            {
                Directory.CreateDirectory(SAVE_FOLDER);
            }

            string path = Path.Combine(SAVE_FOLDER, BLOCKS_FILE_NAME);
            
            // Создаем обертку для корректной сериализации
            BlockDataWrapper wrapper = new BlockDataWrapper();
            wrapper.blocks = blocks.GroupBy(b => b.id)
                                 .Select(g => new BlockData 
                                 { 
                                     id = g.Key, 
                                     count = g.Sum(x => x.count) 
                                 })
                                 .ToList();

            // Сериализуем с красивым форматированием для отладки
            string json = JsonUtility.ToJson(wrapper, true);
            
            File.WriteAllText(path, json);
            Debug.Log($"Успешно сохранено в {path}\n{json}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Ошибка сохранения: {e}");
        }
    }

    public static List<BlockData> LoadBlocks()
    {
        string path = Path.Combine(SAVE_FOLDER, BLOCKS_FILE_NAME);
        
        if (!File.Exists(path))
        {
            Debug.Log("Файл сохранения не найден");
            return new List<BlockData>();
        }

        try
        {
            string json = File.ReadAllText(path);
            BlockDataWrapper wrapper = JsonUtility.FromJson<BlockDataWrapper>(json);
            
            Debug.Log($"Успешно загружено из {path}\n{json}");
            return wrapper?.blocks ?? new List<BlockData>();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Ошибка загрузки: {e}");
            return new List<BlockData>();
        }
    }

    public static void DecreaseBlockCount(int blockId, int amount = 1)
    {
    	List<BlockData> blocks = LoadBlocks();
    	BlockData block = blocks.FirstOrDefault(b => b.id == blockId);

    	if (block != null)
    	{
    		block.DecreaseCount(amount);
    		if (block.count <= 0)
    		{
    			blocks.Remove(block);
    		}
    		SaveBlocks(blocks);
    		Debug.Log("minus");
    	}    	
    }

    public static int GetBlockCount(int blockId)
    {
    	List<BlockData> blocks = LoadBlocks();
    	return blocks.Where(b => b.id == blockId).Sum(b => b.count);
    }

    public static void Save<T>(T data, string fileName)
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(path, json);
    }

    public static T Load<T>(string fileName) where T : new()
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<T>(json);
        }
        return new T();
    }
}