using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    /*カメラの距離を取得*/
    public int cameraDistans;

    /*カメラで撮影する対象のUserを取得*/
    public Transform user;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        /*カメラの位置を指定(*/
        transform.position = CameraPosition(
            /*カメラ自身の位置, 対象のUserの位置, カメラとUserの距離*/
            transform, user, cameraDistans
        );
    }

    /*カメラのPositionを関数で計算*/
    public static Vector3 CameraPosition(
        /*引数：カメラ自身のTransform, 撮影するtargetのTransform, カメラとtargetの距離*/
        Transform camera, Transform targetBall, int cameraDistans
    )
    {
        /*cameraのPositionを初期化*/
        Vector3 cameraPosition = Vector3.zero;

        /*cameraのZ軸にtargetのZ軸から引数の距離を引いたものを代入*/
        cameraPosition.z = targetBall.position.z - cameraDistans;

        /*cameraのPositionを返す*/
        return cameraPosition;
    }
}
