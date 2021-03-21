using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;


//--------------------------------------------------------------------
//Form1.cs // メイン処理を行うスクリプト
//作成:avaice_
//Create date: 2021/03/17
//--------------------------------------------------------------------
namespace hotLauncherWin
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //--------------------------------------------------------------------
        //初期宣言
        //--------------------------------------------------------------------
        System.Net.WebClient downloadClient = null;


        //--------------------------------------------------------------------
        //設定項目をlaunchersettings.csからコピーする※ここから変更しない！

        //--------------------------------------------------------------------
        private readonly string infoURL = launcherSetting.infoURL;
        private readonly string launcherName = launcherSetting.launcherName;
        private readonly string productName = launcherSetting.productName;
        private readonly string launcherVer = launcherSetting.launcherVer;
        private static readonly string launcherParam = launcherSetting.launcherParam;

        //Form1にしかない変数
        private string launcherURL;
        private string newsURL;
        private string lVer;
        private string latestVer;
        private string resourceURL;
        private string currentVer;



        //--------------------------------------------------------------------
        //起動時に呼び出される関数(Form1_Load)※ここで処理を進めます
        //--------------------------------------------------------------------

        private void Form1_Load(object sender, EventArgs e)
        {
            //TLS1.1, TLS1.2に対応させる
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            //Resフォルダがなければ作る
            string path = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + @"\Res\";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            //カレントディレクトリを指定
            System.IO.Directory.SetCurrentDirectory(path);


            //ランチャーの情報(VER, resourceURL, lverURL, launcherURL, newsURL)を取得する
            if (!GetInfoData())
            {
                //取得失敗時
                StatusLabel.Text = "サーバーエラー！ランチャーの情報確認に失敗しました。";
                return;
            }

            //インストール済みVerをcurrentVerに格納する関数を実行
            GetInstalledVer();




            //Main.csで指定したランチャー名に変更する
            this.Text = "【" + launcherName + " " + launcherVer + "】    Powered by hotLauncher";
            
            //最新のランチャーがあるかチェックする
            if (lVer == launcherVer) { }else
            {

                //最新でなければDoUpdate関数へ飛んでアップデート開始
                StatusLabel.Text = "ランチャーをアップデートしています";
                DoLauncherUpdate();
            }

            //launcherSetting.csで設定した設定項目に抜けが無いかチェックする
            string chkSettings = VerifySettings();
            if (chkSettings == "ok") { }else
            {
                //エラーだったらVerifySettingsの返り値に入っていたエラー内容を出力して停止する
                newsBrowser.DocumentText = ("<font face='メイリオ'><center><h1><b>hotLauncher ランチャー設定エラー！</b></h1><br>" + chkSettings + "<br><br><font color=red>このエラーが解決するまでランチャーは動作しません。</font></font></center>");
                return;
            }
            
            //最新情報をブラウザで表示する
            newsBrowser.Url = new Uri(newsURL);
            //キャッシュをクリアすることで最新のページを表示する
            newsBrowser.Refresh();

            //アップデートをチェックする
            
            if (currentVer == latestVer) {
                AllEnd();
            }
            else
            {
                //アップデートがある時
                //DoUpdate関数へ飛んでアップデート開始
                DoUpdate();
            }

            
      
        }



        //--------------------------------------------------------------------
        //処理を行う関数(Public)
        //--------------------------------------------------------------------
        
        void GetInstalledVer()
        {

            //インストール済みVerを記録しているVER.txtが存在するか確認する。無ければcurrentVerに000を代入する
            if (File.Exists("VER.txt"))
            {
                currentVer = ReadByTextFile("VER.txt");
                
            }
            else { currentVer = "000"; }

            return;

        }

        void DoUpdate()
        {
                //VER.txtがあれば削除（上書きエラー防止）
                if (File.Exists(@"VER.txt")){File.Delete(@"VER.txt"); }

                //ダウンロード先のURLを取得出来たらダウンロードを開始する
                DownloadResources(resourceURL);
                return;
           
        }

        void ApplyUpdate()
        {
            string filePath = System.IO.Directory.GetCurrentDirectory() + @"\" + productName;

            // 以前のバージョンが存在するかどうかを確認する
            DirectoryInfo di = new DirectoryInfo(filePath);
            if (di.Exists)
            {
                //存在すれば削除する
                System.IO.Directory.Delete(filePath, true);
            }

           

            try
            {
                //Zip展開処理
                System.IO.Compression.ZipFile.ExtractToDirectory(@"Resource.zip", System.IO.Directory.GetCurrentDirectory());
            }
            catch
            {
                //展開失敗
                StatusLabel.Text = "アップデート適用中にエラーが発生しました。";
                return;
            }

            //終わったら
            AllEnd();
            


        }


        //すべて処理が終わったら呼ばれる
        void AllEnd()
        {
            string path = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + @"\Res\";
            //最新Verを書き込む
            File.WriteAllText("VER.txt", latestVer);


            StatusLabel.Text = ("最新バージョンを実行しています。");
            progressBar1.Value = 100;
            PlayButton.Enabled = true;
            
        }

        //設定項目に抜けがないかチェックする関数
        string VerifySettings()
        {
            string err = "";
            if(infoURL == "")
            {
                err = err + "<br>" + "infoURL(ランチャー情報取得URL)を指定してください！";
            }
            if (resourceURL == "")
            {
                err = err + "<br>" + "infoURLで指定したファイルから、resourceURL(リソース情報取得先URL)を指定してください！";
            }
            if (launcherName == "")
            {
                err = err + "<br>" + "launcherName(ランチャーのタイトル名)を指定してください！";
            }
            if (productName == "")
            {
                err = err + "<br>" + "productName(ゲームのProduct Name)を指定してください！";
            }
            if (newsURL == "")
            {
                err = err + "<br>" + "infoURLで指定したファイルから、newsURL(最新情報表示WebBrowserのURL)を指定してください！";
            }
            if (launcherURL == "")
            {
                err = err + "<br>" + "infoURLで指定したファイルから、launcherURL(最新ランチャーのダウンロードURL)を指定してください！";
            }
            if (err == "")
            {
                err =  "ok";
            }
            return err;
        }



        private void DoLauncherUpdate()
        {
            
            StatusLabel.Text = "ランチャーのアップデートをダウンロードしています";
            if (DownloadFromURL(launcherURL, "lResource.zip"))
            {

                StatusLabel.Text = "アップデートを適用するため再起動します。";
                string filePath = System.IO.Directory.GetCurrentDirectory() + @"\updResources";
                // 展開先のディレクトリがあるかチェックする
                DirectoryInfo di = new DirectoryInfo(filePath);
                if (di.Exists) {System.IO.Directory.Delete(filePath, true);}



                try
                {
                    string[] files;
                    string updResPath = System.IO.Directory.GetCurrentDirectory() + @"\updResources";
                    //Zip展開処理
                    System.IO.Compression.ZipFile.ExtractToDirectory(@"lResource.zip", updResPath);
                    try
                    {
                        files = Directory.GetFiles(updResPath, Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName));
                        updResPath = Path.GetDirectoryName(files[0]);
                    }
                    catch
                    {
                        //展開失敗
                        StatusLabel.Text = "ランチャーアップデートリソースが不正です。";
                        return;
                    }
                    files = Directory.GetFiles(updResPath, "*");

                    //コマンドラインに投げるbatを作る
                    string cmdline = "";

                    for (int i = 0; i < files.Length; ++i) // a.Length は配列 a の長さ。これの例では5。
                    {
                        cmdline = cmdline + "copy \"" + files[i] + "\" \"" + Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + @"\" + Path.GetFileName(files[i]) + "\" /y \n";
                    }

                    cmdline = cmdline + "start " + System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName + "\nexit";
                    

                    //batに書き込む
                    File.WriteAllText(System.IO.Directory.GetCurrentDirectory() + @"\updResources\" + @"upd.bat", cmdline);

                    //完成したbatを起動して自分は消える
                    System.Diagnostics.Process.Start(System.IO.Directory.GetCurrentDirectory() + @"\updResources\" + @"upd.bat");
                    this.Close();



                }
                catch
                {
                    //展開失敗
                    StatusLabel.Text += "ランチャーアップデート適用中にエラーが発生しました。";
                    return;
                }
            }
            else
            {
                StatusLabel.Text = "サーバーエラー！ランチャーのアップデートがダウンロードできませんでした。";
            }
        }

        private bool GetInfoData()
        {
            try
            {
                //infodata.txtを読み込んでinfoDataに配列として格納する
                DownloadFromURL(infoURL, "infodata.txt");
                string[] infoData;
                infoData = File.ReadAllLines("infodata.txt", Encoding.UTF8);

                launcherURL = infoData[7];
                newsURL = infoData[9];
                resourceURL = infoData[3];
                lVer = infoData[5];
                latestVer = infoData[1];
            }
            catch
            {
                return false;
            }

            return true;

        }


        //--------------------------------------------------------------------
        //モジュール化された関数
        //--------------------------------------------------------------------

        //txtファイルを読み込んで返す(1行のファイルを読む前提での設計です)
        string ReadByTextFile(string _file)
        {
            //読み込むテキストファイル
            string textFile = @_file;
            //文字コード(UTF-8)
            System.Text.Encoding enc = System.Text.Encoding.GetEncoding("utf-8");

            //テキストファイルの中身をすべて読み込む
            return System.IO.File.ReadAllText(textFile, enc);
        }


        //ダウンロードが出来ればTrue,　失敗ならFalseを返す。
        //第1引数はダウンロードURL、第2引数は保存ファイル名
        //※リソースデータのダウンロードは別関数です
        bool DownloadFromURL(string _url, string _fName)
        {
            //ダウンロードが成功したかをBool型で格納する
            bool isCompleted = true;
            try
            {
                System.Net.WebClient wc = new System.Net.WebClient();
                wc.DownloadFile(_url, @_fName);
                wc.Dispose();
            }
            catch
            {
                //DL失敗
                isCompleted = false;
            }

            return isCompleted;
        }

        string GetFromURL(string _url)
        {
            string returnstr;
            try
            {
                WebRequest webReq = WebRequest.Create(_url);
                Stream ReqStream = webReq.GetResponse().GetResponseStream();
                StreamReader objReader = new StreamReader(ReqStream);
                returnstr = objReader.ReadLine();
            }
            catch
            {
                //DL失敗
                returnstr = "error";
            }
            
            return returnstr;
        }

        void DownloadResources(string _url)
        {
            //WebClientの作成
            if (downloadClient == null)
            {
                downloadClient = new System.Net.WebClient();
                //イベントハンドラの作成
                downloadClient.DownloadProgressChanged +=
                    new System.Net.DownloadProgressChangedEventHandler(
                        resDLProgressChanged);
                downloadClient.DownloadFileCompleted +=
                    new System.ComponentModel.AsyncCompletedEventHandler(
                        resDLCompleted);
            }
            //非同期ダウンロードを開始する
            downloadClient.DownloadFileAsync(new Uri(_url), "Resource.zip");
        }

        //--------------------------------------------------------------------
        //イベントハンドラ
        //--------------------------------------------------------------------

        private void resDLProgressChanged(object sender,
       System.Net.DownloadProgressChangedEventArgs e)
        {
            string a = e.ProgressPercentage.ToString();
            string b = (e.TotalBytesToReceive / 1024000).ToString();
            string c = (e.BytesReceived / 1024000).ToString();
            progressBar1.Value = (int)(e.ProgressPercentage * 0.8f);
            StatusLabel.Text = (a + "% (" + b + "MB 中 " + c + "MB) ダウンロードが終了しました。");




        }

        private void resDLCompleted(object sender,
            System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
                StatusLabel.Text = ("キャンセルされました。");
            else if (e.Error != null)
                StatusLabel.Text = ("ダウンロードエラー！ アップデート フェーズ2で失敗しました。");
            else
                StatusLabel.Text = ("アップデートを適用しています・・・・");

            //使い終わったイベントハンドラを消す
            downloadClient.DownloadProgressChanged -=
                new System.Net.DownloadProgressChangedEventHandler(
                    resDLProgressChanged);
            downloadClient.DownloadFileCompleted -=
                new System.ComponentModel.AsyncCompletedEventHandler(
                    resDLCompleted);

            DtoI_interval.Enabled = true;


        }



        //--------------------------------------------------------------------
        //各種Formコントロールのイベント
        //--------------------------------------------------------------------

        private void DtoI_interval_Tick(object sender, EventArgs e)
        {
            ApplyUpdate();
            DtoI_interval.Enabled = false;
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            var screen = System.Windows.Forms.Screen.PrimaryScreen;
            int width = screen.Bounds.Width;
            int height = screen.Bounds.Height;

            //ウィンドウ/フルスクリーン　のチェック状態で起動時の引数を変更する
            if (r1.Checked)
            {
                System.Diagnostics.Process.Start(System.IO.Directory.GetCurrentDirectory() + @"\" + productName + @"\" + productName + ".exe", "-screen-fullscreen 0 " + launcherParam );
            }
            if (r2.Checked)
            {
                System.Diagnostics.Process.Start(System.IO.Directory.GetCurrentDirectory() + @"\" + productName + @"\" + productName + ".exe", "-screen-fullscreen 1 -screen-width " + width.ToString() + " -screen-height " + height.ToString() + " " + launcherParam);
            }

            //役目を終えたランチャーはこれにて終了
            this.Close();
        }


    }
}
