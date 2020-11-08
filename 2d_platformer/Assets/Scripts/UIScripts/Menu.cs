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
    //public GameObject Description;
    //public GameObject[] descriptions;
    public Text Description;
    Image[] img;

    public GameObject menu_UI;
    public GameObject selected_Menu;
    Player player;
    PlayerInput pi;
    int selectedCount = 0;

    public GameObject inventoryUI;
    public GameObject keyItemUI;
    public GameObject mappingUI;
    public GameObject settingUI;
    public GameObject logoutUI;

    public bool activated;
    public bool SelectedUI;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player>();
        selectedSlots = new GameObject[SelectedSlot.transform.childCount];
        img = new Image[SelectedSlot.transform.childCount];
        pi = GetComponent<PlayerInput>();
        Description.text = "";
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
        if (activated)//pause상태일때만 키입력받기
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
            switch (selectedCount)
            {
                case 0:
                    Description.text = "장비";
                    break;
                case 1:
                    Description.text = "주요 아이템";
                    break;
                case 2:
                    Description.text = "지도";
                    break;
                case 3:
                    Description.text = "설정";
                    break;
                case 4:
                    Description.text = "시작 화면으로 돌아가기";
                    break;
                default:
                    break;
            }
            img[selectedCount].color = new Color(1f, 1f, 1f, 1f);
            if (Input.GetKeyDown(KeyCode.A))
            {
                SelectedUI = true;
                selected_Menu.SetActive(true);
                AllSelectedMenuSetActiveFalse();
                switch (selectedCount)
                {
                    case 0:
                        inventoryUI.SetActive(true);
                        break;
                    case 1:
                        keyItemUI.SetActive(true);
                        break;
                    case 2:
                        mappingUI.SetActive(true);
                        break;
                    case 3:
                        settingUI.SetActive(true);
                        break;
                    case 4:
                        logoutUI.SetActive(true);//ui가 아니라 타이틀씬으로 이동?
                        break;
                    default:
                        break;
                }
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                if (GameIsPaused && !SelectedUI) Resume();
                if (SelectedUI)
                {
                    SelectedUIOff();
                }
            }
        } 
    }
    public void Resume()
    {
        activated = false;
        SelectedUIOff();
        selectedCount = 0;
        menu_UI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        player.enabled = true;
        pi.enabled = true;
    }
    public void Pause()
    {
        activated = true;
        menu_UI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        player.enabled = false;
        pi.enabled = false;
    }
    public void AllSelectedMenuSetActiveFalse()
    {
        inventoryUI.SetActive(false);
        keyItemUI.SetActive(false);
        mappingUI.SetActive(false);
        settingUI.SetActive(false);
        logoutUI.SetActive(false);
    }
    public void SelectedUIOff()
    {
        SelectedUI = false;
        selected_Menu.SetActive(false);
        AllSelectedMenuSetActiveFalse();
    }
}
