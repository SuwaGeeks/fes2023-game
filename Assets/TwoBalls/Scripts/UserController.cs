using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserController : MonoBehaviour
{
    /*User���~�܂�������true�ɂȂ�bool�^�ϐ�*/
    public static bool userStop = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /*User��Position���֐��Ōv�Z*/
        /*�����F��ʂ��^�b�`����Ă���ꏊ, Z�����ړ�����X�s�[�h, �{�[����Transform, ���[�U�[��Transform*/
    public static Vector3 UserTouchPosition(Vector3 touchPosition, Transform[] Ball, Transform user)
    {
        
        /*����User���~�܂�����user��position�����̏�ɂƂǂ߂鏈��*/
        if (userStop) return user.position;

        /*���[�U�[��position�����E�̃{�[��1��x,y���W�ƌ��݂�User��Z���W�Ɉړ�*/
        user.position = new Vector3(Ball[1].position.x, Ball[1].position.y, user.position.z);

        /*UserPosition��User�̈ʒu�ɏ�����*/
        Vector3 userPosition = user.position;

        /*x=a*y^2+q�̓񎟊֐��Ƃ���a��q���ȉ��̒萔*/
        const float a = 1.85f / 10.24f;
        const float q = 3.7f;

        /*touchPosition��Z���W�͏��������Ȃ��ᔽ�f����Ȃ��H*/
        touchPosition.z = 10;

        /*target�̓X�N���[����ʂ��^�b�`���ꂽ�ꏊ��world���W�ɕϊ���������*/
        Vector3 target = Camera.main.ScreenToWorldPoint(touchPosition);

        /*userPosition��y���W���㉺�̃{�[���ȏ�ɓ������Ȃ��悤�Ɏw��*/
        userPosition.y = Mathf.Clamp(target.y, Ball[0].position.y, Ball[2].position.y);

        /*�����^�b�`���ꂽ�ꏊ����ʏ�̍����Ȃ�*/
            /*userPosition��X���W��x = a * y ^ 2 - q��p���Čv�Z*/
        if (touchPosition.x < Screen.width / 2) userPosition.x = a * Mathf.Pow(userPosition.y, 2) - q;

        /*�����^�b�`���ꂽ�ꏊ����ʏ�̉E���Ȃ�*/
            /*userPosition��X���W��x = a * y ^ 2 + q��p���Čv�Z*/
        if (touchPosition.x > Screen.width / 2) userPosition.x = -a * Mathf.Pow(userPosition.y, 2) + q;

        /*���Ԍo�߂ɂ���Ďw�肳�ꂽ�X�s�[�h��Z����i��*/
        userPosition.z = user.position.z;

        /*UserPosition��Ԃ�*/
        return userPosition;
    }

}
 