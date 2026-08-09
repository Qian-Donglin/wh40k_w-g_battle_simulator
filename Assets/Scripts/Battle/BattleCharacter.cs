using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// バトル中の 1 キャラクターのランタイム状態。
/// NpcData は変更しない。戦闘状態（HP・Shock・弾薬）はこちらで管理する。
/// </summary>
public class BattleCharacter
{
    public NpcData Data { get; }
    public Vector2 WorldPosition { get; set; }
    public bool IsEngaged { get; set; }
    public bool HasActedThisTurn { get; set; }

    public int CurrentWounds { get; private set; }
    public int CurrentShock  { get; private set; }

    public bool IsAlive => CurrentWounds > 0;

    // リロードまでに残っている攻撃可能回数（上限は weapon.magazine）
    private readonly Dictionary<RangedWeaponData, int> _currentAmmo
        = new Dictionary<RangedWeaponData, int>();

    // 所持している弾薬の残数（上限は weapon.armo。-1 は無制限）
    private readonly Dictionary<RangedWeaponData, int> _reserveAmmo
        = new Dictionary<RangedWeaponData, int>();

    public BattleCharacter(NpcData data, Vector2 position)
    {
        Data          = data;
        WorldPosition = position;
        CurrentWounds = data.MaxWounds;
        CurrentShock  = 0;

        foreach (var w in data.weapons)
        {
            if (w is RangedWeaponData r)
            {
                _currentAmmo[r] = r.magazine;
                _reserveAmmo[r] = r.armo;
            }
        }
    }

    // --- 弾薬 ---
    public int GetCurrentAmmo(RangedWeaponData weapon) =>
        _currentAmmo.TryGetValue(weapon, out var v) ? v : 0;

    /// <summary>所持している弾薬の残数。-1 は無制限。</summary>
    public int GetReserveAmmo(RangedWeaponData weapon) =>
        _reserveAmmo.TryGetValue(weapon, out var v) ? v : 0;

    public void ConsumeAmmo(RangedWeaponData weapon, int amount)
    {
        if (_currentAmmo.ContainsKey(weapon))
            _currentAmmo[weapon] = Mathf.Max(0, _currentAmmo[weapon] - amount);
    }

    /// <summary>
    /// 所持弾薬（Armo）から弾倉（Magazine）へ補充する。
    /// 所持弾薬が無制限（-1）なら常に満タンまで補充して true を返す。
    /// 所持弾薬が 0 の場合は補充できず false を返す。
    /// 所持弾薬が満タン分より少ない場合は、補充できた分だけ入れて false を返す（弾倉は満タンにならない）。
    /// </summary>
    public bool ReloadAmmo(RangedWeaponData weapon)
    {
        if (!_currentAmmo.ContainsKey(weapon)) return false;

        int needed = weapon.magazine - _currentAmmo[weapon];
        if (needed <= 0) return true;

        int reserve = GetReserveAmmo(weapon);
        if (reserve < 0) // 無制限
        {
            _currentAmmo[weapon] = weapon.magazine;
            return true;
        }
        if (reserve <= 0) return false;

        int used = Mathf.Min(needed, reserve);
        _currentAmmo[weapon] += used;
        _reserveAmmo[weapon]  = reserve - used;
        return used == needed;
    }

    // --- HP / Shock ---
    public void TakeWounds(int amount)  => CurrentWounds = Mathf.Max(0,             CurrentWounds - amount);
    public void HealWounds(int amount)  => CurrentWounds = Mathf.Min(Data.MaxWounds, CurrentWounds + amount);
    public void TakeShock(int amount)   => CurrentShock  = Mathf.Min(Data.MaxShock,  CurrentShock  + amount);
    public void RecoverShock(int amount)=> CurrentShock  = Mathf.Max(0,             CurrentShock  - amount);

    // --- ステータス補正（装備・状態異常などによる基礎値への加減算） ---
    private readonly Dictionary<string, int> _statModifiers = new Dictionary<string, int>();

    public int  GetStatModifier(string key) =>
        _statModifiers.TryGetValue(key, out var v) ? v : 0;

    public void SetStatModifier(string key, int value) =>
        _statModifiers[key] = value;

    // --- マスターモード用 HP 直接編集（上限クランプなし） ---
    // EditShockHp: delta>0 = 軽傷HP増加（CurrentShock を減らす）
    public void EditShockHp(int delta)  => CurrentShock  -= delta;
    // EditWoundsHp: delta>0 = 重傷HP増加（CurrentWounds を増やす）、下限は 0
    public void EditWoundsHp(int delta) => CurrentWounds  = Mathf.Max(0, CurrentWounds + delta);
}
