# hotLauncherWin
 Onlinegame launcher for indiegame developers!
 
# 概要
 オンラインゲームを制作する際、ゲームのアップデーターを1から開発するのは手間になってしまいます。<br>
 そこで、必要事項を入力するだけで簡単にアップデーターを作成できる「hotLauncher」を開発しました！<br>
 Visual Studioを使ったことがない人でも、いくつかのパラメーターの設定だけで簡単に作れます。<br>
 
# ライセンス
 使用対象のゲームが完全無料(広告なし)でプレイできる場合に限り、個人・法人を問わず無料で使用できます。<br>
 自由に機能追加・削除して使用いただいて構いませんが、タイトルバーの「Powered by hotLauncher」は消さないでください。<br>
 使用報告は不要ですが、教えてくれればフォロワー70人のマイ弱小アカウントで宣伝します。<br>
 バグ修正や素敵な機能追加等をしてくださった場合はPull requestして頂けると嬉しいです。<br>
 
# 注意
 使用は自己責任でお願いします。使用したことで何が起きてもavaice_は責任を負いません。<br>
 多人数での共同開発経験はほぼ皆無なので、ソースコードの処理や書き方に癖があるかもしれません。そこはご了承ください。<br>
 「こうしたほうがいい」みたいなのがあればTwitterのDMでぜひ教えてください。<br>

# 使い方(※VS2019とC#が入っている前提)
初心者向けのやさしい解説は<a href="https://cho-ice.xyz/2021/03/18/%e3%80%90%e8%a9%b3%e8%aa%ac%e3%80%91hotlauncher%e3%81%ae%e4%bd%bf%e3%81%84%e6%96%b9/">こちら</a><br><br>
  1.リポジトリをCloneする　またはZipでダウンロードする　※Cloneすれば今後のアップデートをPullで簡単に適用できます。<br>
  2.hotLauncherWin.csprojをVS2019で開く<br>
  3.ソリューション エクスプローラーからlauncherSettings.csを開く<br>
  4.ゲーム・ダウンロード情報に関する設定項目をセットする<br>
  <b>リソースデータやinfoURLの書式については後述します</b>
    
# infoURLで指定するデータの書式について
 以下のサンプルの内容をコピペして使ってください。保存方式はtxtファイル　エンコードはUTF-8です。<br><br>
 \[VER ゲームバージョン\]<br>
 0.1beta<br>
 \[resourceURL 最新ゲームリソースのURL\]<br>
 https://f.easyuploader.app/eu-prd/upload/1234567890.zip<br>
 \[lverURL ランチャーバージョン\]<br>
 1.00<br>
 \[launcherURL 最新ランチャーのURL\]<br>
 http://res.myawesomegame.com/launcher.zip<br>
 \[newsURL ランチャーで表示するWebページのURL\]<br>
 http://res.myawesomegame.com/news.html<br>

# リソースデータの作り方
 <b>ゲームリソース</b><br>
 Unityで出力されたフォルダをそのままzip化してください。<br>
 フォルダ階層の例：resource.zip -> MyAwesomeGame -> MyAwesomeGame.exe<br>
 <b>ランチャーリソース</b><br>
 フォルダ階層は自動で判別するのでアップデートされたファイルだけ差分で入れてくれればOKです。ランチャー本体のexeはアップデート前とファイル名を変えないでください。<br>
