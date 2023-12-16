# SerialOutputEx(暫定版)
本プラグインはED67900-5様が制作されたシリアル出力プラグイン(SerialOutput)をおーとま様が開発中のAtsEX版に適用させたものとなります。
> [!TIP]
> 主な特徴
- BVE5と6への両対応化
- 車両プラグインへの組込が不要になる
- 既存の設定ファイル(SerialOutput.xml)が利用可能

## 導入方法
1. AtsEX(入力プラグイン版)をインストールします。
インストール方法とインストーラは下記を参照してください。(おーとま様ページ)
https://automatic9045.github.io/AtsEX/

2. SerialOutputExをダウンロードします。

3. ファイルの設置
AtsEXをインストールしたフォルダ内(例：C:\Program Files\mackoy\BveTs6\Input Devices\AtsEx)に、
バージョン番号とその直下にExtentionというフォルダがありますので、その中フォルダ内に、
- SerialOutputEx.dll
- SerialOutput.xml(既存設定ファイル)
- シリアル出力エディタ.exe(任意)
を設置します。

注意：SerialOutput.dllのゾーン識別子(ファイルを右クリックし、プロパティ内下部)は許可してください。

4. BVEを起動しシナリオを開始すると、ポートの設定が正しければ出力が開始されます。

### 暫定版(将来的にやりたいこと)
- [*]出力電文のHEX出力(いそぎます)
- [ ]出力電文のバイナリ対応
- [ ]サウンド出力未対応

> [!WARNING]
> `ご使用は自己責任でお願いいたします。本内容による損害等については一切の責任を負いません。`

## 謝辞
本プラグインはED67900-5様のSerialOutputプラグインを参考に記述しております。
また、設定ファイルフォーマット互換(SerialOutput.xml)とさせていただきました。
(https://twitter.com/ED67900_5/status/1112336446994542592))
この場を借りて、厚く御礼申し上げます。

本プラグインはおーとま様のAtsEXの機能拡張を利用しております。  
(https://automatic9045.github.io/AtsEX/)
この場を借りて、厚く御礼申し上げます。
