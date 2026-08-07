using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 弾薬データを保持する ScriptableObject。
/// 遠距離武器と組み合わせて使用する。
/// ダメージ計算時に武器の baseDamage / extraDamageDice / AP と合算する。
/// 特性（Trait）は弾薬側が担い、武器本体のトレイトは遠距離戦闘では使用しない。
/// </summary>
[CreateAssetMenu(menuName = "Weapons/Ammo", fileName = "NewAmmo")]
public class AmmoData : ScriptableObject
{
    [Header("基本情報")]

    /// <summary>弾薬の表示名。</summary>
    public string ammoName;

    [Header("ダメージ補正")]

    /// <summary>武器の baseDamage に加算するダメージ補正値。</summary>
    public int baseDamage;

    /// <summary>武器の extraDamageDice に加算する ED 補正値。</summary>
    public int extraDamageDice;

    /// <summary>武器の AP に加算するアーマー貫通補正値。</summary>
    public int AP;

    [Header("使用回数")]

    /// <summary>
    /// 使用可能回数。0 は弾数無制限を表す。
    /// 制限がある弾薬（手榴弾、ミサイルなど）はここに回数を設定する。
    /// </summary>
    public int uses;

    [Header("特性")]

    /// <summary>この弾薬が持つ特性の一覧。遠距離攻撃時はこちらのトレイトが適用される。</summary>
    public List<TraitEntry> traits = new List<TraitEntry>();
}
