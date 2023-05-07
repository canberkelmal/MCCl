using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GateSc : MonoBehaviour
{
    public int gateMultipier = 2;

    private void Start()
    {
        transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>().text = "X" + gateMultipier;
    }
}


