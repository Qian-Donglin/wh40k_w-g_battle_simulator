using UnityEngine;

/// <summary>
/// マップ上の 1 キャラクターを描画する MonoBehaviour。
/// BattleCharacter のデータを読むだけで書き換えない。
/// </summary>
public class CharacterView : MonoBehaviour
{
    public BattleCharacter Character { get; private set; }

    private SpriteRenderer _renderer;
    private Color          _baseColor;
    private bool           _isActive;

    public void Initialize(BattleCharacter character, Color color, Sprite sprite)
    {
        Character  = character;
        _baseColor = color;
        _renderer  = gameObject.AddComponent<SpriteRenderer>();
        _renderer.sprite       = sprite;
        _renderer.color        = color;
        _renderer.sortingOrder = 0;
        SyncPosition();
    }

    /// <summary>アクティブ（点滅）状態を切り替える。</summary>
    public void SetActive(bool active)
    {
        _isActive = active;
        if (!active)
            _renderer.color = _baseColor;
    }

    /// <summary>BattleCharacter の WorldPosition を GameObject 座標に反映する。</summary>
    public void SyncPosition()
    {
        transform.position = new Vector3(Character.WorldPosition.x, Character.WorldPosition.y, 0f);
    }

    private void Update()
    {
        if (!_isActive) return;
        float t     = (Mathf.Sin(Time.time * Mathf.PI) + 1f) / 2f;
        float alpha = Mathf.Lerp(0.5f, 1.0f, t);
        _renderer.color = new Color(_baseColor.r, _baseColor.g, _baseColor.b, alpha);
    }
}
