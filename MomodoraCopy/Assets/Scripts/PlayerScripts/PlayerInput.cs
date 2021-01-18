using UnityEngine;

namespace MomodoraCopy
{
    [RequireComponent(typeof(Player))]
    public class PlayerInput : MonoBehaviour
    {
        public bool isKeyDown;
        public Vector2 directionalInput;

        public bool isKeyDownAttack;
        public bool isKeyDownJump;
        public bool isKeyDownRoll;
        public bool isKeyDownBowAttack;

        PlayerMovement playerMovement;
        void Start()
        {
            playerMovement = GetComponent<PlayerMovement>();
        }
        void Update()
        {
            directionalInput = CheckArrowKeyDown();
            CheckInputKey();
        }

        public Vector2 CheckArrowKeyDown()
        {
            float x = 0, y = 0;
            if (Input.GetKey(KeyboardManager.instance.LeftKey))
            {
                x = -1;
            }
            else if (Input.GetKey(KeyboardManager.instance.RightKey))
            {
                x = 1;
            }
            else x = 0;
            if (Input.GetKey(KeyboardManager.instance.UpKey))
            {
                y = 1;
            }
            else if (Input.GetKey(KeyboardManager.instance.DownKey))
            {
                y = -1;
            }
            else y = 0;

            //if(playerMovement.moveType == PlayerMovement.MoveType.StopMove)
            //{
            //    x = 0;
            //}
            return new Vector2(x, y);
        }

        public void CheckInputKey()
        {
            CheckKeyDown();
            CheckKeyUp();
        }

        public void CheckKeyDown()
        {
            if (Input.GetKeyDown(KeyboardManager.instance.JumpKey))
            {
                isKeyDownJump = true;
            }
            else if (Input.GetKeyDown(KeyboardManager.instance.AttackKey))
            {
                isKeyDownAttack = true;
            }
            else if (Input.GetKeyDown(KeyboardManager.instance.BowAttackKey))
            {
                isKeyDownBowAttack = true;
            }
            else if (Input.GetKeyDown(KeyboardManager.instance.RollKey))
            {
                isKeyDownRoll = true;
            }
            else if (Input.GetKeyDown(KeyboardManager.instance.MenuKey))
            {
                MenuManager.instance.IsGamePaused = true;
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
            if (Input.GetKeyUp(KeyboardManager.instance.JumpKey))
            {
                isKeyDownJump = false;
            }
            else if (Input.GetKeyUp(KeyboardManager.instance.AttackKey))
            {
                isKeyDownAttack = false;
            }
            else if (Input.GetKeyUp(KeyboardManager.instance.BowAttackKey))
            {
                isKeyDownBowAttack = false;
            }
            else if (Input.GetKeyUp(KeyboardManager.instance.RollKey))
            {
                isKeyDownRoll = false;
            }
        }

        //public void StopCheckKey()
        //{
        //    stopCheckKey = true;

        //    /*키가 눌린상태에서 stopCheckKey가 true가 되면 
        //     *해당 키가 눌린상태에서 멈추게되어 StopCheckKey상태에서 
        //     *키를 누르지않아도 true로 설정되므로
        //     *StopCheckKey에서 모든 키의 bool값을 초기화*/
        //    isKeyDownAttack = false;
        //    isKeyDownJump = false;
        //    isKeyDownRoll = false;
        //    isKeyDownBowAttack = false;
        //}
        //public void ResetCheckKey()
        //{
        //    stopCheckKey = false;
        //}
    }

}