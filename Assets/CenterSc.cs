using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterSc : MonoBehaviour
{
    GameManager gM;
    // Start is called before the first frame update
    void Start()
    {
        gM = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (gameObject.CompareTag("Center1"))
            {
                gM.TrigCenter1();
            }
            else if (gameObject.CompareTag("Center2"))
            {
                gM.TrigCenter2();
            }
        }
    }
}
