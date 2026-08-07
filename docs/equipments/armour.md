# 防具ガイド（Wrath & Glory）

## 防具プロファイルの読み方

| 統計項目 | 説明 |
|----------|------|
| **Armour Rating（AR）** | 防具の防御力。Toughness に加算して Resilience を決定する |
| **特性** | 防具の特殊能力（後述） |
| **価値** | 入手難度（Common → Uncommon → Rare → Very Rare → Unique）および Influence テストの DN |
| **キーワード** | 防具タイプとファクションの分類 |

---

## 防具特性一覧

| 特性 | 効果 |
|------|------|
| **Bulk (X)** | 重くて制限の多い防具。装備者の Speed を X メートル減少させる |
| **Cumbersome** | 最大級のパワードアーマーに多い特性。装備者は走る・スプリントができない |
| **'Ere We Go!** | オーク製アーマーに多い特性。軽傷または重傷状態のオークは Bulk と Cumbersome を無視できる |
| **Force Shield** | アーキオテクの逸品。個人用フォースシールドで装備者を保護する。Mortal Wound もソーク可能。このARは無敵であり、AP によって減少しない |
| **Powered (X)** | 装備者の筋力を強化する設計のアーマー。装備者の Strength に X のボーナスを付与する。Powered アーマーを着用しているキャラクターは Heavy 武器特性を無視できる |
| **Shield** | 着用ではなく携行する防具。防御武器として wielded し、前方・側方からの攻撃に対して AR を Resilience と Defence の両方に加算する。シールドの保護範囲は GM が最終判断する。一部のシールドは無敵（ARに`*`が付く）で AP によって減少しない |

---

## 防具一覧（Table 6-4）

### 基本アーマー

| 名前 | AR | 特性 | 価値 | キーワード |
|------|----|------|------|------------|
| Bodyglove | 2 | — | 3 Rare | Light, Imperium, Adeptus Ministorum |
| Carapace Armour | 4 | Bulk (1) | 5 Uncommon | Imperium, Officio Assassinorum, Astra Militarum |
| Flak Armour | 3 | — | 4 Common | Flak, Imperium, Astra Militarum |
| Flak Coat | 3 | — | 4 Uncommon | Flak, Imperium, Astra Militarum |
| Mesh Armour | 3 | — | 3 Rare | Light, Imperium, \<Any\> |
| Primitive Armour | 2 | Bulk (2) | 2 Common | Heavy, Primitive |
| Skitarii Auto-Cuirass | 3 | — | 5 Rare | Heavy, Imperium, Adeptus Mechanicus, Skitarii |
| Tempestus Carapace | 4 | — | 6 Very Rare | Heavy, Imperium, Astra Militarum, Militarum Tempestus |

### パワードアーマー

| 名前 | AR | 特性 | 価値 | キーワード |
|------|----|------|------|------------|
| Heavy Power Armour | 6 | Bulk (1), Cumbersome, Powered (3) | 8 Very Rare | Heavy, Powered, Imperium, Inquisition |
| Ignatus Power Armour | 5 | Powered (2) | 7 Very Rare | Powered, Imperium, Inquisition |
| Light Power Armour | 4 | Powered (1) | 6 Very Rare | Powered, Imperium |
| Sororitas Powered Armour | 5 | Powered (2) | 6 Very Rare | Powered, Imperium, Adepta Sororitas |

### アスタルテスアーマー

| 名前 | AR | 特性 | 価値 | キーワード |
|------|----|------|------|------------|
| Aquila Mk VII | 5 | Powered (3) | 8 Very Rare | Powered, Imperium, Adeptus Astartes |
| Scout Armour | 4 | — | 5 Rare | Imperium, Adeptus Astartes |
| Tacticus Mk X | 5 | Powered (4) | 9 Very Rare | Powered, Imperium, Adeptus Astartes, Primaris |
| Terminator Armour | 7 | Powered (5), Cumbersome | 10 Unique | Powered, Imperium, Adeptus Astartes |

### フォースシールド

| 名前 | AR | 特性 | 価値 | キーワード |
|------|----|------|------|------------|
| Refractor Field | \*3 | Force Shield | 5 Rare | Force Field, Imperium, Astra Militarum |
| Rosarius | \*4 | Force Shield | 7 Very Rare | Force Field, Imperium, Adeptus Astartes, Adeptus Ministorum |
| Storm Shield | \*2 | Bulk (1), Force Shield, Shield | 8 Unique | Force Field, Imperium, Adeptus Astartes, Adeptus Ministorum, Inquisition |

### エルダーアーマー

| 名前 | AR | 特性 | 価値 | キーワード |
|------|----|------|------|------------|
| Corsair Armour | 3 | — | 3 Rare | Light, Aeldari, Anhrathe |
| Eldar Mesh Armour | 3 | — | 4 Very Rare | Light, Aeldari, Asuryani |
| Heavy Mesh Armour | 4 | — | 6 Very Rare | Aeldari, Anhrathe |
| Shimmershield | \*2 | Force Shield, Shield | 7 Unique | Force Field, Aeldari, Asuryani |
| Rune Armour | 4 | Force Shield | 6 Unique | Force Field, Aeldari, Asuryani |
| Voidplate Harness | 5 | Bulk (2) | 7 Rare | Aeldari, Anhrathe |

### オークアーマー

| 名前 | AR | 特性 | 価値 | キーワード |
|------|----|------|------|------------|
| 'Eavy Armour | 4 | 'Ere We Go, Bulk (1) | 3 Uncommon | Heavy, Primitive, Ork |
| Mega Armour | 7 | 'Ere We Go, Cumbersome, Powered (4) | 9 Very Rare | Powered, Ork |
| Ork Flak | 2 | — | 2 Uncommon | Primitive, Ork |

---

## Resilience の計算

```
Resilience = Toughness + 1 + Armour Rating
```

- 防具なし: `Toughness + 1`
- **Force Shield** の AR（`*` 付き）は AP によって減少しない（無敵）
- **Shield** の AR は前方・側方からの攻撃に対してのみ Resilience と Defence の両方に加算される
- AP は Resilience から装甲分を差し引く（`*` 付き AR には無効）
