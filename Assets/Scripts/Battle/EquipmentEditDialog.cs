using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 装備の編集・追加・削除を行うモーダルダイアログ。
/// レイアウト:
///   1行目 — 名前・数値フィールドを横並び
///   2行目 — 特性フィールド（全幅）
///   ボタン行 — 削除 / キャンセル / 保存
/// ウィンドウ外クリック → キャンセルと同じ動作
/// </summary>
public class EquipmentEditDialog : MonoBehaviour
{
    public enum EquipmentKind { Melee, Ranged, Ammo, Armour }

    public event Action OnChanged;

    private Canvas     _canvas;
    private GameObject _fieldArea;    // VLG コンテナ。Row1/Row2 を格納
    private GameObject _deleteBtnObj;

    private EquipmentKind _kind;
    private bool          _isNew;
    private object        _target;

    // InputField 参照
    private InputField _fName;
    private InputField _fBaseDmg;
    private InputField _fED;
    private InputField _fAP;
    private InputField _fTraits;
    private InputField _fMeleeRange;
    private InputField _fRange, _fSalvo, _fArmo;
    private InputField _fUses;
    private InputField _fArmourRating;

    // ================================================================== public

    public void Initialize() { BuildCanvas(); }

    public void OpenEdit(MeleeWeaponData data)  { _kind = EquipmentKind.Melee;  _isNew = false; _target = data; PopulateAndShow(); }
    public void OpenEdit(RangedWeaponData data) { _kind = EquipmentKind.Ranged; _isNew = false; _target = data; PopulateAndShow(); }
    public void OpenEdit(AmmoData data)         { _kind = EquipmentKind.Ammo;   _isNew = false; _target = data; PopulateAndShow(); }
    public void OpenEdit(ArmourData data)       { _kind = EquipmentKind.Armour; _isNew = false; _target = data; PopulateAndShow(); }
    public void OpenNew(EquipmentKind kind)     { _kind = kind; _isNew = true;  _target = null; PopulateAndShow(); }

    // ================================================================== Canvas 構築（初回のみ）

    private void BuildCanvas()
    {
        GameObject canvasObj = new GameObject("EditDialog Canvas");
        canvasObj.transform.SetParent(transform, false);
        _canvas = canvasObj.AddComponent<Canvas>();
        _canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 60;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // 暗いオーバーレイ — クリックでキャンセル
        GameObject overlay = new GameObject("Overlay");
        overlay.transform.SetParent(canvasObj.transform, false);
        overlay.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.55f);
        StretchFull(overlay.GetComponent<RectTransform>());
        overlay.AddComponent<Button>().onClick.AddListener(OnCancelClicked);

        // ダイアログ本体 — 水平 90%、高さ ContentSizeFitter で自動決定、垂直中央
        GameObject dialog = new GameObject("Dialog");
        dialog.transform.SetParent(canvasObj.transform, false);
        dialog.AddComponent<Image>().color = new Color(0.12f, 0.15f, 0.22f);

        RectTransform dlgRt = dialog.GetComponent<RectTransform>();
        dlgRt.anchorMin = new Vector2(0.05f, 0.5f);
        dlgRt.anchorMax = new Vector2(0.95f, 0.5f);
        dlgRt.pivot     = new Vector2(0.5f, 0.5f);
        dlgRt.offsetMin = new Vector2(0f, 0f);
        dlgRt.offsetMax = new Vector2(0f, 0f);
        dialog.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        VerticalLayoutGroup dlgVlg = dialog.AddComponent<VerticalLayoutGroup>();
        dlgVlg.padding              = new RectOffset(14, 14, 12, 12);
        dlgVlg.spacing              = 8f;
        dlgVlg.childForceExpandWidth  = true;
        dlgVlg.childForceExpandHeight = false;

        // フィールドエリア（Row1 + Row2 をまとめるコンテナ）
        _fieldArea = new GameObject("FieldArea");
        _fieldArea.transform.SetParent(dialog.transform, false);
        _fieldArea.AddComponent<RectTransform>();
        _fieldArea.AddComponent<LayoutElement>(); // preferredHeight は ContentSizeFitter が決める
        VerticalLayoutGroup faVlg = _fieldArea.AddComponent<VerticalLayoutGroup>();
        faVlg.spacing              = 8f;
        faVlg.padding              = new RectOffset();
        faVlg.childForceExpandWidth  = true;
        faVlg.childForceExpandHeight = false;
        _fieldArea.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // ボタン行
        BuildButtonRow(dialog);

        _canvas.enabled = false;
    }

    // ================================================================== 表示・入力

    private void PopulateAndShow()
    {
        foreach (Transform child in _fieldArea.transform)
            Destroy(child.gameObject);

        BuildFields();
        if (!_isNew) FillFields();
        if (_deleteBtnObj != null) _deleteBtnObj.SetActive(!_isNew);

        _canvas.enabled = true;
    }

    private void BuildFields()
    {
        // ---- Row 1: 非特性フィールドを横並び ----
        GameObject row1 = new GameObject("Row1");
        row1.transform.SetParent(_fieldArea.transform, false);
        row1.AddComponent<RectTransform>();
        LayoutElement r1Le = row1.AddComponent<LayoutElement>();
        r1Le.preferredHeight = 58f;
        HorizontalLayoutGroup r1Hlg = row1.AddComponent<HorizontalLayoutGroup>();
        r1Hlg.spacing              = 6f;
        r1Hlg.padding              = new RectOffset();
        r1Hlg.childForceExpandHeight = true;
        r1Hlg.childForceExpandWidth  = false;
        r1Hlg.childAlignment        = TextAnchor.LowerLeft;

        if (_kind == EquipmentKind.Armour)
        {
            _fName        = AddCompactField(row1, "名前",  "防具名",    flexW: 2f,  prefW: 0f);
            _fArmourRating = AddCompactField(row1, "AR",   "0",         flexW: 0f,  prefW: 65f);
        }
        else
        {
            string namePh = _kind == EquipmentKind.Ammo ? "弾薬名" : "武器名";
            _fName    = AddCompactField(row1, "名前",   namePh, flexW: 2f, prefW: 0f);
            _fBaseDmg = AddCompactField(row1, "Dmg",   "0",    flexW: 0f, prefW: 60f);
            _fED      = AddCompactField(row1, "ED",    "0",    flexW: 0f, prefW: 55f);
            _fAP      = AddCompactField(row1, "AP",    "0",    flexW: 0f, prefW: 55f);

            if (_kind == EquipmentKind.Melee)
                _fMeleeRange = AddCompactField(row1, "射程", "1", flexW: 0f, prefW: 65f);

            if (_kind == EquipmentKind.Ranged)
            {
                _fRange = AddCompactField(row1, "Range", "0", flexW: 0f, prefW: 65f);
                _fSalvo = AddCompactField(row1, "Salvo", "0", flexW: 0f, prefW: 60f);
                _fArmo  = AddCompactField(row1, "Armo",  "0", flexW: 0f, prefW: 60f);
            }

            if (_kind == EquipmentKind.Ammo)
                _fUses = AddCompactField(row1, "Uses", "0", flexW: 0f, prefW: 70f);
        }

        // ---- Row 2: 特性（全幅） ----
        _fTraits = AddFullWidthField(_fieldArea, "特性",
            _kind == EquipmentKind.Armour
                ? "例: Ablative"
                : "例: Brutal, Piercing(2), StatusEffect:OnFire");
    }

    private void FillFields()
    {
        switch (_kind)
        {
            case EquipmentKind.Melee:
            {
                var d = (MeleeWeaponData)_target;
                _fName.text       = d.weaponName;
                _fBaseDmg.text    = d.baseDamage.ToString();
                _fED.text         = d.extraDamageDice.ToString();
                _fAP.text         = d.AP.ToString();
                _fMeleeRange.text = d.meleeRange.ToString();
                _fTraits.text     = TraitsToString(d.traits);
                break;
            }
            case EquipmentKind.Ranged:
            {
                var d = (RangedWeaponData)_target;
                _fName.text    = d.weaponName;
                _fBaseDmg.text = d.baseDamage.ToString();
                _fED.text      = d.extraDamageDice.ToString();
                _fAP.text      = d.AP.ToString();
                _fRange.text   = d.range.ToString();
                _fSalvo.text   = d.salvo.ToString();
                _fArmo.text    = d.armo.ToString();
                _fTraits.text  = TraitsToString(d.traits);
                break;
            }
            case EquipmentKind.Ammo:
            {
                var d = (AmmoData)_target;
                _fName.text    = d.ammoName;
                _fBaseDmg.text = d.baseDamage.ToString();
                _fED.text      = d.extraDamageDice.ToString();
                _fAP.text      = d.AP.ToString();
                _fUses.text    = d.uses.ToString();
                _fTraits.text  = TraitsToString(d.traits);
                break;
            }
            case EquipmentKind.Armour:
            {
                var d = (ArmourData)_target;
                _fName.text         = d.armourName;
                _fArmourRating.text = d.armourRating.ToString();
                _fTraits.text       = string.Join(", ", d.traits);
                break;
            }
        }
    }

    // ================================================================== ボタン処理

    private void BuildButtonRow(GameObject dialog)
    {
        GameObject row = new GameObject("ButtonRow");
        row.transform.SetParent(dialog.transform, false);
        row.AddComponent<RectTransform>();
        row.AddComponent<LayoutElement>().preferredHeight = 42f;
        HorizontalLayoutGroup hlg = row.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing              = 8f;
        hlg.childForceExpandWidth  = false;
        hlg.childForceExpandHeight = true;
        hlg.childAlignment       = TextAnchor.MiddleRight;

        _deleteBtnObj = MakeDialogButton(row, "削除", new Color(0.55f, 0.12f, 0.12f), 100f);
        _deleteBtnObj.GetComponent<Button>().onClick.AddListener(OnDeleteClicked);

        GameObject spacer = new GameObject("Spacer");
        spacer.transform.SetParent(row.transform, false);
        spacer.AddComponent<RectTransform>();
        spacer.AddComponent<LayoutElement>().flexibleWidth = 1f;

        MakeDialogButton(row, "キャンセル", new Color(0.28f, 0.28f, 0.33f), 110f)
            .GetComponent<Button>().onClick.AddListener(OnCancelClicked);

        MakeDialogButton(row, "保存", new Color(0.14f, 0.38f, 0.18f), 100f)
            .GetComponent<Button>().onClick.AddListener(OnSaveClicked);
    }

    private void OnSaveClicked()
    {
        switch (_kind)
        {
            case EquipmentKind.Melee:  SaveMelee();  break;
            case EquipmentKind.Ranged: SaveRanged(); break;
            case EquipmentKind.Ammo:   SaveAmmo();   break;
            case EquipmentKind.Armour: SaveArmour(); break;
        }
        _canvas.enabled = false;
        WeaponDatabase.Reload();
        ArmourDatabase.Reload();
        OnChanged?.Invoke();
    }

    private void OnDeleteClicked()
    {
        switch (_kind)
        {
            case EquipmentKind.Melee:
                WeaponSaver.SaveMeleeWeapons(
                    WeaponDatabase.GetWeaponsByType<MeleeWeaponData>().Where(w => w != _target));
                break;
            case EquipmentKind.Ranged:
                WeaponSaver.SaveRangedWeapons(
                    WeaponDatabase.GetWeaponsByType<RangedWeaponData>().Where(w => w != _target));
                break;
            case EquipmentKind.Ammo:
                WeaponSaver.SaveAmmo(WeaponDatabase.AllAmmo.Where(a => a != _target));
                break;
            case EquipmentKind.Armour:
                ArmourSaver.SaveArmours(ArmourDatabase.AllArmours.Where(a => a != _target));
                break;
        }
        _canvas.enabled = false;
        WeaponDatabase.Reload();
        ArmourDatabase.Reload();
        OnChanged?.Invoke();
    }

    private void OnCancelClicked() { _canvas.enabled = false; }

    // ================================================================== 保存処理

    private void SaveMelee()
    {
        MeleeWeaponData d = _isNew
            ? ScriptableObject.CreateInstance<MeleeWeaponData>()
            : (MeleeWeaponData)_target;
        d.name = d.weaponName = _fName.text;
        d.baseDamage      = ParseInt(_fBaseDmg);
        d.extraDamageDice = ParseInt(_fED);
        d.AP              = ParseInt(_fAP);
        d.meleeRange      = ParseInt(_fMeleeRange);
        d.weaponType      = WeaponType.Melee;
        d.traits          = ParseTraits(_fTraits.text);
        var list = WeaponDatabase.GetWeaponsByType<MeleeWeaponData>().ToList();
        if (_isNew) list.Add(d);
        WeaponSaver.SaveMeleeWeapons(list);
    }

    private void SaveRanged()
    {
        RangedWeaponData d = _isNew
            ? ScriptableObject.CreateInstance<RangedWeaponData>()
            : (RangedWeaponData)_target;
        d.name = d.weaponName = _fName.text;
        d.baseDamage      = ParseInt(_fBaseDmg);
        d.extraDamageDice = ParseInt(_fED);
        d.AP              = ParseInt(_fAP);
        d.range           = ParseInt(_fRange);
        d.salvo           = ParseInt(_fSalvo);
        d.armo            = ParseInt(_fArmo);
        d.weaponType      = WeaponType.Ranged;
        d.traits          = ParseTraits(_fTraits.text);
        var list = WeaponDatabase.GetWeaponsByType<RangedWeaponData>().ToList();
        if (_isNew) list.Add(d);
        WeaponSaver.SaveRangedWeapons(list);
    }

    private void SaveAmmo()
    {
        AmmoData d = _isNew
            ? ScriptableObject.CreateInstance<AmmoData>()
            : (AmmoData)_target;
        d.name = d.ammoName = _fName.text;
        d.baseDamage      = ParseInt(_fBaseDmg);
        d.extraDamageDice = ParseInt(_fED);
        d.AP              = ParseInt(_fAP);
        d.uses            = ParseInt(_fUses);
        d.traits          = ParseTraits(_fTraits.text);
        var list = WeaponDatabase.AllAmmo.ToList();
        if (_isNew) list.Add(d);
        WeaponSaver.SaveAmmo(list);
    }

    private void SaveArmour()
    {
        ArmourData d = _isNew
            ? ScriptableObject.CreateInstance<ArmourData>()
            : (ArmourData)_target;
        d.name = d.armourName = _fName.text;
        d.armourRating = ParseInt(_fArmourRating);
        d.traits = _fTraits.text
            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim())
            .Where(s => s.Length > 0)
            .ToList();
        var list = ArmourDatabase.AllArmours.ToList();
        if (_isNew) list.Add(d);
        ArmourSaver.SaveArmours(list);
    }

    // ================================================================== 特性 変換

    private static List<TraitEntry> ParseTraits(string text)
    {
        var result = new List<TraitEntry>();
        if (string.IsNullOrWhiteSpace(text)) return result;

        foreach (string token in text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        {
            string t = token.Trim();
            if (t.Length == 0) continue;

            if (t.StartsWith("StatusEffect:", StringComparison.OrdinalIgnoreCase))
            {
                string rest    = t.Substring("StatusEffect:".Length);
                int    rating  = 0;
                string effName = rest;
                int    pOpen   = rest.IndexOf('(');
                if (pOpen >= 0)
                {
                    effName = rest.Substring(0, pOpen).Trim();
                    int pClose = rest.IndexOf(')', pOpen);
                    if (pClose > pOpen)
                        int.TryParse(rest.Substring(pOpen + 1, pClose - pOpen - 1), out rating);
                }
                result.Add(new TraitEntry { trait = WeaponTrait.StatusEffect, rating = rating, statusEffectName = effName });
                continue;
            }

            string traitName   = t;
            int    traitRating = 0;
            int    p = t.IndexOf('(');
            if (p >= 0)
            {
                traitName = t.Substring(0, p).Trim();
                int close = t.IndexOf(')', p);
                if (close > p)
                    int.TryParse(t.Substring(p + 1, close - p - 1), out traitRating);
            }

            if (Enum.TryParse(traitName, out WeaponTrait traitEnum))
                result.Add(new TraitEntry { trait = traitEnum, rating = traitRating, statusEffectName = string.Empty });
            else
                Debug.LogWarning($"[EquipmentEditDialog] 未知の特性: '{traitName}'");
        }
        return result;
    }

    private static string TraitsToString(List<TraitEntry> traits)
    {
        if (traits == null || traits.Count == 0) return string.Empty;
        return string.Join(", ", traits.Select(t =>
        {
            if (t.trait == WeaponTrait.StatusEffect)
            {
                string s = $"StatusEffect:{t.statusEffectName}";
                return t.rating > 0 ? $"{s}({t.rating})" : s;
            }
            return t.rating > 0 ? $"{t.trait}({t.rating})" : t.trait.ToString();
        }));
    }

    // ================================================================== UI ヘルパー

    /// <summary>
    /// Row1 用: ラベルを上、InputField を下に積んだコンパクトな縦コンテナ。
    /// flexW > 0 のとき残りスペースを均等配分、prefW > 0 のとき固定幅。
    /// </summary>
    private static InputField AddCompactField(GameObject row, string label,
                                               string placeholder, float flexW, float prefW)
    {
        GameObject cell = new GameObject("CF_" + label);
        cell.transform.SetParent(row.transform, false);
        cell.AddComponent<RectTransform>();
        LayoutElement le = cell.AddComponent<LayoutElement>();
        le.flexibleWidth  = flexW;
        le.preferredWidth = prefW;
        VerticalLayoutGroup vlg = cell.AddComponent<VerticalLayoutGroup>();
        vlg.spacing              = 3f;
        vlg.padding              = new RectOffset();
        vlg.childForceExpandWidth  = true;
        vlg.childForceExpandHeight = false;
        vlg.childAlignment        = TextAnchor.LowerLeft;

        // ラベル
        GameObject lblObj = new GameObject("Label");
        lblObj.transform.SetParent(cell.transform, false);
        lblObj.AddComponent<RectTransform>();
        lblObj.AddComponent<LayoutElement>().preferredHeight = 16f;
        Text lbl = lblObj.AddComponent<Text>();
        lbl.text      = label;
        lbl.font      = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        lbl.fontSize  = 12;
        lbl.color     = new Color(0.72f, 0.78f, 0.90f);
        lbl.alignment = TextAnchor.LowerLeft;

        // InputField
        return MakeInputField(cell, placeholder, 15, 36f);
    }

    /// <summary>
    /// Row2 用: ラベルを左、InputField を右に配置した全幅フィールド。
    /// </summary>
    private static InputField AddFullWidthField(GameObject parent, string label, string placeholder)
    {
        GameObject row = new GameObject("FW_" + label);
        row.transform.SetParent(parent.transform, false);
        row.AddComponent<RectTransform>();
        row.AddComponent<LayoutElement>().preferredHeight = 40f;
        HorizontalLayoutGroup hlg = row.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing              = 8f;
        hlg.padding              = new RectOffset();
        hlg.childForceExpandHeight = true;
        hlg.childForceExpandWidth  = false;
        hlg.childAlignment        = TextAnchor.MiddleLeft;

        GameObject lblObj = new GameObject("Label");
        lblObj.transform.SetParent(row.transform, false);
        lblObj.AddComponent<RectTransform>();
        lblObj.AddComponent<LayoutElement>().preferredWidth = 50f;
        Text lbl = lblObj.AddComponent<Text>();
        lbl.text      = label;
        lbl.font      = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        lbl.fontSize  = 14;
        lbl.color     = new Color(0.72f, 0.78f, 0.90f);
        lbl.alignment = TextAnchor.MiddleLeft;

        GameObject inputContainer = new GameObject("InputBg");
        inputContainer.transform.SetParent(row.transform, false);
        inputContainer.AddComponent<LayoutElement>().flexibleWidth = 1f;

        return MakeInputField(inputContainer, placeholder, 14, 0f);
    }

    /// <summary>親に InputField を構築してアタッチ。height=0 は stretch を意味する。</summary>
    private static InputField MakeInputField(GameObject parent, string placeholder, int fontSize, float height)
    {
        parent.AddComponent<Image>().color = new Color(0.20f, 0.24f, 0.36f);
        InputField inputField = parent.AddComponent<InputField>();
        if (height > 0f)
        {
            LayoutElement le = parent.GetComponent<LayoutElement>() ?? parent.AddComponent<LayoutElement>();
            le.preferredHeight = height;
        }

        GameObject phObj = new GameObject("Placeholder");
        phObj.transform.SetParent(parent.transform, false);
        Text ph = phObj.AddComponent<Text>();
        ph.text      = placeholder;
        ph.font      = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        ph.fontSize  = fontSize;
        ph.color     = new Color(0.42f, 0.42f, 0.48f);
        ph.fontStyle = FontStyle.Italic;
        ph.alignment = TextAnchor.MiddleLeft;
        RectTransform phRt = phObj.GetComponent<RectTransform>();
        phRt.anchorMin = Vector2.zero;
        phRt.anchorMax = Vector2.one;
        phRt.offsetMin = new Vector2(5f, 0f);
        phRt.offsetMax = new Vector2(-5f, 0f);

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(parent.transform, false);
        Text inputText = textObj.AddComponent<Text>();
        inputText.font               = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        inputText.fontSize           = fontSize;
        inputText.color              = Color.white;
        inputText.alignment          = TextAnchor.MiddleLeft;
        inputText.supportRichText    = false;
        inputText.horizontalOverflow = HorizontalWrapMode.Overflow;
        RectTransform tRt = textObj.GetComponent<RectTransform>();
        tRt.anchorMin = Vector2.zero;
        tRt.anchorMax = Vector2.one;
        tRt.offsetMin = new Vector2(5f, 0f);
        tRt.offsetMax = new Vector2(-5f, 0f);

        inputField.textComponent = inputText;
        inputField.placeholder   = ph;
        return inputField;
    }

    private static GameObject MakeDialogButton(GameObject parent, string label, Color bg, float width)
    {
        GameObject obj = new GameObject("Btn_" + label);
        obj.transform.SetParent(parent.transform, false);
        obj.AddComponent<Image>().color = bg;
        obj.AddComponent<Button>();
        obj.AddComponent<LayoutElement>().preferredWidth = width;

        GameObject textObj = new GameObject("Label");
        textObj.transform.SetParent(obj.transform, false);
        Text txt = textObj.AddComponent<Text>();
        txt.text      = label;
        txt.font      = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.fontSize  = 15;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color     = Color.white;
        RectTransform tr = textObj.GetComponent<RectTransform>();
        tr.anchorMin = Vector2.zero;
        tr.anchorMax = Vector2.one;
        tr.offsetMin = Vector2.zero;
        tr.offsetMax = Vector2.zero;
        return obj;
    }

    private static void StretchFull(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    private static int ParseInt(InputField f) =>
        (f != null && int.TryParse(f.text, out int v)) ? v : 0;

}
