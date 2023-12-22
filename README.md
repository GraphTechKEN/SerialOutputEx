# SerialOutputEx(暫定版)
本プラグインはED67900-5様が制作された外部機器連動用シリアル出力プラグイン(SerialOutput)の機能を参考に、  
おーとま様が開発中のAtsEX版に適用させたものとなります。

> [!TIP]
>- BVE5.8と6の両対応
>- 車両プラグインへの組込が不要
>- 既存の設定ファイルが利用可能
>- BVE画面から連動のON/OFFの変更が可能

## 導入方法
1. AtsEX(入力デバイスプラグイン版)を[こちら](https://automatic9045.github.io/AtsEX/download/)からインストールします。(exeインストーラ版を推奨)  
AtsEXについては[こちら](https://automatic9045.github.io/AtsEX/)を参照してください。(おーとま様AtsEXページ)

2. SerialOutputExをダウンロードします。
  [こちら](https://github.com/GraphTechKEN/SerialOutputEx/releases)最新版(Lates)下部のAssetsから、「SerialOutputEx.dll」をダウンロードします。
> [!CAUTION]
> `SerialOutput.dllのゾーン識別子を削除してください。(ファイルを右クリックし、プロパティ内下部の許可をチェック)`

3. ファイルの設置  
AtsEXをインストールしたフォルダ内に、
バージョン番号とその直下にExtentionというフォルダがあります。  
> - `例：`
> - C:\Program Files\mackoy\BveTs6\Input Devices\AtsEx\1.0\Extentions
> - C:\Users\Public\Documents\AtsEx\1.0\Extensions など  

この中フォルダ内に、
  - SerialOutputEx.dll
  - SerialOutputEx.xml(既存設定ファイル名称をSerialOutputEx.xmlに変更する)
  - (任意)シリアル出力エディタ.exe(上の設定ファイルを編集するソフトウェア、入手方法は[下記](#シリアル出力設定ファイルの生成方法))  
  を設置します。  

4. BVEを起動し、設定メニューの入力プラグインのAtsEXにチェックマークを入れる(次回起動時以降は省略)

5. 外部(連動)機器が正しく接続され、ポートの設定が正しければ、連動動作が開始されます。(たぶん...)

## シリアル出力設定ファイルの生成方法
1. SerialOutputプラグイン(シリアル出力エディタ.exeを含む)を[ダウンロード](https://twitter.com/ED67900_5/status/1112336446994542592)します。(E67900-5様 Xサイト)

2. シリアル出力エディタを開き、出力先のポート情報と、出力したい項目の順番を選択します。

3. 保存をクリックすると、設定するxmlファイルを生成することができます。この時の名称は「SerialOutputEx.xml」(プラグイン名と拡張子よりも前と同じ)としてください。

### 暫定版(将来的にやりたいこと)
- [x] SerialOutput.xml設定ファイルフォーマットに対応
- [x] 出力電文のHEX出力
- [ ] 出力電文のバイナリ対応
- [ ] サウンド出力対応

> [!WARNING]
> `ご使用は自己責任でお願いいたします。本内容による損害等については一切の責任を負いません。`

## 謝辞
本プラグインはED67900-5様のSerialOutputプラグインの機能を参考に制作しております。  
また、設定ファイルフォーマット互換(SerialOutput.xml)とさせていただきました。  
この場を借りて、厚く御礼申し上げます。

本プラグインはおーとま様のAtsEXの機能拡張を利用しております。  
この場を借りて、厚く御礼申し上げます。
