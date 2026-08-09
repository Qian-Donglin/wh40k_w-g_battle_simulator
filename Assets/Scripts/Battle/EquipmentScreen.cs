using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 画面 4: 装備データベース画面。武器・弾薬を一覧検索する。
/// BattleSimulator から Show() / Hide() で制御する。
/// </summary>
public class EquipmentScreen : MonoBehaviour
{
    public event Action OnClosed;

    private Canvas     _canvas;
    private ScrollRect _scrollRect;
    private GameObject _scrollContent;
    private bool       _listBuilt;

    private EquipmentEditDialog _editDialog;

    private struct Section
    {
        public GameObject                        Header;
        public List<(GameObject Row, string Key)> Rows;
    }
    private readonly List<Section> _sections = new List<Section>();

    // ---- 列幅・行高定数 ----
    private const float ColNameWidth = 120f;  // 名前列のみ固定幅。他列は均等 flex。
    private const float RowHeight    = 48f;

    // ================================================================== public

    public void Initialize()
    {
        BuildCanvas();
        _editDialog = gameObject.AddComponent<EquipmentEditDialog>();
        _editDialog.Initialize();
        _editDialog.OnChanged += RebuildList;
    }

    public void Show()
    {
        _canvas.enabled = true;
        if (!_listBuilt)
        {
            BuildAllSections();
            _listBuilt = true;
        }
        StartCoroutine(ScrollToTop());
    }

    private System.Collections.IEnumerator ScrollToTop()
    {
        // ContentSizeFitter の高さ計算が確定するまで 1 フレーム待つ
        yield return null;
        _scrollRect.verticalNormalizedPosition = 1f;
    }

    private void RebuildList()
    {
        // 既存の行をすべて破棄して再構築する
        foreach (Transform child in _scrollContent.transform)
            Destroy(child.gameObject);
        _sections.Clear();
        _listBuilt = false;

        BuildAllSections();
        _listBuilt = true;
        StartCoroutine(ScrollToTop());
    }

    public void Hide()
    {
        _canvas.enabled = false;
        OnClosed?.Invoke();
    }

    // ================================================================== UI 構築

    private void BuildCanvas()
    {
        GameObject canvasObj = new GameObject("Equipment Screen Canvas");
        canvasObj.transform.SetParent(transform, false);
        _canvas = canvasObj.AddComponent<Canvas>();
        _canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 40;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // 背景
        GameObject bg = MakePanel(canvasObj, "Background", new Color(0.10f, 0.12f, 0.18f, 1.0f));
        StretchFull(bg.GetComponent<RectTransform>());

        BuildHeader(canvasObj);

        InputField searchField = BuildSearchBar(canvasObj);

        BuildColumnHeader(canvasObj);

        _scrollContent = BuildScrollArea(canvasObj);

        searchField.onValueChanged.AddListener(ApplyFilter);

        _canvas.enabled = false;
    }

    private void BuildHeader(GameObject canvas)
    {
        GameObject header = MakePanel(canvas, "Header", new Color(0.08f, 0.10f, 0.16f));
        RectTransform rt = header.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 0.92f);
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        MakeText(header, "装備データベース", 26, TextAnchor.MiddleCenter,
                 new Vector2(0.1f, 0f), new Vector2(0.9f, 1f));

        GameObject closeBtn = MakeButton(header, "X", 18, new Color(0.60f, 0.15f, 0.15f));
        RectTransform crt = closeBtn.GetComponent<RectTransform>();
        crt.anchorMin        = new Vector2(1f, 0.1f);
        crt.anchorMax        = new Vector2(1f, 0.9f);
        crt.pivot            = new Vector2(1f, 0.5f);
        crt.anchoredPosition = new Vector2(-10f, 0f);
        crt.sizeDelta        = new Vector2(50f, 0f);
        closeBtn.GetComponent<Button>().onClick.AddListener(Hide);
    }

    private InputField BuildSearchBar(GameObject canvas)
    {
        GameObject container = MakePanel(canvas, "Search Bar", new Color(0.15f, 0.18f, 0.26f));
        RectTransform crt = container.GetComponent<RectTransform>();
        crt.anchorMin = new Vector2(0f, 0.86f);
        crt.anchorMax = new Vector2(1f, 0.92f);
        crt.offsetMin = new Vector2(12f, 2f);
        crt.offsetMax = new Vector2(-12f, -2f);

        InputField inputField = container.AddComponent<InputField>();

        // Placeholder
        GameObject phObj = new GameObject("Placeholder");
        phObj.transform.SetParent(container.transform, false);
        Text placeholder = phObj.AddComponent<Text>();
        placeholder.text      = "検索...";
        placeholder.font      = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        placeholder.fontSize  = 18;
        placeholder.color     = new Color(0.5f, 0.5f, 0.5f);
        placeholder.fontStyle = FontStyle.Italic;
        placeholder.alignment = TextAnchor.MiddleLeft;
        RectTransform phRt = phObj.GetComponent<RectTransform>();
        phRt.anchorMin = Vector2.zero;
        phRt.anchorMax = Vector2.one;
        phRt.offsetMin = new Vector2(10f, 0f);
        phRt.offsetMax = new Vector2(-10f, 0f);

        // 入力テキスト
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(container.transform, false);
        Text inputText = textObj.AddComponent<Text>();
        inputText.font               = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        inputText.fontSize           = 18;
        inputText.color              = Color.white;
        inputText.alignment          = TextAnchor.MiddleLeft;
        inputText.supportRichText    = false;
        inputText.horizontalOverflow = HorizontalWrapMode.Overflow;
        RectTransform textRt = textObj.GetComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.offsetMin = new Vector2(10f, 0f);
        textRt.offsetMax = new Vector2(-10f, 0f);

        inputField.textComponent = inputText;
        inputField.placeholder   = placeholder;

        return inputField;
    }

    private void BuildColumnHeader(GameObject canvas)
    {
        GameObject row = MakePanel(canvas, "Column Header", new Color(0.20f, 0.25f, 0.38f));
        RectTransform rt = row.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 0.82f);
        rt.anchorMax = new Vector2(1f, 0.86f);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        HorizontalLayoutGroup hlg = row.AddComponent<HorizontalLayoutGroup>();
        hlg.childForceExpandHeight = true;
        hlg.childForceExpandWidth  = false;
        hlg.padding = new RectOffset(6, 6, 0, 0);
        hlg.spacing = 2f;

        MakeColText(row, "名前",       TextAnchor.MiddleLeft,   0f, ColNameWidth);
        MakeColText(row, "ダメージ",   TextAnchor.MiddleCenter, 1f, 0f);
        MakeColText(row, "AP",         TextAnchor.MiddleCenter, 1f, 0f);
        MakeColText(row, "特性",       TextAnchor.MiddleLeft,   1f, 0f);
        MakeColText(row, "Salvo/Mag", TextAnchor.MiddleCenter, 1f, 0f);
    }

    private GameObject BuildScrollArea(GameObject canvas)
    {
        GameObject sv = new GameObject("Scroll View");
        sv.transform.SetParent(canvas.transform, false);
        sv.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0f);
        _scrollRect = sv.AddComponent<ScrollRect>();
        ScrollRect sr = _scrollRect;
        RectTransform svRt = sv.GetComponent<RectTransform>();
        svRt.anchorMin = new Vector2(0f, 0f);
        svRt.anchorMax = new Vector2(1f, 0.82f);
        svRt.offsetMin = Vector2.zero;
        svRt.offsetMax = Vector2.zero;

        GameObject viewport = new GameObject("Viewport");
        viewport.transform.SetParent(sv.transform, false);
        RectTransform vpRt = viewport.AddComponent<RectTransform>();
        viewport.AddComponent<RectMask2D>();
        StretchFull(vpRt);

        GameObject content = new GameObject("Content");
        content.transform.SetParent(viewport.transform, false);
        RectTransform cRt = content.AddComponent<RectTransform>();
        cRt.anchorMin = new Vector2(0f, 1f);
        cRt.anchorMax = new Vector2(1f, 1f);
        cRt.pivot     = new Vector2(0.5f, 1f);
        cRt.offsetMin = Vector2.zero;
        cRt.offsetMax = Vector2.zero;

        VerticalLayoutGroup vlg = content.AddComponent<VerticalLayoutGroup>();
        vlg.spacing              = 1f;
        vlg.padding              = new RectOffset(4, 4, 4, 4);
        vlg.childForceExpandWidth  = true;
        vlg.childForceExpandHeight = false;
        content.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        sr.content           = cRt;
        sr.viewport          = vpRt;
        sr.horizontal        = false;
        sr.vertical          = true;
        sr.scrollSensitivity = 30f;

        return content;
    }

    // ================================================================== リスト構築

    private void BuildAllSections()
    {
        var meleeList  = WeaponDatabase.GetWeaponsByType<MeleeWeaponData>().ToList();
        var rangedList = WeaponDatabase.GetWeaponsByType<RangedWeaponData>().ToList();
        var ammoList   = WeaponDatabase.AllAmmo.ToList();

        Section sec;

        sec = NewSection("── 近接武器 ──", EquipmentEditDialog.EquipmentKind.Melee);
        foreach (var w in meleeList)
        {
            MeleeWeaponData captured = w;
            GameObject row = BuildDataRow(w.weaponName,
                                          FormatMeleeDamage(w.baseDamage, w.extraDamageDice),
                                          FormatAP(w.AP),
                                          GetTraitLabels(w.traits), "-",
                                          () => _editDialog.OpenEdit(captured));
            sec.Rows.Add((row, w.weaponName.ToLower()));
        }
        _sections.Add(sec);

        sec = NewSection("── 遠距離武器 ──", EquipmentEditDialog.EquipmentKind.Ranged);
        foreach (var w in rangedList)
        {
            RangedWeaponData captured = w;
            GameObject row = BuildDataRow(w.weaponName,
                                          FormatDamage(w.baseDamage, w.extraDamageDice),
                                          FormatAP(w.AP),
                                          GetTraitLabels(w.traits), $"{w.salvo}/{w.magazine}",
                                          () => _editDialog.OpenEdit(captured));
            sec.Rows.Add((row, w.weaponName.ToLower()));
        }
        _sections.Add(sec);

        sec = NewSection("── 弾薬 ──", EquipmentEditDialog.EquipmentKind.Ammo);
        foreach (var a in ammoList)
        {
            AmmoData captured = a;
            GameObject row = BuildDataRow(a.ammoName,
                                          FormatDamage(a.baseDamage, a.extraDamageDice),
                                          FormatAP(a.AP),
                                          GetTraitLabels(a.traits), "-",
                                          () => _editDialog.OpenEdit(captured));
            sec.Rows.Add((row, a.ammoName.ToLower()));
        }
        _sections.Add(sec);

        var armourList = ArmourDatabase.AllArmours.ToList();
        sec = NewSection("── 防具 ──", EquipmentEditDialog.EquipmentKind.Armour);
        foreach (var a in armourList)
        {
            ArmourData captured = a;
            GameObject row = BuildDataRow(a.armourName, $"AR {a.armourRating}", "-", a.traits, "-",
                                          () => _editDialog.OpenEdit(captured));
            sec.Rows.Add((row, a.armourName.ToLower()));
        }
        _sections.Add(sec);
    }

    private Section NewSection(string title, EquipmentEditDialog.EquipmentKind kind)
    {
        GameObject hdr = MakePanel(_scrollContent, "Section_" + title, new Color(0.18f, 0.22f, 0.32f));
        hdr.AddComponent<LayoutElement>().preferredHeight = 28f;

        HorizontalLayoutGroup hlg = hdr.AddComponent<HorizontalLayoutGroup>();
        hlg.childForceExpandHeight = true;
        hlg.childForceExpandWidth  = false;
        hlg.padding = new RectOffset(6, 4, 0, 0);
        hlg.spacing = 4f;

        // セクションタイトル
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(hdr.transform, false);
        titleObj.AddComponent<RectTransform>();
        titleObj.AddComponent<LayoutElement>().flexibleWidth = 1f;
        Text txt = titleObj.AddComponent<Text>();
        txt.text      = title;
        txt.font      = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.fontSize  = 14;
        txt.alignment = TextAnchor.MiddleLeft;
        txt.color     = Color.white;

        // "+" 追加ボタン
        GameObject addBtn = MakeButton(hdr, "+", 16, new Color(0.18f, 0.42f, 0.22f));
        RectTransform addRt = addBtn.GetComponent<RectTransform>();
        addRt.sizeDelta = new Vector2(26f, 0f);
        addBtn.AddComponent<LayoutElement>().preferredWidth = 26f;

        EquipmentEditDialog.EquipmentKind capturedKind = kind;
        addBtn.GetComponent<Button>().onClick.AddListener(() => _editDialog.OpenNew(capturedKind));

        return new Section { Header = hdr, Rows = new List<(GameObject, string)>() };
    }

    private GameObject BuildDataRow(string name, string dmg, string ap, List<string> traitLabels,
                                     string salvoArmo, Action onClickEdit = null)
    {
        GameObject row = MakePanel(_scrollContent, "Row_" + name, new Color(0.12f, 0.15f, 0.22f));
        row.AddComponent<LayoutElement>().preferredHeight = RowHeight;

        if (onClickEdit != null)
        {
            Button btn = row.AddComponent<Button>();
            ColorBlock cb = btn.colors;
            cb.normalColor      = new Color(0.12f, 0.15f, 0.22f);
            cb.highlightedColor = new Color(0.20f, 0.26f, 0.38f);
            cb.pressedColor     = new Color(0.10f, 0.12f, 0.18f);
            btn.colors = cb;
            btn.targetGraphic = row.GetComponent<Image>();
            btn.onClick.AddListener(() => onClickEdit());
        }

        HorizontalLayoutGroup hlg = row.AddComponent<HorizontalLayoutGroup>();
        hlg.childForceExpandHeight = true;
        hlg.childForceExpandWidth  = false;
        hlg.childAlignment         = TextAnchor.MiddleLeft;
        hlg.padding = new RectOffset(6, 6, 0, 0);
        hlg.spacing = 2f;

        // 名前列 (固定幅)
        GameObject nameCell = new GameObject("Cell_Name");
        nameCell.transform.SetParent(row.transform, false);
        nameCell.AddComponent<RectTransform>();
        LayoutElement nameLe = nameCell.AddComponent<LayoutElement>();
        nameLe.preferredWidth = ColNameWidth;
        nameLe.flexibleWidth  = 0f;
        MakeText(nameCell, name, 16, TextAnchor.MiddleLeft,
                 new Vector2(0.01f, 0f), Vector2.one);

        // ダメージ列 (均等 flex)
        MakeFlexCell(row, dmg, TextAnchor.MiddleCenter, 16);

        // AP 列 (均等 flex)
        MakeFlexCell(row, ap, TextAnchor.MiddleCenter, 14);

        // 特性列 (チップ、均等 flex)
        BuildTraitCell(row, traitLabels);

        // Salvo/Armo 列 (均等 flex)
        MakeFlexCell(row, salvoArmo, TextAnchor.MiddleCenter, 14);

        return row;
    }

    private void BuildTraitCell(GameObject parent, List<string> traitLabels)
    {
        GameObject cell = new GameObject("Cell_Traits");
        cell.transform.SetParent(parent.transform, false);
        cell.AddComponent<RectTransform>();
        LayoutElement le = cell.AddComponent<LayoutElement>();
        le.preferredWidth = 0f;
        le.flexibleWidth  = 1f;

        cell.AddComponent<RectMask2D>();

        // チップを横並びにするコンテナ
        GameObject chipContainer = new GameObject("ChipContainer");
        chipContainer.transform.SetParent(cell.transform, false);
        RectTransform ccRt = chipContainer.AddComponent<RectTransform>();
        ccRt.anchorMin = Vector2.zero;
        ccRt.anchorMax = Vector2.one;
        ccRt.offsetMin = Vector2.zero;
        ccRt.offsetMax = Vector2.zero;

        HorizontalLayoutGroup hlg = chipContainer.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing              = 3f;
        hlg.padding              = new RectOffset(4, 4, 6, 6);
        hlg.childForceExpandWidth  = false;
        hlg.childForceExpandHeight = true;
        hlg.childAlignment       = TextAnchor.MiddleLeft;

        foreach (string label in traitLabels)
        {
            GameObject chip = new GameObject("Chip");
            chip.transform.SetParent(chipContainer.transform, false);
            chip.AddComponent<Image>().color = new Color(0.25f, 0.30f, 0.48f);
            LayoutElement chipLe = chip.AddComponent<LayoutElement>();
            chipLe.preferredWidth = Mathf.Max(40f, label.Length * 7.5f + 10f);
            chipLe.flexibleWidth  = 0f;
            MakeText(chip, label, 11, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one);
        }
    }

    // ================================================================== フィルター

    private void ApplyFilter(string query)
    {
        string lower = query == null ? "" : query.ToLower();
        bool searching = lower.Length > 0;
        foreach (Section sec in _sections)
        {
            bool anyVisible = !searching; // 検索なしは常に見せる
            foreach (var (row, key) in sec.Rows)
            {
                bool show = !searching || key.Contains(lower);
                row.SetActive(show);
                if (show) anyVisible = true;
            }
            // セクションヘッダー（"+"ボタン含む）は検索中に行が0件のときのみ隠す
            sec.Header.SetActive(!searching || anyVisible);
        }
    }

    // ================================================================== データ変換

    private static string FormatDamage(int baseDmg, int ed)
    {
        if (baseDmg == 0 && ed == 0) return "-";
        if (ed == 0)      return baseDmg.ToString();
        if (baseDmg == 0) return $"+{ed}ED";
        return $"{baseDmg}+{ed}ED";
    }

    private static string FormatMeleeDamage(int baseDmg, int ed)
    {
        if (baseDmg == 0 && ed == 0) return "STR";
        if (ed == 0)      return $"STR+{baseDmg}";
        if (baseDmg == 0) return $"STR+{ed}ED";
        return $"STR+{baseDmg}+{ed}ED";
    }

    private static string FormatAP(int ap) => ap > 0 ? ap.ToString() : "-";

    private static List<string> GetTraitLabels(List<TraitEntry> traits)
    {
        var result = new List<string>();
        foreach (TraitEntry t in traits)
        {
            string name = t.trait == WeaponTrait.StatusEffect && !string.IsNullOrEmpty(t.statusEffectName)
                ? t.statusEffectName
                : t.trait.ToString();
            result.Add(t.rating > 0 ? $"{name}({t.rating})" : name);
        }
        return result;
    }

    // ================================================================== UI ヘルパー

    private static GameObject MakePanel(GameObject parent, string name, Color color)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent.transform, false);
        obj.AddComponent<Image>().color = color;
        return obj;
    }

    private static void StretchFull(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    private static Text MakeText(GameObject parent, string text, int size,
                                  TextAnchor align, Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject obj = new GameObject("Text");
        obj.transform.SetParent(parent.transform, false);
        Text t = obj.AddComponent<Text>();
        t.font      = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.text      = text;
        t.fontSize  = size;
        t.alignment = align;
        t.color     = Color.white;
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        return t;
    }

    private static GameObject MakeButton(GameObject parent, string label, int fontSize, Color bgColor)
    {
        GameObject obj = new GameObject("Btn_" + label);
        obj.transform.SetParent(parent.transform, false);
        obj.AddComponent<Image>().color = bgColor;
        obj.AddComponent<Button>();

        GameObject textObj = new GameObject("Label");
        textObj.transform.SetParent(obj.transform, false);
        Text txt = textObj.AddComponent<Text>();
        txt.text             = label;
        txt.font             = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.fontSize         = fontSize;
        txt.alignment        = TextAnchor.MiddleCenter;
        txt.color            = Color.white;
        txt.verticalOverflow = VerticalWrapMode.Overflow;
        RectTransform tr = textObj.GetComponent<RectTransform>();
        tr.anchorMin = Vector2.zero;
        tr.anchorMax = Vector2.one;
        tr.offsetMin = Vector2.zero;
        tr.offsetMax = Vector2.zero;
        return obj;
    }

    private static void MakeColText(GameObject parent, string text, TextAnchor align,
                                     float flexWidth, float prefWidth)
    {
        GameObject obj = new GameObject("ColHdr_" + text);
        obj.transform.SetParent(parent.transform, false);
        obj.AddComponent<RectTransform>();
        LayoutElement le = obj.AddComponent<LayoutElement>();
        le.flexibleWidth  = flexWidth;
        le.preferredWidth = prefWidth;
        Text txt = obj.AddComponent<Text>();
        txt.font      = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.text      = text;
        txt.fontSize  = 14;
        txt.alignment = align;
        txt.color     = new Color(0.75f, 0.80f, 0.90f);
    }

    private static void MakeFixedCell(GameObject parent, string text, float width,
                                       TextAnchor align, int fontSize)
    {
        GameObject obj = new GameObject("Cell");
        obj.transform.SetParent(parent.transform, false);
        obj.AddComponent<RectTransform>();
        LayoutElement le = obj.AddComponent<LayoutElement>();
        le.preferredWidth = width;
        le.flexibleWidth  = 0f;
        Text txt = obj.AddComponent<Text>();
        txt.font      = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.text      = text;
        txt.fontSize  = fontSize;
        txt.alignment = align;
        txt.color     = Color.white;
    }

    private static void MakeFlexCell(GameObject parent, string text, TextAnchor align, int fontSize)
    {
        GameObject obj = new GameObject("Cell");
        obj.transform.SetParent(parent.transform, false);
        obj.AddComponent<RectTransform>();
        LayoutElement le = obj.AddComponent<LayoutElement>();
        le.flexibleWidth = 1f;
        Text txt = obj.AddComponent<Text>();
        txt.font      = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.text      = text;
        txt.fontSize  = fontSize;
        txt.alignment = align;
        txt.color     = Color.white;
    }
}
