using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndTrigger : MonoBehaviour
{
    [SerializeField] private GameObject interactText;
    bool canChangeScene = false;
    void Start()
    {
        interactText.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        canChangeScene = true;
        
    }
    void OnTriggerStay(Collider other)
    {
        canChangeScene = true;
        interactText.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        canChangeScene = false;
        interactText.SetActive(false);

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canChangeScene == true)
        {
            SceneManager.LoadScene("EndScene");
        }
    }
}
