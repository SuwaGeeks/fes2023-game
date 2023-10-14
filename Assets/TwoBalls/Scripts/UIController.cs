using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using PlayFab;
using PlayFab.ClientModels;

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

    // Start is called before the first frame update
    void Start()
    {
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









            /*コメントつけろ！*/
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
                PlayFabController.SubmitScore(CalcScore());
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
        SceneManager.LoadScene("Main");
        
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
}
