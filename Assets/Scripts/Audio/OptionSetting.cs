using System.Collections;
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
        LoadVolume();
    }

    private void Update()
    {
        TogglePanel();
    }

    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        myMixer.SetFloat("music", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    public void SetSFXVolume()
    {
        float volume = SFXSlider.value;
        myMixer.SetFloat("sfx", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("sfxVolume", volume);
    }

    private void LoadVolume()
    {
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
            SFXSlider.value = PlayerPrefs.GetFloat("sfxVolume");
            SetMusicVolume();
            SetSFXVolume();
        }
    }

    private void TogglePanel()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && optionPanel != null)
        {
            optionPanel.SetActive(!optionPanel.activeSelf);
            Time.timeScale = optionPanel.activeSelf ? 0f : 1f; // Pause and Resume the game
        }
    }

    public void StartGame()
    {
        StartCoroutine(FadeAndLoadScene());
    }

    private IEnumerator FadeAndLoadScene()
    {
        if (fadeImage != null)
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
        }
    }

    public void ShowOption()
    {
        if (optionPanel != null)
        {
            optionPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void HideOption()
    {
        if (optionPanel != null)
        {
            optionPanel.SetActive(false);
            Time.timeScale = 1f;
        }
        else
        {
            Debug.LogError("optionPanel is not assigned!");
        }
    }

    public void BackToMainMenu()
    {
        // Set the time scale before loading the MainMenu scene
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void ExitGame()
    {
        Debug.Log("GameExit");
        Application.Quit();
    }
}
