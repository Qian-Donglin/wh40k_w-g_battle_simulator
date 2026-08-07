キャラクターの持つステータスをここで記述する。

- `int tier`: [1, 5]の値をとるキャラクターの格。PCのみ存在し、NPCは空である。
- `int rank`: tierの中でのキャラの格。PCのみ存在し、NPCは空である。
- `int total_exp`: PCの持っている経験値の上限値。NPCは空である。
- `int used_exp`: PCの使った経験値の上限値。NPCは空である。
- 基礎ステータス
    - `int strength`: 力の強さ。
    - `int toughness`: 頑丈さ
    - `int agility`: 敏捷性
    - `int initiative`: 戦闘で先手を取れるかのイニシアチブ。
    - `int willpower`: 精神力
    - `int intellect`: 知力
    - `int fellowship`: 交友
- Skills: 基礎ステータスに依存しているスキル。このスキル値と(装備などの補正後の)基礎ステータスを足したものが最終的な判定に使われる。
    - `int athletics`: STR依存。運動能力。
    - `int ballisticSkill`: Agi依存。射撃能力。
    - `int pilot`: Agi依存。運転能力。
    - `int stealth`: Agi依存。ステルス能力。
    - `int weaponSkill`: Init依存。近接戦闘能力。
    - `int intimidation`: Will依存。威圧力。
    - `int leadership`: Will依存。リーダーシップ。
    - `int psychicMastery`: Will依存。サイキック能力。
    - `int survival`: Will依存。過酷な環境でのサバイバル。
    - `int awareness`: Int依存。受動的な状況に対する警戒スキル。
    - `int investigation`: Int依存。調査スキル。
    - `int medicae`: Int依存。医療スキル。
    - `int scholar`: Int依存。知識スキル。
    - `int tech`: Int依存。技術系のスキル。
    - `int cunning`: Fel依存。交友関係における嘘を看破するスキル。
    - `int deception`: Fel依存。交友関係において嘘をつくスキル。
    - `int insight`: Fel依存。交友関係における心の中を読むスキル。
    - `int persuasion`: Fel依存。人を説得するスキル。
- Trait: 基礎ステータスなどから計算される、キャラの指標。
    - `int defense`: 攻撃に対する回避値。
    - `int resilience`: 攻撃に対する防御値。
    - `int determination`: 攻撃によるHPダメージを一時的HPに転移する力。
    - `int tempWounds, maxTempWounds`: 一次的HP。ここに傷がとどまると軽傷を意味する。
    - `int wound, maxWound`: HP値。
    - `int shock, maxShock`: ショック値。精神的な体力に当たる。
    - `int speed`: スピード。
    - `int conviction`: 精神的なダメージに対して軽減する能力。
    - `int resolve`: 
    - `int corruption`: 混沌による汚染度。
    - `int influence`: 社会的な権力やコネ。
    - `int wealth`: 個人の持つ資産。
    - もともとある`passiveAwareness`は`awareness`と被るので消した。

キャラクターはそれぞれweaponを複数持つことができる。

## Traitの計算

Traitは基礎ステータスから自動で計算される。

- `defense`: Initiative(I) - 1
- `resilience`: Toughness(T) + 1 + 装甲のAR（防具なしならT+1のみ）
- `determination`: Toughness(T)と同値。フリーアクションとして使用し、ダイスプールはToughness値と同数のd6を振る
- `maxWound`: Toughness(T) × 2
- `maxTempWounds`: Toughness(T)
- `maxShock`: Willpower(Wil) 
- `speed`: 種族によって異なる固定値（人間は基本6）　なので自動計算はなし。
- `conviction`: Willpower(Wil)と同値。コラプションテストのダイスプールに使用
- `resolve`: Willpower(Wil) - 1
- `corruption`: 初期値0。コラプションテスト失敗などで増加。5ごとにCorruption Levelが1上昇し変異テストが発生
- `influence`: Fellowship(Fel) - 1
- `wealth`: Tierと同値（キャラクター作成時の初期値）


Wounds, shockはもともとTier補正がついていた。これを後で何かしらの形で保障することを忘れない。
