using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// ゲーム全体で武器・弾薬データを提供する静的データベース。
/// 初回アクセス時に自動ロードし、以降はキャッシュを返す。
/// </summary>
public static class WeaponDatabase
{
    private static List<WeaponData> _weapons;
    private static List<AmmoData>   _ammo;

    /// <summary>ロード済みの全武器データ。</summary>
    public static IReadOnlyList<WeaponData> AllWeapons
    {
        get { EnsureLoaded(); return _weapons; }
    }

    /// <summary>ロード済みの全弾薬データ。</summary>
    public static IReadOnlyList<AmmoData> AllAmmo
    {
        get { EnsureLoaded(); return _ammo; }
    }

    /// <summary>名前で武器を検索する。見つからない場合は null を返す。</summary>
    public static WeaponData GetWeaponByName(string weaponName)
    {
        EnsureLoaded();
        return _weapons.FirstOrDefault(w => w.weaponName == weaponName);
    }

    /// <summary>名前で弾薬を検索する。見つからない場合は null を返す。</summary>
    public static AmmoData GetAmmoByName(string ammoName)
    {
        EnsureLoaded();
        return _ammo.FirstOrDefault(a => a.ammoName == ammoName);
    }

    /// <summary>指定した武器データ型の武器を列挙する。</summary>
    public static IEnumerable<T> GetWeaponsByType<T>() where T : WeaponData
    {
        EnsureLoaded();
        return _weapons.OfType<T>();
    }

    /// <summary>JSON ファイルを再読み込みしてキャッシュを更新する。</summary>
    public static void Reload()
    {
        var (weapons, ammo) = WeaponLoader.LoadFromSplitFiles();
        _weapons = weapons;
        _ammo    = ammo;
        Debug.Log($"[WeaponDatabase] 武器 {_weapons.Count} 件、弾薬 {_ammo.Count} 件をロードしました。");
    }

    private static void EnsureLoaded()
    {
        if (_weapons == null)
            Reload();
    }
}
