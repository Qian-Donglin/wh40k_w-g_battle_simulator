using UnityEngine;

/// <summary>
/// 近接武器のデータを保持する ScriptableObject。
/// ダメージ計算時に使用者の STR が <see cref="WeaponData.baseDamage"/> に加算される。
/// この武器で攻撃された対象は次のターン「接敵」状態となり、一部の特性を持つ遠距離武器を使用できなくなる。
/// </summary>
[CreateAssetMenu(menuName = "Weapons/Melee", fileName = "NewMeleeWeapon")]
public class MeleeWeaponData : WeaponData
{
    [Header("近接武器")]

    /// <summary>
    /// 武器の攻撃半径。キャラクターの中心を起点とした距離で表す。
    /// この範囲内の対象を攻撃できる。
    /// </summary>
    public int meleeRange;
}
