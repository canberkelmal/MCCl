using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinalPanelSc : MonoBehaviour
{
    GameManager gM;
    public GameObject victory, tx0, tx1, tx2, tx3;
    GameObject[] objs = new GameObject[5];
    public float animSens = 1f;

    int objCounter = 0;

    // Start is called before the first frame update 
    void Start()
    {
        gM = GameObject.Find("GameManager").GetComponent<GameManager>();
        victory = transform.GetChild(0).gameObject;
        tx0 = transform.GetChild(1).gameObject;
        tx1 = transform.GetChild(2).gameObject;
        tx2 = transform.GetChild(3).gameObject;
        tx3 = transform.GetChild(4).gameObject;

        objs[0] = victory;
        objs[1] = tx3;
        objs[2] = tx2;
        objs[3] = tx1;
        objs[4] = tx0;

        StartCoroutine(StartFinalTextAnim());
    }

    IEnumerator StartFinalTextAnim()
    {
        float tempColorA = 0;
        Color tempColor = objs[objCounter].GetComponent<Text>().color;

        objs[objCounter].SetActive(true);
        while (objs[objCounter].GetComponent<Text>().color.a < 1)
        {
            tempColorA = Mathf.MoveTowards(tempColorA, 255, animSens * Time.deltaTime);
            tempColor.a = tempColorA;
            objs[objCounter].GetComponent<Text>().color = tempColor;
            objs[objCounter].GetComponent<Text>().fontSize = 75 + (int)(tempColorA * 25);
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

        yield return new WaitForSeconds(1);
        while (objs[objCounter].GetComponent<Text>().color.a > 0)
        {
            tempColorA = Mathf.MoveTowards(tempColorA, 0, animSens * Time.deltaTime);
            tempColor.a = tempColorA;
            objs[objCounter].GetComponent<Text>().color = tempColor;
            objs[objCounter].GetComponent<Text>().fontSize = 75 + (int)(tempColor.a * 25);
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
        objs[objCounter].SetActive(false);
        if(objCounter < 4)
        {
            objCounter++;
            StartCoroutine(StartFinalTextAnim());
        }
        else{
            gM.AttackToVillageTimeDone();
        }

    }
}
