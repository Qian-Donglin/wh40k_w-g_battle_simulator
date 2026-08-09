using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// StreamingAssets 内の JSON ファイルを読み込み、WeaponData / AmmoData インスタンスのリストを返す静的ローダー。
/// </summary>
public static class WeaponLoader
{
    /// <summary>
    /// Application.streamingAssetsPath 以下の指定ファイルを読み込み、武器と弾薬をまとめて返す。
    /// </summary>
    public static (List<WeaponData> weapons, List<AmmoData> ammo) LoadFromStreamingAssets(string fileName = "weapons.json")
    {
        string path = Path.Combine(Application.streamingAssetsPath, fileName);
        if (!File.Exists(path))
        {
            Debug.LogError($"[WeaponLoader] ファイルが見つかりません: {path}");
            return (new List<WeaponData>(), new List<AmmoData>());
        }

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    /// <summary>
    /// 分割 JSON ファイル (melee_weapons.json / ranged_weapons.json / ammo.json) から読み込む。
    /// </summary>
    public static (List<WeaponData> weapons, List<AmmoData> ammo) LoadFromSplitFiles()
    {
        var allWeapons = new List<WeaponData>();
        var allAmmo    = new List<AmmoData>();

        foreach (string file in new[] { "melee_weapons.json", "ranged_weapons.json" })
        {
            string path = Path.Combine(Application.streamingAssetsPath, file);
            if (!File.Exists(path))
            {
                Debug.LogWarning($"[WeaponLoader] ファイルが見つかりません: {path}");
                continue;
            }
            var (w, _) = LoadFromJson(File.ReadAllText(path));
            allWeapons.AddRange(w);
        }

        string ammoPath = Path.Combine(Application.streamingAssetsPath, "ammo.json");
        if (File.Exists(ammoPath))
        {
            var (_, a) = LoadFromJson(File.ReadAllText(ammoPath));
            allAmmo.AddRange(a);
        }
        else
        {
            Debug.LogWarning($"[WeaponLoader] ファイルが見つかりません: {ammoPath}");
        }

        return (allWeapons, allAmmo);
    }

    /// <summary>
    /// JSON 文字列から武器・弾薬リストを生成する。テストからも直接呼べる。
    /// </summary>
    public static (List<WeaponData> weapons, List<AmmoData> ammo) LoadFromJson(string json)
    {
        WeaponDatabaseJson db;
        try
        {
            db = JsonUtility.FromJson<WeaponDatabaseJson>(json);
        }
        catch (Exception e)
        {
            Debug.LogError($"[WeaponLoader] JSON パースエラー: {e.Message}");
            return (new List<WeaponData>(), new List<AmmoData>());
        }

        var weapons = ParseWeapons(db?.weapons);
        var ammo    = ParseAmmo(db?.ammo);
        return (weapons, ammo);
    }

    private static List<WeaponData> ParseWeapons(List<WeaponDto> dtos)
    {
        var result = new List<WeaponData>();
        if (dtos == null) return result;

        foreach (var dto in dtos)
        {
            WeaponData weapon = ConvertWeaponDto(dto);
            if (weapon != null)
                result.Add(weapon);
        }
        return result;
    }

    private static List<AmmoData> ParseAmmo(List<AmmoDto> dtos)
    {
        var result = new List<AmmoData>();
        if (dtos == null) return result;

        foreach (var dto in dtos)
        {
            AmmoData ammo = ConvertAmmoDto(dto);
            if (ammo != null)
                result.Add(ammo);
        }
        return result;
    }

    private static WeaponData ConvertWeaponDto(WeaponDto dto)
    {
        if (string.IsNullOrEmpty(dto.weaponName))
        {
            Debug.LogWarning("[WeaponLoader] weaponName が空の武器エントリをスキップします。");
            return null;
        }

        WeaponData weapon;
        switch (dto.type)
        {
            case "Melee":
                var melee = ScriptableObject.CreateInstance<MeleeWeaponData>();
                melee.weaponType = WeaponType.Melee;
                melee.meleeRange = dto.meleeRange;
                weapon = melee;
                break;

            case "Ranged":
                var ranged = ScriptableObject.CreateInstance<RangedWeaponData>();
                ranged.weaponType = WeaponType.Ranged;
                ranged.range    = dto.range;
                ranged.salvo    = dto.salvo;
                ranged.magazine = dto.magazine;
                ranged.armo     = dto.armo;
                weapon = ranged;
                break;

            default:
                Debug.LogWarning($"[WeaponLoader] 未知の武器タイプ '{dto.type}'（武器名: '{dto.weaponName}'）をスキップします。");
                return null;
        }

        weapon.name           = dto.weaponName;
        weapon.weaponName     = dto.weaponName;
        weapon.baseDamage     = dto.baseDamage;
        weapon.extraDamageDice = dto.extraDamageDice;
        weapon.AP             = dto.AP;

        ApplyTraits(weapon.traits, dto.traits, dto.weaponName);
        return weapon;
    }

    private static AmmoData ConvertAmmoDto(AmmoDto dto)
    {
        if (string.IsNullOrEmpty(dto.ammoName))
        {
            Debug.LogWarning("[WeaponLoader] ammoName が空の弾薬エントリをスキップします。");
            return null;
        }

        var ammo = ScriptableObject.CreateInstance<AmmoData>();
        ammo.name           = dto.ammoName;
        ammo.ammoName       = dto.ammoName;
        ammo.baseDamage     = dto.baseDamage;
        ammo.extraDamageDice = dto.extraDamageDice;
        ammo.AP             = dto.AP;
        ammo.uses           = dto.uses;

        ApplyTraits(ammo.traits, dto.traits, dto.ammoName);
        return ammo;
    }

    private static void ApplyTraits(List<TraitEntry> target, List<TraitEntryDto> source, string ownerName)
    {
        if (source == null) return;

        foreach (var t in source)
        {
            if (!Enum.TryParse(t.trait, out WeaponTrait traitEnum))
            {
                Debug.LogWarning($"[WeaponLoader] 未知の特性 '{t.trait}'（所有者: '{ownerName}'）をスキップします。");
                continue;
            }

            target.Add(new TraitEntry
            {
                trait            = traitEnum,
                rating           = t.rating,
                statusEffectName = t.statusEffectName ?? string.Empty,
            });
        }
    }
}
