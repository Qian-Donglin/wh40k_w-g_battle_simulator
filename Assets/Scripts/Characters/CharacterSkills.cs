using UnityEngine;

/// <summary>
/// キャラクターのスキル値を保持する構造体。
/// 各スキルは依存する基礎ステータスと合算して最終的な判定値となる。
/// </summary>
[System.Serializable]
public struct CharacterSkills
{
    [Header("STR 依存")]
    /// <summary>運動能力。STR 依存。</summary>
    public int athletics;

    [Header("AGI 依存")]
    /// <summary>射撃能力。AGI 依存。</summary>
    public int ballisticSkill;

    /// <summary>運転能力。AGI 依存。</summary>
    public int pilot;

    /// <summary>ステルス能力。AGI 依存。</summary>
    public int stealth;

    [Header("INIT 依存")]
    /// <summary>近接戦闘能力。INIT 依存。</summary>
    public int weaponSkill;

    [Header("WILL 依存")]
    /// <summary>サイキック能力。WILL 依存。</summary>
    public int psychicMastery;

    /// <summary>過酷な環境でのサバイバル能力。WILL 依存。</summary>
    public int survival;

    [Header("INT 依存")]
    /// <summary>受動的な状況への警戒。INT 依存。</summary>
    public int awareness;

    /// <summary>調査能力。INT 依存。</summary>
    public int investigation;

    /// <summary>医療能力。INT 依存。</summary>
    public int medicae;

    /// <summary>知識。INT 依存。</summary>
    public int scholar;

    /// <summary>技術系の能力。INT 依存。</summary>
    public int tech;

    [Header("FEL 依存")]
    /// <summary>交友関係において嘘を看破する能力。FEL 依存。</summary>
    public int cunning;

    /// <summary>交友関係において嘘をつく能力。FEL 依存。</summary>
    public int deception;

    /// <summary>交友関係において相手の心の中を読む能力。FEL 依存。</summary>
    public int insight;

    /// <summary>人を説得する能力。FEL 依存。</summary>
    public int persuasion;

    /// <summary>威圧力。FEL 依存。</summary>
    public int intimidation;

    /// <summary>リーダーシップ。FEL 依存。</summary>
    public int leadership;
}
