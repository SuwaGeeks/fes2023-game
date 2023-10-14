using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    /*�J�����̋������擾*/
    public int cameraDistans;

    /*�J�����ŎB�e����Ώۂ�User���擾*/
    public Transform user;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        /*�J�����̈ʒu���w��(*/
        transform.position = CameraPosition(
            /*�J�������g�̈ʒu, �Ώۂ�User�̈ʒu, �J������User�̋���*/
            transform, user, cameraDistans
        );
    }

    /*�J������Position���֐��Ōv�Z*/
    public static Vector3 CameraPosition(
        /*�����F�J�������g��Transform, �B�e����target��Transform, �J������target�̋���*/
        Transform camera, Transform targetBall, int cameraDistans
    )
    {
        /*camera��Position��������*/
        Vector3 cameraPosition = Vector3.zero;

        /*camera��Z����target��Z����������̋��������������̂���*/
        cameraPosition.z = targetBall.position.z - cameraDistans;

        /*camera��Position��Ԃ�*/
        return cameraPosition;
    }
}
