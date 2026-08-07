using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// StreamingAssets/armours.json を読み込み、ArmourData インスタンスのリストを返す静的ローダー。
/// </summary>
public static class ArmourLoader
{
    public static List<ArmourData> LoadFromStreamingAssets(string fileName = "armours.json")
    {
        string path = Path.Combine(Application.streamingAssetsPath, fileName);
        if (!File.Exists(path))
        {
            Debug.LogError($"[ArmourLoader] ファイルが見つかりません: {path}");
            return new List<ArmourData>();
        }
        return LoadFromJson(File.ReadAllText(path));
    }

    public static List<ArmourData> LoadFromJson(string json)
    {
        ArmourDatabaseJson db;
        try
        {
            db = JsonUtility.FromJson<ArmourDatabaseJson>(json);
        }
        catch (Exception e)
        {
            Debug.LogError($"[ArmourLoader] JSON パースエラー: {e.Message}");
            return new List<ArmourData>();
        }

        var result = new List<ArmourData>();
        if (db?.armours == null) return result;

        foreach (var dto in db.armours)
        {
            if (string.IsNullOrEmpty(dto.armourName))
            {
                Debug.LogWarning("[ArmourLoader] armourName が空のエントリをスキップします。");
                continue;
            }
            var armour = ScriptableObject.CreateInstance<ArmourData>();
            armour.name        = dto.armourName;
            armour.armourName  = dto.armourName;
            armour.armourRating = dto.armourRating;
            armour.traits      = dto.traits ?? new List<string>();
            result.Add(armour);
        }
        return result;
    }
}
