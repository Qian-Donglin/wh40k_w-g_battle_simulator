using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// アプリ全体のルートコントローラー兼画面 1 (シミュレーション画面) の実装。
/// 起動時はタイトル画面 (画面 3) を表示し、ユーザー操作に応じて各画面へ遷移する。
/// </summary>
public class BattleSimulator : MonoBehaviour
{
    // ---- 定数 ----
    private const float MoveRadius      = 5f;
    private const float MinSeparation   = 1.2f;
    private const float CharacterRadius = 0.5f;

    private static readonly Color[] CharacterColors =
    {
        new Color(0.18f, 0.45f, 0.78f),
        new Color(0.23f, 0.65f, 0.31f),
        new Color(0.52f, 0.29f, 0.71f),
        new Color(0.85f, 0.35f, 0.20f),
        new Color(0.75f, 0.60f, 0.10f),
    };

    // ---- 画面管理 ----
    private TitleScreen     _titleScreen;
    private EquipmentScreen _equipmentScreen;
    private bool            _battleSceneInitialized;
    private bool            _battleActive;

    // ---- バトルシーン状態 ----
    private BattleState   _state;
    private TurnManager   _turnManager;

    private readonly List<CharacterView> _views = new List<CharacterView>();
    private BattleHUD             _hud;
    private CharacterStatusScreen _statusScreen;

    private GameObject _moveIndicator;
    private bool       _moveIndicatorVisible;
    private Sprite     _circleSprite;
    private Sprite     _moveCircleSprite;

    // ------------------------------------------------------------------ Unity

    private void Awake()
    {
        SetupCamera();

        // 画面 3: タイトル
        _titleScreen = gameObject.AddComponent<TitleScreen>();
        _titleScreen.Initialize();
        _titleScreen.OnSimulatorRequested += StartBattle;
        _titleScreen.OnEquipmentRequested += OpenEquipmentScreen;

        // 画面 4: 装備データベース
        _equipmentScreen = gameObject.AddComponent<EquipmentScreen>();
        _equipmentScreen.Initialize();
        _equipmentScreen.OnClosed += ReturnToTitle;

        _titleScreen.Show();
    }

    private void Update()
    {
        HandleScroll();
        HandleInput();
    }

    // ------------------------------------------------------------------ 画面遷移

    private void StartBattle()
    {
        if (!_battleSceneInitialized)
            InitBattleScene();

        _titleScreen.Hide();
        _battleActive = true;
    }

    private void ReturnToTitle()
    {
        _battleActive = false;
        if (_statusScreen != null && _statusScreen.IsVisible)
            _statusScreen.Hide();
        _titleScreen.Show();
    }

    private void OpenEquipmentScreen()
    {
        _titleScreen.Hide();
        _equipmentScreen.Show();
    }

    private void OpenStatusScreen()
    {
        _statusScreen.Show();
    }

    private void CloseStatusScreen()
    {
        // 特になし（画面 1 は常に背後にある）
    }

    // ------------------------------------------------------------------ バトルシーン初期化

    private void InitBattleScene()
    {
        CreateGridBackground();

        _circleSprite     = GenerateCircleSprite(128, Color.white, Color.white, 0);
        _moveCircleSprite = GenerateCircleSprite(128, new Color(0.2f, 0.4f, 0.9f, 0.15f),
                                                      new Color(0.2f, 0.4f, 0.9f, 0.15f), 0);

        _state       = new BattleState();
        _turnManager = new TurnManager(_state);
        _turnManager.OnTurnAdvanced  += OnTurnAdvanced;
        _turnManager.OnRoundAdvanced += OnRoundAdvanced;

        CreateDemoCharacters();
        CreateMoveIndicator();

        _hud = gameObject.AddComponent<BattleHUD>();
        _hud.Initialize();
        _hud.UpdateTurnText(_state);
        _hud.OnCharacterListRequested += OpenStatusScreen;
        _hud.OnTitleRequested         += ReturnToTitle;

        _statusScreen = gameObject.AddComponent<CharacterStatusScreen>();
        _statusScreen.Initialize(_state);
        _statusScreen.OnClosed += CloseStatusScreen;

        _battleSceneInitialized = true;
    }

    // ------------------------------------------------------------------ ターン

    private void OnTurnAdvanced()
    {
        RefreshActiveHighlight();
        _hud.UpdateTurnText(_state);
    }

    private void OnRoundAdvanced()
    {
        // ラウンド開始時の処理（将来: 状態異常のティックなど）
    }

    // ------------------------------------------------------------------ 入力

    private void HandleScroll()
    {
        if (!_battleActive) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll == 0f) return;
        Camera.main.orthographicSize = Mathf.Clamp(
            Camera.main.orthographicSize - scroll * 5f, 2f, 30f);
    }

    private void HandleInput()
    {
        if (!_battleActive) return;
        if (!Input.GetMouseButtonDown(0) || _state.Characters.Count == 0) return;

        // ステータス画面表示中は入力を受け付けない
        if (_statusScreen != null && _statusScreen.IsVisible) return;

        Vector3 worldClick = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldClick.z = 0f;

        CharacterView activeView = GetActiveView();
        if (activeView == null) return;

        bool clickedOnActive = Vector3.Distance(worldClick, activeView.transform.position) <= CharacterRadius;

        if (!_moveIndicatorVisible)
        {
            if (clickedOnActive) ShowMoveIndicator(activeView);
            return;
        }

        float dist = Vector3.Distance(worldClick, activeView.transform.position);
        if (dist <= MoveRadius && IsPositionAvailable(worldClick))
        {
            _state.ActiveCharacter.WorldPosition = new Vector2(worldClick.x, worldClick.y);
            activeView.SyncPosition();
            HideMoveIndicator();
            _turnManager.AdvanceTurn();
        }
        else
        {
            HideMoveIndicator();
        }
    }

    // ------------------------------------------------------------------ 移動インジケーター

    private void ShowMoveIndicator(CharacterView view)
    {
        float scale = MoveRadius / 0.64f;
        _moveIndicator.transform.position  = view.transform.position;
        _moveIndicator.transform.localScale = new Vector3(scale, scale, 1f);
        _moveIndicator.SetActive(true);
        _moveIndicatorVisible = true;
    }

    private void HideMoveIndicator()
    {
        _moveIndicator.SetActive(false);
        _moveIndicatorVisible = false;
    }

    private bool IsPositionAvailable(Vector3 pos)
    {
        foreach (var view in _views)
        {
            if (view == GetActiveView()) continue;
            if (Vector3.Distance(pos, view.transform.position) < MinSeparation)
                return false;
        }
        return true;
    }

    // ------------------------------------------------------------------ ビュー管理

    private CharacterView GetActiveView()
    {
        int i = _state.ActiveIndex;
        return (i >= 0 && i < _views.Count) ? _views[i] : null;
    }

    private void RefreshActiveHighlight()
    {
        for (int i = 0; i < _views.Count; i++)
            _views[i].SetActive(i == _state.ActiveIndex);
    }

    // ------------------------------------------------------------------ キャラ生成

    private void CreateDemoCharacters()
    {
        var startPositions = new Vector2[]
        {
            new Vector2(-4f,  0f),
            new Vector2( 0f,  0f),
            new Vector2( 4f,  0f),
        };

        var definitions = new (string name, CharacterStats stats)[]
        {
            ("Space Marine", new CharacterStats { strength=5, toughness=4, agility=4, initiative=3, willpower=3, intellect=3, fellowship=3 }),
            ("Guardsman",    new CharacterStats { strength=3, toughness=3, agility=3, initiative=3, willpower=3, intellect=3, fellowship=3 }),
            ("Cultist",      new CharacterStats { strength=3, toughness=2, agility=3, initiative=2, willpower=2, intellect=2, fellowship=2 }),
        };

        for (int i = 0; i < definitions.Length; i++)
        {
            NpcData data = ScriptableObject.CreateInstance<NpcData>();
            data.characterName = definitions[i].name;
            data.stats         = definitions[i].stats;
            data.traits        = new CharacterTraits { wounds = data.MaxWounds, speed = 6 };

            if (WeaponDatabase.AllWeapons.Count > i)
                data.weapons.Add(WeaponDatabase.AllWeapons[i]);

            var bc = new BattleCharacter(data, startPositions[i]);
            _state.AddCharacter(bc);

            Color color = CharacterColors[i % CharacterColors.Length];
            GameObject obj = new GameObject(data.characterName);
            obj.transform.localScale = new Vector3(CharacterRadius * 2f, CharacterRadius * 2f, 1f);

            CharacterView view = obj.AddComponent<CharacterView>();
            view.Initialize(bc, color, _circleSprite);
            view.SetActive(i == 0);
            _views.Add(view);
        }
    }

    private void CreateMoveIndicator()
    {
        _moveIndicator = new GameObject("Move Indicator");
        SpriteRenderer r = _moveIndicator.AddComponent<SpriteRenderer>();
        r.sprite       = _moveCircleSprite;
        r.sortingOrder = -1;
        _moveIndicator.SetActive(false);
    }

    // ------------------------------------------------------------------ カメラ・グリッド

    private void SetupCamera()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            GameObject go = new GameObject("Main Camera");
            cam = go.AddComponent<Camera>();
            cam.tag = "MainCamera";
        }
        cam.backgroundColor    = Color.white;
        cam.clearFlags         = CameraClearFlags.SolidColor;
        cam.orthographic       = true;
        cam.orthographicSize   = 10f;
        cam.transform.position = new Vector3(0f, 0f, -10f);
    }

    private void CreateGridBackground()
    {
        const float gridSize  = 1f;
        const float lineWidth = 0.05f;
        Color gridColor = new Color(0.7f, 0.7f, 0.7f);

        Camera cam = Camera.main;
        float vExt = cam.orthographicSize * 2f + gridSize;
        float hExt = cam.orthographicSize * cam.aspect * 2f + gridSize;

        Sprite lineSprite = GenerateLineSprite();
        GameObject parent = new GameObject("Grid Background");
        parent.transform.position = new Vector3(0f, 0f, 5f);

        int vCount = Mathf.RoundToInt(hExt * 2f / gridSize) + 1;
        float xStart = -Mathf.Floor(hExt / gridSize) * gridSize;
        for (int i = 0; i < vCount; i++)
        {
            CreateGridLine(parent, lineSprite, gridColor,
                new Vector3(xStart + i * gridSize, 0f, 0.01f),
                new Vector3(lineWidth, vExt * 2f, 1f));
        }

        int hCount = Mathf.RoundToInt(vExt * 2f / gridSize) + 1;
        float yStart = -Mathf.Floor(vExt / gridSize) * gridSize;
        for (int i = 0; i < hCount; i++)
        {
            CreateGridLine(parent, lineSprite, gridColor,
                new Vector3(0f, yStart + i * gridSize, 0.02f),
                new Vector3(hExt * 2f, lineWidth, 1f));
        }
    }

    private static void CreateGridLine(GameObject parent, Sprite sprite, Color color,
                                       Vector3 pos, Vector3 scale)
    {
        GameObject line = new GameObject("Line");
        line.transform.SetParent(parent.transform, false);
        line.transform.position   = pos;
        line.transform.localScale = scale;
        SpriteRenderer r = line.AddComponent<SpriteRenderer>();
        r.sprite       = sprite;
        r.color        = color;
        r.sortingOrder = -100;
    }

    // ------------------------------------------------------------------ スプライト生成

    private static Sprite GenerateLineSprite()
    {
        Texture2D tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
    }

    private static Sprite GenerateCircleSprite(int size, Color fill, Color border, int borderWidth)
    {
        Texture2D tex    = new Texture2D(size, size, TextureFormat.ARGB32, false);
        tex.filterMode   = FilterMode.Bilinear;
        Color clear      = new Color(0, 0, 0, 0);
        Vector2 center   = new Vector2(size * 0.5f, size * 0.5f);
        float radius     = size * 0.5f;
        float innerRadius = radius - borderWidth;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float d = Vector2.Distance(new Vector2(x, y), center);
                Color pixel = d <= innerRadius ? fill : (d <= radius ? border : clear);
                tex.SetPixel(x, y, pixel);
            }
        }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
    }
}
