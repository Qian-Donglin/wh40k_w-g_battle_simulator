using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 武器の種類を表す列挙型。
/// </summary>
public enum WeaponType
{
    /// <summary>近接武器。攻撃力に使用者の STR が加算される。</summary>
    Melee,

    /// <summary>遠距離武器。Salvo と弾倉（Armo）を持つ。STR は加算されない。トレイトは装填する弾薬が担う。</summary>
    Ranged,
}

/// <summary>
/// 全武器種共通のデータを保持する抽象 ScriptableObject。
/// 各武器種は本クラスを継承し、固有フィールドを追加する。
/// </summary>
public abstract class WeaponData : ScriptableObject
{
    [Header("基本情報")]

    /// <summary>武器の表示名。</summary>
    public string weaponName;

    /// <summary>武器の種類。</summary>
    public WeaponType weaponType;

    [Header("ダメージ")]

    /// <summary>
    /// 武器の基礎ダメージ。
    /// 近接武器の場合はここに使用者の STR を加算して最終的な基礎ダメージを算出する。
    /// 遠距離武器は STR を加算しない。弾薬のダメージと合算して使用する。
    /// </summary>
    public int baseDamage;

    /// <summary>
    /// 追加で振るダイス（ED）の数。
    /// 通常ルール: 出目 1〜3 は +0、4〜5 は +1、6 は +2 のダメージ追加。
    /// </summary>
    public int extraDamageDice;

    /// <summary>
    /// アーマー貫通値（Armour Penetration）。
    /// ダメージ計算時に対象のアーマー防御性能をこの値だけ低下させる。
    /// </summary>
    public int AP;

    [Header("特性")]

    /// <summary>この武器が持つ特性の一覧。</summary>
    public List<TraitEntry> traits = new List<TraitEntry>();
}
