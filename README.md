# SerialOutputEx
本プラグインはED67900-5様が制作された外部機器連動用シリアル出力プラグイン(SerialOutput)の機能を参考に、  
おーとま様が開発中のAtsEX版に適用させたものとなります。

> [!TIP]
>- BVE5.8と6の両方に対応(AtsEXに準拠)
>- 車両プラグインへの組込が不要
>- DetailManagerを使用しない車両データにも対応
>- 既存の設定ファイルが利用可能
>- BVE画面から連動のON/OFFの変更が可能
>   
> コンテクストメニューに追加されます。  
> ![コンテクストメニュー](https://github.com/GraphTechKEN/SerialOutputEx/blob/image/ContextMenu.png "コンテクストメニュー")  
> 従来通りデバッグ画面も表示できます。  
> ![コンソール表示](https://github.com/GraphTechKEN/SerialOutputEx/blob/image/Console.png "コンソール表示")  

## 導入方法

> 1. AtsEX入力デバイスプラグインをインストール
> 2. SerialOutputExプラグインをダウンロード
> 3. SerialOutputプラグイン+設定ツールをダウンロード
> 4. 設定ファイルをダウンロードし、2と3のファイルを配置
> 5. AtsEXを有効化
> 6. 車両側連動プラグインの無効化(過去に連動していた場合)
> 7. ポート設定の確認


1. AtsEX(入力デバイスプラグイン版)を[こちら](https://automatic9045.github.io/AtsEX/)より入手します。(おーとま様AtsEXページ)
- ダウンロードページから、最新の入力デバイスプラグイン版をインストールします。(exeインストーラ版を推奨)
- AtsEXについては同ページをご確認ください。

2. SerialOutputExをダウンロードします
  [こちら](https://github.com/GraphTechKEN/SerialOutputEx/releases)最新版リリース下部のAssetsから、「SerialOutputEx.zip」をダウンロードします。
> [!CAUTION]
> `ダウンロード後、SerialOutputEx.zipのゾーン識別子を削除して展開ください。(ファイルを右クリックし、プロパティ内下部の許可をチェック)`  
> ![識別子の削除](https://github.com/GraphTechKEN/SerialOutputEx/blob/image/Property.png "識別子の削除")  

3. SerialOutputをダウンロードします
SerialOutputプラグイン(シリアル出力エディタ.exeを含む)を[ダウンロード](https://x.com/ED67900_5/status/1773725982970859961)します。(E67900-5様 Xサイト)
> [!TIP]
> シリアル出力エディタ_V1.1.exeのファイル名を「シリアル出力エディタ.exe」にリネームし以下の5.項以降に適用させてください。

4. ファイルの設置  
AtsEXをインストールしたフォルダ内に、
バージョン番号(1.0)とその直下にExtentionsというフォルダがあります。  
> `デフォルト：`
>- `<Userフォルダ>\Public\Documents\AtsEx\1.0\Extensions` など
>- Userフォルダは、`C:\ユーザーやD:\ユーザーなど`

> [!TIP]
>- BVEプログラムのインストールフォルダのInput Deviceフォルダ(例：C:\Program Files\mackoy\BveTs6\Input Devices)内のAtsEx.Caller.InputDevice.txtに上記ディレクトリが明示されています。その中のバージョン番号(1.0)の直下(Extensions)の中に配置します。

この中フォルダ内に、
>- SerialOutputEx.dll
>- SerialOutputEx.xml(設定ファイル、既存のxml設定ファイルはSerialOutputEx.xmlに変更。  新規生成方法は[下記の通り](#設定ファイルの生成方法))
>- シリアル出力エディタ.exe(設定ファイルを編集するソフトウェア)  
> を設置します。  
> 設置例  
>  ![ExtentionsFiles](https://github.com/GraphTechKEN/SerialOutputEx/blob/image/ExtentionsFiles.png)
>
> 推奨の設定ファイルは同梱しています。詳細は[こちら](https://github.com/GraphTechKEN/MC53_ME38_BVE_VM/blob/main/SerialOutputEx.xml)

5. BVEを起動し、ホーム画面の右クリック->設定メニュー->入力プラグイン->AtsEX にチェックマークを入れる(次回起動時以降は省略)
![AtsEX設定](https://github.com/GraphTechKEN/SerialOutputEx/blob/image/AtsExCheck.png "AtsEX設定")  

6. 車両プラグイン側で連動設定している場合(detailmodules.txtに組み込んである場合)は、#でコメントアウトするか削除します。

7. 外部(連動)機器が正しく接続され、ポートの設定が正しければ、連動動作が開始されます。(たぶん...バグ報告お待ちしております)。ポート番号はデバイスマネージャー等で確認してください。
   ポート変更は、シナリオ選択画面を閉じた状態のホーム画面を右クリックし、[SerialOutputEx 連動]メニューから選択できます。

9. 車両の(パネル)データや外部機器の必要な情報別によってxml設定ファイルを切り替えて使用します。(このあたりの運用は今後改良予定です)

## 設定ファイルの生成方法
1. シリアル出力エディタを開き(BVEコンテクストメニューの[SerialOutputEx 設定]メニューからも開けます)、出力先のポート情報と、出力したい項目の順番を選択します。

2. 保存をクリックすると、xml設定ファイルを生成することができます。この時の名称は「SerialOutputEx.xml」(プラグイン名と拡張子よりも前と同じ)としてください。

### 暫定版(将来的にやりたいこと)
- [x] SerialOutput.xml設定ファイルフォーマットに対応
- [x] 出力電文のHEX出力
- [ ] 出力電文のバイナリ対応
- [ ] サウンド出力対応(準備中)
- [ ] xml設定ファイルの簡易切り替え

> [!WARNING]
> `ご使用は自己責任でお願いいたします。本内容による損害等については一切の責任を負いかねます。`

## 参考
Arduinoなどのマイコンなどで連動機器を開発する際は[こちら](https://github.com/GraphTechKEN/MC53_ME38_BVE_VM#mc53-me38_bve_vm)を参考にしていただくと
連動ガジェットなどが製作できるかもしれません。`

## 謝辞
本プラグインはED67900-5様のSerialOutputプラグインの機能を参考に制作しております。  
また、設定ファイルフォーマット互換(SerialOutput.xml)のご了承を賜りました。  
この場を借りて、厚く御礼申し上げます。

本プラグインはおーとま様のAtsEXの機能拡張を利用しております。  
また、コードの一部をサポートしていただきました。
この場を借りて、厚く御礼申し上げます。

またBVE開発に携わられている皆さま、日頃よりご指導いただいている皆さまに、心より感謝申し上げます。
