# SerialOutputEx(暫定版)
本プラグインはED67900-5様が制作された外部機器連動用シリアル出力プラグイン(SerialOutput)の機能を参考に、  
おーとま様が開発中のAtsEX版に適用させたものとなります。

> [!TIP]
> 主な特徴
>- BVE5と6への両対応化
>- 車両プラグインへの組込が不要
>- 既存の設定ファイル(SerialOutput.xml)が利用可能
- ※暫定版：十進数のアスキー送信のみ

## 導入方法
1. AtsEX(入力プラグイン版)を[こちら](https://automatic9045.github.io/AtsEX/download/)からインストールします。
AtsEXについては[こちら](https://automatic9045.github.io/AtsEX/)を参照してください。(おーとま様AtsEXページ)

2. SerialOutputExをダウンロードします。
  ダウンロードは[こちら](https://github.com/GraphTechKEN/SerialOutputEx/releases)下部のAssetsから、「SerialOutputEx.dll」をダウンロードします。

4. ファイルの設置  
AtsEXをインストールしたフォルダ内に、
バージョン番号とその直下にExtentionというフォルダがあります。  
> `例：`
> - C:\Program Files\mackoy\BveTs6\Input Devices\AtsEx\1.0\Extentions
> - C:\Users\Public\Documents\AtsEx\1.0\Extensions など

この中フォルダ内に、
- SerialOutputEx.dll
- SerialOutput.xml(既存設定ファイル)
- シリアル出力エディタ.exe(任意、上SerialOutput.xml編集用)
を設置します。  


> [!CAUTION]
> `SerialOutput.dllのゾーン識別子(ファイルを右クリックし、プロパティ内下部)は許可して削除してください。`

4. BVEを起動し、設定メニューの入力プラグインのAtsEXにチェックマークを入れる(次回起動時以降は省略)

5. 外部(連動)機器が正しく接続され、ポートの設定が正しければ、連動動作が開始されます。(たぶん...)

## シリアル出力設定ファイルの生成方法
1. SerialOutputプラグインを[ダウンロード](https://twitter.com/ED67900_5/status/1112336446994542592)します。(E67900-5様 Xサイト)

2. シリアル出力エディタを開き、出力先のポート情報と、出力したい項目の順番を選択します。

3. 保存をクリックすると、設定するxmlファイルを生成することができます。この時の名称は「SerialOutput.xml」としてください。

### 暫定版(将来的にやりたいこと)
- [x] SerialOutput.xmlに対応
- [ ] 出力電文のHEX出力(いそぎます)
- [ ] 出力電文のバイナリ対応
- [ ] サウンド出力未対応

> [!WARNING]
> `ご使用は自己責任でお願いいたします。本内容による損害等については一切の責任を負いません。`

## 謝辞
本プラグインはED67900-5様のSerialOutputプラグインの機能を参考に制作しております。  
また、設定ファイルフォーマット互換(SerialOutput.xml)とさせていただきました。  
この場を借りて、厚く御礼申し上げます。

本プラグインはおーとま様のAtsEXの機能拡張を利用しております。  　
この場を借りて、厚く御礼申し上げます。
