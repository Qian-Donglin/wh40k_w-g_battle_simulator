using System;
using System.Collections.Generic;

/// <summary>
/// weapons.json のルートオブジェクト。JsonUtility はトップレベル配列を直接扱えないためラッパーを用意する。
/// </summary>
[Serializable]
public class WeaponDatabaseJson
{
    public List<WeaponDto> weapons = new List<WeaponDto>();
    public List<AmmoDto> ammo = new List<AmmoDto>();
}

/// <summary>
/// JSON 上の武器 1 エントリに対応するフラットな DTO。
/// 武器種によって使わないフィールドは 0 / 空文字のままにする。
/// </summary>
[Serializable]
public class WeaponDto
{
    /// <summary>"Melee" / "Ranged" のいずれか。</summary>
    public string type;

    public string weaponName;
    public int baseDamage;
    public int extraDamageDice;
    public int AP;

    /// <summary>Melee のみ使用。</summary>
    public int meleeRange;

    /// <summary>Ranged が使用。</summary>
    public int range;

    /// <summary>Ranged のみ使用。フルレート射撃時の弾薬消費数。</summary>
    public int salvo;

    /// <summary>Ranged のみ使用。弾倉サイズ。</summary>
    public int armo;

    public List<TraitEntryDto> traits = new List<TraitEntryDto>();
}

/// <summary>
/// JSON 上の特性 1 エントリに対応する DTO。
/// WeaponTrait 列挙型をそのまま JSON に書くと整数になるため、文字列として保持してパース時に変換する。
/// </summary>
[Serializable]
public class TraitEntryDto
{
    /// <summary>WeaponTrait 列挙型のメンバー名と一致する文字列。例: "Assault", "Blast"。</summary>
    public string trait;

    /// <summary>レーティング値を持つ特性に設定する数値。不要な場合は 0。</summary>
    public int rating;

    /// <summary>StatusEffect 特性の場合に付与する状態異常名。例: "On Fire"。それ以外は空文字。</summary>
    public string statusEffectName;
}

/// <summary>
/// JSON 上の弾薬 1 エントリに対応する DTO。
/// </summary>
[Serializable]
public class AmmoDto
{
    public string ammoName;

    /// <summary>武器の baseDamage に加算するダメージ補正値。</summary>
    public int baseDamage;

    /// <summary>武器の extraDamageDice に加算する ED 補正値。</summary>
    public int extraDamageDice;

    /// <summary>武器の AP に加算するアーマー貫通補正値。</summary>
    public int AP;

    /// <summary>使用可能回数。0 は弾数無制限。</summary>
    public int uses;

    public List<TraitEntryDto> traits = new List<TraitEntryDto>();
}
