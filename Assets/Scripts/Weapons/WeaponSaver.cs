using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

/// <summary>
/// 武器・弾薬データを StreamingAssets の JSON ファイルへ書き出す静的セーバー。
/// </summary>
public static class WeaponSaver
{
    public static void SaveMeleeWeapons(IEnumerable<MeleeWeaponData> weapons)
    {
        var db = new WeaponDatabaseJson
        {
            weapons = weapons.Select(w => ToDto(w)).ToList()
        };
        WriteJson("melee_weapons.json", JsonUtility.ToJson(db, true));
    }

    public static void SaveRangedWeapons(IEnumerable<RangedWeaponData> weapons)
    {
        var db = new WeaponDatabaseJson
        {
            weapons = weapons.Select(w => ToDto(w)).ToList()
        };
        WriteJson("ranged_weapons.json", JsonUtility.ToJson(db, true));
    }

    public static void SaveAmmo(IEnumerable<AmmoData> ammoList)
    {
        var db = new WeaponDatabaseJson
        {
            ammo = ammoList.Select(a => ToAmmoDto(a)).ToList()
        };
        WriteJson("ammo.json", JsonUtility.ToJson(db, true));
    }

    private static WeaponDto ToDto(WeaponData w)
    {
        var dto = new WeaponDto
        {
            type           = w is MeleeWeaponData ? "Melee" : "Ranged",
            weaponName     = w.weaponName,
            baseDamage     = w.baseDamage,
            extraDamageDice = w.extraDamageDice,
            AP             = w.AP,
            traits         = w.traits.Select(TraitToDto).ToList()
        };
        if (w is MeleeWeaponData m)  dto.meleeRange = m.meleeRange;
        if (w is RangedWeaponData r) { dto.range = r.range; dto.salvo = r.salvo; dto.magazine = r.magazine; }
        return dto;
    }

    private static AmmoDto ToAmmoDto(AmmoData a) => new AmmoDto
    {
        ammoName        = a.ammoName,
        baseDamage      = a.baseDamage,
        extraDamageDice = a.extraDamageDice,
        AP              = a.AP,
        uses            = a.uses,
        traits          = a.traits.Select(TraitToDto).ToList()
    };

    private static TraitEntryDto TraitToDto(TraitEntry t) => new TraitEntryDto
    {
        trait            = t.trait.ToString(),
        rating           = t.rating,
        statusEffectName = t.statusEffectName ?? string.Empty
    };

    private static void WriteJson(string fileName, string json)
    {
        string path = Path.Combine(Application.streamingAssetsPath, fileName);
        File.WriteAllText(path, json);
        Debug.Log($"[WeaponSaver] 保存しました: {path}");
    }
}
