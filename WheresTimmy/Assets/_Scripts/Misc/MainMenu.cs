using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string Level;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StartGame()
    {
        SceneManager.LoadScene(Level);
    }
    public void OpenOptions()
    {

    }
    public void CloseOptions()
    {
        
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
