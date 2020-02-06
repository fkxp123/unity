using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour
{
    public static PlayerInput instance;
    Player player;
    Animator animator;
    Controller2D controll;
    Menu menu;
    public bool stopAllInput;
    float x;
    float y;

    void Start()
    {
        player = GetComponent<Player>();
        //player = Player.instance;
        controll = GetComponent<Controller2D>();
        animator = GetComponent<Animator>();
        menu = Menu.instance;
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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menu.GameIsPaused)
            {
                menu.Resume();
            }
            else
            {
                menu.Pause();
            }
        }
        if (!stopAllInput)
        {
            Vector2 directionalInput = CheckArrowKEY();
            player.SetDirectionalInput(directionalInput);

            if (Input.GetKeyDown(KeyCode.A))
            {
                player.OnJumpInputDown();
            }
            else if (Input.GetKeyUp(KeyCode.A))
            {
                player.OnJumpInputUp();
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                player.Roll();
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                player.attackCount += 1;
                player.Attack();
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                player.BowAttack();
            }
        }
    }

    Vector2 CheckArrowKEY()
    {
        x = 0; y = 0;
        if (Input.GetKey(KeyCode.RightArrow))
        {
            x = 1;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            x = -1;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            y = -1;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            y = 1;
        }
        return new Vector2(x, y);
    }
}