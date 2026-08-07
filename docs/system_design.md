# WH40K TRPG システム設計書
## Unity プロジェクト引継ぎ文書

**バージョン**: 0.1（初期設計フェーズ）  
**ステータス**: コア設計確定・実装前

---

## 1. プロジェクト概要

### 目的
Warhammer 40,000世界観のTRPGシステムをUnityで実装する。
複数プレイヤーがそれぞれキャラクターを所持し、帝国のために戦う。
Space Marineを中心としつつ、他種族・他キャラタイプにも拡張できるフレームワーク。

### プレイスタイルのイメージ
- Space Marineを複数人（Kill-team規模）で運用
- 個体敵との対抗ロール、Horde（大群）への一掃戦闘の両方
- キャラクターを複数セッションにわたって育てる（思い入れを持てる）

---

## 2. ベースシステム：Wrath & Glory（改造版）

### 採用理由
- WH40K公式TRPGのため世界観・用語・敵データを流用しやすい
- d6ダイスプールはWarhammer卓上ゲームとの親和性が高い
- Space Marine〜一般人まで同一フレームワークで扱える

### W&G オリジナルの判定構造（参照）
```
ダイスプール数 = Attribute + Skill
d6を振る → 4以上の目 = 成功1個
Wrathダイス（1個）: 出目1=Complication, 出目6=Glory（特別演出）
目標成功数（DN）に到達したら判定成功
対抗判定：両者が成功数を出し、多い方が勝ち
```

---

## 3. コアルール改造：自動成功システム【撤廃】

> **撤廃済み**：本セクションで検討していた「自動成功（Auto-Success）」システムは廃止した。
>
> - **個々のキャラクター（PC/NPC）の判定**：素のWrath & Gloryのルールに戻す。ダイスプール（Attribute + Skill）とWrathダイスのみで判定する（`docs/rules/core_rules_ja.md` 参照）。Tierによる強さの差は自動成功という追加ボーナスではなく、プールサイズ（属性値そのもの）で表現する。どんなに強弱差があるマッチアップでも、ダイスの結果だけで決まるため理論上は勝ち目が残る。
> - **Hordeとの戦闘**：Hordeの強度を「自動成功値」として扱う対抗判定方式も撤廃した。Hordeの補正方式は別途（別セッションで）検討中のため、以下の旧設計は参考として残すのみとし、本ドキュメントには未反映。

以下、旧設計（撤廃済み・参考用）。

<details>
<summary>旧設計：自動成功システム（撤廃済み）</summary>

### 設計方針（旧）
オリジナルW&Gのダイスプールに「**自動成功（Auto-Success）**」を追加する。
自動成功とは、**ダイスを振る前から確定している成功数**のこと。

### 自動成功の算出（旧・廃案）

| Power Tier | 対象例 | 自動成功 |
|---|---|---|
| T1：一般市民 | 民間人・雑兵 | **+1** |
| T2：訓練済み人間 | Guardsman・傭兵 | **+2** |
| T3：エリート人間 | Inquisitor・Assassin | **+4** |
| T4：Space Marine | 標準Astartes | **+6** |
| T5：伝説のAstartes | Chapter Master・1st Company | **+9** |
| T6：Primarch級 | Daemon Prince・Legion Champion | **+14** |

### 判定の全体式（旧）

```
最終成功数 = 自動成功（Tier依存） + ダイス成功数（4d6以上の個数）

対抗判定：
　攻撃側 最終成功数  vs  防御側 最終成功数
　→ 高い方が勝ち
　→ 差の大きさ = Margin（余裕度）、演出・ダメージに活用
```

</details>

### Horde（大群）との戦闘【要再検討】

> **撤廃済み**：Hordeの強度を「自動成功値」として扱う対抗判定方式は廃止した。Marine側の判定も上記の通り素のW&Gダイスプールに戻るため、以下の「自動成功」を前提とした記述は旧設計であり、別セッションでの補正方式の検討結果を踏まえて改めて書き直す必要がある。

以下、旧設計（撤廃済み・参考用）。

<details>
<summary>旧設計：Hordeの対抗判定（撤廃済み）</summary>

Hordeには個別ロールをさせず、**固定の自動成功値＝Hordeの強度（Magnitude）**として扱う。
Marine側だけロールし、Hordeの固定値を成功数で上回れば優勢。

```
HordeはMagnitude値を持つ（例：Magnitude 10）
Marineの最終成功数 - Horde自動成功 = Margin
Margin > 0 → Marine優勢、MarginをそのままMagnitudeへのダメージとして適用
```

</details>

---

## 4. キャラクターデータ構造

### 確定フィールド

```json
{
  "id": "uuid",
  "name": "Brother Cassius",
  "faction": "Space Marine",
  "chapter": "Ultramarines",
  "specialty": "Tactical",

  "attributes": {
    "Strength":     6,
    "Agility":      5,
    "Toughness":    6,
    "Intellect":    4,
    "Willpower":    5,
    "Fellowship":   3,
    "Initiative":   5
  },

  "skills": {
    "Athletics":    2,
    "Ballistic":    4,
    "WeaponSkill":  3,
    "Medicae":      0,
    "Stealth":      1,
    "Scholar":      1
  },

  "combat": {
    "wounds":       14,
    "maxWounds":    14,
    "shock":        5,
    "maxShock":     5,
    "defence":      3,
    "resilience":   8,
    "armour":       4
  },

  "traits": ["Adeptus Astartes", "Unnatural Toughness", "And They Shall Know No Fear"],
  "equipment": ["Bolter", "Chainsword", "Power Armour MkVII"],

  "experience": 1200,
  "renown": "Initiate"
}
```

> **未決定事項（方向性のみ決定）**：Specialtyごとの固有アビリティ、Chapterボーナスは**特性（Trait）ベースで実現する方針**。Chapterボーナスは**キャラクターのTierに応じてスケールさせる**（Tierが上がるほど恩恵が大きくなる）。具体的な特性の中身・発動形式（パッシブ／アクティブ／トリガー等）およびスケール式は、Trait設計を行う際にまとめて設計する。

---

## 5. Unity プロジェクト構成

### 推奨ディレクトリ構成

```
Assets/
├── Data/
│   ├── Characters/          # ScriptableObject: CharacterTemplate
│   ├── Traits/              # ScriptableObject: TraitDefinition
│   ├── Weapons/             # ScriptableObject: WeaponDefinition
│   └── Rules/               # JSON: RuleParams, TierTable, HordeTable
├── Scripts/
│   ├── Core/
│   │   ├── CharacterData.cs       # キャラクターのランタイムモデル
│   │   ├── DiceRoller.cs          # d6プール判定
│   │   └── RuleResolver.cs        # 対抗判定・成功数計算
│   ├── Combat/
│   │   ├── CombatManager.cs       # ターン管理・State Machine
│   │   ├── CombatUnit.cs          # 戦闘中エンティティ
│   │   ├── HordeUnit.cs           # Horde専用クラス
│   │   └── ActionResolver.cs      # アクション効果適用
│   ├── Traits/
│   │   ├── TraitEffect.cs         # 基底クラス
│   │   ├── PassiveEffect.cs       # 常時発動
│   │   └── TriggeredEffect.cs     # 条件発動
│   ├── Network/
│   │   ├── FirebaseManager.cs     # Firebase接続
│   │   ├── CharacterSync.cs       # キャラシート同期
│   │   └── CombatSync.cs          # 戦闘状態同期
│   └── UI/
│       ├── CharacterSheetUI.cs
│       ├── DiceRollUI.cs
│       └── CombatUI.cs
└── Resources/
    └── Rules/
        ├── TierTable.json
        ├── HordeTable.json
        └── RuleParams.json
```

### コア判定の実装イメージ

```csharp
// DiceRoller.cs
public static class DiceRoller
{
    public static int RollPool(int diceCount)
    {
        int successes = 0;
        for (int i = 0; i < diceCount; i++)
            if (Random.Range(1, 7) >= 4) successes++;
        return successes;
    }

    public static int Resolve(CharacterData actor, CharacterData target, string skillName)
    {
        int actorPool  = actor.GetAttribute(skillName) + actor.GetSkill(skillName);
        int targetPool = target.GetAttribute(skillName) + target.GetSkill(skillName);

        int actorTotal  = RollPool(actorPool);
        int targetTotal = RollPool(targetPool);

        return actorTotal - targetTotal; // 正 = 攻撃側勝利、値 = Margin
    }
}
```

> **撤廃済み**：自動成功システムの撤廃に伴い、`TierTable.json`（Tierごとの自動成功値テーブル）は不要になった。

---

## 6. バックエンド構成（Firebase）

### データ設計

```
Firestore（永続データ）
  /campaigns/{campaignId}/
      characters/{characterId}    ← プレイヤーのキャラシート
      npcs/{npcId}                ← GM管理の敵・NPC

Realtime Database（セッション中の揮発データ）
  /sessions/{sessionId}/
      combatState/
          currentTurn: "player_uuid"
          round: 3
          units/{unitId}/
              currentWounds: 8
              currentShock: 2
              activeEffects: [...]
          log: [...]
```

### 権限設計（Security Rules 方針）
- プレイヤーは自分の `characterId` のみ書き込み可
- GM（ホスト）は全キャラ読み書き可
- `combatState` はGMのみ書き込み可、全員読み取り可

---

## 7. 未決定事項（今後詰めること）

| 項目 | 現状 | 優先度 |
|---|---|---|
| 自動成功の算出式 | **撤廃済み**：個人判定は素のW&Gダイスプールに戻す（§3参照） | 解決済み |
| スキルリストの確定 | **解決済み**：18スキルを確定。Intimidation・LeadershipはFel依存に修正し公式準拠とした（`docs/character/character_status_skill.md`参照） | 解決済み |
| Margin→ダメージ変換表 | **該当なし**：通常攻撃は素のW&Gのシフト/ED変換で完結するため専用の変換表は不要。Horde戦でのMargin活用は別途検討中のHorde補正方式側の話（`docs/rules/battle_rules_ja.md` 7章・14章参照） | 解決済み |
| Specialty固有アビリティ | 方向性決定：特性（Trait）ベースで実現。詳細設計は将来のタスクとして保留 | 低（先送り） |
| ChapterボーナスのTier反映 | 方向性決定：Tierに応じてスケールさせる。特性（Trait）ベースで実現し、詳細はTrait設計時にまとめて行う | 低（先送り） |
| Wrathダイス（W&G特有）採用有無 | **決定**：採用する（個人判定を素のW&Gに戻したため、標準ルール通り使用） | 解決済み |
| Hordeのルート条件 | **解決済み**：別セッションにて決定済み（Horde補正方式とあわせて本ドキュメントへ反映予定） | 解決済み |
| キャラクター成長（XP→何が上がるか）| 未設計。方針も未検討のため先送り | 低（先送り） |

---

## 8. 直近の実装ステップ（推奨順）

1. **CharacterData.cs** + JSON読み込み → キャラシートをUnity上で表示
2. **DiceRoller.cs** → 対抗判定ロジックの単体テスト
3. **Firebase接続** → キャラシートの保存・読み込み
4. **CombatManager.cs** → ターン管理のState Machine骨格
5. **HordeUnit.cs** → Magnitude式の戦闘解決
6. **UI** → キャラシート画面・ダイスロール演出

---

*このドキュメントはセッション設計を継続するための引継ぎ資料です。未決定事項はプレイテストを通じて随時更新してください。*
