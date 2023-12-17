using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionSetting : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider SFXSlider;
    [SerializeField] private GameObject optionPanel;
    [SerializeField] private Image fadeImage;

    private AudioManager audioManager;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        optionPanel.SetActive(false);

        if (PlayerPrefs.HasKey("musicVolume"))
        {
            loadVolume();

        }
        else
        {
            setMusicVolume();
            setSFXVolume();
        }
    }

    private void Update()
    {
        TogglePanel();
    }

    public void setMusicVolume()
    {
        float volume = musicSlider.value;
        myMixer.SetFloat("music", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    public void setSFXVolume()
    {
        float volume = SFXSlider.value;
        myMixer.SetFloat("sfx", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("sfxVolume", volume);

        if (Input.GetMouseButtonDown(0))
        {
            //audioManager.PlaySFX(audioManager.PlaySFX()); 
        }

    }

    private void loadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        SFXSlider.value = PlayerPrefs.GetFloat("sfxVolume");

        setMusicVolume();
        setSFXVolume();
    }

    private void TogglePanel()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            optionPanel.SetActive(!optionPanel.activeSelf);
            Time.timeScale = optionPanel.activeSelf ? 0f : 1f; //// Pause and Resume the game
        }
    }

    public void StartGame()
    {
        StartCoroutine(FadeAndLoadScene());
    }

    private IEnumerator FadeAndLoadScene()
    {
        fadeImage.gameObject.SetActive(true);

        // Fade out
        for (float alpha = 0f; alpha <= 1f; alpha += Time.deltaTime)
        {
            fadeImage.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }

        // Load the next scene in the build index
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        // Check if there is a next scene
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("No next scene available. Check Build Settings.");
        }

        // // Fade in
        // for (float alpha = 1f; alpha >= 0f; alpha -= Time.deltaTime)
        // {
        //     fadeImage.color = new Color(0f, 0f, 0f, alpha);
        //     yield return null;
        // }

        // fadeImage.gameObject.SetActive(false);
    }


    public void ShowOption()
    {
        optionPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void HideOption()
    {
        optionPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void BackToMainMenu()
    {
        StartCoroutine(FadeAndLoadMainMenu());
    }

    private IEnumerator FadeAndLoadMainMenu()
    {
        optionPanel.SetActive(false);
        fadeImage.gameObject.SetActive(true);
        Time.timeScale = 1f;

        // Set the initial alpha value to 0 for the fade-in effect
        fadeImage.color = new Color(0f, 0f, 0f, 0f);

        // Fade in
        for (float alpha = 0f; alpha <= 1f; alpha += Time.deltaTime)
        {
            fadeImage.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }

        // Load the MainMenu scene
        SceneManager.LoadScene("MainMenu");

        // // Wait for a short duration before fading out
        // yield return new WaitForSeconds(0.5f);

        // // Fade out
        // for (float alpha = 1f; alpha >= 0f; alpha -= Time.deltaTime)
        // {
        //     fadeImage.color = new Color(0f, 0f, 0f, alpha);
        //     yield return null;
        // }

        // fadeImage.gameObject.SetActive(false);
    }

    public void ExitGame()
    {
        Debug.Log("GameExit");
        Application.Quit();
    }
}
