using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageGenerator : MonoBehaviour
{
    /*userのTransform*/
    public Transform user;

    /*ドアのスピード*/
    public float speedDoor;
    
    /*ステージ関連*/
        /*ステージの長さ*/
    public int StageChipSize;
        /*その時に使用しているステージの数*/
    int currentChipIndex;
        /*開始位置のStageのChipのIndex番号*/
    public int startChipIndex;
        /*生成しておくステージの枚数*/
    public int preInstantiate;
        /*現在生成されているステージを入れるリスト*/
    public List<GameObject> generatedStageList = new List<GameObject>();
        /*ステージと壁の配列は対応させる*/
            /*複製するステージの配列*/
    public GameObject[] randomStageChips;
            /*ステージの壁を入れる配列*/
    public GameObject[] stageWallChips;
            /*ステージと壁を対応させたDictionaryを作成*/
    public Dictionary<GameObject, GameObject> stageWallDictionary = new Dictionary<GameObject, GameObject>();

    /*壁の場所を入れる配列*/
        /*左の壁*/
    public Transform[] leftWallPosition;
        /*右の壁*/
    public Transform[] rightWallPosition;

    /*左右の壁の枚数を操作する変数*/
    public int min;
    public int max;

    /*ゲーム画面に表示する壁をすべて入れる配列*/
    GameObject[] wallObject;

    // Start is called before the first frame update
    void Start()
    {
        /*0~ステージの枚数分だけ繰り返す*/
        for(int i = 0; i < randomStageChips.Length; i++)
        {
            /*ステージと壁をDirectionaryないに対応させる*/
            stageWallDictionary.Add(randomStageChips[i], stageWallChips[i]);
        }

        /*wallObjectを壁の枚数分だけ初期化*/
        /*wallObject = new GameObject[leftWallNomber+rightWallNomber];*/

        /*ステージを消すから消す前に一個残すやつ*/
        currentChipIndex = startChipIndex - 1;
        
        /*preInstantiateの枚数だけステージを生成*/
            /*UpdateStage：指定のIndexまでのステージチップを生成して管理下におく自作関数*/
                /*引数：生成するステージのIndex番号*/
        UpdateStage(preInstantiate);
    }

    // Update is called once per frame
    void Update()
    {
        /*Userの位置から現在のステージチップのインデックスを計算*/
        int userPositionIndex = (int)(user.position.z / StageChipSize);

        /*次のステージチップに入ったらステージ更新処理を行う*/
        if (userPositionIndex + preInstantiate > currentChipIndex)
        {
            /*charaPositionIndex+preInstantiateをIndex番号としてステージを生成*/
                /*UpdateStage：指定のIndexまでのステージチップを生成して管理下におく自作関数*/
                    /*引数：生成するステージのIndex番号*/
            UpdateStage(userPositionIndex + preInstantiate);

        }

        /*ドアを開く処理*/
            /*OpenDoor：ドアを開ける処理を行う自作関数*/
                /*引数：開きたいドアをもっているGameObject*/
                /*generatedStageListの３つ目のドアを開ける*/
        OpenDoor(generatedStageList[2]);
    }

    /*指定のIndexまでのステージチップを生成して管理下におく*/
    void UpdateStage(int toChipIndex)
    {
        /*指定されたIndexが実際のステージの枚数より少ない場合はそのまま返す*/
        if (toChipIndex <= currentChipIndex) return;

        /*指定のステージチップまでを生成*/
        for(int i = currentChipIndex + 1; i <= toChipIndex; i++)
        {
            /*ランダムなステージを生成*/
                /*GenerateStage：指定のインデックス位置にStageオブジェクトをランダムに生成する自作関数*/
                    /*引数：ステージを生成したい場所のIndex番号*/
            GameObject stageObject = GenerateStage(i, min, max, stageWallDictionary);

            /*生成したステージチップを管理リストに追加*/
            generatedStageList.Add(stageObject);
        }

        /*ステージ保持上限内になるまで古いステージを削除*/
        while (generatedStageList.Count > preInstantiate + 2) DestroyOldestStage();

        /*実際のステージの枚数を変更*/
        currentChipIndex = toChipIndex;
    }

  

    /*指定のインデックス位置にStageオブジェクトをランダムに生成*/
    GameObject GenerateStage(int chipIndex, int min, int max, Dictionary<GameObject, GameObject> wall)
    {
        /*ステージの枚数の中からランダムで数字を生成*/
        int nextStageChip = Random.Range(0, randomStageChips.Length);

        /*ランダムで指定したステージを指定の場所に複製*/
        GameObject stageObject = (GameObject)Instantiate(
            /*生成するステージのプレハブ*/
            randomStageChips[nextStageChip],
            /*生成するステージの場所*/
            new Vector3(0, 0, chipIndex * StageChipSize),
            /*回転なしの設定*/
            Quaternion.identity
        );

        /*右側の壁の枚数をランダムで指定する変数*/
        int rightWallNomber = Random.Range(min, max);

        /*0～右の壁の枚数分繰り返す*/
        for(int i = 0; i < rightWallNomber; i++)
        {
            /*右の壁の場所の中からどこに置くかをランダムで生成*/
            int wallPositionIndex = Random.Range(0, rightWallPosition.Length);

            /*Z軸の座標を　ステージのサイズ×(何枚目か＋１)/(右の壁の枚数+1)で指定*/
            float wallPositionZ = StageChipSize * (i + 1) / (rightWallNomber + 1);

            /*wallObjectのインスタンス化*/
            wallObject = new GameObject[rightWallNomber];

            /*i番目の壁をプレハブとして複製*/
            wallObject[i] = (GameObject)Instantiate(
                /*複製するもの*/
                    /*現在生成されているステージに対応する壁をDirectionaryから*/
                wall[randomStageChips[nextStageChip]],
                /*複製する場所*/
                    /*新しいVector3をインスタンス化*/
                new Vector3(
                    /*右の壁のプレハブからランダムで指定した場所のX座標*/
                    rightWallPosition[wallPositionIndex].position.x, 
                    /*右の壁のプレハブからランダムで指定した場所のY座標*/
                    rightWallPosition[wallPositionIndex].position.y,
                    /*生成するステージの枚数×ステージの枚数＋ステージ内のどこに置くか*/
                    chipIndex * StageChipSize + wallPositionZ
                ),
                /*回転*/
                    /*右の壁のプレハブからランダムで指定した場所の回転*/
                rightWallPosition[wallPositionIndex].rotation
            );

            /*壁のオブジェクトをステージの子要素にする*/
            wallObject[i].transform.parent = stageObject.transform;
        }

        /*左側の壁の枚数をランダムで指定する変数*/
        int leftWallNomber = Random.Range(min, max);

        /*0～左の壁の枚数分繰り返す*/
        for(int i = 0; i < leftWallNomber; i++)
        {
            /*左の壁の場所の中からどこに置くかをランダムで生成*/
            int wallPositionIndex = Random.Range(0, leftWallPosition.Length);

            /*Z軸の座標を　ステージのサイズ×(何枚目か＋１)/(左の壁の枚数+1)で指定*/
            float wallPositionZ = StageChipSize * (i+1) / (leftWallNomber + 1);

            /*wallObjectのインスタンス化*/
            wallObject = new GameObject[leftWallNomber];

            /*i番目の壁をプレハブとして複製*/
            wallObject[i] = (GameObject)Instantiate(
                /*複製するもの*/
                    /*現在生成されているステージに対応する壁をDirectionaryから*/
                wall[randomStageChips[nextStageChip]],
                /*複製する場所*/
                    /*新しいVector3をインスタンス化*/
                new Vector3(
                    /*左の壁のプレハブからランダムで指定した場所のX座標*/
                    leftWallPosition[wallPositionIndex].position.x, 
                    /*左の壁のプレハブからランダムで指定した場所のY座標*/
                    leftWallPosition[wallPositionIndex].position.y,
                    /*生成するステージの枚数×ステージの枚数＋ステージ内のどこに置くか*/
                    chipIndex * StageChipSize + wallPositionZ
                ),
                /*回転*/
                    /*左の壁のプレハブからランダムで指定した場所の回転*/
                leftWallPosition[wallPositionIndex].rotation
            );

            /*壁のオブジェクトをステージの子要素にする*/
            wallObject[i].transform.parent = stageObject.transform;
        }

        /*生成するステージを返す*/
        return stageObject;
    }

    /*一番古いステージを削除*/
    void DestroyOldestStage()
    {
        /*一番古いステージを取得*/
        GameObject oldStage = generatedStageList[0];
        
        /*リストから一番古いステージを削除*/
        generatedStageList.RemoveAt(0);
        
        /*画面から一番古いステージを削除*/
        Destroy(oldStage);
    }

    /*ドアを開ける処理を行う自作関数*/
        /*引数：開くドア開きたいドアをもっているGameObject*/
    void OpenDoor(GameObject nextStage)
    {
        /*ステージのプレハブの”最初”の子要素の”最初の”子要素をステージの”次の”子要素の”最初の”子要素の位置に線形補間で移動する*/
        /*Right側のドアの移動*/
        nextStage.transform.GetChild(0).GetChild(0).position = Vector3.Lerp(
            nextStage.transform.GetChild(0).GetChild(0).position,
            nextStage.transform.GetChild(1).GetChild(0).position,
            Time.deltaTime * speedDoor
        );

        /*ステージのプレハブの”最初”の子要素の”次の”子要素をステージの”次の”子要素の”次の”子要素の位置に線形補間で移動する*/
        /*Left側のドアの移動*/
        nextStage.transform.GetChild(0).GetChild(1).position = Vector3.Lerp(
            nextStage.transform.GetChild(0).GetChild(1).position,
            nextStage.transform.GetChild(1).GetChild(1).position,
            Time.deltaTime * speedDoor
        );

    }
}
