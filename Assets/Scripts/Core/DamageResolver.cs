using System;
using System.Collections.Generic;

/// <summary>
/// ダメージロール：武器の基本EDに、シフト由来の追加ED（武器の基本ED値を上限にキャップ）を
/// 加えて振る。MonoBehaviour を持たない純粋な C# クラス（`docs/system_design.md` 「シフト枠」）。
/// </summary>
public static class DamageResolver
{
    /// <summary>
    /// ダメージロールを行う。
    /// EDダイス数 = 武器の基本extraDamageDice + min(シフト, 武器の基本extraDamageDice)。
    /// 各EDダイスは出目4-5で+1、6で+2のダメージを追加する（`WeaponData.extraDamageDice`と同じ変換）。
    /// </summary>
    public static DamageResult Resolve(int baseDamage, int weaponExtraDamageDice, int shift)
    {
        int shiftEd = Math.Min(shift, weaponExtraDamageDice);
        int totalEdDice = weaponExtraDamageDice + shiftEd;

        var edFaces = new int[totalEdDice];
        for (int i = 0; i < totalEdDice; i++)
            edFaces[i] = DiceRoller.RollDie();

        return new DamageResult(edFaces, baseDamage);
    }
}

/// <summary>
/// ダメージロールの結果。
/// EdFaces に振ったEDダイスの出目をそのまま記録し、BonusDamage はそこから導出する。
/// </summary>
public readonly struct DamageResult
{
    /// <summary>振ったEDダイスの出目一覧。</summary>
    public IReadOnlyList<int> EdFaces { get; }

    /// <summary>武器の基礎ダメージ。</summary>
    public int BaseDamage { get; }

    /// <summary>EDダイスから得られた追加ダメージ（出目4-5で+1、6で+2の合計）。</summary>
    public int BonusDamage { get; }

    /// <summary>BaseDamage + BonusDamage の最終ダメージ。</summary>
    public int TotalDamage => BaseDamage + BonusDamage;

    public DamageResult(IReadOnlyList<int> edFaces, int baseDamage)
    {
        EdFaces = edFaces;
        BaseDamage = baseDamage;

        int bonus = 0;
        for (int i = 0; i < edFaces.Count; i++)
            bonus += DiceRoller.IconsFromFace(edFaces[i]);
        BonusDamage = bonus;
    }
}
