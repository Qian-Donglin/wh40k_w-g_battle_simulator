using UnityEngine;

/// <summary>
/// 遠距離武器のデータを保持する ScriptableObject。
/// <see cref="WeaponData.baseDamage"/> に STR は加算されない。
/// 射程距離に基づいて BS や攻撃相手の Defence に修正値が加わる。
/// <see cref="magazine"/>（リロードするまでに攻撃できる回数）が 0 になるとリロードが必要になる。
/// リロード時は <see cref="armo"/>（所持弾薬。武器ごとの具体的な数値は別途設定する）を消費して補充する。
/// </summary>
[CreateAssetMenu(menuName = "Weapons/Ranged", fileName = "NewRangedWeapon")]
public class RangedWeaponData : WeaponData
{
    [Header("遠距離武器")]

    /// <summary>
    /// 武器の射程距離。
    /// 射程距離に基づいて BS や攻撃相手の Defence に修正値が加わる。
    /// </summary>
    public int range;

    /// <summary>
    /// フルレート射撃（Salvo 攻撃）時の弾薬消費数。
    /// </summary>
    public int salvo;

    /// <summary>
    /// リロードするまでに攻撃できる回数の上限。
    /// 0 になるとリロードアクションが必要になる。
    /// </summary>
    public int magazine;

    /// <summary>
    /// 所持している弾薬の数（持ち物としてのストック）。
    /// リロード時にここから <see cref="magazine"/> へ補充する。
    /// -1 は無制限（所持数を管理しない）を意味する。武器ごとの具体的な数値は別途設定する。
    /// </summary>
    public int armo;
}
