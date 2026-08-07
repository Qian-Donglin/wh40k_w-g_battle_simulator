using UnityEngine;

/// <summary>
/// 武器が持つ特性の種類を表す列挙型。
/// 各特性のロジックは今後ルール確定後に実装する。現段階ではデータとして保持するのみ。
/// </summary>
public enum WeaponTrait
{
    /// <summary>移動しながら射撃しても不利なし。</summary>
    Assault,

    /// <summary>Salvo 攻撃時に Blast レーティングが Salvo 値分増加する。</summary>
    Barrage,

    /// <summary>DN3 の Ballistic Skill テストで指定点を狙い、Blast 範囲内の全対象に命中する。<see cref="TraitEntry.rating"/> に範囲値を設定する。</summary>
    Blast,

    /// <summary>ED の各ダイスに +1 する。</summary>
    Brutal,

    /// <summary>命中時に DN(X) の Toughness テストを強制する。失敗で Staggered、ダメージが Strength を超えると Prone にもなる。<see cref="TraitEntry.rating"/> に DN 値を設定する。</summary>
    Concussive,

    /// <summary>アクションとして使用。範囲 50m 内の敵サイカーの能力発動を妨害・打ち消す。</summary>
    DenyTheWitch,

    /// <summary>直線範囲攻撃。カバー無視。直線上の全対象に命中する。</summary>
    Flamer,

    /// <summary>サイカーが使用するとウォープエネルギーが乗りフル性能を発揮。非サイカーが使うとダメージ -2。Warp Weapon 特性も持つ。</summary>
    Force,

    /// <summary>Brace が必要。Brace なしでは通常射撃不可。<see cref="TraitEntry.rating"/> に Heavy レーティングを設定する。</summary>
    Heavy,

    /// <summary>
    /// ダメージを与えた際に状態異常を付与する汎用特性。
    /// 状態異常の名称は <see cref="TraitEntry.statusEffectName"/> に、強度は <see cref="TraitEntry.rating"/> に設定する。
    /// </summary>
    StatusEffect,

    /// <summary>Short Range 以内の対象に対し追加 AP と追加 ED を加える。<see cref="TraitEntry.rating"/> に Melta レーティングを設定する。</summary>
    Melta,

    /// <summary>近接戦闘での Defence に +1 する。</summary>
    Parry,

    /// <summary>常時、対象の AR を減少させる装甲貫通。Invulnerable には無効。<see cref="TraitEntry.rating"/> に AP 値を設定する。</summary>
    Piercing,

    /// <summary>近接戦闘（Engaged）中でも射撃可能。</summary>
    Pistol,

    /// <summary>Free Action として使用。範囲 50m 内のウォープ現象・PSYKER キーワード持ちを感知する。</summary>
    Psyniscience,

    /// <summary>Half Range ではレーティング分のボーナスダイスを得る。使用後 Reload 必要。<see cref="TraitEntry.rating"/> にレーティングを設定する。</summary>
    RapidFire,

    /// <summary>シーンに 1 回、コンプリケーションを無視可能。修理・整備テストに +1 ボーナスダイス。</summary>
    Reliable,

    /// <summary>攻撃テストで Exalted Icon（6 の目）を Shift した際、Rending レーティング分 AP が向上する。<see cref="TraitEntry.rating"/> にレーティングを設定する。</summary>
    Rending,

    /// <summary>音を立てずに攻撃。Stealth Score を -3 する。</summary>
    Silent,

    /// <summary>Aim 時に追加ボーナスダイス +1 と Sniper 値分の追加 ED を得る。Short Range の標的に Aim/Brace なしで撃つと DN が Sniper 値分増加する。<see cref="TraitEntry.rating"/> に Sniper 値を設定する。</summary>
    Sniper,

    /// <summary>カバーによる Defence ボーナスを無視する。</summary>
    Sonic,

    /// <summary>扇状範囲への射撃が可能。</summary>
    Spread,

    /// <summary>コンプリケーションへの安定耐性を持つ。</summary>
    Steadfast,

    /// <summary>Supercharge モードで射撃可能。コンプリケーション発生時に 1d6 Mortal Wounds。命中時は +3 ED。</summary>
    Supercharge,

    /// <summary>命中時に毒状態を付与し、Poisoned 状態のペナルティ（全テスト +2 DN）を与える。</summary>
    Toxic,

    /// <summary>攻撃の DN が Unwieldy レーティング分増加する。<see cref="TraitEntry.rating"/> にレーティングを設定する。</summary>
    Unwieldy,

    /// <summary>オーク専用。オークがこの武器で攻撃する際 +1 ボーナスダイス。Wounded 状態なら追加 +1 ED。</summary>
    Waaagh,

    /// <summary>サイキックエネルギー・エイリアン技術・ケイオスの力による武器。ダエモンなど非物質的な存在にも有効。</summary>
    WarpWeapon,

    /// <summary>アーマー専用特性。装備者の STR を +X として扱ってダメージ計算を行う。<see cref="TraitEntry.rating"/> に X 値を設定する。</summary>
    Power,
}

/// <summary>
/// 武器に付与する特性の 1 エントリ。Inspector から設定する。
/// </summary>
[System.Serializable]
public struct TraitEntry
{
    /// <summary>特性の種類。</summary>
    public WeaponTrait trait;

    /// <summary>
    /// Blast(X)、Concussive(X) など、レーティング値を持つ特性に使用する数値。
    /// レーティングが不要な特性の場合は 0 を設定する。
    /// </summary>
    public int rating;

    /// <summary>
    /// <see cref="WeaponTrait.StatusEffect"/> の場合に付与する状態異常の名称。
    /// 例: "On Fire"、"Poison"。それ以外の特性では空文字のままにする。
    /// </summary>
    public string statusEffectName;
}
