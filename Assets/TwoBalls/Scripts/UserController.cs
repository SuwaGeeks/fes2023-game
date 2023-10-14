using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserController : MonoBehaviour
{
    /*Userが止まった時にtrueになるbool型変数*/
    public static bool userStop = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /*UserのPositionを関数で計算*/
        /*引数：画面をタッチされている場所, Z軸を移動するスピード, ボールのTransform, ユーザーのTransform*/
    public static Vector3 UserTouchPosition(Vector3 touchPosition, Transform[] Ball, Transform user)
    {
        
        /*もしUserが止まった時userのpositionをその場にとどめる処理*/
        if (userStop) return user.position;

        /*ユーザーのpositionを左右のボール1のx,y座標と現在のUserのZ座標に移動*/
        user.position = new Vector3(Ball[1].position.x, Ball[1].position.y, user.position.z);

        /*UserPositionをUserの位置に初期化*/
        Vector3 userPosition = user.position;

        /*x=a*y^2+qの二次関数としてaとqが以下の定数*/
        const float a = 1.85f / 10.24f;
        const float q = 3.7f;

        /*touchPositionのZ座標は少し離さなきゃ反映されない？*/
        touchPosition.z = 10;

        /*targetはスクリーン画面をタッチされた場所をworld座標に変換したもの*/
        Vector3 target = Camera.main.ScreenToWorldPoint(touchPosition);

        /*userPositionのy座標を上下のボール以上に動かせないように指定*/
        userPosition.y = Mathf.Clamp(target.y, Ball[0].position.y, Ball[2].position.y);

        /*もしタッチされた場所が画面上の左側なら*/
            /*userPositionのX座標をx = a * y ^ 2 - qを用いて計算*/
        if (touchPosition.x < Screen.width / 2) userPosition.x = a * Mathf.Pow(userPosition.y, 2) - q;

        /*もしタッチされた場所が画面上の右側なら*/
            /*userPositionのX座標をx = a * y ^ 2 + qを用いて計算*/
        if (touchPosition.x > Screen.width / 2) userPosition.x = -a * Mathf.Pow(userPosition.y, 2) + q;

        /*時間経過によって指定されたスピードでZ軸を進む*/
        userPosition.z = user.position.z;

        /*UserPositionを返す*/
        return userPosition;
    }

}
 