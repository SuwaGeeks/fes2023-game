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
    /*�Q�[������User���g��UI*/
    public GameObject UserUI;
    
    /*���ʂ�\������Ƃ���UI*/
    public GameObject ResultUI;
    
    /*User��u���z��*/
    public GameObject[] User;

    /*Text�̕ϐ�*/
        /*�Q�[�����\�������Score��Text*/
    public Text scoreText;
        /*���ʉ�ʂ�UI*/
    public Text resultScoreText;
        /*�x�X�g�X�R�A��Text*/
    public Text bestScoreText;

    // �e��p�����[�^�[�̓C���X�y�N�^�[����ݒ肷��
    [SerializeField] Button tweetButton;                        // �c�C�[�g����{�^��
    [SerializeField] string text = "�c�C�[�g�@�\�̃e�X�g��";    // �c�C�[�g�ɑ}������e�L�X�g
    [SerializeField] string linkUrl = "http://negi-lab.blog.jp/";   // �c�C�[�g�ɑ}������URL
    [SerializeField] string hashtags = "Unity,�˂����";        // �c�C�[�g�ɑ}������n�b�V���^�O

    private string imgPath;
    [Header("�ۑ���̐ݒ�")]
    [SerializeField]
    string folderName = "Screenshots";

    bool isCreatingScreenShot = false;
    string path;


    public Sprite img;

    // Start is called before the first frame update
    void Start()
    {
        /*�ŏ���UI�̃A�N�e�B�u�ݒ�*/
            /*UserUI���A�N�e�B�u��*/
        UserUI.SetActive(true);
            /*ResultUI���A�N�e�B�u��*/
        ResultUI.SetActive(false);
        
        /*userStop��false�ɂ��ē�����悤�ɂ���*/
        UserController.userStop = false;

        path = Application.dataPath + "/" + folderName + "/";

        Debug.Log(folderName);

        // UI�{�^���̃N���b�N�Ńc�C�[�g��ʂ��J���ꍇ
        tweetButton.onClick.AddListener(() =>
        {
            Tweeting();
        });



        
    }

    // Update is called once per frame
    void Update()
    {
        /*�f�o�b�O�p*/
            /*����Enter�L�[����͂��ꂽ�烊�X�^�[�g*/
        if (Input.GetKeyDown(KeyCode.Return)) OnRestartButtonClicked();
        
        /*���݂̃X�R�A����ʏ�ɕ\��*/
        scoreText.text = "Score:" + CalcScore() + "m";

        /*����userStop��true(User����Q���ɓ�������)�Ȃ��*/
        if (UserController.userStop)
        {
            /*�A�N�e�B�u�ݒ�̐؂�ւ�*/
                /*UserUI���A�N�e�B�u��*/
            UserUI.SetActive(false);
                /*ResultUI���A�N�e�B�u��*/
            ResultUI.SetActive(true);
            
            /*���ʂ̃X�R�A��Text�ɕ\��*/
            resultScoreText.text = CalcScore() + "m";

            /*BestScore�̐ݒ�*/
                /*�������ʂ�HighScore��荂�����*/
            if (PlayerPrefs.GetInt("HighScore") < CalcScore())
            {
                /*���݂̃X�R�A��HighScore�ɓo�^*/
                PlayerPrefs.SetInt("HighScore", CalcScore());
            }

            /*�x�X�g�X�R�A��Text�ɕ\��*/
            bestScoreText.text = "Best:" + PlayerPrefs.GetInt("HighScore") + "m";









            /*�R�����g����I*/
            PlayFabClientAPI.LoginWithCustomID(
            new LoginWithCustomIDRequest
            {
                TitleId = PlayFabSettings.TitleId,
                CustomId = TitleController.userName.text,
                CreateAccount = true,
            }
            , result =>
            {
                Debug.Log("���O�C�������I");
                PlayFabController.SubmitScore(CalcScore());
            }
            , errorCallback =>
            {
                Debug.Log("���O�C�����s�I");
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

        // �}�E�X�̉E�N���b�N�Ńc�C�[�g��ʂ��J���ꍇ
        
        if (Input.GetMouseButtonDown(1))
        {
            Tweeting();
        }
    }

    /*�z�[���{�^�����N���b�N���ꂽ���̏���*/
    public void OnHomeButtonClicked()
    {
        /*�A�N�e�B�u�ݒ�̐؂�ւ�*/
            /*UserUI���A�N�e�B�u��*/
        UserUI.SetActive(true);
            /*ResultUI���A�N�e�B�u��*/
        ResultUI.SetActive(false);

        /*Title�V�[���ɐ؂�ւ�*/
        SceneManager.LoadScene("Title_Sinpusai");
    }

    /*�Đ��{�^��(���X�^�[�g)���N���b�N���ꂽ�Ƃ�*/
    public void OnRestartButtonClicked()
    {
        /*Main�V�[��(�����V�[��)�ɐ؂�ւ�*/
        SceneManager.LoadScene("Main");
        
    }

    /*�_���̌v�Z*/
    int CalcScore()
    {
        /*User(Right)��Z����Ԃ�*/
        return (int)User[0].transform.position.z;
    }

    // �c�C�[�g��ʂ��J��
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
                                    // WebGL�̏ꍇ�́A�Q�[���v���C��ʂƓ����E�B���h�E�Ńc�C�[�g��ʂ��J���Ȃ��悤�A������ς���
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
                                    // WebGL�̏ꍇ�́A�Q�[���v���C��ʂƓ����E�B���h�E�Ńc�C�[�g��ʂ��J���Ȃ��悤�A������ς���
                                    Application.ExternalEval(string.Format("window.open('{0}','_blank')", url));
        #else
                                    Application.OpenURL(url);
        #endif*/
    }
}
