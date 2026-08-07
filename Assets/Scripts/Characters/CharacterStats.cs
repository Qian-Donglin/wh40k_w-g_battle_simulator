using UnityEngine;

/// <summary>
/// キャラクターの基礎ステータスを保持する構造体。
/// 装備などによる補正後の値をここに格納し、スキル判定や Trait 計算に使用する。
/// </summary>
[System.Serializable]
public struct CharacterStats
{
    /// <summary>力の強さ。近接武器のダメージ計算に使用する。</summary>
    public int strength;

    /// <summary>頑丈さ。ダメージ軽減や Resilience の算出に関わる。</summary>
    public int toughness;

    /// <summary>敏捷性。射撃・運転・ステルス系スキルの基準値。</summary>
    public int agility;

    /// <summary>戦闘で先手を取れるかのイニシアチブ。近接戦闘スキルの基準値。</summary>
    public int initiative;

    /// <summary>精神力。ウィルパワー系スキルおよびサイキック能力の基準値。</summary>
    public int willpower;

    /// <summary>知力。知識・技術・医療系スキルの基準値。</summary>
    public int intellect;

    /// <summary>交友。対人スキルの基準値。</summary>
    public int fellowship;
}
