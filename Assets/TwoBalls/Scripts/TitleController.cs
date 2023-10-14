using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleController : MonoBehaviour
{
    [SerializeField]
    private Text userNameTemp;

    public static Text userName;
    

    // Start is called before the first frame update
    void Start()
    {
        userName = userNameTemp;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*再生ボタンをクリックされた時の処理*/
    public void OnPlayButtonClicked()
    {
        /*Mainシーン(ゲーム画面)への切り替え*/
        SceneManager.LoadScene("Main");
    }
}
