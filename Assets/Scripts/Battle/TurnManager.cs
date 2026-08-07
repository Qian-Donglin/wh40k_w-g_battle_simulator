using System;

/// <summary>
/// ターン進行ロジック。BattleState を受け取り、アクティブキャラクターを切り替える。
/// </summary>
public class TurnManager
{
    private readonly BattleState _state;

    public event Action OnTurnAdvanced;
    public event Action OnRoundAdvanced;

    public TurnManager(BattleState state) => _state = state;

    /// <summary>
    /// 次のキャラクターへ移行する。全員が行動したらラウンドを進める。
    /// 死亡キャラクター（IsAlive == false）はスキップする。
    /// </summary>
    public void AdvanceTurn()
    {
        if (_state.Characters.Count == 0) return;

        int next = (_state.ActiveIndex + 1) % _state.Characters.Count;

        // 死亡キャラクターをスキップ（無限ループ防止のために最大 1 周）
        int guard = 0;
        while (!_state.Characters[next].IsAlive && guard < _state.Characters.Count)
        {
            next = (next + 1) % _state.Characters.Count;
            guard++;
        }

        // index が 0 に戻ったらラウンド終了
        bool newRound = next == 0 && _state.ActiveIndex != 0;
        if (newRound)
        {
            _state.RoundCount++;
            OnRoundAdvanced?.Invoke();
        }

        _state.ActiveIndex = next;
        OnTurnAdvanced?.Invoke();
    }
}
