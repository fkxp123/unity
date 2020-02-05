using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public static Menu instance;
    public bool GameIsPaused = false;

    public GameObject menu_UI;
    Player player;
    PlayerInput pi;

    // Start is called before the first frame update
    void Start()
    {
        player = Player.instance;
        //pi = PlayerInput.instance;
    }
    #region Singleton
    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    #endregion Sigleton

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Resume()
    {
        menu_UI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        //pi.stopAllInput = false;
        player.enabled = true;
    }
    public void Pause()
    {
        menu_UI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        //pi.stopAllInput = true;
        player.enabled = false;
    }
}
