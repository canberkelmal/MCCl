using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconScript : MonoBehaviour
{
    GameManager gM;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(transform.localPosition);
        gM = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.localPosition = Vector2.Lerp(transform.localPosition, Vector2.zero, gM.iconMovingSens * Time.deltaTime);
        if(transform.localPosition.y < 65 && transform.localPosition.y > -65 && transform.localPosition.x < 137 && transform.localPosition.x > -137)
        {
            if (gameObject.CompareTag("BoxIcon"))
            {
                gM.IncreaseBoxCount();
            }
            else if (gameObject.CompareTag("CoinIcon"))
            {
                gM.IncreaseCoinCount();
            }
            Destroy(gameObject);
        }
    }
}
