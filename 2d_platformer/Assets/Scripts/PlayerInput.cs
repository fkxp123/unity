using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour
{

    Player player;
    Animator animator;
    Controller2D controll;
    bool stopAllInput;
    float x;
    float y;

    void Start()
    {
        player = GetComponent<Player>();
        controll = GetComponent<Controller2D>();
    }

    void Update()
    {
        CheckStopAllInput();
        if (!stopAllInput)
        {
            CheckArrowKEY();
            Vector2 directionalInput = new Vector2(x, y);
            //Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            player.SetDirectionalInput(directionalInput);
            //player.SetDirectionalInput(new Vector2(1,0));

            if (Input.GetKeyDown(KeyCode.A))
            {
                player.OnJumpInputDown();
            }
            if (Input.GetKeyUp(KeyCode.A))
            {
                player.OnJumpInputUp();
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                player.Roll();
            }
            //if (Input.GetKeyDown(KeyCode.X))
            //{
            //    player.testAttack();
            //}
            
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            player.attackCount += 1;
            player.Attack();
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
}