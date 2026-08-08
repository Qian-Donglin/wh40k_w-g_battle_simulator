ここにて、作るアプリの設計について述べます。

このアプリは、Warhammer 40kのTRPGのルールである、Wrath ＆ Gloryの一部を実現した戦闘シミュレーターです。

# ゲームの設計

1. まず各キャラクターについてが1ターンの間にそれぞれの決まった優先度で動くターン制である。
2. 各キャラクターには、属性というのがある。属性は別途`docs/character/character_status_skill.md`に記載している。
3. 各キャラクターは自分のターンになると移動や攻撃ができ、HPが0になると取り除かれる。
4. 以上のものは通常モードであり、マスターモードと相互で切り替えることもできる。マスターモードではキャラクターを増加、減少させたり、キャラクターのステータスをそれぞれ変更させたりすることができる。

# マテリアル

**ここに記述していないファイルについては、まず何もコード生成しないこと。**
- キャラクターの持つパラメタは`docs/character/character_status_skill.md`にある。
- キャラクターが持つ武器は`docs/equipments/weapon.md`にある。
- キャラクターの持つ防具は`docs/equipments/armour.md`にある。
- シミュレータ本体の設計は`docs/simulator_app_design.md`
- システム全体の設計・オリジナルルール・未決定事項は`docs/system_design.md`
- 範囲攻撃（Blast/Flamer）の設計は`docs/area_attack_design.md`
- W&G公式ルールの和訳は`docs/rules/`配下（`core_rules_ja.md`＝判定、`battle_rules_ja.md`＝戦闘、`weapons_ja.md`＝武器一覧）

> `docs/old_rules/`は再編前のアーカイブ。**参照・編集しないこと**（正は上記のパス）。

