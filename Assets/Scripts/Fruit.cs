using UnityEngine;

public class Fruit : MonoBehaviour
{
    public int fruitLevel; //과일 레벨
    public int productNum; //생산 번호
    public int fruitScore; //과일 점수

    public enum ControlType
    {
        Ready, Drop, End
    }
    public ControlType controlType = ControlType.Ready; //과일 조작 상태

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //과일 생산 번호 부여
    public void FruitNumberOn(int num, int lv)
    {
        productNum = num;
        fruitScore = lv * 2; //과일 점수 
    }

    //과일 초기화
    public void FruitInitialize(int lv)
    {
        fruitLevel = lv; //레벨 초기화
        controlType = ControlType.End; //낙하 완료 상태
    }

    public void FruitDropStart()
    {
        controlType = ControlType.Drop;
    }

    //충돌 시작 함수
    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch(controlType)
        {
            case ControlType.Drop:
                {
                    if (collision.transform.CompareTag("Mat"))
                    {
                        GameManager.instance.FruitDropOn(); //과일 낙하 완료
                        GameManager.instance.RandomFruit(); //다음 과일 뽑기
                        controlType = ControlType.End;
                    }
                    if(collision.transform.CompareTag("Fruit"))
                    {
                        GameManager.instance.FruitDropOn(); //과일 낙하 완료
                        GameManager.instance.RandomFruit(); //다음 과일 뽑기

                        //나와 충돌 과일의 레벨이 같다면
                        if(fruitLevel == collision.transform.GetComponent<Fruit>().fruitLevel)
                        {
                            //다음과일의 레벨이 모든 과일의 수보다 작다면
                            if (fruitLevel + 1 < GameManager.instance.fruitsPrefab.Count)
                            {
                                //과일을 합치는 함수를 호출한다.(다음 레벨, 자신, 충돌한 과일)
                                GameManager.instance.CombineFruit(fruitLevel + 1, transform, collision.transform);

                                Destroy(gameObject);
                                Destroy(collision.transform.gameObject);
                            }
                        }
                        controlType = ControlType.End;
                    }
                    break;
                }
            case ControlType.End: //낙하가 완료된 상태
                {
                    //충돌 대상이 Fruit 컴포넌트를 가지고 있다면
                    if(collision.transform.GetComponent<Fruit>())
                    {
                        //나와 충돌 과일의 레벨이 같다면
                        if (fruitLevel == collision.transform.GetComponent<Fruit>().fruitLevel)
                        {
                            //다음과일의 레벨이 모든 과일의 수보다 작다면
                            if (fruitLevel + 1 < GameManager.instance.fruitsPrefab.Count)
                            {
                                //과일을 합치는 함수를 호출한다.(다음 레벨, 자신, 충돌한 과일)
                                GameManager.instance.CombineFruit(fruitLevel + 1, transform, collision.transform);

                                Destroy(gameObject);
                                Destroy(collision.transform.gameObject);
                            }
                        }
                    }
                    break;
                }
        }
    }
}
