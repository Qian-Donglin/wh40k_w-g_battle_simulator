using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲーム全体で防具データを提供する静的データベース。
/// 初回アクセス時に自動ロードし、以降はキャッシュを返す。
/// </summary>
public static class ArmourDatabase
{
    private static List<ArmourData> _armours;

    /// <summary>ロード済みの全防具データ。</summary>
    public static IReadOnlyList<ArmourData> AllArmours
    {
        get { EnsureLoaded(); return _armours; }
    }

    /// <summary>JSON ファイルを再読み込みしてキャッシュを更新する。</summary>
    public static void Reload()
    {
        _armours = ArmourLoader.LoadFromStreamingAssets();
        Debug.Log($"[ArmourDatabase] 防具 {_armours.Count} 件をロードしました。");
    }

    private static void EnsureLoaded()
    {
        if (_armours == null) Reload();
    }
}
