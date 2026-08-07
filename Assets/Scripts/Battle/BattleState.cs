using System.Collections.Generic;

/// <summary>
/// バトル全体の状態スナップショット。MonoBehaviour を持たない純粋なデータクラス。
/// </summary>
public class BattleState
{
    public List<BattleCharacter> Characters { get; } = new List<BattleCharacter>();
    public int ActiveIndex { get; set; } = 0;
    public int RoundCount  { get; set; } = 1;

    public BattleCharacter ActiveCharacter =>
        Characters.Count > 0 ? Characters[ActiveIndex] : null;

    public void AddCharacter(BattleCharacter c) => Characters.Add(c);
}
