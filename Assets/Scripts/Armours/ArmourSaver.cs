using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

/// <summary>
/// 防具データを StreamingAssets/armours.json へ書き出す静的セーバー。
/// </summary>
public static class ArmourSaver
{
    public static void SaveArmours(IEnumerable<ArmourData> armours)
    {
        var db = new ArmourDatabaseJson
        {
            armours = armours.Select(a => new ArmourDto
            {
                armourName   = a.armourName,
                armourRating = a.armourRating,
                traits       = new List<string>(a.traits)
            }).ToList()
        };
        string path = Path.Combine(Application.streamingAssetsPath, "armours.json");
        File.WriteAllText(path, JsonUtility.ToJson(db, true));
        Debug.Log($"[ArmourSaver] 保存しました: {path}");
    }
}
