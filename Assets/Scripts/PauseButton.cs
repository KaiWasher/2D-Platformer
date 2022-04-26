using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseButton : MonoBehaviour
{
    [SerializeField] private GameObject PauseMenu;

    public void PauseGame()
    {
        Debug.Log("Paused");
        Time.timeScale = 0;
        Instantiate(PauseMenu, new Vector3(0, 0, 0), Quaternion.identity);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        Destroy(PauseMenu.gameObject);
    }

    public void LoadLevel(int level)
    {
        SceneManager.LoadScene(level);
    }
}
