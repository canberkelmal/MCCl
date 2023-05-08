using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalVillageSc : MonoBehaviour
{
    GameManager gM;
    public int waveEnemyCount = 10;
    int tempThrowedCount = 0;
    public int hitCount = 0;
    float timer = 0;
    public int health = 100;

    bool animating = false;
    bool fired = false;
    // Start is called before the first frame update
    void Start()
    {
        gM = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Char") || collision.transform.CompareTag("Giant"))
        {
            HitToCastle(collision.gameObject);
        }
        if(!fired)
        {
            fired = true;
            gM.villageFires.SetActive(true);
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.transform.CompareTag("Char") || collision.transform.CompareTag("Giant"))
        {
            timer += Time.deltaTime;
        }

        if (timer >= gM.hitTimeDiff)
        {
            timer = 0;

            if (collision.transform.CompareTag("Char") || collision.transform.CompareTag("Giant"))
            {
                HitToCastle(collision.gameObject);
            }
        }
    }

    void HitToCastle(GameObject hittingChar)
    {
        gM.HitToAnyCastle(gameObject);
        if (!animating)
        {
            StartCoroutine(CastleHitAnimation());
        }
        hitCount++;
        if (hittingChar.transform.CompareTag("Giant"))
        {
            hittingChar.GetComponent<CharSc>().Punch();
        }
    }


    IEnumerator CastleHitAnimation()
    {
        animating = true;

        SkinnedMeshRenderer renderer = gameObject.GetComponent<SkinnedMeshRenderer>();
        int rnd = (int)UnityEngine.Random.Range(0, 10);
        int ind = rnd < 5 ? 0 : 1; 

        for (int i = 0; i < 51; i++)
        {
            i ++;
            renderer.SetBlendShapeWeight(ind, i);
            yield return new WaitForSeconds(gM.castleHitAnimSens / 10000);
        }
        for (int i = (int)renderer.GetBlendShapeWeight(ind); i > 0; i--)
        {
            i --;
            renderer.SetBlendShapeWeight(ind, i);
            yield return new WaitForSeconds(gM.castleHitAnimSens / 10000);
        }
        animating = false;
    }
}
