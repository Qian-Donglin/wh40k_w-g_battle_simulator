using UnityEngine;

/// <summary>
/// キャラクターの Trait のうち、自動計算できない手動管理フィールドを保持する構造体。
/// 自動計算される Trait（Defense、Resilience など）は <see cref="NpcData"/> の computed property として定義する。
/// </summary>
[System.Serializable]
public struct CharacterTraits
{
    [Header("HP")]
    /// <summary>HP の現在値。戦闘中に変化する。</summary>
    public int wounds;

    /// <summary>一時的 HP の現在値。戦闘中に変化する。</summary>
    public int tempWounds;

    [Header("ショック")]
    /// <summary>ショック値の現在値。精神的な体力に相当し、戦闘中に変化する。</summary>
    public int shock;

    [Header("その他")]
    /// <summary>移動速度。種族によって異なる固定値（人間は基本 6）。</summary>
    public int speed;

    /// <summary>混沌による汚染度。初期値 0。コラプションテスト失敗などで増加する。</summary>
    public int corruption;

    /// <summary>個人の持つ資産。PC の初期値は Tier と同値。</summary>
    public int wealth;
}
