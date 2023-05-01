using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float horizontalSens = 1f, 
                 xMin = 1f,
                 xMax = 1f;

    public Transform player;
    public GameObject char1, char2, castleLeft, castleRight;
    public float throwForce = 1f;
    public float defCharForwardForce = 1f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            StartCharSpawning();
        }

        if (Input.GetMouseButton(0))
        {
            UpdatePlayerPositionX();
        }

        if (Input.GetMouseButtonUp(0))
        {
            StopCharSpawning();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }
    }


    // Update the players's horizontal/X position while mouse/finger is moving
    void UpdatePlayerPositionX()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float moveAmount = mouseX * horizontalSens;
        Vector3 newPosX = player.position + new Vector3(moveAmount, 0, 0);
        newPosX.x = Mathf.Clamp(newPosX.x, xMin, xMax);
        player.transform.position = newPosX;
    }

    void StartCharSpawning()
    {
        InvokeRepeating("SpawnAndThrowChar",0f , 0.5f);
    }
    void StopCharSpawning()
    {
        CancelInvoke("SpawnAndThrowChar");
    }

    void SpawnAndThrowChar()
    {
        GameObject spawnedChar = Instantiate(char1, player.position + Vector3.forward * 4, Quaternion.identity);
        spawnedChar.GetComponent<CharSc>().ThrowChar();
    }

    // Reload the current scene to restart the game
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
