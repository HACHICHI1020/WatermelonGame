using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum GameState
    {
        Play, GameOver
    }
    public GameState gameState = GameState.Play; //게임 상태

    [Header("레벨")]
    public int gameLevel = 1; //게임 레벨

    public enum FruitControlState
    {
        Ready, Drag, Drop
    }
    [Header("과일")]
    public FruitControlState fruitControlState = FruitControlState.Ready; //준비
    public List<Sprite> fruitsIcon = new List<Sprite>(); //과일 아이콘
    public Image fruitsUI; //과일 UI
    public List<GameObject> fruitsPrefab = new List<GameObject>(); //과일 프리팹
    private int fruitNum; //과일 번호
    public Transform fruitsPos; //과일 시작 위치
    public GameObject selectFruit; //선택된 과일
    public float fruitSpeed; //과일 이동 속도
    private float mouseX; //마우스 X좌표
    private int productNum; //생산 번호
    public List<string> productFruits = new List<string>();//생산된 과일리스트

    public enum ScoreState
    {
        None, ScoreUp
    }
    [Header("점수")]
    public ScoreState scoreState = ScoreState.None; //점수 상태
    private float nowScore;//현재 점수
    private float targetScore;//목표 점수
    public float scoreSpeed; //점수 속도
    public Text fruitScoreText; //과일 점수 텍스트
    public Text nowScoreText; //현재 기록 텍스트
    public int highScore; //최고 기록
    public Text highScoreText; //최고 기록 텍스트

    [Header("사운드")]
    public GameObject combineSoundPrefab; //과일 합성 사운드

    [Header("파티클")]
    public List<float> vfxScale = new List<float>(); //파티클 크기
    public GameObject vfxPrefab; //파티클 프리팹
    
    [Header("게임 오버")]
    public GameObject gameOverPanel; //게임오버 패널

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        RandomFruit();

        //최고 기록 불러오기
        highScore = PlayerPrefs.GetInt("HighScore");
    }

    // Update is called once per frame
    void Update()
    {
        switch(gameState)
        {
            case GameState.Play: //게임 중 상태
                {
                    DragFruit();
                    ScoreUp();
                    break;
                }
        }

        //R키를 누른다면
        if(Input.GetKeyDown(KeyCode.R))
        {
            //기록을 초기화한다.
            PlayerPrefs.DeleteAll();
            //PlayerPrefs.SetInt("HighScore", 0);
        }

        //ESC 입력 시 게임 종료
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Application.Quit();
        }
    }

    //과일 뽑기
    public void RandomFruit()
    {
        //과일 생성 (원본, 위치, 회전, 부모)
        GameObject fruit = Instantiate(fruitsPrefab[fruitNum], 
            fruitsPos.position, Quaternion.identity, fruitsPos);

        //과일의 이름 + 생산번호
        fruit.transform.name = fruitsPrefab[fruitNum].transform.name + productNum.ToString();

        //과일 생산번호 부여
        fruit.GetComponent<Fruit>().FruitNumberOn(productNum, fruitNum);

        productNum++; //생산 번호 증가

        //과일의 반지름 크기 만큼 높이를 내린다.
        fruit.transform.position = new Vector3(fruit.transform.position.x,
            fruit.transform.position.y - fruit.GetComponent<CircleCollider2D>().radius,
            fruit.transform.position.z);

        productFruits.Add(fruit.transform.name); //생산 과일 리스트에 등록

        selectFruit = fruit; //선택된 과일로 등록한다.


        if (productNum > 0 && productNum < 10)
        {
            gameLevel = 2;//딸기 추가
        }
        else if (productNum >= 10 && productNum < 20)
        {
            gameLevel = 3;//포도 추가
        }
        else if (productNum >= 20 && productNum < 30)
        {
            gameLevel = 4;//사과 추가
        }
        else if (productNum >= 30 && productNum < 40)
        {
            gameLevel = 5;//오렌지 추가
        }
        else if (productNum >= 40 && productNum < 50)
        {
            gameLevel = 6;//배 추가
        }
        else if (productNum >= 50 && productNum < 60)
        {
            gameLevel = 7;//복숭아 추가
        }
        else if (productNum >= 60 && productNum < 70)
        {
            gameLevel = 8;//레모네이드 추가
        }
        else if (productNum >= 70 && productNum < 80)
        {
            gameLevel = 9;//멜론 추가
        }
        else if (productNum >= 80)
        {
            gameLevel = 10;//수박 추가
        }

        //0번부터 게임레벨 번호까지 무작위로 번호 저장
        fruitNum = Random.Range(0, gameLevel);

        //과일UI 스프라이트에 fruitNum번째의 과일 이미지를 등록한다.
        fruitsUI.sprite = fruitsIcon[fruitNum];
    }

    //과일 드래그
    void DragFruit()
    {
        switch(fruitControlState)
        {
            case FruitControlState.Ready:
                {
                    //왼쪽 마우스 버튼을 누른다면
                    if(Input.GetButtonDown("Fire1"))
                    {
                        fruitControlState = FruitControlState.Drag; //드래그 상태로 전환
                    }
                    break;
                }
            case FruitControlState.Drag: //과일 드래그
                {
                    //마우스 왼쪽 버튼을 누르고 있다면
                    if (Input.GetButton("Fire1"))
                    {
                        //선택한 과일이 있다면
                        if (selectFruit)
                        {
                            RaycastHit hit;
                            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
                            {
                                mouseX = hit.point.x; //X좌표 저장
                                //선택된 과일의 X좌표를 마우스가 드래그하는 지점으로 fruitSpeed의 속도로 이동시킨다.
                                selectFruit.transform.position = new Vector3(Mathf.MoveTowards
                                    (selectFruit.transform.position.x, mouseX, fruitSpeed * Time.deltaTime),
                                    selectFruit.transform.position.y, selectFruit.transform.position.z);
                            }
                        }
                    }
                    //왼쪽 마우스 버튼을 떼었을 때
                    if (Input.GetButtonUp("Fire1"))
                    {
                        selectFruit.GetComponent<Fruit>().FruitDropStart(); //과일 낙하 상태
                        fruitControlState = FruitControlState.Drop; //과일 낙하 상태
                    }
                    break;
                }
            case FruitControlState.Drop:
                {
                    if (selectFruit)
                    {
                        //선택된 과일의 X좌표를 마우스가 드래그하는 지점으로 fruitSpeed의 속도로 이동시킨다.
                        selectFruit.transform.position = new Vector3(Mathf.MoveTowards
                            (selectFruit.transform.position.x, mouseX, fruitSpeed * Time.deltaTime),
                            selectFruit.transform.position.y, selectFruit.transform.position.z);
                        //선택된 과일의 X좌표가 마우스X좌표와 같아질 경우
                        if (selectFruit.transform.position.x == mouseX)
                        {
                            //선택한 과일을 떨어지게 한다.
                            selectFruit.GetComponent<Rigidbody2D>().simulated = true;
                            mouseX = fruitsPos.position.x; //마우스X좌표 초기화
                            selectFruit = null; //선택된 과일 해제
                        }
                    }
                    break;
                }
        }
    }

    //과일 낙하 완료
    public void FruitDropOn()
    {
        fruitControlState = FruitControlState.Ready; //준비 상태로 초기화
    }

    //과일 조합
    public void CombineFruit(int fruitType, Transform fruitA, Transform fruitB)
    {
        //과일이름 미리 부여
        string fruitName = fruitsPrefab[fruitType].transform.name + productNum.ToString();

        //생산 예정인 과일 이름이 현재 생산 리스트 이름에 없다면
        if (!productFruits.Contains(fruitName))
        {
            //A과일과 B과일의 중앙 포지션
            Vector3 center = (fruitA.position + fruitB.position) * 0.5f;

            //다음 레벨의 과일을 생성한다 (원본, 위치, 회전, 부모)
            GameObject fruit = Instantiate(fruitsPrefab[fruitType],
                center, Quaternion.identity, fruitsPos);

            //과일의 이름 + 생산번호
            fruit.transform.name = fruitName;

            //과일 생산번호 부여
            fruit.GetComponent<Fruit>().FruitNumberOn(productNum, fruitType);

            //과일의 시뮬레이트 true
            fruit.GetComponent<Rigidbody2D>().simulated = true;

            //과일 레벨 지정
            fruit.GetComponent<Fruit>().FruitInitialize(fruitType);

            //점수 추가
            AddScore(fruit.GetComponent<Fruit>().fruitScore);

            //생산 리스트에 과일 이름 추가
            productFruits.Add(fruit.transform.name);

            //과일 합성 사운드 생성
            GameObject combineSound = Instantiate(combineSoundPrefab);
            Destroy(combineSound, 2.0f); //2초 뒤에 삭제

            //파티클 생성 (원본, 위치, 회전, 부모)
            GameObject vfx = Instantiate(vfxPrefab, center, Quaternion.identity, null);
            Destroy(vfx, 1.0f); //1초 뒤에 파티클 삭제
        }

        //전체 과일 이름 중에서 없어질 과일들을 찾아 리스트에서 제거
        for (int i = 0; i < productFruits.Count; i++)
        {
            if (productFruits[i] == fruitA.transform.name)
            {
                productFruits.RemoveAt(i);
            }
            if (productFruits[i] == fruitB.transform.name)
            {
                productFruits.RemoveAt(i);
            }
        }
    }

    //점수 획득
    public void AddScore(int addScore)
    {
        targetScore += addScore; //목표 점수에 추가 점수를 더한다
        scoreState = ScoreState.ScoreUp; //점수 증가 상태로 변환
    }

    //점수 증가
    void ScoreUp()
    {
        switch (scoreState)
        {
            case ScoreState.ScoreUp:
                {
                    //현재 점수를 목표 점수까지 scoreSpeed의 속도로 증가시킨다.
                    nowScore = Mathf.MoveTowards(nowScore, targetScore, scoreSpeed * Time.deltaTime);
                    //현재 점수를 표시한다. N0은 소수 0번째 자리부터 표시 -> 소수 표기 안함
                    fruitScoreText.text = nowScore.ToString("N0");

                    //현재 점수와 목표 점수가 같아지면
                    if(nowScore == targetScore)
                    {
                        //점수 증가 상태 종료
                        scoreState = ScoreState.None;
                    }
                    break;
                }
        }
    }

    //게임 오버
    public void FruitGameOver()
    {
        scoreState = ScoreState.None; //스코어 증가 중지
        gameOverPanel.SetActive(true); //게임 오버 패널 켜기

        //현재 기록이 최고 기록보다 클 경우
        if((int)nowScore > highScore)
        {
            highScore = (int)nowScore; //현재 기록을 최고 기록으로 경신

            //최고 기록을 저장
            PlayerPrefs.SetInt("HighScore", highScore);
        }
        nowScoreText.text = nowScore.ToString("N0"); //현재 기록 표시
        highScoreText.text = highScore.ToString("N0"); //최고 기록 표시
    }
}
