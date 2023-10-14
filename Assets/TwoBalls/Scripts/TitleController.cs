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

public class TitleController : MonoBehaviour
{
    [SerializeField]
    private Text userNameTemp;

    public static Text userName;

    Text textName = userName;
    

    // Start is called before the first frame update
    void Start()
    {
        userName = userNameTemp;


        NativeLeakDetection.Mode = NativeLeakDetectionMode.EnabledWithStackTrace;
    }

    // Update is called once per frame
    void Update()
    {
        /*if (textName != userName)
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

            *//*foreach(string name in LeaderboardName)*//*

                UpdateUserName();
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

            textName = userName;
        }*/
    }

    /*再生ボタンをクリックされた時の処理*/
    public void OnPlayButtonClicked()
    {
        /*Mainシーン(ゲーム画面)への切り替え*/
        SceneManager.LoadScene("Main_Sinpusai");
    }







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
}
