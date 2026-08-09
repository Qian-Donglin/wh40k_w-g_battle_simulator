using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 画面 2: キャラクターのステータス確認・編集画面。
/// BattleSimulator から Show() / Hide() で制御する。
/// </summary>
public class CharacterStatusScreen : MonoBehaviour
{
    public event Action OnClosed;
    public bool IsVisible => _canvas != null && _canvas.enabled;

    // ---- 参照 ----
    private BattleState _state;
    private BattleCharacter _selected;

    // ---- UI ルート ----
    private Canvas    _canvas;
    private Transform _listColumn;
    private Transform _detailColumn;
    private bool      _editMode;

    // ---- ステータス行の参照（編集モード切り替えで再描画） ----
    private readonly List<StatRow> _statRows = new List<StatRow>();

    private struct StatRow
    {
        public string Label;
        public Func<int>     Getter;
        public Action<int>   Setter;  // null なら読み取り専用
        public Text          ValueText;
    }

    // ================================================================== public

    public void Initialize(BattleState state)
    {
        _state = state;
        BuildCanvas();
    }

    public void Show()
    {
        _canvas.enabled = true;
        RefreshCharacterList();
        if (_state.Characters.Count > 0)
            SelectCharacter(_state.Characters[0]);
    }

    public void Hide()
    {
        _canvas.enabled = false;
        _editMode = false;
        OnClosed?.Invoke();
    }

    // ================================================================== UI 構築

    private void BuildCanvas()
    {
        GameObject canvasObj = new GameObject("Status Screen Canvas");
        canvasObj.transform.SetParent(transform, false);
        _canvas = canvasObj.AddComponent<Canvas>();
        _canvas.renderMode  = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 10;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // 全画面背景
        GameObject bg = MakePanel(canvasObj, "Background", new Color(0.1f, 0.12f, 0.18f, 0.97f));
        StretchFull(bg.GetComponent<RectTransform>());

        // ヘッダー
        BuildHeader(canvasObj);

        // 左列（キャラクターリスト）
        GameObject leftPanel = MakePanel(canvasObj, "Left Panel", new Color(0.12f, 0.15f, 0.22f));
        RectTransform lrt = leftPanel.GetComponent<RectTransform>();
        lrt.anchorMin = new Vector2(0f, 0.05f);
        lrt.anchorMax = new Vector2(0.28f, 0.9f);
        lrt.offsetMin = new Vector2(12f,  0f);
        lrt.offsetMax = new Vector2(-4f,  0f);
        _listColumn = AddVerticalLayout(leftPanel, 6f, new RectOffset(8,8,8,8)).transform;

        // 右列（詳細）
        GameObject rightPanel = MakePanel(canvasObj, "Right Panel", new Color(0.12f, 0.15f, 0.22f));
        RectTransform rrt = rightPanel.GetComponent<RectTransform>();
        rrt.anchorMin = new Vector2(0.3f, 0.05f);
        rrt.anchorMax = new Vector2(1f,   0.9f);
        rrt.offsetMin = new Vector2(4f,   0f);
        rrt.offsetMax = new Vector2(-12f, 0f);
        // 絶対位置レイアウトを採用。スクロールは使用しない。
        _detailColumn = rightPanel.transform;

        _canvas.enabled = false;
    }

    private void BuildHeader(GameObject canvas)
    {
        GameObject header = MakePanel(canvas, "Header", new Color(0.08f, 0.10f, 0.16f));
        RectTransform rt = header.GetComponent<RectTransform>();
        rt.anchorMin        = new Vector2(0f, 0.9f);
        rt.anchorMax        = Vector2.one;
        rt.offsetMin        = Vector2.zero;
        rt.offsetMax        = Vector2.zero;

        // タイトル
        MakeText(header, "キャラクター一覧", 26, TextAnchor.MiddleCenter, new Vector2(0f, 0f), new Vector2(1f, 1f));

        // ボタンの寸法（元の 40%）
        // - 元の幅 60 → 24
        // - 元の高さ ヘッダー全高 → ヘッダーの 40%（anchor で表現）
        const float btnWidth = 24f;

        // 閉じるボタン
        GameObject closeBtn = MakeButton(header, "X", 14, new Color(0.6f, 0.15f, 0.15f));
        RectTransform crt = closeBtn.GetComponent<RectTransform>();
        crt.anchorMin        = new Vector2(1f, 0.3f);
        crt.anchorMax        = new Vector2(1f, 0.7f);
        crt.pivot            = new Vector2(1f, 0.5f);
        crt.anchoredPosition = new Vector2(-8f, 0f);
        crt.sizeDelta        = new Vector2(btnWidth, 0f);
        closeBtn.GetComponent<Button>().onClick.AddListener(Hide);

        // 編集モード切り替えボタン（鍵アイコン）
        // 鍵かけてる ＝ 編集不可（閲覧モード）
        // 鍵が解けてる ＝ 編集可（編集モード）
        GameObject editBtn = MakeButton(header, LockGlyph(false), 14, new Color(0.2f, 0.45f, 0.2f));
        RectTransform ert = editBtn.GetComponent<RectTransform>();
        ert.anchorMin        = new Vector2(1f, 0.3f);
        ert.anchorMax        = new Vector2(1f, 0.7f);
        ert.pivot            = new Vector2(1f, 0.5f);
        // 閉じるボタンの左端 (-8 - 24 = -32) からさらに 8px 余白
        ert.anchoredPosition = new Vector2(-40f, 0f);
        ert.sizeDelta        = new Vector2(btnWidth, 0f);
        Text editLabel = editBtn.GetComponentInChildren<Text>();
        editBtn.GetComponent<Button>().onClick.AddListener(() =>
        {
            _editMode = !_editMode;
            editLabel.text = LockGlyph(_editMode);
            if (_selected != null) ShowCharacterDetail(_selected);
        });
    }

    // ================================================================== キャラクターリスト

    private void RefreshCharacterList()
    {
        foreach (Transform child in _listColumn) Destroy(child.gameObject);

        foreach (var bc in _state.Characters)
        {
            BattleCharacter captured = bc;
            GameObject btn = MakeButton(_listColumn.gameObject, bc.Data.characterName, 18,
                                        new Color(0.2f, 0.25f, 0.38f));
            btn.GetComponent<Button>().onClick.AddListener(() => SelectCharacter(captured));

            LayoutElement le = btn.AddComponent<LayoutElement>();
            le.preferredHeight = 44f;
            le.flexibleWidth   = 1f;
        }
    }

    private void SelectCharacter(BattleCharacter bc)
    {
        _selected = bc;
        ShowCharacterDetail(bc);
    }

    // ================================================================== 詳細表示

    /// <summary>
    /// 右ペインに 6 セクションを配置：
    ///   左上 (CharInfo) : 名前 + 軽傷/重傷 HP バー + 未定ボックス
    ///   中上 (Stats)    : 基礎ステータス
    ///   右上 (Skills)   : 戦闘関連スキル
    ///   中央             : 特性/武器タブ
    ///   左下 (Weapons)  : 近接 / 遠距離 / 防具 のスロット
    ///   右下 (Ammo)     : 弾薬枠
    /// 各セクションは絶対位置で配置するため、_detailColumn は単純なコンテナとして扱う。
    /// </summary>
    private void ShowCharacterDetail(BattleCharacter bc)
    {
        foreach (Transform child in _detailColumn) Destroy(child.gameObject);
        _statRows.Clear();

        NpcData d = bc.Data;
        GameObject root = _detailColumn.gameObject;

        BuildCharInfoBox(root, bc);
        BuildStatsBox   (root, d, bc);
        BuildSkillsBox  (root, d);
        BuildTabsSection(root, d, bc);
    }

    // ---- 左上: キャラ情報 ----

    private void BuildCharInfoBox(GameObject parent, BattleCharacter bc)
    {
        NpcData d = bc.Data;
        GameObject box = MakeBorderedBox(parent.transform, new Vector2(0.00f, 0.50f), new Vector2(0.30f, 1.00f));

        // キャラ名
        MakeAnchoredText(box, d.characterName, 20, TextAnchor.MiddleLeft,
                         new Vector2(0.06f, 0.74f), new Vector2(0.95f, 0.95f));

        // 軽傷HP（Shock の残量を表示。CurrentShock=0 のとき満タン）
        int shockCurrent = d.MaxShock - bc.CurrentShock;
        MakeAnchoredText(box, "軽傷HP", 14, TextAnchor.MiddleLeft,
                         new Vector2(0.06f, 0.60f), new Vector2(0.30f, 0.70f));
        GameObject shockBg = BuildHpBar(box, (float)shockCurrent / Mathf.Max(1, d.MaxShock),
                   $"{shockCurrent} / {d.MaxShock}",
                   new Color(0.35f, 0.52f, 0.18f),
                   new Vector2(0.30f, 0.62f), new Vector2(0.95f, 0.68f));

        // 重傷HP（Wounds）
        MakeAnchoredText(box, "重傷HP", 14, TextAnchor.MiddleLeft,
                         new Vector2(0.06f, 0.50f), new Vector2(0.30f, 0.60f));
        GameObject woundBg = BuildHpBar(box, (float)bc.CurrentWounds / Mathf.Max(1, d.MaxWounds),
                   $"{bc.CurrentWounds} / {d.MaxWounds}",
                   new Color(0.55f, 0.18f, 0.18f),
                   new Vector2(0.30f, 0.52f), new Vector2(0.95f, 0.58f));

        if (_editMode)
        {
            // 軽傷HP 編集ボタン（バー内左端 / 右端）
            GameObject sMin = MakeButton(shockBg, "-", 10, new Color(0.45f, 0.15f, 0.15f, 0.92f));
            SetAnchors(sMin, new Vector2(0.01f, 0.04f), new Vector2(0.10f, 0.96f));
            sMin.GetComponent<Button>().onClick.AddListener(() => { bc.EditShockHp(-1); ShowCharacterDetail(bc); });

            GameObject sPls = MakeButton(shockBg, "+", 10, new Color(0.15f, 0.42f, 0.15f, 0.92f));
            SetAnchors(sPls, new Vector2(0.90f, 0.04f), new Vector2(0.99f, 0.96f));
            sPls.GetComponent<Button>().onClick.AddListener(() => { bc.EditShockHp(1); ShowCharacterDetail(bc); });

            // 重傷HP 編集ボタン（バー内左端 / 右端）
            GameObject wMin = MakeButton(woundBg, "-", 10, new Color(0.45f, 0.15f, 0.15f, 0.92f));
            SetAnchors(wMin, new Vector2(0.01f, 0.04f), new Vector2(0.10f, 0.96f));
            wMin.GetComponent<Button>().onClick.AddListener(() => { bc.EditWoundsHp(-1); ShowCharacterDetail(bc); });

            GameObject wPls = MakeButton(woundBg, "+", 10, new Color(0.15f, 0.42f, 0.15f, 0.92f));
            SetAnchors(wPls, new Vector2(0.90f, 0.04f), new Vector2(0.99f, 0.96f));
            wPls.GetComponent<Button>().onClick.AddListener(() => { bc.EditWoundsHp(1); ShowCharacterDetail(bc); });
        }
    }

    // ---- 中上: 基礎ステータス ----

    private void BuildStatsBox(GameObject parent, NpcData d, BattleCharacter bc)
    {
        GameObject box = MakeBorderedBox(parent.transform, new Vector2(0.32f, 0.50f), new Vector2(0.64f, 1.00f));

        var labels  = new[] { "STR", "TGH", "AGI", "INI", "WIL", "INT", "FEL" };
        var getters = new Func<int>[]
        {
            () => d.stats.strength,   () => d.stats.toughness, () => d.stats.agility,
            () => d.stats.initiative, () => d.stats.willpower, () => d.stats.intellect,
            () => d.stats.fellowship,
        };

        const float top = 0.95f, bottom = 0.03f;
        float h = (top - bottom) / labels.Length;
        for (int i = 0; i < labels.Length; i++)
        {
            float y0 = top - (i + 1) * h;
            float y1 = top -  i      * h;
            BuildStatRowAbsolute(box, labels[i], getters[i], bc.GetStatModifier(labels[i]),
                                 new Vector2(0.10f, y0), new Vector2(0.90f, y1));
        }
    }

    // ---- 右上: 戦闘関連スキル ----

    private void BuildSkillsBox(GameObject parent, NpcData d)
    {
        GameObject box = MakeBorderedBox(parent.transform, new Vector2(0.66f, 0.50f), new Vector2(1.00f, 1.00f));

        MakeAnchoredText(box, "スキル：", 14, TextAnchor.UpperLeft,
                         new Vector2(0.06f, 0.88f), new Vector2(0.95f, 0.97f));

        var labels = new[] { "WS 近接", "BS 射撃", "警戒", "運動", "ステルス", "威圧", "サバイバル", "医療" };
        var values = new[]
        {
            d.skills.weaponSkill,   d.skills.ballisticSkill, d.skills.awareness,
            d.skills.athletics,     d.skills.stealth,        d.skills.intimidation,
            d.skills.survival,      d.skills.medicae,
        };

        const float top = 0.82f, bottom = 0.18f;
        float h = (top - bottom) / labels.Length;
        for (int i = 0; i < labels.Length; i++)
        {
            float y0 = top - (i + 1) * h;
            float y1 = top -  i      * h;
            GameObject row = MakeAnchoredContainer(box, new Vector2(0.10f, y0), new Vector2(0.90f, y1));
            MakeAnchoredText(row, labels[i],          14, TextAnchor.MiddleLeft,
                             new Vector2(0f, 0f), new Vector2(0.6f, 1f));
            MakeAnchoredText(row, values[i].ToString(), 14, TextAnchor.MiddleRight,
                             new Vector2(0.6f, 0f), new Vector2(1f, 1f));
        }

        GameObject shortcut = MakeButton(box, "[#]", 11, new Color(0.22f, 0.28f, 0.45f));
        SetAnchors(shortcut, new Vector2(0.75f, 0.03f), new Vector2(0.97f, 0.16f));
    }

    // ---- 中下: 武器防具弾薬 / 特性 タブ + コンテンツ ----

    private void BuildTabsSection(GameObject parent, NpcData d, BattleCharacter bc)
    {
        Color activeTab   = new Color(0.40f, 0.45f, 0.55f);
        Color inactiveTab = new Color(0.22f, 0.26f, 0.36f);

        // 武器防具弾薬コンテンツ（初期表示）
        GameObject weaponContent = MakeAnchoredContainer(parent, new Vector2(0.00f, 0.00f), new Vector2(1.00f, 0.42f));
        BuildWeaponsInContainer(weaponContent, d);
        BuildAmmoInContainer(weaponContent, d, bc);

        // 特性コンテンツ
        GameObject traitsContent = MakeAnchoredContainer(parent, new Vector2(0.00f, 0.00f), new Vector2(1.00f, 0.42f));
        GameObject traitBox = MakeBorderedBox(traitsContent.transform, Vector2.zero, Vector2.one);
        MakeAnchoredText(traitBox, "キャラクターの特性", 16, TextAnchor.MiddleCenter,
                         new Vector2(0.1f, 0.3f), new Vector2(0.9f, 0.7f));
        traitsContent.SetActive(false);

        // タブボタン
        GameObject t1 = MakeButton(parent, "武器防具弾薬", 12, activeTab);
        RectTransform rt1 = t1.GetComponent<RectTransform>();
        rt1.anchorMin = new Vector2(0.02f, 0.43f);
        rt1.anchorMax = new Vector2(0.16f, 0.49f);
        rt1.offsetMin = Vector2.zero; rt1.offsetMax = Vector2.zero;

        GameObject t2 = MakeButton(parent, "特性", 12, inactiveTab);
        RectTransform rt2 = t2.GetComponent<RectTransform>();
        rt2.anchorMin = new Vector2(0.17f, 0.43f);
        rt2.anchorMax = new Vector2(0.25f, 0.49f);
        rt2.offsetMin = Vector2.zero; rt2.offsetMax = Vector2.zero;

        Image img1 = t1.GetComponent<Image>();
        Image img2 = t2.GetComponent<Image>();

        t1.GetComponent<Button>().onClick.AddListener(() =>
        {
            weaponContent.SetActive(true);
            traitsContent.SetActive(false);
            img1.color = activeTab;
            img2.color = inactiveTab;
        });
        t2.GetComponent<Button>().onClick.AddListener(() =>
        {
            weaponContent.SetActive(false);
            traitsContent.SetActive(true);
            img1.color = inactiveTab;
            img2.color = activeTab;
        });
    }

    private void BuildWeaponsInContainer(GameObject container, NpcData d)
    {
        GameObject box = MakeBorderedBox(container.transform, new Vector2(0.00f, 0.00f), new Vector2(0.64f, 1.00f));

        var melee  = d.weapons.OfType<MeleeWeaponData>().Select(w => w.weaponName).ToArray();
        var ranged = d.weapons.OfType<RangedWeaponData>().Select(w => w.weaponName).ToArray();
        var armour = d.equippedArmour != null ? new[] { d.equippedArmour.name } : new string[0];

        BuildWeaponColumn(box, "近接",   new Vector2(0.03f, 0.05f), new Vector2(0.36f, 0.95f), melee,  3);
        BuildWeaponColumn(box, "遠距離", new Vector2(0.38f, 0.05f), new Vector2(0.71f, 0.95f), ranged, 3);
        BuildWeaponColumn(box, "防具",   new Vector2(0.73f, 0.05f), new Vector2(0.97f, 0.95f), armour, 2);
    }

    private void BuildWeaponColumn(GameObject parent, string title,
                                   Vector2 anchorMin, Vector2 anchorMax,
                                   string[] items, int slots)
    {
        GameObject col = MakeAnchoredContainer(parent, anchorMin, anchorMax);
        MakeAnchoredText(col, title, 14, TextAnchor.UpperLeft,
                         new Vector2(0f, 0.83f), new Vector2(1f, 1f));

        const float top    = 0.80f;
        const float bottom = 0.00f;
        float       h      = (top - bottom) / slots;
        const float gap    = 0.02f;

        for (int i = 0; i < slots; i++)
        {
            float y0 = top - (i + 1) * h + gap * 0.5f;
            float y1 = top -  i      * h - gap * 0.5f;
            GameObject slot = MakeBorderedBox(col.transform, new Vector2(0f, y0), new Vector2(1f, y1));
            if (i < items.Length)
                MakeAnchoredText(slot, items[i], 12, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one);
        }
    }

    // ---- 右下: 弾薬枠 ----

    private void BuildAmmoInContainer(GameObject container, NpcData d, BattleCharacter bc)
    {
        GameObject box = MakeBorderedBox(container.transform, new Vector2(0.66f, 0.00f), new Vector2(1.00f, 1.00f));

        var slotAnchors = new (Vector2 min, Vector2 max)[]
        {
            (new Vector2(0.06f, 0.62f), new Vector2(0.46f, 0.92f)),
            (new Vector2(0.50f, 0.62f), new Vector2(0.94f, 0.92f)),
            (new Vector2(0.06f, 0.30f), new Vector2(0.46f, 0.58f)),
        };

        var ranged = d.weapons.OfType<RangedWeaponData>().ToArray();
        for (int i = 0; i < slotAnchors.Length; i++)
        {
            GameObject slot = MakeBorderedBox(box.transform, slotAnchors[i].min, slotAnchors[i].max);
            if (i < ranged.Length)
            {
                var rw = ranged[i];
                MakeAnchoredText(slot, $"{rw.weaponName}\n{bc.GetCurrentAmmo(rw)} / {rw.magazine}",
                                 12, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one);
            }
        }

        MakeAnchoredText(box, "ここが弾薬枠", 14, TextAnchor.MiddleCenter,
                         new Vector2(0.10f, 0.05f), new Vector2(0.90f, 0.25f));
    }

    // ================================================================== 行ウィジェット

    /// <summary>絶対位置レイアウト用のステータス行。補正がある場合は緑/赤の差分を隣に表示する。</summary>
    private static void BuildStatRowAbsolute(GameObject parent, string label,
                                             Func<int> getter, int modifier,
                                             Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject row = MakeAnchoredContainer(parent, anchorMin, anchorMax);

        MakeAnchoredText(row, label, 16, TextAnchor.MiddleLeft,
                         new Vector2(0f, 0f), new Vector2(0.30f, 1f));

        float valueRight = modifier == 0 ? 1f : 0.75f;
        MakeAnchoredText(row, getter().ToString(), 16, TextAnchor.MiddleRight,
                         new Vector2(0.6f, 0f), new Vector2(valueRight, 1f));

        if (modifier != 0)
        {
            string modStr   = modifier > 0 ? $"(+{modifier})" : $"({modifier})";
            Color  modColor = modifier > 0
                ? new Color(0.35f, 1.00f, 0.35f)
                : new Color(1.00f, 0.45f, 0.45f);
            Text modTxt = MakeAnchoredText(row, modStr, 11, TextAnchor.MiddleLeft,
                                           new Vector2(0.76f, 0.05f), new Vector2(1f, 0.95f));
            modTxt.color = modColor;
        }
    }

    // ================================================================== 新規 UI ヘルパー

    /// <summary>白枠付きの矩形ボックス。中身を配置できるよう内側 GameObject を返す。</summary>
    private static GameObject MakeBorderedBox(Transform parent, Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject outer = new GameObject("Bordered");
        outer.transform.SetParent(parent, false);
        Image outerImg = outer.AddComponent<Image>();
        outerImg.color = new Color(0.85f, 0.90f, 1.00f, 0.85f);
        RectTransform oRt = outer.GetComponent<RectTransform>();
        oRt.anchorMin = anchorMin;
        oRt.anchorMax = anchorMax;
        oRt.offsetMin = Vector2.zero;
        oRt.offsetMax = Vector2.zero;

        GameObject inner = new GameObject("Inner");
        inner.transform.SetParent(outer.transform, false);
        Image innerImg = inner.AddComponent<Image>();
        innerImg.color = new Color(0.10f, 0.13f, 0.20f);
        RectTransform iRt = inner.GetComponent<RectTransform>();
        iRt.anchorMin = Vector2.zero;
        iRt.anchorMax = Vector2.one;
        iRt.offsetMin = new Vector2(2f, 2f);
        iRt.offsetMax = new Vector2(-2f, -2f);

        return inner;
    }

    /// <summary>
    /// HP バー：背景（暗）の上に幅 ratio の塗りバーを乗せ、左寄りに valueText を表示する。
    /// 呼び出し元がボタンなどを追加できるよう BG GameObject を返す。
    /// </summary>
    private static GameObject BuildHpBar(GameObject parent, float ratio, string valueText, Color fillColor,
                                         Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject bg = new GameObject("HpBar BG");
        bg.transform.SetParent(parent.transform, false);
        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(0.20f, 0.20f, 0.25f);
        RectTransform bgRt = bg.GetComponent<RectTransform>();
        bgRt.anchorMin = anchorMin;
        bgRt.anchorMax = anchorMax;
        bgRt.offsetMin = Vector2.zero;
        bgRt.offsetMax = Vector2.zero;

        GameObject fill = new GameObject("HpBar Fill");
        fill.transform.SetParent(bg.transform, false);
        Image fillImg = fill.AddComponent<Image>();
        fillImg.color = fillColor;
        RectTransform fillRt = fill.GetComponent<RectTransform>();
        fillRt.anchorMin = Vector2.zero;
        fillRt.anchorMax = new Vector2(Mathf.Clamp01(ratio), 1f);
        fillRt.offsetMin = Vector2.zero;
        fillRt.offsetMax = Vector2.zero;

        // 数値テキストはバー全体の中央に配置
        GameObject label = new GameObject("HpBar Value");
        label.transform.SetParent(bg.transform, false);
        Text valueTxt = label.AddComponent<Text>();
        valueTxt.font             = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        valueTxt.text             = valueText;
        valueTxt.fontSize         = 16;
        valueTxt.alignment        = TextAnchor.MiddleCenter;
        valueTxt.color            = Color.white;
        valueTxt.verticalOverflow = VerticalWrapMode.Overflow;
        RectTransform lRt = label.GetComponent<RectTransform>();
        lRt.anchorMin = Vector2.zero;
        lRt.anchorMax = Vector2.one;
        lRt.offsetMin = Vector2.zero;
        lRt.offsetMax = Vector2.zero;

        return bg;
    }

    /// <summary>Anchor で位置とサイズを指定するテキスト。</summary>
    private static Text MakeAnchoredText(GameObject parent, string text, int size,
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

    /// <summary>子要素を入れるための透明な anchored コンテナ。</summary>
    private static GameObject MakeAnchoredContainer(GameObject parent, Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject obj = new GameObject("Container");
        obj.transform.SetParent(parent.transform, false);
        RectTransform rt = obj.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        return obj;
    }

    private static void SetAnchors(GameObject obj, Vector2 anchorMin, Vector2 anchorMax)
    {
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    /// <summary>
    /// 編集ボタンに表示する鍵グリフ。
    /// 組み込みフォントは絵文字 🔒/🔓 を描画できないため、ASCII で「鍵がかかっている／開いている」を表現する。
    /// </summary>
    private static string LockGlyph(bool unlocked) => unlocked ? "[/]" : "[#]";

    // ================================================================== UI ユーティリティ

    private static GameObject MakePanel(GameObject parent, string name, Color color)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent.transform, false);
        Image img = obj.AddComponent<Image>();
        img.color = color;
        obj.AddComponent<RectTransform>();
        return obj;
    }

    private static void StretchFull(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    private static GameObject AddVerticalLayout(GameObject parent, float spacing, RectOffset padding)
    {
        VerticalLayoutGroup vlg = parent.AddComponent<VerticalLayoutGroup>();
        vlg.spacing             = spacing;
        vlg.padding             = padding;
        vlg.childForceExpandWidth  = true;
        vlg.childForceExpandHeight = false;
        ContentSizeFitter csf = parent.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        return parent;
    }

    // ScrollView を作り、その content Transform を返す
    private static GameObject AddScrollView(GameObject parent)
    {
        GameObject sv = new GameObject("Scroll View");
        sv.transform.SetParent(parent.transform, false);
        ScrollRect sr = sv.AddComponent<ScrollRect>();
        RectTransform svRt = sv.GetComponent<RectTransform>();
        StretchFull(svRt);

        Image svBg = sv.AddComponent<Image>();
        svBg.color = new Color(0, 0, 0, 0);

        GameObject viewport = new GameObject("Viewport");
        viewport.transform.SetParent(sv.transform, false);
        RectTransform vpRt = viewport.AddComponent<RectTransform>();
        StretchFull(vpRt);
        viewport.AddComponent<Mask>().showMaskGraphic = false;
        viewport.AddComponent<Image>().color = new Color(0, 0, 0, 0);

        GameObject content = new GameObject("Content");
        content.transform.SetParent(viewport.transform, false);
        RectTransform cRt = content.AddComponent<RectTransform>();
        cRt.anchorMin = new Vector2(0f, 1f);
        cRt.anchorMax = new Vector2(1f, 1f);
        cRt.pivot     = new Vector2(0.5f, 1f);
        cRt.offsetMin = Vector2.zero;
        cRt.offsetMax = Vector2.zero;

        VerticalLayoutGroup vlg = content.AddComponent<VerticalLayoutGroup>();
        vlg.spacing              = 4f;
        vlg.padding              = new RectOffset(10, 10, 10, 10);
        vlg.childForceExpandWidth  = true;
        vlg.childForceExpandHeight = false;
        ContentSizeFitter csf = content.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        sr.content      = cRt;
        sr.viewport     = vpRt;
        sr.horizontal   = false;
        sr.vertical     = true;
        sr.scrollSensitivity = 30f;

        return content;
    }

    private static Text MakeText(GameObject parent, string text, int size,
                                 TextAnchor anchor, Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject obj = new GameObject("Text");
        obj.transform.SetParent(parent.transform, false);
        Text t = obj.AddComponent<Text>();
        t.font      = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.text      = text;
        t.fontSize  = size;
        t.alignment = anchor;
        t.color     = Color.white;
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        return t;
    }

    private static Text MakeFixedText(GameObject parent, string text, int size,
                                      TextAnchor anchor, float width)
    {
        GameObject obj = new GameObject("Text_" + text);
        obj.transform.SetParent(parent.transform, false);
        Text t = obj.AddComponent<Text>();
        t.font      = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.text      = text;
        t.fontSize  = size;
        t.alignment = anchor;
        t.color     = Color.white;
        LayoutElement le = obj.AddComponent<LayoutElement>();
        le.preferredWidth  = width;
        le.preferredHeight = 28f;
        return t;
    }

    private static GameObject MakeButton(GameObject parent, string label, int fontSize, Color bgColor)
    {
        GameObject obj = new GameObject("Btn_" + label);
        obj.transform.SetParent(parent.transform, false);
        Image img = obj.AddComponent<Image>();
        img.color = bgColor;
        obj.AddComponent<Button>();

        GameObject textObj = new GameObject("Label");
        textObj.transform.SetParent(obj.transform, false);
        Text txt = textObj.AddComponent<Text>();
        txt.font             = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.text             = label;
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
}
