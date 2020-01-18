using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour
{
    public static PlayerInput instance;
    Player player;
    Animator animator;
    Controller2D controll;
    public bool stopAllInput;
    float x;
    float y;
    public Transform pos;
    public Vector2 boxSize;

    void Start()
    {
        instance = this;
        player = GetComponent<Player>();
        //player = Player.instance;
        controll = GetComponent<Controller2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        CheckStopAllInput();
        if (!stopAllInput)
        {
            CheckArrowKEY();
            Vector2 directionalInput = new Vector2(x, y);
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
                Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(pos.position, boxSize, 0);
                foreach (Collider2D collider in collider2Ds)
                {
                    Debug.Log(collider.tag);
                }
            }
        }
    }
    void CheckStopAllInput()
    {
        stopAllInput = player.stopAllInput;
    }
    void CheckArrowKEY()
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
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(pos.position, boxSize);
    }
}