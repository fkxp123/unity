using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public static Menu instance;
    public bool GameIsPaused = false;

    public GameObject SelectedSlot;
    public GameObject[] selectedSlots;
    public GameObject Description;
    public GameObject[] descriptions;
    Image[] img;

    public GameObject menu_UI;
    Player player;
    int selectedCount = 0;

    public GameObject inventoryUI;
    public GameObject keyItemUI;
    public GameObject mappingUI;
    public GameObject settingUI;
    public GameObject logoutUI;

    public bool activated;

    // Start is called before the first frame update
    void Start()
    {
        player = Player.instance;
        Debug.Log(SelectedSlot.transform.childCount);
        selectedSlots = new GameObject[SelectedSlot.transform.childCount];
        img = new Image[SelectedSlot.transform.childCount];
        descriptions = new GameObject[Description.transform.childCount];
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
        if (activated)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                selectedCount += 1;
                if (selectedCount >= SelectedSlot.transform.childCount)
                {
                    selectedCount = 0;
                }
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                selectedCount -= 1;
                if (selectedCount < 0)
                {
                    selectedCount = SelectedSlot.transform.childCount - 1;
                }
            }
            for (int i = 0; i < SelectedSlot.transform.childCount; i++)
            {
                selectedSlots[i] = SelectedSlot.transform.GetChild(i).gameObject;
                img[i] = selectedSlots[i].GetComponent<Image>();
                img[i].color = new Color(1f, 1f, 1f, 0f);
            }
            for (int i = 0; i < Description.transform.childCount; i++)
            {
                descriptions[i] = Description.transform.GetChild(i).gameObject;
                descriptions[i].SetActive(false);
            }
            img[selectedCount].color = new Color(1f, 1f, 1f, 1f);
            descriptions[selectedCount].SetActive(true);
        }
    }
    public void Resume()
    {
        activated = false;
        selectedCount = 0;
        menu_UI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        player.enabled = true;
    }
    public void Pause()
    {
        activated = true;
        menu_UI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        player.enabled = false;
    }
}
