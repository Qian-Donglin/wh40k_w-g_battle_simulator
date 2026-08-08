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

### Horde（大群）との戦闘【オリジナルルール】

Hordeは多数の個体をまとめて1ユニットとして扱う。個体ごとにダイスを振らせないための抽象化であり、以下のオリジナルルール群で構成される。

> **判定方式について（撤廃済み）**：かつては「Hordeの強度を**自動成功値**として扱い、PC側の最終成功数と比較する」という対抗判定方式を採っていたが、自動成功システムの撤廃（§3）に伴いこの方式も廃止した。PC側の攻撃判定は素のW&Gルール（アイコン数 vs Defence）で解決する。旧方式は本節末尾に保存してある。

#### HordeがPCを攻撃する場合

Hordeがキャラクターを攻撃する際、単純なN倍ダメージではなく、**√(2N) スケーリング**を適用する。
これは連携の限界・射線の重複・集団戦の混乱を表現するものである。

**攻撃ダメージ式**

```
Horde攻撃ダメージ = 武器の基本ダメージ + (武器の基本ED + 追加ED) 分のダイスをロール

追加EDの計算：
  N_eff    = N × (1 + leadership / 20)     ← 指揮による連携補正
  scale    = √(2 × N_eff)
  確定ED   = floor(scale)
  確率的1ED = scale の小数部の確率で追加1ダイス
```

- `N` = Horde内の総人数
- 例：N=25・leadership 0 のとき scale=7.07 → 確定+7ED、7%の確率でさらに+1ED

**leadershipによる連携補正（N_eff）**

leadershipを「実効人数を増やす係数」として扱う。√(2N)の√が「連携の限界による損失」を表している以上、
指揮が優れているほど損失が減る＝実効人数が増える、と読むのが自然なため。
**leadershipをNと同じ√の減衰に通す**ことで、加算形（`√(2N) + leadership`）のように急激に強くならない。

| leadership | 倍率 | N=25 | N=100 |
|---|---|---|---|
| 0 | ×1.000 | 7.07 | 14.14 |
| 2 | ×1.049 | 7.42 | 14.83 |
| 4 | ×1.095 | 7.75 | 15.49 |
| 6 | ×1.140 | 8.06 | 16.13 |
| 8 | ×1.183 | 8.37 | 16.73 |

- **除数20が調整ダイヤル**。指揮官キャラはたいていleadershipを持つため、意図的に控えめに置いている。
- **上限は不要**。leadership 8 でも×1.18に留まり、√が自動的に頭打ちにする。
- この式はHordeが**PCを攻撃する**側の話なので、戦意システムの較正（k=3.6、分岐点N≈28）や
  範囲攻撃の数値（`docs/area_attack_design.md`）には影響しない。独立して調整できる。

> **加算形（`scale = √(2N) + leadership`）を採らない理由**：Nは√で減衰されるのにleadershipだけ減衰なしで乗るため効きすぎる。
> N=25・leadership 2 で scale 7.07→9.07 となり、ラスガンHordeの期待ダメージが11→12、
> Resilience 8 のPCへのウーンドが3→4と**33%増**してしまう。

**スケール参考表**

| 人数 N | √(2N) | 確定追加ED | 備考 |
|--------|--------|-----------|------|
| 1      | 1.41   | +1ED      | 41%で+2ED |
| 5      | 3.16   | +3ED      | |
| 10     | 4.47   | +4ED      | |
| 25     | 7.07   | +7ED      | |
| 50     | 10.00  | +10ED     | |
| 100    | 14.14  | +14ED     | |

**例：ラスガン（7+1ED）Horde 100人の攻撃**

```
scale = √(2 × 100) = √200 ≈ 14.14
→ 確定+14ED、14%の確率でさらに+1ED
→ 攻撃ダメージ = 7 + (1 + 14)ED = 7 + 15ED ロール
```

#### BSオーバーフロー→ED変換

命中判定で余った成功数を、**ダメージEDに変換**できる。**個体キャラクター（PC・NPC）に適用し、Hordeには適用しない**。

```
余剰成功数 = 攻撃ロール成功数 - 対象のDefence
余剰成功数 1個 = 追加ED 1個（ダメージロールに加算）

【上限】シフトによる追加ED ≤ その武器の基本ED値
```

例：ボルトガン(10+1ED)→最大+1ED / ラスキャノン(18+3ED)→最大+3ED

上限を武器ED値にすることで、重火器ほど腕前が活きる挙動になり、
武器プロファイルの差別化と戦意システムの較正（10体/ターン前提）が保たれる。

**追加EDの2つの枠は排他**（どちらか一方のみが適用される）

| 枠 | 対象 | モデル化対象 | 上限 |
|---|---|---|---|
| シフト枠（余剰成功→ED） | 個体（PC・NPC） | 撃ち手の腕（BS/WS） | **武器の基本ED値** |
| 物量枠（√(2N)） | Horde | 同時に撃っている人数 | **上限なし** |

**Hordeがシフト枠を使わない理由**：物量枠に対してシフト枠は誤差であり（N=100で +14ED vs +1ED）、計算ステップに見合わない。Hordeの「質」はBSの命中率、resolve/leadership（戦意の折れにくさ）、武器プロファイル、toughness（Magnitude）で十分表現される。RAWでもTroopはWrathダイス・クリティカル・Soakを持たず、集団ユニットから機構を削ぐのはW&Gの作法（`docs/rules/battle_rules_ja.md` §3）。

> **物量枠に上限を設けない理由**：武器ED値で縛ると、ラスガン持ちHordeは何人集まっても+1EDで頭打ちになり√(2N)スケーリングが無効化される。また大群が無害になるため、戦意システムの分岐点（N≈28）の前提も崩れる。
>
> **既知の性質**：Nが大きいほどHordeの武器選択は相対的に効かなくなる（N=100ではラスガンもボルトガンも+14EDが乗り、差は基本ダメージのみ）。「大群は質より量」という表現として意図的に許容する。

> RAWでは余剰Exaltedアイコン（6の目）のみシフト可能（`docs/rules/core_rules_ja.md` §10）。
> 本ルールは変換対象を余剰成功数全体に拡張し、上限を武器ED値に付け替えたもので、
> RAWの当該規定を置き換える（併用しない）。

#### Hordeの人数（N）とMagnitude（HP）の連動

HordeのHPは「Magnitude」というプールで管理する。**Magnitudeは個体のwound × Nに相当し、Nに対して線形にスケールする**（攻撃側の√(2N)スケーリングとは別軸）。

```
Magnitude = 個体のwound（HP） × N
```

累積ダメージが個体1体分のwoundに達するたびに、Nを1減らす。Nが減ると、Hordeが攻撃する際のEDスケーリング（√(2N)）や、後述する戦意の初期値算出に使うNも連動して下がる。
これにより、削るほどHordeが弱くなる**二重の抑制**（攻撃力低下＋戦意低下）が働く。

#### Hordeへのダメージ解決（常時Spread）

```
プールへの入力 = max(0, ダメージ - 個体のResilience)     ← Resilienceは1ヒットにつき1回だけ引く
Magnitude -= プールへの入力
戦死者数    = floor(累積ダメージ / 個体のwound)           ← 端数はプールに残り、以降の攻撃へ繰り越す
N          = ceil(Magnitude / 個体のwound)
```

例：個体wound 5・Resilience 4 のHorde（N=10、Magnitude 50）に素ダメージ17
→ 入力 13 → Magnitude 37 → **2体死亡**（N=10→8）＋**余り3は3体目に入った状態で保持**

- **Hordeは常にSpread**として扱う。オーバーキル分は隣の個体へ自動的に波及し、端数は戦闘中ずっとプールに残る。
  これはプールを1本にしている以上、追加ルールなしで自動的に成立する（実装は `int magnitude` 1個でよく、Nは導出値）。
- **Resilienceは1ヒットにつき1回だけ引く**（RAWのダメージ解決と同じ）。倒すたびに個体ごとに引く方式は採らない。
  後者だと1体あたりの必要ダメージが `Resilience + wound` になって撃破速度がほぼ半減し、
  戦意システムの較正（通常兵器で10体/ターン前提、k=3.6）が崩れて分岐点N≈28がずれるため。
- **範囲攻撃（Blast / Spread 特性を持つ武器）には、この常時Spreadを適用しない。**
  巻き込んだ体数ぶんの「1ヒット」が発生する扱いとし、各ヒットは1体分のwoundで打ち切る：

  ```
  プールへの入力 = 巻き込み体数 × min(個体のwound, max(0, ダメージ - Resilience))
  ```

  面積で体数倍している上にオーバーキルまで波及させると範囲効果を二重取りするため。
  判定は巻き込み体数の値ではなく**Blast / Spread 特性を持つかどうか**で行う。詳細は `docs/area_attack_design.md`。

> **副作用**：この常時Spread化により、公式和訳のSpread特性（＝群れでのオーバーキル波及）の役割は
> Horde共通ルールが引き受ける。空いた `Spread` の特性名は「扇形の攻撃範囲」を指すものとして再定義する
> （`Assets/Scripts/Weapons/WeaponTrait.cs` の記述と一致）。詳細は `docs/area_attack_design.md` §5。

集団に対して大ダメージを与える武器（Blast/Flamer等の範囲攻撃）で、この線形なMagnitudeプールを効率的に削れるようにする。
方針は**2層構造**（①攻撃範囲の図形内にいる全キャラを対象とする幾何判定 → ②Hordeにはさらに占有領域との重なりに応じた追加削り）で確定済み。
式・パラメータ・実装は未設計。詳細は `docs/area_attack_design.md` を参照。**【方針のみ確定・式は未設計】**

---

### 統率・戦意システム（Morale / Rout）【オリジナルルール】

キャラクター種別にTierを持たせない方針（NPCはもともとTierを持たない。`docs/character/character_status_skill.md` 参照）に合わせ、Hordeの「質」（雑兵/標準/精鋭）はTierのような分類ラベルを使わず、既存ステータスの `resolve`（＝Willpower−1）と `leadership` スキルから直接算出する。

#### 戦意ゲージの初期値

```
MoraleBonus_initial = resolve × ln(1 + N)      ※ ln = 自然対数
```

数が多いほど自信は増すが、伸びは緩やか（攻撃側の√(2N)よりさらに控えめな伸び方）。数の暴力は攻撃側のスケーリングで十分表現されているため、心理的な安心感としての数の効果はここでは抑える。

> **対数の底は自然対数（ln）**とする。常用対数だとゲージが約1/2.3になり、後述の較正が崩れるため。

#### 戦意ダメージ（そのターンの戦死者数 → 戦意への影響）

戦意の減り方は、集団の総量に対する割合ではなく「**1ターンでどれだけ目に見えて崩れたか**」で決まる。
そのため分母には線形なMagnitudeではなく、**√N**（攻撃側の√(2N)スケーリングと揃えたオーダー）を使う。

```
casualtyRatio = そのターンの戦死者数 ÷ √N

MoraleDamage(そのターン) = k / (1 + exp(-s × (casualtyRatio − θ)))
```

**パラメータ確定値**

| 記号 | 値 | 役割 |
|---|---|---|
| **k** | **3.6** | 1ターンで削れる戦意ダメージの上限（頭打ち）。**殲滅 vs 戦意の分岐点を決める主ダイヤル** |
| **θ** | **0.3** | 戦意が崩れ始めるしきい値。これ未満の散発的な削りではほぼ減らない |
| **s** | **10** | θ付近の立ち上がりの鋭さ |

- θ・s は「散発的なチクチクでは折れず、集中砲火で折れる」という**burst誘導**のための値であり、後述の分岐点には影響しない。
- 1回のヒット単位ではなく**そのターンの合計**で評価することで、同じ総戦果でも1ターンに集中させた方が戦意へのダメージが大きくなる。
- θ・s・k は固定値とする（N・leadershipに応じて動的に変える拡張は今後の課題）。

#### 分岐点の設計意図（k = 3.6 の根拠）

現実的な火力では casualtyRatio が θ を大きく超えるため、シグモイドは**飽和して MoraleDamage ≈ k で一定**になる。その結果：

```
Rout までのターン数  = MoraleBonus ÷ k = resolve·ln(1+N) / k   … log オーダー（火力に依存しない）
HP削り切りターン数   = N ÷ 毎ターン戦死者数                      … 線形オーダー（火力に反比例）
```

**較正の基準**：1プレイヤーあたり通常兵器で2〜3体/ターン撃破、4人パーティ ＝ **10体/ターン**。resolve = 3。

| N | HP削り切り | Rout | 速い方 |
|---|---|---|---|
| 5 | 0.5ターン | 1.5ターン | HP |
| 10 | 1.0 | 2.0 | HP |
| 20 | 2.0 | 2.5 | HP（僅差） |
| 30 | 3.0 | 2.9 | 戦意（僅差） |
| 40 | 4.0 | 3.1 | 戦意 |
| 100 | 10.0 | 3.8 | **戦意（圧倒的）** |

**範囲攻撃の扱いは自動的に成立する**：シグモイドが飽和しているため火力を上げても Rout のターン数は変わらないが、HP削りは火力に反比例して速くなる。よってフレイマー・グレネード等を持ち込むと殲滅側だけが加速する。

| N=20 | 通常火力(10体/T) | 範囲攻撃あり(20体/T) |
|---|---|---|
| HP削り切り | 2.0ターン | **1.0ターン** |
| Rout | 2.5ターン | 2.5ターン（不変） |

なお N=100 で火力20体/ターンでも HP削り5.0ターン vs Rout 3.8ターンとなり、「大群には戦意」の原則は崩れない。

> **パーティ人数による分岐点の移動**：戦意攻めのターン数は火力に依存しないため、分岐点はパーティ規模で動く（2人→N≈10、**4人→N≈28**、6人→N≈49）。少人数ほど早く戦意戦術に依存する形になる。上表は4人パーティ基準。

#### 戦意の立て直し（統率スキルによる条件付き回復）

```
そのターンの casualtyRatio < θ のとき:
    MoraleBonus += leadership × r        （上限は MoraleBonus_initial）

r = 0.5
```

優れた指揮官は、小康状態のあいだに部隊を立て直す。

**この機構は較正済みの曲線に一切影響しない。** 発動閾値は `戦死者数 < θ × √N` なので：

| N | 回復が発動する戦死者数 |
|---|---|
| 20 | 1.3体/ターン未満 |
| 100 | 3.0体/ターン未満 |

基準火力は10体/ターンであり、**押し続けている限り発動しない**。分岐点N≈28も範囲攻撃の数値も動かない。
発動するのは、火力を分散したとき・Hordeを放置したターンがあったとき・遠距離から細く削っているときである。

これは θ・s が狙っていた**burst誘導を強化する**。「チクチクでは折れない」が「チクチクでは戻される」になる。

#### 戦意の下限（MoraleFloor）は採用しない【撤廃】

かつて `MoraleFloor = leadership × c` という下限を検討したが、**採用しない（c = 0）**。

- **戦意システムそのものを無効化するため**。Routの存在意義は「線形に膨らむMagnitudeを削り切らずに済む近道」であり、
  下限がRout判定を実質不可能にすると、そのHordeは近道が塞がれてN=100なら11.5ターンの殲滅戦に戻る。
  サブシステムが1つ丸ごと消えることになる。
- **スカラーの下限は影響が見えないため**。leadershipがいくつからRout不能になるのかが式を追わないと分からず、
  気づかないうちに崩壊しない敵ができてしまう。

#### 崩壊しない集団：`fearless` フラグ

ティラニッドのように「そもそも士気が折れない」集団は、下限ではなく**明示的なフラグ**で表現する。

```
fearless: bool     ← true なら戦意システムを一切持たず、Magnitudeのみで戦う
```

- 戦意ゲージ・戦意ダメージ・回復・Rout判定のすべてを持たない。敗北条件は `Magnitude ≤ 0` のみ。
- **フラグにする利点は、影響が設計時に見えること**。「この敵は殲滅しか道がない」と分かった上で N や wound を決められる。
  Fearless な N=100 は11.5ターンの泥沼になると承知の上で配置する、という判断が設計者の手元に残る。
- RAWでもTroopはWrathダイス・クリティカル・Soakを持たない（`docs/rules/battle_rules_ja.md` §3）。
  集団ユニットから機構ごと削ぐのはW&Gの作法に沿う。

#### leadership の3経路（まとめ）

| 経路 | 式 | 性質 |
|---|---|---|
| **立て直し** | 小康時に `MoraleBonus += leadership × 0.5` | 条件付き |
| **粘り** | Rout判定プールに加算 | ボーナスダイス |
| **連携** | `N_eff = N × (1 + leadership/20)` | √で減衰 |

いずれも**加算的な確定ボーナスではなく、条件付き・減衰つき**で入っている。
これは自動成功システムを撤廃した趣旨（確定成功を持ち込まず、どんな対決でも可能性を残す）と整合する。

なお **resolve と leadership の役割分担**は以下の通り：

| 能力 | 担当 |
|---|---|
| **resolve** | 打たれ強さ（受動）— 初期ゲージ・Rout判定プール |
| **leadership** | 立て直しと連携（能動）— 回復・Rout判定プール・攻撃連携 |

初期ゲージは resolve が担当済みのため、leadership は初期値には入れない（2つの能力値が同じ仕事をして片方が冗長になるため）。

#### 崩壊（Rout）判定

その時点の MoraleBonus を**ボーナスダイス**として加えた Resolve 判定に失敗すると、**Hordeは即座に崩壊し戦線から除去される**（Magnitudeが0になった場合と同じ「撃破」扱い）。逃走中の追跡・残党狩り・再結集などの追加処理は行わない。

```
判定プール = resolve + leadership + 現在のMoraleBonus（ボーナスダイス）
DN = 5（Challenging）
```

- **ボーナスダイスとして扱う理由**：自動成功システムを撤廃した趣旨（確定成功を持ち込まず、どんな対決でも可能性を残す）と整合させるため。
- **DN 5 の理由**：DN 3 ではゲージが枯渇してもHordeがほぼ失敗しない（プール6個で期待アイコン4個）。DN 5 ならゲージ満タン時はまず落ちず、枯渇時に五分五分となり、欲しい勾配が出る。

Hordeの敗北条件は以下の2つ：
1. **Magnitude ≤ 0**（物理的な殲滅）
2. **Resolve判定失敗による崩壊（Rout）**

<details>
<summary>旧設計：Hordeの対抗判定・Rout判定（自動成功ベース／撤廃済み・参考用）</summary>

**旧：Hordeとの対抗判定**

Hordeには個別ロールをさせず、**固定の自動成功値＝Hordeの強度（Magnitude）**として扱う。
Marine側だけロールし、Hordeの固定値を成功数で上回れば優勢。

```
HordeはMagnitude値を持つ（例：Magnitude 10）
Marineの最終成功数 - Horde自動成功 = Margin
Margin > 0 → Marine優勢、MarginをそのままMagnitudeへのダメージとして適用
```

**旧：崩壊（Rout）判定**

戦意ゲージ（その時点のMoraleBonusを自動成功として使用）を含めたResolve判定に失敗すると、**Hordeは即座に崩壊し戦線から除去される**（Magnitudeが0になった場合と同じ「撃破」扱い）。逃走中の追跡・残党狩り・再結集などの追加処理は行わない。

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
| Hordeのルート条件 | **解決済み**：別セッションで決定した内容を §3 に統合済み（Magnitude 0 または Rout判定失敗の2条件） | 解決済み |
| HordeのPC攻撃ダメージ式 | 設計済：√(2N) EDスケーリング（§3参照） | 解決済み |
| HordeのMagnitude-N連動 | 設計済：線形Magnitude、個体wound単位でN減少（§3参照） | 解決済み |
| 統率・戦意システム（Morale/Rout） | **解決済み**：√N正規化のシグモイド式。k=3.6 / θ=0.3 / s=10、対数は自然対数。4人パーティ基準で分岐点N≈28（§3参照） | 解決済み |
| Rout判定でのMoraleBonus適用方法 | **解決済み**：ボーナスダイスとして加算、DN 5（Challenging）（§3参照） | 解決済み |
| MoraleFloor の係数 c | **解決済み**：`c = 0`（下限は採用しない。戦意システム自体を無効化するため）。leadershipは代わりに「条件付き回復（r=0.5）」「Rout判定プール」「連携補正 `N_eff`」の3経路で効かせる。崩壊しない集団は `fearless` フラグで表現（§3参照） | 解決済み |
| Hordeの `fearless` フラグ | **新設**：戦意システムを持たない集団（ティラニッド等）を表す。敗北条件はMagnitude 0のみ（§3参照） | 解決済み |
| Hordeの連携補正（leadership → 攻撃） | **解決済み**：`N_eff = N × (1 + leadership/20)` を √(2N) の内側に入れる。除数20が調整ダイヤル（§3参照） | 解決済み |
| BSオーバーフロー→ED変換と素のW&Gシフトの関係 | **解決済み**：RAWの規定を置き換える（併用しない）。上限は武器の基本ED値（§3参照） | 解決済み |
| BSオーバーフロー→ED変換の適用範囲 | **解決済み**：個体（PC・NPC）のみ適用し武器ED値を上限とする。Hordeは適用外で、代わりに上限なしの物量枠（√(2N)）を使う（両枠は排他。§3参照） | 解決済み |
| 対集団武器（Blast/Flamer等の範囲攻撃）のMagnitude削り | **解決済み**：2層構造（幾何判定＋Horde占有領域との重なり）。基準密度 a=4m²、`n_hit = min(N, round(重なり面積÷a), Blast Size対象数)`、範囲攻撃は常時Spread適用外（`docs/area_attack_design.md` 参照）。残るは実装と、その前提となる戦場座標系の定義 | 解決済み |
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
