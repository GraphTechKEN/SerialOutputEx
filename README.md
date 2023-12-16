# SerialOutputEx(暫定版)
本プラグインはED67900-5様が制作されたシリアル出力プラグイン(SerialOutput)をおーとま様が開発中のAtsEX版に適用させたものとなります。
::: note
主な特徴
:::
- BVE5と6への両対応化
- 車両プラグインへの組込が不要になる
- 既存の設定ファイル(SerialOutput.xml)が利用可能

## 導入方法
1. AtsEX(入力プラグイン版)をインストールします。
インストール方法とインストーラは下記を参照してください。(おーとま様ページ)
https://automatic9045.github.io/AtsEX/

2. SerialOutputExをダウンロードします。

3.AtsEXをインストールしたフォルダ内(例：C:\Program Files\mackoy\BveTs6\Input Devices\AtsEx)に、
バージョン番号とその直下にExtentionというフォルダがありますので、その中フォルダ内に、
- SerialOutputEx.dll
- SerialOutput.xml(既存設定ファイル)
- シリアル出力エディタ.exe(任意)
を設置します。
注意：SerialOutput.dllのゾーン識別子(ファイルを右クリックし、プロパティ内下部)は許可してください。

4.BVEを起動しシナリオを開始すると、ポートの設定が正しければ出力が開始されます。

### 暫定版(将来的にやりたいこと)
- 出力電文はアスキーのみ
- HEX出力は未対応(いそぎます)
- サウンド出力未対応
