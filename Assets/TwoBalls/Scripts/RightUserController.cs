using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightUserController : MonoBehaviour
{
    /*�E���̃{�[�������ׂĔz��Ŏ擾*/
    public Transform[] rightBall;


    /*���[���֘A�̒l*/
        /*���[���̈ړ��X�s�[�h*/
    public float speedLane;
        /*���[���̍ŏ��l*/
    const int MinLane = 0;
        /*���[���̍ő�l*/
    int MaxLane;
        /*�^�[�Q�b�g�̃��[��*/
    int targetLane;


    /*���ɐi�ރX�s�[�h(Z��)*/
    public float keybordSpeedZ;
    public float touchSpeedZ;

    /*��]�̃X�s�[�h*/
    public float rotationSpeed;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        /*��ʂ��^�b�`���ꂽ���̏���*/
            /*0�`��ʂ��^�b�`����Ă�w�̖{�����J��Ԃ�*/
        for (int i = 0; i < Input.touchCount; i++)
        {
            /*������ʂ̉E�����^�b�`����Ă���Ȃ�*/
            if (Input.touches[i].position.x > Screen.width / 2)
            {
                /*User��Position�������UserTouchPosition�֐���p���Čv�Z*/
                    /*UserTouchPosition�F�^�b�`���ꂽ�ꏊ����User��Position���v�Z����֐�*/
                        /*�����F�^�b�`����Ă���w��Vector3, �ړ��̑���, ���ׂẴ{�[���������Ă���z���Transform, User��Transform*/
                transform.position = UserController.UserTouchPosition(Input.touches[i].position, rightBall, transform);
            }
        }

        /*����User���ǂɏՓ˂��Ă��Ȃ����*/
        if (!UserController.userStop)
        {
            /*�������͂��ꂽ��*/
            if (Input.GetKey(KeyCode.UpArrow))
            {
                /*userPosition��user��Position�����Đ���*/
                Vector3 userPosition = transform.position;

                /*target�Ƃ��Č��݂̈ʒu�ɃX�s�[�h�𑫂����킹���l��ϐ��Ƃ��Đ���*/
                float target = transform.position.y + speedLane;

                /*user��Y����Position����Ɖ��̏����ݒ�*/
                userPosition.y = Mathf.Clamp(target, rightBall[0].position.y, rightBall[2].position.y);

                /*x=a*y^2+q�̓񎟊֐��Ƃ���a��q���ȉ��̒萔*/
                const float a = 1.85f / 10.24f;
                const float q = 3.7f;

                /*user��X����Position���v�Z*/
                userPosition.x = -a * Mathf.Pow(userPosition.y, 2) + q;

                /*�v�Z�������W��transform��Position�ɓ����*/
                transform.position = new Vector3(userPosition.x, userPosition.y, userPosition.z);

            }

            /*��������͂��ꂽ��*/
            if (Input.GetKey(KeyCode.DownArrow))
            {
                /*userPosition��user��Position�����Đ���*/
                Vector3 userPosition = transform.position;

                /*target�Ƃ��Č��݂̈ʒu�ɃX�s�[�h�𑫂����킹���l��ϐ��Ƃ��Đ���*/
                float target = transform.position.y - speedLane;

                /*user��Y����Position����Ɖ��̏����ݒ�*/
                userPosition.y = Mathf.Clamp(target, rightBall[0].position.y, rightBall[2].position.y);

                /*x=a*y^2+q�̓񎟊֐��Ƃ���a��q���ȉ��̒萔*/
                const float a = 1.85f / 10.24f;
                const float q = 3.7f;

                /*user��X����Position���v�Z*/
                userPosition.x = -a * Mathf.Pow(userPosition.y, 2) + q;

                /*�v�Z�������W��transform��Position�ɓ����*/
                transform.position = new Vector3(userPosition.x, userPosition.y, userPosition.z);
            }
        }

        /*����User���ǂɓ������Ă��Ȃ����Z����i�߂�*/
        if (!UserController.userStop) transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + (keybordSpeedZ / Time.deltaTime));




        /*�R�����g�܂�����`�`�`�`�`�`�`*/
        /*User�̉�]���v�Z*/

        /*UserController.UserRotate�FUser�̉�]���v�Z���鎩��֐�*/
        /*�����F�^�[�Q�b�g�̃��[��(�����), User��Transform, �s��̃{�[���̔z��(�C����)*/

        transform.rotation = rightBall[targetLane].rotation;
        transform.GetChild(0).localRotation = UserRotation(transform, rotationSpeed);
        if (UserController.userStop) transform.GetChild(0).localRotation = Quaternion.identity;

    }

    /*User��Wall�ɓ����������̏���*/
    private void OnTriggerEnter(Collider wall)
    {
        /*����Wall�^�O��Trigger�ɓ���������~�܂�*/
            /*userStop�Fuser���~�܂�����true�ɂȂ�bool�^�ϐ�*/
        if (wall.CompareTag("Wall")) UserController.userStop = true;
    }

    public void MoveToUp()
    {
        /*�^�[�Q�b�g�̃��[�������[���̍ő�l��菬������΃��[����+1����*/
        if (targetLane < MaxLane) targetLane++;
    }

    public void MoveToUnder()
    {
        /*�^�[�Q�b�g�̃��[�������[���̍ŏ��l��菬������΃��[����-1����*/
        if (targetLane > MinLane) targetLane--;
    }

    /*�{�[���̉�]�ɂ��Čv�Z*/
    public Quaternion UserRotation(Transform User, float angle)
    {
        Quaternion rotationBall = Quaternion.AngleAxis(angle/Time.deltaTime, Vector3.up * (-1));

        User.GetChild(0).localRotation = User.GetChild(0).localRotation * rotationBall;
        return User.GetChild(0).localRotation;
    }
}
