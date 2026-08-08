using System.Collections.Generic;
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

    /// <summary>
    /// d6を指定個数振る。プールの1個目をWrathダイスとして扱う（`docs/rules/core_rules_ja.md` §13）。
    /// 各ダイスの出目は結果の Faces にそのまま記録される。
    /// </summary>
    public static DicePoolResult RollPoolWithWrath(int diceCount)
    {
        var faces = new int[diceCount];
        for (int i = 0; i < diceCount; i++)
            faces[i] = RollDie();
        return new DicePoolResult(faces);
    }
}

/// <summary>
/// Wrathダイスを含むダイスプール判定の結果。
/// Faces[0] が Wrathダイスの出目（プールの1個目として区別する）。
/// </summary>
public readonly struct DicePoolResult
{
    /// <summary>各ダイスの出目（1〜6）。Faces[0] がWrathダイス。</summary>
    public IReadOnlyList<int> Faces { get; }

    /// <summary>Wrathダイスの出目。</summary>
    public int WrathFace => Faces[0];

    /// <summary>合計アイコン数。</summary>
    public int Icons { get; }

    /// <summary>Wrathダイスの出目が1（Complication発生）。</summary>
    public bool Complication => WrathFace == 1;

    /// <summary>Wrathダイスの出目が6（Glory獲得）。</summary>
    public bool Glory => WrathFace == 6;

    public DicePoolResult(IReadOnlyList<int> faces)
    {
        Faces = faces;
        int icons = 0;
        for (int i = 0; i < faces.Count; i++)
            icons += DiceRoller.IconsFromFace(faces[i]);
        Icons = icons;
    }
}
