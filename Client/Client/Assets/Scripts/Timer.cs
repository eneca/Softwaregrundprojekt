using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    private float timeout;
    private bool start;
    public bool pause;

    public Text TextRoundTimer;
    public GameObject _Menu;


    // Start is called before the first frame update
    void Start()
    {
        enabled = false;
        start = false;
        pause = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!pause && start)
            timeout = timeout - Time.deltaTime;
        if (timeout <= 0)
        {
            _Menu.GetComponent<Menu>().Animation("Timeout");
        }
        TextRoundTimer.text = "" + (int)timeout;

    }

    public void GUITimer(int i)
    {
        timeout = (float)(i / 1000);
    }

    public void EnableScript()
    {
        start = true;
        enabled = true;
    }

}
