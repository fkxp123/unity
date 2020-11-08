using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour
{
    public bool stopCheckKey;
    public bool isKeyDown;
    public Vector2 directionalInput;

    //StateMachine stateMachine;
    public bool isKeyDownAttack;
    public bool isKeyDownJump;
    public bool isKeyDownRoll;
    public bool isKeyDownBowAttack;

    void Update()
    {
        //directionalInput = new Vector2(0, 0);
        if (!stopCheckKey)
        {
            directionalInput = CheckArrowKeyDown();
            //player.playerMovement.SetDirectionalInput(directionalInput);
            CheckInputKey();
        }
    }

    public Vector2 CheckArrowKeyDown()
    {
        float x = 0, y = 0;
        if (Input.GetKey(KeyCode.RightArrow))
        {
            x = 1;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            x = -1;
        }
        else x = 0;
        if (Input.GetKey(KeyCode.DownArrow))
        {
            y = -1;
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            y = 1;
        }
        else y = 0;
        return new Vector2(x, y);
    }//just checking player arrow_key and return vector(x,y)
    public void CheckInputKey()
    {
        CheckKeyDown();
        CheckKeyUp();
    }
    public void CheckKeyDown()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            isKeyDownJump = true;
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            isKeyDownRoll = true;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            isKeyDownAttack = true;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            isKeyDownBowAttack = true;
        }
        if (isKeyDownAttack || isKeyDownBowAttack || isKeyDownJump || isKeyDownRoll)
        {
            isKeyDown = true;
        }
        else
        {
            isKeyDown = false;
        }
    }
    public void CheckKeyUp()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            isKeyDownJump = false;
        }
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            isKeyDownRoll = false;
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            isKeyDownAttack = false;
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            isKeyDownBowAttack = false;
        }
    }

    public void StopCheckKey()
    {
        stopCheckKey = true;

        //키가 눌린상태에서 stopCheckKey가 true가 되면 
        //해당 키가 눌린상태에서 멈추게되어 StopCheckKey상태에서 
        //키를 누르지않아도 true로 설정되므로
        //StopCheckKey에서 모든 키의 bool값을 초기화
        isKeyDownAttack = false;
        isKeyDownJump = false;
        isKeyDownRoll = false;
        isKeyDownBowAttack = false;
    }
    public void ResetCheckKey()
    {
        stopCheckKey = false;
    }
}
