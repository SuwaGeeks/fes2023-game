using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightUserController : MonoBehaviour
{
    /*右側のボールをすべて配列で取得*/
    public Transform[] rightBall;


    /*レーン関連の値*/
        /*レーンの移動スピード*/
    public float speedLane;
        /*レーンの最小値*/
    const int MinLane = 0;
        /*レーンの最大値*/
    int MaxLane;
        /*ターゲットのレーン*/
    int targetLane;


    /*奥に進むスピード(Z軸)*/
    public float keybordSpeedZ;
    public float touchSpeedZ;

    /*回転のスピード*/
    public float rotationSpeed;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        /*画面をタッチされた時の処理*/
            /*0～画面をタッチされてる指の本数分繰り返す*/
        for (int i = 0; i < Input.touchCount; i++)
        {
            /*もし画面の右側をタッチされているなら*/
            if (Input.touches[i].position.x > Screen.width / 2)
            {
                /*UserのPositionを自作のUserTouchPosition関数を用いて計算*/
                    /*UserTouchPosition：タッチされた場所からUserのPositionを計算する関数*/
                        /*引数：タッチされている指のVector3, 移動の速さ, すべてのボールが入っている配列のTransform, UserのTransform*/
                transform.position = UserController.UserTouchPosition(Input.touches[i].position, rightBall, transform);
            }
        }

        /*もしUserが壁に衝突していなければ*/
        if (!UserController.userStop)
        {
            /*上矢印を入力された時*/
            if (Input.GetKey(KeyCode.UpArrow))
            {
                /*userPositionをuserのPositionを入れて生成*/
                Vector3 userPosition = transform.position;

                /*targetとして現在の位置にスピードを足し合わせた値を変数として生成*/
                float target = transform.position.y + speedLane;

                /*userのY軸のPositionを上と下の上限を設定*/
                userPosition.y = Mathf.Clamp(target, rightBall[0].position.y, rightBall[2].position.y);

                /*x=a*y^2+qの二次関数としてaとqが以下の定数*/
                const float a = 1.85f / 10.24f;
                const float q = 3.7f;

                /*userのX軸のPositionを計算*/
                userPosition.x = -a * Mathf.Pow(userPosition.y, 2) + q;

                /*計算した座標をtransformのPositionに入れる*/
                transform.position = new Vector3(userPosition.x, userPosition.y, userPosition.z);

            }

            /*下矢印を入力された時*/
            if (Input.GetKey(KeyCode.DownArrow))
            {
                /*userPositionをuserのPositionを入れて生成*/
                Vector3 userPosition = transform.position;

                /*targetとして現在の位置にスピードを足し合わせた値を変数として生成*/
                float target = transform.position.y - speedLane;

                /*userのY軸のPositionを上と下の上限を設定*/
                userPosition.y = Mathf.Clamp(target, rightBall[0].position.y, rightBall[2].position.y);

                /*x=a*y^2+qの二次関数としてaとqが以下の定数*/
                const float a = 1.85f / 10.24f;
                const float q = 3.7f;

                /*userのX軸のPositionを計算*/
                userPosition.x = -a * Mathf.Pow(userPosition.y, 2) + q;

                /*計算した座標をtransformのPositionに入れる*/
                transform.position = new Vector3(userPosition.x, userPosition.y, userPosition.z);
            }
        }

        /*もしUserが壁に当たっていなければZ軸を進める*/
        if (!UserController.userStop) transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + (keybordSpeedZ / Time.deltaTime));




        /*コメントまだだよ～～～～～～～*/
        /*Userの回転を計算*/

        /*UserController.UserRotate：Userの回転を計算する自作関数*/
        /*引数：ターゲットのレーン(いらん), UserのTransform, 行先のボールの配列(イラン)*/

        transform.rotation = rightBall[targetLane].rotation;
        transform.GetChild(0).localRotation = UserRotation(transform, rotationSpeed);
        if (UserController.userStop) transform.GetChild(0).localRotation = Quaternion.identity;

    }

    /*UserがWallに当たった時の処理*/
    private void OnTriggerEnter(Collider wall)
    {
        /*もしWallタグのTriggerに当たったら止まる*/
            /*userStop：userが止まった時trueになるbool型変数*/
        if (wall.CompareTag("Wall")) UserController.userStop = true;
    }

    public void MoveToUp()
    {
        /*ターゲットのレーンがレーンの最大値より小さければレーンに+1する*/
        if (targetLane < MaxLane) targetLane++;
    }

    public void MoveToUnder()
    {
        /*ターゲットのレーンがレーンの最小値より小さければレーンに-1する*/
        if (targetLane > MinLane) targetLane--;
    }

    /*ボールの回転について計算*/
    public Quaternion UserRotation(Transform User, float angle)
    {
        Quaternion rotationBall = Quaternion.AngleAxis(angle/Time.deltaTime, Vector3.up * (-1));

        User.GetChild(0).localRotation = User.GetChild(0).localRotation * rotationBall;
        return User.GetChild(0).localRotation;
    }
}
