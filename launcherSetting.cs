﻿//--------------------------------------------------------------------
//launcherSetting.cs // ゲームに関する設定を行うためのスクリプト
//作成:avaice_    最終更新:avaice_
//Create date: 2021/03/17   Last update: 2021/03/17
//変更内容: 新規作成
//--------------------------------------------------------------------
namespace hotLauncherWin
{
    class launcherSetting
    {
        //--------------------------------------------------------------------
        //ゲーム・ダウンロード情報に関する設定項目です！必ず設定してください！
        //--------------------------------------------------------------------

        //ゲームの最新バージョンを示したtxtファイルのURL
        public static readonly string verURL = ("");
        //ゲームのバイナリデータの場所を示したtxtファイルのURL
        public static readonly string resourceURL = ("");
        //このアプリケーションのタイトル
        public static readonly string launcherName = "MyAwesomeGame Updater";
        //UnityのProject Settings→Product Name
        public static readonly string productName = "MyAwesomeProductName";

        //ゲームの最新情報を表示するWebBrowserのURL
        public static readonly string newsURL = "";

        //ゲーム起動時に送る引数（ランチャーからしか起動させたくない場合などに
        //Unity側の引数処理はsangoさんの https://qiita.com/sango/items/582468d3038330c59308 の記事が参考になります）
        //設定例: launcherParam = "/param fromLauncher"; これで引数[0]に「/param」が、引数[1]に「fromLauncher」が入ります。
        public static readonly string launcherParam = "";

    }
}