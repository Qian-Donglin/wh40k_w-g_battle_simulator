# 武器ガイド（Wrath & Glory）

## 武器プロファイルの読み方

武器は以下の統計値で表される。

| 統計項目 | 説明 |
|----------|------|
| **名前** | 武器の名称 |
| **AP（装甲貫通）** | 命中時にこの値分だけ対象の装甲（Armour）を無視する。アスタリスク（*）付き値は「無敵の防護（invulnerable）」には無効 |
| **ダメージ評価** | 基本ダメージ値。多くは追加ダメージダイス（+ED）もある |
| **射程** | 有効射程。近接=1m（一部2m）、遠距離=中距離有効射程、投擲=STR×4m |
| **サルボ** | 攻撃1回あたりの射撃ボーナス数。「—」の場合は毎射撃ごとにリロード1消費 |
| **特性** | 武器の特殊能力（後述） |
| **価値** | 入手難度（Common → Uncommon → Rare → Very Rare → Unique） |
| **キーワード** | 武器タイプとファクションの分類 |

---

## リロードと弾薬

弾薬は **リロード（Reloads）** という抽象リソースで管理される。

- キャラクターはセッション開始時にリロード **3個**（またはSTR÷2の高い方）を持つ
- 通常のサルボ武器は弾薬切れを気にしなくてよい（サルボが弾薬消費を表す）
- **サルボ「—」の武器**は毎回の射撃でリロード1を消費する（例：メルタガン、フレイマーなど）
- リロードが0になると遠距離攻撃ができない

### 特殊弾薬（Special Ammunition）

特殊弾薬はリロード1個分として購入・携行する。効果はリロードが尽きるまで続く。

| 弾薬名 | 効果 | 価値 | キーワード |
|--------|------|------|------------|
| Hellfire Bolt Rounds | 有機生命体に対し+2ED、+3ED | 7 Very Rare | Imperium, Adeptus Astartes |
| Kraken Bolt Rounds | AP -2 | 7 Very Rare | Imperium, Adeptus Astartes |
| Manstopper Rounds | +1ED（Projectileキーワード武器のみ、Heavy不可） | 5 Uncommon | Imperium, Scum |
| Vengeance Bolt Rounds | Spread特性付与、カバーボーナス無視 | 7 Very Rare | Imperium, Adeptus Astartes |

### 弾薬アクセサリー

| 名前 | 効果 | 価値 | キーワード |
|------|------|------|------------|
| Ammunition Bandolier | リロード+2追加携行 | 2 Common | \<Any\> |
| Ammunition Backpack | リロード+10追加携行 | 5 Uncommon | \<Any\> |

---

## 武器特性一覧

特性の数値は「X」で表される場合、各武器ごとに固有の数値が設定される。

| 特性 | 効果 |
|------|------|
| **Agonizing** | 傷を与えるたびに被害者は1ショックも受ける。消耗状態（Exhausted）のキャラクターが命中を受けると意識不明になる |
| **Arc (X)** | 車両に非常に有効。車両への攻撃時、EDボーナスが評価値と同数追加される |
| **Assault** | 移動しながら射撃可能。Runアクション中も射撃できるが、命中難度+2 |
| **Blast (Size)** | 1回の攻撃で複数の目標にダメージを与える（Small/Medium/Large）。外れた場合は着弾点がずれる |
| **Blaze** | 炎・化学物質使用。命中した対象・物体・車両が燃え上がる |
| **Brutal** | EDダイスの各結果に+1を加える |
| **Force** | Psykerが使用する武器。Psykerキーワードがなければ特性が機能せずダメージ-2。Psykerが使うとWillpower属性を基本ダメージに追加 |
| **Heavy (X)** | 重火器。最低Strength要件あり。要件未満は攻撃に+2DNペナルティ。固定台（三脚等）への設置はアクションが必要だがペナルティ軽減 |
| **Melta** | 超高熱射撃。至近距離でEDに+1。車両・陣地にも追加+1ED |
| **Parry** | 防御姿勢。近接攻撃に対してDefenceが+1 |
| **Penetrating (X)** | ダイスがシフトされる際、評価値と同額のAPが付与される |
| **Pistol** | 近接でも射撃可能（Ballistic Skillの代わりにWeapon Skillを使用） |
| **Rad (X)** | 放射性弾薬。EDの各結果に評価値分のボーナスを加算 |
| **Rapid Fire (X)** | 至近距離で評価値と同数のボーナスダイスを攻撃ロールに追加 |
| **Sniper (X)** | 高精度狙撃。照準ボーナスが+2dになり、近接射撃ペナルティを無視。照準後、評価値分のEDボーナス |
| **Spread** | 広範囲弾薬。群れの中の敵を倒すと余剰ダメージが隣接する敵に及ぶ |
| **Steadfast** | 高信頼性設計。戦闘中、最初のコンプリケーションを1回無視。整備・修理時に+1dボーナス |
| **Supercharge** | プラズマ武器の過充電設定。+2Dの追加ダメージだが、コンプリケーション発生時に使用者が1d6モータルウーンドを受ける |
| **Toxic (X)** | 毒・感染物質配達機構。命中したキャラクターはラウンド終了時にToughness判定（DN X）。失敗でXウーンドを受ける |
| **Unwieldy (X)** | 扱いにくい武器。攻撃時にDNペナルティが評価値と同額 |
| **Waaagh!** | オーク製武器。Orkが使用すると攻撃に+1dボーナスと+1ED（軽傷か重傷状態なら）。戦闘終了かオーク回復まで持続 |
| **Warp Weapon** | ワープエネルギー武器。ダメージ評価はリストされた値かターゲットのResilience-4の高い方 |

---

## 武器アップグレード

武器にはアップグレードを最大3個まで装着可能（Distinctionを除く）。同種のアップグレード（グリップ、スコープ等）は1種1個まで。

| 名前 | 効果 | 価値 | キーワード |
|------|------|------|------------|
| Ammunition Drum | リロード+1追加携行 | 3 Common | Imperium, Scum |
| Autoloader | リロードが無料アクションになる | 5 Rare | Imperium |
| Bayonet Lug | 銃にナイフ装着。近接でKnifeプロファイル使用可（カウント外） | 1 Common | \<Any\> |
| Chain Bayonet | 近接でChain Bayonetプロファイル使用可（カウント外） | 4 Rare | Imperium, Chaos |
| Combi-Weapon | 2武器を1つに合体。1ラウンドで両方または片方を射撃できる | 6 Rare | Imperium, Chaos, Scum |
| Distinction | Intimidation判定に+1d（アップグレード数にカウントしない） | 5 Uncommon | \<Any\> |
| Duelling Grip | 遠距離または近接攻撃に+1d（ピストルと片手近接のみ） | 3 Uncommon | \<Any\> |
| Gene-Grip Bio-Veritor | バイオメトリック安全装置。正規ユーザー以外が使用不可 | 5 Rare | Imperium |
| Master-Crafted | Steadfast特性付与と攻撃に+2d | 7 Very Rare | \<Any\> |
| Megathoule Accelerator | Las武器のSalvoに+2、Steadfast特性喪失 | 6 Very Rare | Imperium, Astra Militarum |
| Monoscope | 射程ペナルティを2減少 | 4 Rare | Imperium, Astra Militarum |
| Percussive Muzzle Brake | Projectile武器のSalvoを+1（Heavy不可） | 3 Uncommon | Imperium, Scum |
| Preysense Sight | 暗闇でも視認可能（熱感知光学） | 6 Rare | Imperium, Scum, \<Any\> |
| Red-Dot Sight | 遠距離攻撃に+1d | 5 Uncommon | Imperium, Scum |
| Silencer | 銃声をほぼ消音（Awareness判定に+4DNペナルティ）。Boltと実弾武器のみ、Heavy不可 | 3 Uncommon | Imperium, Scum, \<Any\> |

---

## 遠距離武器一覧（Table 6-1）

### ボルト兵器（Bolt Weapons）

| 武器名 | ダメージ | AP | 射程 | サルボ | 特性 | 価値 | キーワード |
|--------|----------|----|------|--------|------|------|------------|
| Assault Bolter | 12+2ED | -1 | 36m | 3 | Assault, Brutal | 8 Very Rare | Bolt, Imperium, Adeptus Astartes, Primaris |
| Bolt Rifle | 10+1ED | -1 | 60m | 2 | Brutal, Rapid Fire (2) | 7 Very Rare | Bolt, Imperium, Adeptus Astartes, Primaris |
| Bolt Pistol | 10+1ED | 0 | 20m | 1 | Brutal, Pistol | 4 Uncommon | Bolt, Imperium |
| Boltgun | 10+1ED | 0 | 40m | 2 | Brutal, Rapid Fire (2) | 4 Uncommon | Bolt, Imperium |
| Heavy Bolter | 12+2ED | -1 | 60m | 3 | Brutal, Heavy | 6 Uncommon | Bolt, Imperium |
| Heavy Bolt Pistol | 10+1ED | -1 | 24m | 1 | Brutal, Pistol | 7 Very Rare | Bolt, Imperium, Adeptus Astartes, Primaris |
| Storm Bolter | 10+1ED | 0 | 40m | 4 | Brutal, Rapid Fire (2) | 6 Rare | Bolt, Imperium |

### 炎兵器（Flame Weapons）

| 武器名 | ダメージ | AP | 射程 | サルボ | 特性 | 価値 | キーワード |
|--------|----------|----|------|--------|------|------|------------|
| Flamer | 10+1ED | 0 | 16m | 1 | Assault, Blast (Medium), Blaze, Spread | 5 Uncommon | Fire, Imperium |
| Hand Flamer | 7+1ED | 0 | 12m | 1 | Blast (Small), Blaze, Pistol, Spread | 5 Rare | Fire, Imperium |
| Heavy Flamer | 12+2ED | -1 | 16m | 2 | Blast (Large), Blaze, Heavy, Spread | 5 Rare | Fire, Imperium |

### ラス兵器（Las Weapons）

| 武器名 | ダメージ | AP | 射程 | サルボ | 特性 | 価値 | キーワード |
|--------|----------|----|------|--------|------|------|------------|
| Duelling Laspistol | 10+1ED | 0 | 24m | 1 | Pistol | 6 Very Rare | Las, Imperium |
| Hot-Shot Lasgun | 7+1ED | -2 | 36m | 2 | Rapid Fire (1), Steadfast | 6 Very Rare | Las, Imperium, Astra Militarum |
| Hot-Shot Laspistol | 7+1ED | -2 | 12m | 1 | Pistol, Steadfast | 6 Very Rare | Las, Imperium, Astra Militarum |
| Hot-Shot Volley Gun | 10+1ED | -2 | 48m | 4 | Heavy, Steadfast | 6 Very Rare | Las, Imperium, Astra Militarum |
| Lascannon | 18+3ED | -3 | 150m | 1 | Heavy, Steadfast | 9 Uncommon | Las, Imperium |
| Lasgun | 7+1ED | 0 | 48m | 2 | Rapid Fire (1), Steadfast | 3 Common | Las, Imperium |
| Laspistol | 7+1ED | 0 | 24m | 1 | Pistol, Steadfast | 3 Common | Las, Imperium |
| Long Las | 10+1ED | 0 | 140m | 0 | Sniper (1), Steadfast | 6 Uncommon | Las, Imperium, Astra Militarum |

### メルタ兵器（Melta Weapons）

| 武器名 | ダメージ | AP | 射程 | サルボ | 特性 | 価値 | キーワード |
|--------|----------|----|------|--------|------|------|------------|
| Inferno Pistol | 16+1ED | -4 | 12m | 1 | Melta, Pistol | 6 Very Rare | Melta, Imperium, Adepta Sororitas, Adeptus Astartes |
| Meltagun | 16+2ED | -4 | 24m | 1 | Assault, Melta | 6 Uncommon | Melta, Imperium |
| Multi-Melta | 16+3ED | -4 | 48m | 1 | Heavy, Melta | 7 Rare | Melta, Imperium |

### プラズマ兵器（Plasma Weapons）

| 武器名 | ダメージ | AP | 射程 | サルボ | 特性 | 価値 | キーワード |
|--------|----------|----|------|--------|------|------|------------|
| Plasma Cannon | 15+2ED | -3 | 72m | 3 | Heavy, Supercharge | 7 Very Rare | Plasma, Imperium |
| Plasma Gun | 15+1ED | -3 | 48m | 2 | Rapid Fire (1), Supercharge | 6 Rare | Plasma, Imperium |
| Plasma Pistol | 15+1ED | -3 | 24m | 1 | Pistol, Supercharge | 6 Rare | Plasma, Imperium |

### 実弾兵器（Projectile Weapons）

| 武器名 | ダメージ | AP | 射程 | サルボ | 特性 | 価値 | キーワード |
|--------|----------|----|------|--------|------|------|------------|
| Assault Cannon | 14+2ED | -1 | 48m | 6 | Heavy | 6 Uncommon | Projectile, Imperium, Adeptus Astartes |
| Astartes Shotgun | 10+1ED | 0 | 24m | 2 | Assault, Spread, Steadfast | 7 Rare | Projectile, Imperium, Adeptus Astartes |
| Astartes Sniper Rifle | 10+1ED | 0 | 150m | 0 | Sniper (2) | 6 Uncommon | Projectile, Imperium, Adeptus Astartes |
| Autocannon | 16+1ED | -1 | 96m | 3 | Heavy | 5 Common | Projectile, Imperium |
| Autogun | 7+1ED | 0 | 48m | 3 | Rapid Fire (1) | 3 Common | Projectile, Imperium, Scum |
| Autopistol | 7+1ED | 0 | 20m | 2 | Pistol | 3 Common | Projectile, Imperium, Scum |
| Combat Shotgun | 9+1ED | 0 | 24m | 2 | Assault, Rapid Fire (1), Spread | 3 Uncommon | Projectile, Imperium |
| Hand Cannon | 9+1ED | 0 | 20m | 1 | Pistol | 4 Common | Projectile, Imperium, Scum |
| Heavy Stubber | 10+2ED | 0 | 72m | 3 | Heavy | 5 Uncommon | Projectile, Imperium, Scum |
| Shotgun | 8+1ED | 0 | 20m | 1 | Assault, Spread | 3 Common | Projectile, Imperium, Scum |
| Stubber | 7+1ED | 0 | 20m | 1 | Pistol | 2 Common | Projectile, Imperium, Scum |
| Stubcannon | 9+1ED | 0 | 30m | 1 | Brutal | 3 Common | Projectile, Imperium, Scum |

### ミサイルとミサイルランチャー（Missiles and Missile Launchers）

ミサイルランチャー自体はダメージとAPを持たず、装填するミサイルのプロファイルを使用する。

| 武器名 | ダメージ | AP | 射程 | サルボ | 特性 | 価値 | キーワード |
|--------|----------|----|------|--------|------|------|------------|
| Cyclone Missile Launcher | ミサイルに依存 | ミサイルに依存 | 150m | 1* | Heavy | 11 Very Rare | Explosive, Imperium, Adeptus Astartes |
| Missile Launcher | ミサイルに依存 | ミサイルに依存 | 150m | — | Heavy | 4 Common | Explosive, Imperium |
| Frag Missile | 10+2ED | 0 | — | — | Blast (Large) | 4 Common | Explosive, Imperium, \<Any\> |
| Krak Missile | 16+3ED | -2 | — | — | Blast (Small) | 6 Uncommon | Explosive, Imperium |

### グレネードとグレネードランチャー（Grenades and Grenade Launchers）

グレネードランチャーはグレネードのダメージ・APをそのまま使用し、射程をランチャーの値で延長する。

| 武器名 | ダメージ | AP | 射程 | サルボ | 特性 | 価値 | キーワード |
|--------|----------|----|------|--------|------|------|------------|
| Militarum Tempestus Grenade Launcher | グレネードに依存 | グレネードに依存 | 50m | 1 | Assault | 6 Uncommon | Explosive, Imperium, Astra Militarum |
| Voss Pattern Grenade Launcher | グレネードに依存 | グレネードに依存 | 40m | 1 | Assault | 5 Uncommon | Explosive, Imperium, Astra Militarum |
| Frag Grenade | 10+1ED | 0 | STR×4m（またはランチャー） | — | Blast (Medium) | 2 Common | Explosive, Imperium |
| Krak Grenade | 14+2ED | -2 | STR×4m（またはランチャー） | — | Blast (Small) | 4 Uncommon | Explosive, Imperium |
| Plasma Grenade | 10+1ED | -1 | STR×4m（またはランチャー） | — | Blast (Medium) | 7 Very Rare | Explosive, Aeldari |

### エキゾチック遠距離兵器（Exotic Ranged Weapons）

| 武器名 | ダメージ | AP | 射程 | サルボ | 特性 | 価値 | キーワード |
|--------|----------|----|------|--------|------|------|------------|
| Arc Pistol | 14+1ED | -1 | 24m | 1 | Arc (2), Pistol | 5 Rare | Arc, Adeptus Mechanicus |
| Arc Rifle | 14+1ED | -1 | 48m | 2 | Arc (2), Rapid Fire (1) | 6 Rare | Arc, Adeptus Mechanicus |
| Galvanic Rifle | 10+1ED | 0 | 60m | 2 | Rapid Fire (1), Penetrating (1) | 5 Rare | Projectile, Adeptus Mechanicus |
| Radium Carbine | 7+1ED | 0 | 36m | 3 | Assault, Rad (2) | 6 Very Rare | Projectile, Adeptus Mechanicus |
| Radium Pistol | 7+1ED | 0 | 24m | 1 | Pistol, Rad (2) | 6 Rare | Projectile, Adeptus Mechanicus |

### エルダー遠距離兵器（Eldar Ranged Weapons）

| 武器名 | ダメージ | AP | 射程 | サルボ | 特性 | 価値 | キーワード |
|--------|----------|----|------|--------|------|------|------------|
| Fusion Gun | 16+2ED | -4 | 24m | 1 | Assault, Melta | 6 Rare | Melta, Aeldari |
| Lasblaster | 7+1ED | 0 | 48m | 4 | Assault | 5 Very Rare | Las, Aeldari |
| Ranger Long Rifle | 10+1ED | 0 | 150m | 0 | Sniper (2) | 7 Very Rare | Las, Aeldari |
| Shuriken Catapult | 10+1ED | 0 | 24m | 3 | Assault, Penetrating (3) | 6 Rare | Shuriken, Aeldari, Asuryani |
| Shuriken Pistol | 10+1ED | 0 | 24m | 2 | Pistol, Penetrating (3) | 6 Rare | Shuriken, Aeldari, Asuryani |

### オーク遠距離兵器（Ork Ranged Weapons）

| 武器名 | ダメージ | AP | 射程 | サルボ | 特性 | 価値 | キーワード |
|--------|----------|----|------|--------|------|------|------------|
| Big Shoota | 12+2ED | 0 | 72m | 3 | Assault, Waaagh! | 5 Uncommon | Projectile, Ork |
| Burna | 10+1ED | 0 | 16m | 1 | Assault, Blast (Small), Blaze, Spread | 5 Uncommon | Fire, Ork |
| Rokkit Launcha | 16* | -2 | 48m | — | Blast (Small) | 7 Rare | Explosive, Ork |
| Shoota | 10+1ED | 0 | 36m | 2 | Assault, Waaagh! | 4 Uncommon | Projectile, Ork |
| Slugga | 10+1ED | 0 | 24m | 1 | Pistol, Waaagh! | 3 Common | Projectile, Ork |
| Snazzgun | 12+2ED | -2 | 48m | 3 | Heavy* | 8 Unique | Ork |
| Stikkbomb | 7+1ED | 0 | STR×4m（またはランチャー） | — | Blast (Medium) | 2 Uncommon | Explosive, Ork |

---

## 近接武器一覧（Table 6-2）

近接武器はデフォルトで射程1m。射程欄に記載のある武器はそれが交戦範囲となる。

### 基本刃物・打撃武器

| 武器名 | ダメージ | AP | 射程 | 特性 | 価値 | キーワード |
|--------|----------|----|------|------|------|------------|
| Astartes Combat Knife | 3+1ED | 0 | — | Steadfast | 3 Uncommon | Blade, Imperium, Adeptus Astartes |
| Industrial Bludgeon | 4+2ED | 0 | — | Brutal, Unwieldy (1) | 3 Uncommon | Imperium, Ork, Scum, \<Any\> |
| Knife | 2+1ED | 0 | — | — | 2 Common | Blade, Imperium, Aeldari, Ork, Scum, \<Any\> |
| Mono Knife | 3+2ED | -1 | — | Penetrating (1) | 3 Uncommon | Blade, Imperium, Scum |
| Psykana Mercy Blade | 2+1ED | -1 | — | — | 2 Uncommon | Blade, Imperium, Adeptus Astra Telepathica |
| Sword | 3+1ED | 0 | — | Parry | 3 Common | Blade, Imperium, Aeldari, \<Any\> |
| Throwing Knife | 2+1ED | 0 | STR×4m | — | 2 Common | Blade, Aeldari, Scum, \<Any\> |

### チェーン兵器（Chain Weapons）

チェーン刃による高速回転で装甲を切り裂く。

| 武器名 | ダメージ | AP | 射程 | 特性 | 価値 | キーワード |
|--------|----------|----|------|------|------|------------|
| Chain Axe | 5+2ED | 0 | — | Brutal, Penetrating (1) | 5 Rare | Chain, Chaos |
| Chain Bayonet | 4+1ED | 0 | — | Brutal | 4 Rare | Chain, Imperium, Chaos |
| Chain Fist | 7+3ED | -4 | — | Brutal, Unwieldy (3) | 10 Very Rare | Chain, Power Field, Imperium, Chaos, Adeptus Astartes |
| Chain Sword | 5+1ED | 0 | — | Brutal, Parry | 5 Uncommon | Chain, Aeldari, Imperium, Chaos |
| Eviscerator | 6+2ED | -4 | 2m | Brutal, Unwieldy (2) | 6 Rare | Chain, Adeptus Ministorum, Adepta Sororitas, Two-Handed |

### フォース兵器（Force Weapons）

Psykerがワープとの繋がりを通じて発揮する精神的エネルギーによる武器。PsykerキーワードのないキャラクターにはForce特性が機能せず、ダメージ評価が2低下する。

| 武器名 | ダメージ | AP | 射程 | 特性 | 価値 | キーワード |
|--------|----------|----|------|------|------|------------|
| Force Axe | 5+2ED | -2 | — | Force | 6 Very Rare | Force, Imperium, Inquisition, Adeptus Astartes |
| Force Hammer | 6+2ED | -3 | 2m | Force, Unwieldy (2) | 7 Very Rare | Force, Imperium, Inquisition, Two-Handed, Adeptus Astartes |
| Force Rod | 4+1ED | -1 | 2m | Brutal, Force | 6 Uncommon | Force, Imperium, Inquisition, Adeptus Astartes, Adeptus Astra Telepathica, Two-Handed |
| Force Sword | 5+1ED | -3 | — | Force, Parry | 6 Rare | Force, Imperium, Inquisition, Adeptus Astartes |

### パワー兵器（Power Weapons）

パワーフィールドに包まれた高エネルギー武器。通常の装甲を容易に切り裂く。

| 武器名 | ダメージ | AP | 射程 | 特性 | 価値 | キーワード |
|--------|----------|----|------|------|------|------------|
| Death Cult Powerblade | 5+1ED | -2 | — | Parry | 6 Very Rare | Power Field, Imperium, Adeptus Ministorum |
| Omnissian Axe | 5+2ED | -2 | 2m | — | 6 Very Rare | Power Field, Imperium, Adeptus Mechanicus, Two-Handed |
| Power Axe | 5+2ED | -2 | — | Penetrating (1) | 6 Rare | Power Field, Imperium, Adeptus Astartes, Adeptus Mechanicus, Aeldari |
| Power Fist | 7+2ED | -3 | — | Brutal, Unwieldy (2) | 8 Very Rare | Power Field, Imperium, Adeptus Astartes |
| Power Sword | 5+1ED | -3 | — | Parry | 6 Rare | Power Field, Imperium, Aeldari |
| Thunder Hammer | 8+3ED | -3 | 2m | Brutal, Unwieldy (2) | 9 Unique | Power Field, Imperium, Adeptus Astartes, Inquisition, Two-Handed |
| Void Sabre | 5+1ED | -3 | — | Brutal, Parry | 8 Very Rare | Power Field, Aeldari, Anhrathe |

### エキゾチック近接兵器（Exotic Melee Weapons）

| 武器名 | ダメージ | AP | 射程 | 特性 | 価値 | キーワード |
|--------|----------|----|------|------|------|------------|
| Neural Whip | 3+1ED | -2 | 4m | Agonizing | 5 Rare | Exotic, Chaos, Inquisition |
| Shock Maul | 4+2ED | -1 | — | Agonizing, Brutal | 5 Uncommon | Exotic, Imperium, Adeptus Arbites |
| Shock Whip | 4+1ED | 0 | 4m | Agonizing, Penetrating (2) | 5 Very Rare | Exotic, Scum, \<Any\> |
| Whip | 1+1ED | 0 | 4m | Agonizing | 2 Common | Primitive, Imperium, Ork |

### エルダー近接兵器（Eldar Melee Weapons）

| 武器名 | ダメージ | AP | 射程 | 特性 | 価値 | キーワード |
|--------|----------|----|------|------|------|------------|
| Singing Spear | 6+3ED | 0 | 2m（または STR×5m） | Assault, Force, Warp Weapon | 11 Unique | Force, Aeldari, Asuryani |
| Witchblade | 6+3ED | 0 | — | Force, Parry, Warp Weapon | 9 Very Rare | Force, Aeldari, Asuryani |

> **備考**：Void Sabreを所持するキャラクターは、Asuryaniキャラクターが絡む非敵対的なInteraction判定に+4DNペナルティを受ける。

### オーク近接兵器（Ork Melee Weapons）

| 武器名 | ダメージ | AP | 射程 | 特性 | 価値 | キーワード |
|--------|----------|----|------|------|------|------------|
| Big Choppa | 5+2ED | -1 | — | Waaagh! | 4 Rare | Blade, Ork, Two-Handed |
| Chain Choppa | 5+1ED | 0 | — | Brutal, Waaagh! | 5 Very Rare | Chain, Ork |
| Choppa | 3+2ED | 0 | — | Steadfast, Waaagh! | 2 Common | Blade, Ork |
| Power Klaw | 6+3ED | -3 | 2m | Brutal, Unwieldy (3) | 8 Very Rare | Power Field, Ork |
| Weirdboy Staff | 4+1ED | -1 | 2m | Force, Waaagh! | 5 Very Rare | Force, Ork, Two-Handed |
