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

    /*�Đ��{�^�����N���b�N���ꂽ���̏���*/
    public void OnPlayButtonClicked()
    {
        /*Main�V�[��(�Q�[�����)�ւ̐؂�ւ�*/
        SceneManager.LoadScene("Main");
    }
}
