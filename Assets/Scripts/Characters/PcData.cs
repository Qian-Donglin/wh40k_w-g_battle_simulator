using UnityEngine;

/// <summary>
/// PC（プレイヤーキャラクター）のデータを保持する ScriptableObject。
/// NPC との共通データは基底クラス <see cref="NpcData"/> が持つ。
/// PC 専用の成長・経験値情報を追加する。
/// </summary>
[CreateAssetMenu(menuName = "Characters/PC", fileName = "NewPc")]
public class PcData : NpcData
{
    [Header("PC 専用")]
    /// <summary>
    /// キャラクターの格。1〜5 の値をとる。
    /// NPC には存在しない概念。
    /// </summary>
    [Range(1, 5)]
    public int tier;

    /// <summary>
    /// Tier 内でのキャラクターの格。
    /// NPC には存在しない概念。
    /// </summary>
    public int rank;

    /// <summary>キャラクターが持つ経験値の上限値（総獲得経験値）。</summary>
    public int totalExp;

    /// <summary>キャラクターがすでに使用した経験値の合計。</summary>
    public int usedExp;

    /// <summary>残りの使用可能な経験値。<see cref="totalExp"/> から <see cref="usedExp"/> を引いた値。</summary>
    public int RemainingExp => totalExp - usedExp;

    // -----------------------------------------------------------------------
    // PC 専用編集メソッド
    // -----------------------------------------------------------------------

    /// <summary>Tier を変更する。1〜5 の範囲にクランプされる。</summary>
    public void SetTier(int newTier) => tier = Mathf.Clamp(newTier, 1, 5);

    /// <summary>Rank を変更する。</summary>
    public void SetRank(int newRank) => rank = newRank;

    /// <summary>
    /// 経験値を獲得する。<see cref="totalExp"/> を増やす。
    /// </summary>
    public void GainExp(int amount) => totalExp += Mathf.Max(0, amount);

    /// <summary>
    /// 経験値を消費する。<see cref="RemainingExp"/> が不足する場合は消費せず false を返す。
    /// </summary>
    /// <returns>消費に成功した場合 true。</returns>
    public bool SpendExp(int amount)
    {
        if (amount > RemainingExp) return false;
        usedExp += amount;
        return true;
    }
}
