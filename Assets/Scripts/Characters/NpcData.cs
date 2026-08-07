using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NPC キャラクターのデータを保持する ScriptableObject。
/// PC は本クラスを継承した <see cref="PcData"/> を使用する。
/// </summary>
[CreateAssetMenu(menuName = "Characters/NPC", fileName = "NewNpc")]
public class NpcData : ScriptableObject
{
    [Header("基本情報")]
    /// <summary>キャラクターの表示名。</summary>
    public string characterName;

    [Header("基礎ステータス")]
    /// <summary>基礎ステータス一覧。</summary>
    public CharacterStats stats;

    [Header("スキル")]
    /// <summary>スキル一覧。各スキル値と依存する基礎ステータスを合算して判定値を算出する。</summary>
    public CharacterSkills skills;

    [Header("Trait（手動）")]
    /// <summary>自動計算できない Trait の手動管理フィールド。</summary>
    public CharacterTraits traits;

    [Header("装備")]
    /// <summary>このキャラクターが装備している防具。なければ null のままにする。</summary>
    public ArmourData equippedArmour;

    /// <summary>このキャラクターが所持する武器の一覧。</summary>
    public List<WeaponData> weapons = new List<WeaponData>();

    // -----------------------------------------------------------------------
    // Trait 自動計算プロパティ
    // -----------------------------------------------------------------------

    /// <summary>攻撃に対する回避値。Initiative - 1。</summary>
    public int Defense => stats.initiative - 1;

    /// <summary>
    /// 攻撃に対する防御値。Toughness + 1 + 装備アーマーの AR。
    /// 防具を装備していない場合は Toughness + 1。
    /// </summary>
    public int Resilience => stats.toughness + 1 + (equippedArmour != null ? equippedArmour.armourRating : 0);

    /// <summary>
    /// HP ダメージを一時的 HP に転移する力。Toughness と同値。
    /// フリーアクションとして使用し、ダイスプールは Toughness 値と同数の d6 を振る。
    /// </summary>
    public int Determination => stats.toughness;

    /// <summary>HP の上限値。Toughness × 2。</summary>
    public int MaxWounds => stats.toughness * 2;

    /// <summary>一時的 HP の上限値。Toughness と同値。</summary>
    public int MaxTempWounds => stats.toughness;

    /// <summary>ショック値の上限値。Willpower と同値。</summary>
    public int MaxShock => stats.willpower;

    /// <summary>精神的ダメージを軽減する能力。Willpower と同値。コラプションテストのダイスプールに使用する。</summary>
    public int Conviction => stats.willpower;

    /// <summary>精神的な安定度。Willpower - 1。</summary>
    public int Resolve => stats.willpower - 1;

    /// <summary>社会的な権力やコネ。Fellowship - 1。</summary>
    public int Influence => stats.fellowship - 1;

    // -----------------------------------------------------------------------
    // 基本情報・ステータス編集
    // -----------------------------------------------------------------------

    /// <summary>キャラクターの表示名を変更する。</summary>
    public void SetName(string newName) => characterName = newName;

    /// <summary>基礎ステータス全体を置き換える。</summary>
    public void SetStats(CharacterStats newStats) => stats = newStats;

    /// <summary>スキル全体を置き換える。</summary>
    public void SetSkills(CharacterSkills newSkills) => skills = newSkills;

    /// <summary>手動 Trait フィールド全体を置き換える。</summary>
    public void SetTraits(CharacterTraits newTraits) => traits = newTraits;

    // -----------------------------------------------------------------------
    // 装備管理
    // -----------------------------------------------------------------------

    /// <summary>防具を装備する。既存の装備は上書きされる。</summary>
    public void EquipArmour(ArmourData armour) => equippedArmour = armour;

    /// <summary>防具を外す。</summary>
    public void UnequipArmour() => equippedArmour = null;

    /// <summary>武器を所持品に追加する。同一アセットが既に存在する場合は追加しない。</summary>
    public void AddWeapon(WeaponData weapon)
    {
        if (!weapons.Contains(weapon))
            weapons.Add(weapon);
    }

    /// <summary>指定した武器を所持品から取り除く。</summary>
    public void RemoveWeapon(WeaponData weapon) => weapons.Remove(weapon);

    /// <summary>所持武器をすべて取り除く。</summary>
    public void ClearWeapons() => weapons.Clear();

    // -----------------------------------------------------------------------
    // 戦闘状態の変更
    // -----------------------------------------------------------------------

    /// <summary>Wounds（HP）にダメージを与える。0 未満にはならない。</summary>
    public void TakeWounds(int amount)
    {
        var t = traits;
        t.wounds = Mathf.Max(0, t.wounds - amount);
        traits = t;
    }

    /// <summary>Wounds（HP）を回復する。<see cref="MaxWounds"/> を超えない。</summary>
    public void HealWounds(int amount)
    {
        var t = traits;
        t.wounds = Mathf.Min(MaxWounds, t.wounds + amount);
        traits = t;
    }

    /// <summary>TempWounds（一時的 HP）にダメージを与える。0 未満にはならない。</summary>
    public void TakeTempWounds(int amount)
    {
        var t = traits;
        t.tempWounds = Mathf.Max(0, t.tempWounds - amount);
        traits = t;
    }

    /// <summary>TempWounds（一時的 HP）を回復する。<see cref="MaxTempWounds"/> を超えない。</summary>
    public void HealTempWounds(int amount)
    {
        var t = traits;
        t.tempWounds = Mathf.Min(MaxTempWounds, t.tempWounds + amount);
        traits = t;
    }

    /// <summary>Shock 値を増加させる。<see cref="MaxShock"/> を超えない。</summary>
    public void TakeShock(int amount)
    {
        var t = traits;
        t.shock = Mathf.Min(MaxShock, t.shock + amount);
        traits = t;
    }

    /// <summary>Shock 値を回復する。0 未満にはならない。</summary>
    public void RecoverShock(int amount)
    {
        var t = traits;
        t.shock = Mathf.Max(0, t.shock - amount);
        traits = t;
    }

    /// <summary>Corruption 値を増加させる。</summary>
    public void GainCorruption(int amount)
    {
        var t = traits;
        t.corruption += amount;
        traits = t;
    }

    /// <summary>Wounds と TempWounds を上限値まで回復し、Shock を 0 にリセットする。</summary>
    public void ResetCombatState()
    {
        var t = traits;
        t.wounds = MaxWounds;
        t.tempWounds = MaxTempWounds;
        t.shock = 0;
        traits = t;
    }
}
