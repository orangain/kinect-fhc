Kinect-FHC
==========

[Future Home Controller](http://rti-giken.jp/) の音声認識にKinectを使う試み。


実行方法
========

```
Kinect-FHC.exe [OPTIONS] HOST APIKEY 呼びかけ

Options:
      --threshold=THRESHOLD  確信度の閾値：この閾値以上の確信度が得られた時にマッチしたものとする。デフォルトは0.7
      --sound=FILE           サウンドファイル名：音声認識に成功した時に再生する音。FHC上に存在する必要がある。デフォルトは何も再生しない。
```

* HOST: フューチャーホームコントローラーのホスト（IPアドレス）
* APIKEY: フューチャーホームコントローラーのAPIキー
* 呼びかけ: 呼びかけの言葉

例：
```
Kinect-FHC.exe 192.168.1.5 webapi_XXXXX ジュイス
```

ビルド環境
==========

* Visual Studio
  * Visual Studio Express 2012 for Windows Desktop
* 対象のフレームワーク
  * .NET Framework 4.5
* 依存するライブラリ
  * Kinect for Windows SDK 1.6
  * Speech Platform SDK 11.0
  * Json.NET (NuGetでインストール)
  * log4net (NuGetでインストール)
  * NDesk.Options (NuGetでインストール)
