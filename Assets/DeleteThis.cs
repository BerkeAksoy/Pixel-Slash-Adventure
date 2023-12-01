using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeleteThis : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(loadNextLevel());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator loadNextLevel()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(1);
    }
}
