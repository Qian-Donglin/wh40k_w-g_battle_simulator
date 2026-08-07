using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 画面 3: タイトル画面。シミュレーターへの入口となる最初の画面。
/// BattleSimulator から Show() / Hide() で制御する。
/// </summary>
public class TitleScreen : MonoBehaviour
{
    public event Action OnSimulatorRequested;
    public event Action OnEquipmentRequested;

    private Canvas _canvas;

    // ================================================================== public

    public void Initialize()
    {
        EnsureEventSystem();
        BuildCanvas();
    }

    public void Show() => _canvas.enabled = true;
    public void Hide() => _canvas.enabled = false;

    // ================================================================== UI 構築

    private void BuildCanvas()
    {
        GameObject canvasObj = new GameObject("Title Screen Canvas");
        canvasObj.transform.SetParent(transform, false);
        _canvas = canvasObj.AddComponent<Canvas>();
        _canvas.renderMode  = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 50;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // 背景
        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(canvasObj.transform, false);
        bg.AddComponent<Image>().color = new Color(0.08f, 0.10f, 0.16f);
        RectTransform bgRt = bg.GetComponent<RectTransform>();
        bgRt.anchorMin = Vector2.zero;
        bgRt.anchorMax = Vector2.one;
        bgRt.offsetMin = Vector2.zero;
        bgRt.offsetMax = Vector2.zero;

        // タイトルテキスト
        GameObject titleObj = new GameObject("Title Text");
        titleObj.transform.SetParent(canvasObj.transform, false);
        Text titleText = titleObj.AddComponent<Text>();
        titleText.text      = "WH40k W&G\nBattle Simulator";
        titleText.font      = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleText.fontSize  = 40;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color     = new Color(0.90f, 0.82f, 0.55f);
        RectTransform titleRt = titleObj.GetComponent<RectTransform>();
        titleRt.anchorMin = new Vector2(0.2f, 0.60f);
        titleRt.anchorMax = new Vector2(0.8f, 0.85f);
        titleRt.offsetMin = Vector2.zero;
        titleRt.offsetMax = Vector2.zero;

        // 「シミュレーション開始」ボタン
        GameObject btnSim = MakeButton(canvasObj, "シミュレーション開始", 22, new Color(0.18f, 0.35f, 0.65f));
        RectTransform rtSim = btnSim.GetComponent<RectTransform>();
        rtSim.anchorMin = new Vector2(0.35f, 0.44f);
        rtSim.anchorMax = new Vector2(0.65f, 0.54f);
        rtSim.offsetMin = Vector2.zero;
        rtSim.offsetMax = Vector2.zero;
        btnSim.GetComponent<Button>().onClick.AddListener(() => OnSimulatorRequested?.Invoke());

        // 「装備データベース」ボタン
        GameObject btnEquip = MakeButton(canvasObj, "装備データベース", 22, new Color(0.28f, 0.30f, 0.40f));
        RectTransform rtEquip = btnEquip.GetComponent<RectTransform>();
        rtEquip.anchorMin = new Vector2(0.35f, 0.32f);
        rtEquip.anchorMax = new Vector2(0.65f, 0.42f);
        rtEquip.offsetMin = Vector2.zero;
        rtEquip.offsetMax = Vector2.zero;
        btnEquip.GetComponent<Button>().onClick.AddListener(() => OnEquipmentRequested?.Invoke());

        _canvas.enabled = false;
    }

    // ================================================================== ユーティリティ

    private static void EnsureEventSystem()
    {
        if (FindObjectOfType<EventSystem>() != null) return;
        GameObject es = new GameObject("Event System");
        es.AddComponent<EventSystem>();
        es.AddComponent<StandaloneInputModule>();
    }

    private static GameObject MakeButton(GameObject parent, string label, int fontSize, Color bgColor)
    {
        GameObject obj = new GameObject("Btn_" + label);
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
