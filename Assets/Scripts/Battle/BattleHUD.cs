using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// シミュレーション画面 HUD。ターンテキストとハンバーガーメニューを管理する。
/// </summary>
public class BattleHUD : MonoBehaviour
{
    public event Action OnCharacterListRequested;
    public event Action OnTitleRequested;

    private Text       _turnText;
    private GameObject _menuPanel;
    private bool       _menuOpen;

    public void Initialize()
    {
        EnsureEventSystem();

        GameObject canvasObj = new GameObject("Battle HUD Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode  = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 0;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        CreateTurnText(canvasObj);
        CreateHamburgerButton(canvasObj);
        CreateMenuPanel(canvasObj);
        CreateBackButton(canvasObj);
    }

    public void UpdateTurnText(BattleState state)
    {
        if (_turnText == null || state == null) return;
        string name = state.ActiveCharacter?.Data.characterName ?? "なし";
        _turnText.text = $"Round {state.RoundCount}  ―  {name} のターン";
    }

    // ------------------------------------------------------------------ private

    private void CreateTurnText(GameObject canvas)
    {
        GameObject obj = new GameObject("Turn Text");
        obj.transform.SetParent(canvas.transform, false);
        _turnText = obj.AddComponent<Text>();
        _turnText.font      = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        _turnText.fontSize  = 28;
        _turnText.alignment = TextAnchor.UpperCenter;
        _turnText.color     = Color.black;

        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchorMin        = new Vector2(0f, 1f);
        rt.anchorMax        = new Vector2(1f, 1f);
        rt.pivot            = new Vector2(0.5f, 1f);
        rt.anchoredPosition = new Vector2(0f, -10f);
        rt.sizeDelta        = new Vector2(0f, 60f);
    }

    private void CreateHamburgerButton(GameObject canvas)
    {
        GameObject btn = MakeButton(canvas, "MENU", 18, new Color(0.2f, 0.2f, 0.2f));
        RectTransform rt = btn.GetComponent<RectTransform>();
        rt.anchorMin        = new Vector2(1f, 1f);
        rt.anchorMax        = new Vector2(1f, 1f);
        rt.pivot            = new Vector2(1f, 1f);
        rt.anchoredPosition = new Vector2(-12f, -12f);
        rt.sizeDelta        = new Vector2(56f, 56f);

        btn.GetComponent<Button>().onClick.AddListener(ToggleMenu);
    }

    private void CreateMenuPanel(GameObject canvas)
    {
        _menuPanel = new GameObject("Menu Panel");
        _menuPanel.transform.SetParent(canvas.transform, false);

        Image bg = _menuPanel.AddComponent<Image>();
        bg.color = new Color(0.15f, 0.15f, 0.15f, 0.95f);

        RectTransform rt = _menuPanel.GetComponent<RectTransform>();
        rt.anchorMin        = new Vector2(1f, 1f);
        rt.anchorMax        = new Vector2(1f, 1f);
        rt.pivot            = new Vector2(1f, 1f);
        rt.anchoredPosition = new Vector2(-12f, -80f);
        rt.sizeDelta        = new Vector2(220f, 60f);

        // メニュー項目: キャラクター一覧
        GameObject item = MakeButton(_menuPanel, "キャラクター一覧", 22, new Color(0.3f, 0.3f, 0.3f));
        RectTransform ir = item.GetComponent<RectTransform>();
        ir.anchorMin        = Vector2.zero;
        ir.anchorMax        = Vector2.one;
        ir.offsetMin        = new Vector2(4f, 4f);
        ir.offsetMax        = new Vector2(-4f, -4f);
        item.GetComponent<Button>().onClick.AddListener(() =>
        {
            _menuOpen = false;
            _menuPanel.SetActive(false);
            OnCharacterListRequested?.Invoke();
        });

        _menuPanel.SetActive(false);
    }

    private void CreateBackButton(GameObject canvas)
    {
        GameObject btn = MakeButton(canvas, "< タイトル", 16, new Color(0.2f, 0.2f, 0.2f));
        RectTransform rt = btn.GetComponent<RectTransform>();
        rt.anchorMin        = new Vector2(0f, 1f);
        rt.anchorMax        = new Vector2(0f, 1f);
        rt.pivot            = new Vector2(0f, 1f);
        rt.anchoredPosition = new Vector2(12f, -12f);
        rt.sizeDelta        = new Vector2(120f, 40f);
        btn.GetComponent<Button>().onClick.AddListener(() => OnTitleRequested?.Invoke());
    }

    private void ToggleMenu()
    {
        _menuOpen = !_menuOpen;
        _menuPanel.SetActive(_menuOpen);
    }

    // ---- EventSystem ----

    private static void EnsureEventSystem()
    {
        if (FindObjectOfType<EventSystem>() != null) return;
        GameObject es = new GameObject("Event System");
        es.AddComponent<EventSystem>();
        es.AddComponent<StandaloneInputModule>();
    }

    // ---- ユーティリティ ----

    private static GameObject MakeButton(GameObject parent, string label, int fontSize, Color bgColor)
    {
        GameObject obj = new GameObject(label);
        obj.transform.SetParent(parent.transform, false);

        Image img = obj.AddComponent<Image>();
        img.color = bgColor;

        Button btn = obj.AddComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.highlightedColor = bgColor * 1.3f;
        btn.colors = cb;

        GameObject textObj = new GameObject("Label");
        textObj.transform.SetParent(obj.transform, false);
        Text txt = textObj.AddComponent<Text>();
        txt.text      = label;
        txt.font      = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.fontSize  = fontSize;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color     = Color.white;

        RectTransform tr = textObj.GetComponent<RectTransform>();
        tr.anchorMin = Vector2.zero;
        tr.anchorMax = Vector2.one;
        tr.offsetMin = Vector2.zero;
        tr.offsetMax = Vector2.zero;

        return obj;
    }
}
