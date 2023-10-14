using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using PlayFab;
using PlayFab.ClientModels;
using Unity.Collections;

public class UIController : MonoBehaviour
{
    /*ゲーム中のUserが使うUI*/
    public GameObject UserUI;
    
    /*結果を表示するときのUI*/
    public GameObject ResultUI;
    
    /*Userを置く配列*/
    public GameObject[] User;

    /*Textの変数*/
        /*ゲーム中表示されるScoreのText*/
    public Text scoreText;
        /*結果画面のUI*/
    public Text resultScoreText;
        /*ベストスコアのText*/
    public Text bestScoreText;

    // 各種パラメーターはインスペクターから設定する
    [SerializeField] Button tweetButton;                        // ツイートするボタン
    [SerializeField] string text = "ツイート機能のテスト中";    // ツイートに挿入するテキスト
    [SerializeField] string linkUrl = "http://negi-lab.blog.jp/";   // ツイートに挿入するURL
    [SerializeField] string hashtags = "Unity,ねぎらぼ";        // ツイートに挿入するハッシュタグ

    private string imgPath;
    [Header("保存先の設定")]
    [SerializeField]
    string folderName = "Screenshots";

    bool isCreatingScreenShot = false;
    string path;


    public Sprite img;



    private bool playFabBool = true;
    public Text ranking;

    // Start is called before the first frame update
    void Start()
    {
        NativeLeakDetection.Mode = NativeLeakDetectionMode.EnabledWithStackTrace;




        /*最初のUIのアクティブ設定*/
        /*UserUIをアクティブ化*/
        UserUI.SetActive(true);
            /*ResultUIを非アクティブ化*/
        ResultUI.SetActive(false);
        
        /*userStopをfalseにして動けるようにする*/
        UserController.userStop = false;

        path = Application.dataPath + "/" + folderName + "/";

        Debug.Log(folderName);

        // UIボタンのクリックでツイート画面を開く場合
        tweetButton.onClick.AddListener(() =>
        {
            Tweeting();
        });



    }

    // Update is called once per frame
    void Update()
    {
        /*デバッグ用*/
            /*もしEnterキーを入力されたらリスタート*/
        if (Input.GetKeyDown(KeyCode.Return)) OnRestartButtonClicked();
        
        /*現在のスコアを画面上に表示*/
        scoreText.text = "Score:" + CalcScore() + "m";

        /*もしuserStopがtrue(Userが障害物に当たった)ならば*/
        if (UserController.userStop)
        {
            /*アクティブ設定の切り替え*/
                /*UserUIを非アクティブ化*/
            UserUI.SetActive(false);
                /*ResultUIをアクティブ化*/
            ResultUI.SetActive(true);








            
            /*結果のスコアをTextに表示*/
            resultScoreText.text = CalcScore() + "m";

            /*BestScoreの設定*/
                /*もし結果がHighScoreより高ければ*/
            if (PlayerPrefs.GetInt("HighScore") < CalcScore())
            {
                /*現在のスコアをHighScoreに登録*/
                PlayerPrefs.SetInt("HighScore", CalcScore());
            }

            /*ベストスコアをTextに表示*/
            bestScoreText.text = "Best:" + PlayerPrefs.GetInt("HighScore") + "m";





            if (playFabBool)
            {
                PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest
                {
                    CustomId = TitleController.userName.text,
                    CreateAccount = true
                }
                , result =>
                {
                    Debug.Log("Success LOGIN");
                    UpdateUserName();
                    UpdatePlayerStatistics();
                    GetLeaderboard();
                    GetLeaderboardAroundPlayer();
                    ranking.text = $"Score:  {CalcScore()}\n";
                }
                , error =>
                {
                    Debug.Log(error.GenerateErrorReport());
                });


                playFabBool = false;
            }

            /*コメントつけろ！*/
            /*if (playFabBool)
            {
                PlayFabClientAPI.LoginWithCustomID(
                new LoginWithCustomIDRequest
                {
                    TitleId = PlayFabSettings.TitleId,
                    CustomId = TitleController.userName.text,
                    CreateAccount = true,
                }
                , result =>
                {
                    Debug.Log("ログイン成功！");
                    SubmitScore(CalcScore());
                    GetLeaderboard();
                    GetLeaderboardAroundPlayer();
                }
                , errorCallback =>
                {
                    Debug.Log("ログイン失敗！");
                });

                PlayFabClientAPI.GetLeaderboard(new GetLeaderboardRequest
                {
                    StatisticName = "HighScore"
                }, result =>
                {
                    foreach (var item in result.Leaderboard)
                    {
                        item.DisplayName = TitleController.userName.text;
                    }
                }, error =>
                {
                    Debug.Log(error.GenerateErrorReport());
                });


                playFabBool = false;
            }*/
        }

        // マウスの右クリックでツイート画面を開く場合
        
        if (Input.GetMouseButtonDown(1))
        {
            Tweeting();
        }
    }

    /*ホームボタンをクリックされた時の処理*/
    public void OnHomeButtonClicked()
    {
        /*アクティブ設定の切り替え*/
            /*UserUIをアクティブ化*/
        UserUI.SetActive(true);
            /*ResultUIを非アクティブ化*/
        ResultUI.SetActive(false);

        /*Titleシーンに切り替え*/
        SceneManager.LoadScene("Title_Sinpusai");
    }

    /*再生ボタン(リスタート)をクリックされたとき*/
    public void OnRestartButtonClicked()
    {
        /*Mainシーン(同じシーン)に切り替え*/
        SceneManager.LoadScene("Main_Sinpusai");
        
    }

    /*点数の計算*/
    int CalcScore()
    {
        /*User(Right)のZ軸を返す*/
        return (int)User[0].transform.position.z;
    }

    // ツイート画面を開く
    public IEnumerator Share()
    {
        if (isCreatingScreenShot)
        {
            yield break;
        }

        isCreatingScreenShot = true;

        yield return null;

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string date = DateTime.Now.ToString("yy-MM-dd_HH-mm-ss");
        string fileName = path + date + ".png";

        ScreenCapture.CaptureScreenshot(fileName);

        var url = "https://twitter.com/intent/tweet?"
            + "text=" + text
            + "&url=" + linkUrl
            + "&hashtags=" + hashtags;

        

        #if UNITY_EDITOR
                Application.OpenURL(url);
        #elif UNITY_WEBGL
                                    // WebGLの場合は、ゲームプレイ画面と同じウィンドウでツイート画面が開かないよう、処理を変える
                                    Application.ExternalEval(string.Format("window.open('{0}','_blank')", url));
        #else
                                    Application.OpenURL(url);
        #endif

        yield return new WaitUntil(() => File.Exists(fileName));

        isCreatingScreenShot = false;
    }

    /*public void Tweeting()
    {
        string text = "Your Score is 30000!! \n #GameTitle";
    
        string url = "https://itunes.apple.com/jp/app/id0000000000?mt=8";
        string image = "tweetImage.png";
        SocialConnector.PostMessage(SocialConnector.ServiceType.Twitter, text, url, image);
    }*/

    public void Tweeting()
    {
        StartCoroutine("Share");



        /*var url = "https://twitter.com/intent/tweet?"
            + "text=" + text
            + "&url=" + linkUrl
            + "&hashtags=" + hashtags
            + path;

        #if UNITY_EDITOR
                Application.OpenURL(url);
        #elif UNITY_WEBGL
                                    // WebGLの場合は、ゲームプレイ画面と同じウィンドウでツイート画面が開かないよう、処理を変える
                                    Application.ExternalEval(string.Format("window.open('{0}','_blank')", url));
        #else
                                    Application.OpenURL(url);
        #endif*/
    }




    /*ユーザーネームの更新*/
    public void UpdateUserName()
    {
        //ユーザ名を指定して、UpdateUserTitleDisplayNameRequestのインスタンスを生成
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = TitleController.userName.text
        };

        //ユーザ名の更新
        Debug.Log($"ユーザ名の更新開始");
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnUpdateUserNameSuccess, OnUpdateUserNameFailure);
    }

    //ユーザ名の更新成功
    private void OnUpdateUserNameSuccess(UpdateUserTitleDisplayNameResult result)
    {
        //result.DisplayNameに更新した後のユーザ名が入ってる
        Debug.Log($"ユーザ名の更新が成功しました : {result.DisplayName}");
    }

    //ユーザ名の更新失敗
    private void OnUpdateUserNameFailure(PlayFabError error)
    {
        Debug.LogError($"ユーザ名の更新に失敗しました\n{error.GenerateErrorReport()}");
    }



    /*スコアの更新*/
    public void UpdatePlayerStatistics()
    {
        //UpdatePlayerStatisticsRequestのインスタンスを生成
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>{
        new StatisticUpdate{
          StatisticName = "HighScore",   //ランキング名(統計情報名)
          Value = CalcScore(), //スコア(int)
        }
      }
        };

        //ユーザ名の更新
        Debug.Log($"スコア(統計情報)の更新開始");
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnUpdatePlayerStatisticsSuccess, OnUpdatePlayerStatisticsFailure);
    }

    //スコア(統計情報)の更新成功
    private void OnUpdatePlayerStatisticsSuccess(UpdatePlayerStatisticsResult result)
    {
        Debug.Log($"スコア(統計情報)の更新が成功しました");
    }

    //スコア(統計情報)の更新失敗
    private void OnUpdatePlayerStatisticsFailure(PlayFabError error)
    {
        Debug.LogError($"スコア(統計情報)更新に失敗しました\n{error.GenerateErrorReport()}");
    }




    public static void SubmitScore(int playerScore)
    {
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = "HighScore",
                    Value = playerScore
                }
            }
        }, result =>
        {
            Debug.Log($"スコア {playerScore} 送信完了！");
        }, error =>
        {
            Debug.Log(error.GenerateErrorReport());
        });
    }


    








    public void GetLeaderboard()
    {
        //GetLeaderboardRequestのインスタンスを生成
        var request = new GetLeaderboardRequest
        {
            StatisticName = "HighScore", //ランキング名(統計情報名)
            StartPosition = 0,                 //何位以降のランキングを取得するか
            MaxResultsCount = 5                  //ランキングデータを何件取得するか(最大100)
        };



        /*OnGetLeaderboardSuccess(new GetLeaderboardResult a){
            a=
        }*/

        //ランキング(リーダーボード)を取得
        Debug.Log($"ランキング(リーダーボード)の取得開始");
        PlayFabClientAPI.GetLeaderboard(request, OnGetLeaderboardSuccess, OnGetLeaderboardFailure);
    }

    //ランキング(リーダーボード)の取得成功
    private void OnGetLeaderboardSuccess(GetLeaderboardResult result)
    {
        Debug.Log($"ランキング(リーダーボード)の取得に成功しました");
        ranking.text += "\nランキング\n";

        //result.Leaderboardに各順位の情報(PlayerLeaderboardEntry)が入っている
        foreach (var entry in result.Leaderboard)
        {
            ranking.text += $"{entry.Position+1}位:{entry.StatValue}   {entry.DisplayName}\n";
        }
    }

    //ランキング(リーダーボード)の取得失敗
    private void OnGetLeaderboardFailure(PlayFabError error)
    {
        Debug.LogError($"ランキング(リーダーボード)の取得に失敗しました\n{error.GenerateErrorReport()}");
    }


    public void GetLeaderboardAroundPlayer()
    {
        //GetLeaderboardAroundPlayerRequestのインスタンスを生成
        var request = new GetLeaderboardAroundPlayerRequest
        {
            StatisticName = "HighScore", //ランキング名(統計情報名)
            MaxResultsCount = 1                  //自分を含め前後何件取得するか
        };

        //自分の順位周辺のランキング(リーダーボード)を取得
        Debug.Log($"自分の順位周辺のランキング(リーダーボード)の取得開始");
        PlayFabClientAPI.GetLeaderboardAroundPlayer(request, OnGetLeaderboardAroundPlayerSuccess, OnGetLeaderboardAroundPlayerFailure);
    }

    //自分の順位周辺のランキング(リーダーボード)の取得成功
    private void OnGetLeaderboardAroundPlayerSuccess(GetLeaderboardAroundPlayerResult result)
    {
        Debug.Log($"自分の順位周辺のランキング(リーダーボード)の取得に成功しました");

        //result.Leaderboardに各順位の情報(PlayerLeaderboardEntry)が入っている
        foreach (var entry in result.Leaderboard)
        {
            ranking.text += "\nあなたの順位\n";
            ranking.text += $"{entry.Position+1}位:{entry.StatValue}  {entry.DisplayName}\n";
        }
    }

    //自分の順位周辺のランキング(リーダーボード)の取得失敗
    private void OnGetLeaderboardAroundPlayerFailure(PlayFabError error)
    {
        Debug.LogError($"自分の順位周辺のランキング(リーダーボード)の取得に失敗しました\n{error.GenerateErrorReport()}");
    }

}
