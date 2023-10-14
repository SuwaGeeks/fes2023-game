using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageGenerator : MonoBehaviour
{
    /*user��Transform*/
    public Transform user;

    /*�h�A�̃X�s�[�h*/
    public float speedDoor;
    
    /*�X�e�[�W�֘A*/
        /*�X�e�[�W�̒���*/
    public int StageChipSize;
        /*���̎��Ɏg�p���Ă���X�e�[�W�̐�*/
    int currentChipIndex;
        /*�J�n�ʒu��Stage��Chip��Index�ԍ�*/
    public int startChipIndex;
        /*�������Ă����X�e�[�W�̖���*/
    public int preInstantiate;
        /*���ݐ�������Ă���X�e�[�W�����郊�X�g*/
    public List<GameObject> generatedStageList = new List<GameObject>();
        /*�X�e�[�W�ƕǂ̔z��͑Ή�������*/
            /*��������X�e�[�W�̔z��*/
    public GameObject[] randomStageChips;
            /*�X�e�[�W�̕ǂ�����z��*/
    public GameObject[] stageWallChips;
            /*�X�e�[�W�ƕǂ�Ή�������Dictionary���쐬*/
    public Dictionary<GameObject, GameObject> stageWallDictionary = new Dictionary<GameObject, GameObject>();

    /*�ǂ̏ꏊ������z��*/
        /*���̕�*/
    public Transform[] leftWallPosition;
        /*�E�̕�*/
    public Transform[] rightWallPosition;

    /*���E�̕ǂ̖����𑀍삷��ϐ�*/
    public int min;
    public int max;

    /*�Q�[����ʂɕ\������ǂ����ׂē����z��*/
    GameObject[] wallObject;

    // Start is called before the first frame update
    void Start()
    {
        /*0~�X�e�[�W�̖����������J��Ԃ�*/
        for(int i = 0; i < randomStageChips.Length; i++)
        {
            /*�X�e�[�W�ƕǂ�Directionary�Ȃ��ɑΉ�������*/
            stageWallDictionary.Add(randomStageChips[i], stageWallChips[i]);
        }

        /*wallObject��ǂ̖���������������*/
        /*wallObject = new GameObject[leftWallNomber+rightWallNomber];*/

        /*�X�e�[�W��������������O�Ɉ�c�����*/
        currentChipIndex = startChipIndex - 1;
        
        /*preInstantiate�̖��������X�e�[�W�𐶐�*/
            /*UpdateStage�F�w���Index�܂ł̃X�e�[�W�`�b�v�𐶐����ĊǗ����ɂ�������֐�*/
                /*�����F��������X�e�[�W��Index�ԍ�*/
        UpdateStage(preInstantiate);
    }

    // Update is called once per frame
    void Update()
    {
        /*User�̈ʒu���猻�݂̃X�e�[�W�`�b�v�̃C���f�b�N�X���v�Z*/
        int userPositionIndex = (int)(user.position.z / StageChipSize);

        /*���̃X�e�[�W�`�b�v�ɓ�������X�e�[�W�X�V�������s��*/
        if (userPositionIndex + preInstantiate > currentChipIndex)
        {
            /*charaPositionIndex+preInstantiate��Index�ԍ��Ƃ��ăX�e�[�W�𐶐�*/
                /*UpdateStage�F�w���Index�܂ł̃X�e�[�W�`�b�v�𐶐����ĊǗ����ɂ�������֐�*/
                    /*�����F��������X�e�[�W��Index�ԍ�*/
            UpdateStage(userPositionIndex + preInstantiate);

        }

        /*�h�A���J������*/
            /*OpenDoor�F�h�A���J���鏈�����s������֐�*/
                /*�����F�J�������h�A�������Ă���GameObject*/
                /*generatedStageList�̂R�ڂ̃h�A���J����*/
        OpenDoor(generatedStageList[2]);
    }

    /*�w���Index�܂ł̃X�e�[�W�`�b�v�𐶐����ĊǗ����ɂ���*/
    void UpdateStage(int toChipIndex)
    {
        /*�w�肳�ꂽIndex�����ۂ̃X�e�[�W�̖�����菭�Ȃ��ꍇ�͂��̂܂ܕԂ�*/
        if (toChipIndex <= currentChipIndex) return;

        /*�w��̃X�e�[�W�`�b�v�܂ł𐶐�*/
        for(int i = currentChipIndex + 1; i <= toChipIndex; i++)
        {
            /*�����_���ȃX�e�[�W�𐶐�*/
                /*GenerateStage�F�w��̃C���f�b�N�X�ʒu��Stage�I�u�W�F�N�g�������_���ɐ������鎩��֐�*/
                    /*�����F�X�e�[�W�𐶐��������ꏊ��Index�ԍ�*/
            GameObject stageObject = GenerateStage(i, min, max, stageWallDictionary);

            /*���������X�e�[�W�`�b�v���Ǘ����X�g�ɒǉ�*/
            generatedStageList.Add(stageObject);
        }

        /*�X�e�[�W�ێ�������ɂȂ�܂ŌÂ��X�e�[�W���폜*/
        while (generatedStageList.Count > preInstantiate + 2) DestroyOldestStage();

        /*���ۂ̃X�e�[�W�̖�����ύX*/
        currentChipIndex = toChipIndex;
    }

  

    /*�w��̃C���f�b�N�X�ʒu��Stage�I�u�W�F�N�g�������_���ɐ���*/
    GameObject GenerateStage(int chipIndex, int min, int max, Dictionary<GameObject, GameObject> wall)
    {
        /*�X�e�[�W�̖����̒����烉���_���Ő����𐶐�*/
        int nextStageChip = Random.Range(0, randomStageChips.Length);

        /*�����_���Ŏw�肵���X�e�[�W���w��̏ꏊ�ɕ���*/
        GameObject stageObject = (GameObject)Instantiate(
            /*��������X�e�[�W�̃v���n�u*/
            randomStageChips[nextStageChip],
            /*��������X�e�[�W�̏ꏊ*/
            new Vector3(0, 0, chipIndex * StageChipSize),
            /*��]�Ȃ��̐ݒ�*/
            Quaternion.identity
        );

        /*�E���̕ǂ̖����������_���Ŏw�肷��ϐ�*/
        int rightWallNomber = Random.Range(min, max);

        /*0�`�E�̕ǂ̖������J��Ԃ�*/
        for(int i = 0; i < rightWallNomber; i++)
        {
            /*�E�̕ǂ̏ꏊ�̒�����ǂ��ɒu�����������_���Ő���*/
            int wallPositionIndex = Random.Range(0, rightWallPosition.Length);

            /*Z���̍��W���@�X�e�[�W�̃T�C�Y�~(�����ڂ��{�P)/(�E�̕ǂ̖���+1)�Ŏw��*/
            float wallPositionZ = StageChipSize * (i + 1) / (rightWallNomber + 1);

            /*wallObject�̃C���X�^���X��*/
            wallObject = new GameObject[rightWallNomber];

            /*i�Ԗڂ̕ǂ��v���n�u�Ƃ��ĕ���*/
            wallObject[i] = (GameObject)Instantiate(
                /*�����������*/
                    /*���ݐ�������Ă���X�e�[�W�ɑΉ�����ǂ�Directionary����*/
                wall[randomStageChips[nextStageChip]],
                /*��������ꏊ*/
                    /*�V����Vector3���C���X�^���X��*/
                new Vector3(
                    /*�E�̕ǂ̃v���n�u���烉���_���Ŏw�肵���ꏊ��X���W*/
                    rightWallPosition[wallPositionIndex].position.x, 
                    /*�E�̕ǂ̃v���n�u���烉���_���Ŏw�肵���ꏊ��Y���W*/
                    rightWallPosition[wallPositionIndex].position.y,
                    /*��������X�e�[�W�̖����~�X�e�[�W�̖����{�X�e�[�W���̂ǂ��ɒu����*/
                    chipIndex * StageChipSize + wallPositionZ
                ),
                /*��]*/
                    /*�E�̕ǂ̃v���n�u���烉���_���Ŏw�肵���ꏊ�̉�]*/
                rightWallPosition[wallPositionIndex].rotation
            );

            /*�ǂ̃I�u�W�F�N�g���X�e�[�W�̎q�v�f�ɂ���*/
            wallObject[i].transform.parent = stageObject.transform;
        }

        /*�����̕ǂ̖����������_���Ŏw�肷��ϐ�*/
        int leftWallNomber = Random.Range(min, max);

        /*0�`���̕ǂ̖������J��Ԃ�*/
        for(int i = 0; i < leftWallNomber; i++)
        {
            /*���̕ǂ̏ꏊ�̒�����ǂ��ɒu�����������_���Ő���*/
            int wallPositionIndex = Random.Range(0, leftWallPosition.Length);

            /*Z���̍��W���@�X�e�[�W�̃T�C�Y�~(�����ڂ��{�P)/(���̕ǂ̖���+1)�Ŏw��*/
            float wallPositionZ = StageChipSize * (i+1) / (leftWallNomber + 1);

            /*wallObject�̃C���X�^���X��*/
            wallObject = new GameObject[leftWallNomber];

            /*i�Ԗڂ̕ǂ��v���n�u�Ƃ��ĕ���*/
            wallObject[i] = (GameObject)Instantiate(
                /*�����������*/
                    /*���ݐ�������Ă���X�e�[�W�ɑΉ�����ǂ�Directionary����*/
                wall[randomStageChips[nextStageChip]],
                /*��������ꏊ*/
                    /*�V����Vector3���C���X�^���X��*/
                new Vector3(
                    /*���̕ǂ̃v���n�u���烉���_���Ŏw�肵���ꏊ��X���W*/
                    leftWallPosition[wallPositionIndex].position.x, 
                    /*���̕ǂ̃v���n�u���烉���_���Ŏw�肵���ꏊ��Y���W*/
                    leftWallPosition[wallPositionIndex].position.y,
                    /*��������X�e�[�W�̖����~�X�e�[�W�̖����{�X�e�[�W���̂ǂ��ɒu����*/
                    chipIndex * StageChipSize + wallPositionZ
                ),
                /*��]*/
                    /*���̕ǂ̃v���n�u���烉���_���Ŏw�肵���ꏊ�̉�]*/
                leftWallPosition[wallPositionIndex].rotation
            );

            /*�ǂ̃I�u�W�F�N�g���X�e�[�W�̎q�v�f�ɂ���*/
            wallObject[i].transform.parent = stageObject.transform;
        }

        /*��������X�e�[�W��Ԃ�*/
        return stageObject;
    }

    /*��ԌÂ��X�e�[�W���폜*/
    void DestroyOldestStage()
    {
        /*��ԌÂ��X�e�[�W���擾*/
        GameObject oldStage = generatedStageList[0];
        
        /*���X�g�����ԌÂ��X�e�[�W���폜*/
        generatedStageList.RemoveAt(0);
        
        /*��ʂ����ԌÂ��X�e�[�W���폜*/
        Destroy(oldStage);
    }

    /*�h�A���J���鏈�����s������֐�*/
        /*�����F�J���h�A�J�������h�A�������Ă���GameObject*/
    void OpenDoor(GameObject nextStage)
    {
        /*�X�e�[�W�̃v���n�u�́h�ŏ��h�̎q�v�f�́h�ŏ��́h�q�v�f���X�e�[�W�́h���́h�q�v�f�́h�ŏ��́h�q�v�f�̈ʒu�ɐ��`��Ԃňړ�����*/
        /*Right���̃h�A�̈ړ�*/
        nextStage.transform.GetChild(0).GetChild(0).position = Vector3.Lerp(
            nextStage.transform.GetChild(0).GetChild(0).position,
            nextStage.transform.GetChild(1).GetChild(0).position,
            Time.deltaTime * speedDoor
        );

        /*�X�e�[�W�̃v���n�u�́h�ŏ��h�̎q�v�f�́h���́h�q�v�f���X�e�[�W�́h���́h�q�v�f�́h���́h�q�v�f�̈ʒu�ɐ��`��Ԃňړ�����*/
        /*Left���̃h�A�̈ړ�*/
        nextStage.transform.GetChild(0).GetChild(1).position = Vector3.Lerp(
            nextStage.transform.GetChild(0).GetChild(1).position,
            nextStage.transform.GetChild(1).GetChild(1).position,
            Time.deltaTime * speedDoor
        );

    }
}
