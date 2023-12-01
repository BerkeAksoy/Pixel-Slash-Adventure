using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogActivator : MonoBehaviour
{

    public string[] lines;

    private bool canActivate = false, isActive = false;
    public bool isPerson = true;
    private TextTyper tT;

    private void Start()
    {
        tT = TextTyper.Instance;
    }

    private void Update()
    {
        if(canActivate && Input.GetKeyDown(KeyCode.E) && !isActive)
        {
            tT.startDialog(lines, isPerson);
            isActive = true;
        }
        else if (canActivate && isActive && Input.GetKeyDown(KeyCode.E))
        {
            if (tT.IsTyping())
            {
                tT.QuickSkip();
                Debug.Log("Skipped");
            }
            else
            {
                isActive = tT.UpdateText();
            }
        }

        if (isActive && !canActivate)
        {
            isActive = false;
            tT.closeDialog();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canActivate = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canActivate = false;
        }
    }


}
