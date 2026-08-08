/// <summary>
/// 攻撃判定：ダイスプールを振り、対象の Defense（DN）と比較して命中とシフトを求める。
/// MonoBehaviour を持たない純粋な C# クラス（`docs/rules/core_rules_ja.md` §9, §10）。
/// </summary>
public static class AttackResolver
{
    /// <summary>
    /// d6を diceCount 個振り、合計アイコン数が defense 以上なら命中とする。
    /// 命中時、defense を超えた余剰アイコン数をシフトとして返す。
    /// </summary>
    public static AttackResult Resolve(int diceCount, int defense)
    {
        DicePoolResult roll = DiceRoller.RollPoolWithWrath(diceCount);
        bool hit = roll.Icons >= defense;
        int shift = hit ? roll.Icons - defense : 0;
        return new AttackResult(roll, hit, shift);
    }
}

/// <summary>攻撃判定の結果。</summary>
public readonly struct AttackResult
{
    /// <summary>攻撃側のダイスプール判定結果（出目・アイコン数・Wrathダイスの結果）。</summary>
    public DicePoolResult Roll { get; }

    /// <summary>命中したか（アイコン数が対象の Defense 以上）。</summary>
    public bool Hit { get; }

    /// <summary>Defense を超えた余剰アイコン数。命中していない場合は0。</summary>
    public int Shift { get; }

    public AttackResult(DicePoolResult roll, bool hit, int shift)
    {
        Roll = roll;
        Hit = hit;
        Shift = shift;
    }
}
