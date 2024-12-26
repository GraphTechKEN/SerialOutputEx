# SerialOutputEx
本プラグインはED67900-5様が制作された外部機器連動用シリアル出力プラグイン(SerialOutput)の機能を参考に、  
おーとま様が開発中のBveEX(旧AtsEX)版に適用させたものとなります。

- BVE5.8と6の両方に対応(BveEXに準拠)
- 車両プラグインへの組込が不要
- DetailManagerを使用しない車両データにも一部対応
- 既存のSerialOutput設定ファイルが利用可能
- BVE画面から連動のON/OFFの変更が可能
コンテクストメニューに追加されます。(※最新版と表示内容は異なります)  
![コンテクストメニュー](https://github.com/GraphTechKEN/SerialOutputEx/blob/image/ContextMenu_BveEX.png "コンテクストメニュー")
![コンテクストメニュー2](https://github.com/GraphTechKEN/SerialOutputEx/blob/image/ContextMenu2.png "コンテクストメニュー2")  
従来通りデバッグ画面も表示できます。  
![コンソール表示](https://github.com/GraphTechKEN/SerialOutputEx/blob/image/Console.png "コンソール表示")  

## 導入方法
### 概要
1. [BveEX入力デバイスプラグインのインストール](https://github.com/GraphTechKEN/SerialOutputEx/tree/main#1-atsex%E5%85%A5%E5%8A%9B%E3%83%87%E3%83%90%E3%82%A4%E3%82%B9%E3%83%97%E3%83%A9%E3%82%B0%E3%82%A4%E3%83%B3%E3%81%AE%E3%82%A4%E3%83%B3%E3%82%B9%E3%83%88%E3%83%BC%E3%83%AB)
2. [SerialOutputExプラグインのダウンロード](https://github.com/GraphTechKEN/SerialOutputEx/tree/main?tab=readme-ov-file#2-serialoutputex%E3%83%97%E3%83%A9%E3%82%B0%E3%82%A4%E3%83%B3%E3%81%AE%E3%83%80%E3%82%A6%E3%83%B3%E3%83%AD%E3%83%BC%E3%83%89)
3. [SerialOutputプラグインとシリアル出力エディタのダウンロード](https://github.com/GraphTechKEN/SerialOutputEx/tree/main?tab=readme-ov-file#3-serialoutput%E3%83%97%E3%83%A9%E3%82%B0%E3%82%A4%E3%83%B3%E3%81%A8%E3%82%B7%E3%83%AA%E3%82%A2%E3%83%AB%E5%87%BA%E5%8A%9B%E3%82%A8%E3%83%87%E3%82%A3%E3%82%BF%E3%81%AE%E3%83%80%E3%82%A6%E3%83%B3%E3%83%AD%E3%83%BC%E3%83%89)
4. [ダウンロードしたファイルの配置](https://github.com/GraphTechKEN/SerialOutputEx/blob/main/README.md#4-%E3%83%80%E3%82%A6%E3%83%B3%E3%83%AD%E3%83%BC%E3%83%89%E3%81%97%E3%81%9F%E3%83%95%E3%82%A1%E3%82%A4%E3%83%AB%E3%81%AE%E9%85%8D%E7%BD%AE)
5. [BveEXの有効化](https://github.com/GraphTechKEN/SerialOutputEx/tree/main?tab=readme-ov-file#5-atsex%E3%81%AE%E6%9C%89%E5%8A%B9%E5%8C%96)
6. [既存連動プラグインの無効化](https://github.com/GraphTechKEN/SerialOutputEx/tree/main?tab=readme-ov-file#6-%E6%97%A2%E5%AD%98%E9%80%A3%E5%8B%95%E3%83%97%E3%83%A9%E3%82%B0%E3%82%A4%E3%83%B3%E3%81%AE%E7%84%A1%E5%8A%B9%E5%8C%96)
7. [ポート設定の確認](https://github.com/GraphTechKEN/SerialOutputEx/tree/main?tab=readme-ov-file#7-%E3%83%9D%E3%83%BC%E3%83%88%E8%A8%AD%E5%AE%9A%E3%81%AE%E7%A2%BA%E8%AA%8D)
8. [動作確認、その他](https://github.com/GraphTechKEN/SerialOutputEx/tree/main?tab=readme-ov-file#8-%E5%8B%95%E4%BD%9C%E7%A2%BA%E8%AA%8D%E3%81%9D%E3%81%AE%E4%BB%96)

## 詳細
### 1. BveEX入力デバイスプラグインのインストール
[ダウンロードページ(おーとま様BveEXページ)](https://automatic9045.github.io/AtsEX/)から、最新の入力デバイスプラグイン版をインストールします。(exeインストーラ版を推奨)
> [!TIP]
> BveEXについては同ページをご確認ください。

### 2. SerialOutputExプラグインのダウンロード
  [こちら](https://github.com/GraphTechKEN/SerialOutputEx/releases)最新版リリース下部のAssetsから、「SerialOutputEx.BveEX.zip」をダウンロードします。
> [!CAUTION]
> ダウンロード後、SerialOutputEx.BveEX.zipのゾーン識別子を削除して展開ください。(ファイルを右クリックし、プロパティ内下部の許可をチェック)  
> ![識別子の削除](https://github.com/GraphTechKEN/SerialOutputEx/blob/image/Property.png "識別子の削除")  

### 3. SerialOutputプラグインとシリアル出力エディタのダウンロード
SerialOutputプラグイン(シリアル出力エディタ.exeを含む)を[ダウンロード](https://x.com/ED67900_5/status/1773725982970859961)します。(E67900-5様 Xサイト)
※シリアル出力エディタ_V1.1.exeのファイル名を「シリアル出力エディタ.exe」にリネームし以下の5.項以降に適用させてください。

### 4. ダウンロードしたファイルの配置
> [!CAUTION]
> **BveEX版の場合**
>  
> 展開した[BveEX]フォルダごと、パブリックドキュメントに配置してください。
> 
> `パブリックドキュメントのデフォルトディレクトリ：<Userフォルダ>\Public\Documents\` Userフォルダは、`C:\ユーザーやD:\ユーザーなど`
>
> **旧AtsEX版の場合**
>
> 展開した[BveEX\Legacy\Extensions]内のファイルを、以下のAtsEXをインストールしたフォルダ内に配置してください。
>
> `デフォルトディレクトリ：<Userフォルダ>\Public\Documents\AtsEx\1.0\Extensions` Userフォルダは、`C:\ユーザーやD:\ユーザーなど`

> [!TIP]
>- BVEプログラムのインストールフォルダのInput Deviceフォルダ(例：C:\Program Files\mackoy\BveTs6\Input Devices)内のAtsEx.Caller.InputDevice.txtに上記ディレクトリが明示されています。

以下の2つのファイルを、[BveEX\2.0\Extensions]フォルダと[BveEX\Legacy\Extensions]内両方に配置してください。
1. SerialOutputEx.xml(設定ファイル、既存のxml設定ファイルはSerialOutputEx.xmlに変更。  新規生成方法は[下記の通り](#設定ファイルの生成方法))
2. シリアル出力エディタ.exe(設定ファイルを編集するソフトウェア)を配置します。
> 設置例(ディレクトリ例は旧AtsEX版なので無視してください)  
> ![ExtentionsFiles](https://github.com/GraphTechKEN/SerialOutputEx/blob/image/ExtentionsFiles.png)
> 
> 推奨の設定ファイルは同梱しています。詳細は[こちら](https://github.com/GraphTechKEN/MC53_ME38_BVE_VM/blob/main/SerialOutputEx.xml)

### 5. BveEX(AtsEX)の有効化
BVEを起動し、ホーム画面の右クリック->設定メニュー->入力プラグイン->BveEX(旧AtsEX) にチェックマークを入れる(次回起動時以降は省略)

> [!CAUTION]
>- BveEX版の場合  
> `(旧)AtsEXはチェックを外してください(何らかのエラー表示が出た場合はBVEを再起動)、他に使用しないプラグインはチェックを外してください`
>
> ![BveEX設定](https://github.com/GraphTechKEN/SerialOutputEx/blob/image/BveEXCheck.png "BveEX設定")
>- [旧AtsEX版の場合]  
![AtsEX設定](https://github.com/GraphTechKEN/SerialOutputEx/blob/image/AtsExCheck.png "AtsEX設定")  

### 6. 既存連動プラグインの無効化
車両プラグイン側で連動設定している場合(detailmodules.txtに組み込んである場合)は、#でコメントアウトするか削除します。

### 7. ポート設定の確認
外部(連動)機器が正しく接続され、ポートの設定が正しければ、連動動作が開始されます。(たぶん...バグ報告お待ちしております)。ポート番号はデバイスマネージャー等で確認してください。
ポート変更は、シナリオ選択画面を閉じた状態のホーム画面を右クリックし、[SerialOutputEx 連動]メニューから選択できます。

### 8. 動作確認、その他
車両の(パネル)データや外部機器の必要な情報別によってxml設定ファイルを切り替えて使用します。(このあたりの運用は今後改良予定です)

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

本プラグインはおーとま様のBveEX(旧AtsEX)の機能拡張を利用しております。  
また、コードのサポートしていただきました。
この場を借りて、厚く御礼申し上げます。

またBVE開発に携わられている皆さま、日頃よりご指導いただいている皆さまに、心より感謝申し上げます。
