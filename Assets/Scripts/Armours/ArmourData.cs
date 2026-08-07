using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 防具のデータを保持する ScriptableObject。
/// </summary>
[CreateAssetMenu(menuName = "Armours/Armour", fileName = "NewArmour")]
public class ArmourData : ScriptableObject
{
    [Header("基本情報")]
    /// <summary>防具の表示名。</summary>
    public string armourName;

    [Header("ステータス")]
    /// <summary>防具の防御力。攻撃を受けた際のダメージ軽減に使用する。</summary>
    public int armourRating;

    [Header("Trait")]
    /// <summary>
    /// この防具が持つ特性の一覧。
    /// 現段階では特性の種類が未確定のため、テキストとして保持する。
    /// </summary>
    public List<string> traits = new List<string>();
}
