using UnityEngine;

/// <summary>
/// Wrath &amp; Glory の d6 ダイスプール判定。MonoBehaviour を持たない純粋な C# クラス。
/// 出目4-5=アイコン1、出目6=Exalted Icon=アイコン2（`docs/rules/core_rules_ja.md` §2）。
/// </summary>
public static class DiceRoller
{
    /// <summary>d6を1個振り、出目（1〜6）を返す。</summary>
    public static int RollDie() => Random.Range(1, 7);

    /// <summary>出目をアイコン数に変換する。</summary>
    public static int IconsFromFace(int face) => face >= 6 ? 2 : (face >= 4 ? 1 : 0);

    /// <summary>d6を指定個数振り、合計アイコン数を返す。</summary>
    public static int RollPool(int diceCount)
    {
        int icons = 0;
        for (int i = 0; i < diceCount; i++)
            icons += IconsFromFace(RollDie());
        return icons;
    }
}
