using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndTrigger : MonoBehaviour
{
    [SerializeField] private GameObject interactText;
    void Start()
    {
        interactText.SetActive(false);
    }
    void OnTriggerStay(Collider other)
    {
        interactText.SetActive(true);
        if (Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadScene("EndScene");
        }
    }
}
