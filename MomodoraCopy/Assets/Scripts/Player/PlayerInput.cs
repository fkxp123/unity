using UnityEngine;
using System.Collections;

namespace MomodoraCopy
{
    public class PlayerInput : MonoBehaviour
    {
        public bool isKeyDown;
        public Vector2 directionalInput;

        public bool isKeyDownAttack;
        public bool isKeyDownJump;
        public bool isKeyDownRoll;
        public bool isKeyDownBowAttack;

        public ParticleSystem startChargingEffect;
        public ParticleSystem chargingEffect;
        public bool isBowCharging;
        public bool IsBowCharging
        {
            get { return isBowCharging; }
            set
            {
                if (value)
                {
                    //startChargingEffect.Play();
                    //chargingEffect.Play();
                }
                else
                {
                    //startChargingEffect.Stop();
                    //chargingEffect.Stop();
                }
                isBowCharging = value;
            }
        }
        bool maxBowCharged;
        public bool MaxBowCharged
        {
            get { return maxBowCharged; }
            set
            {
                maxBowCharged = value;
                if (value)
                {
                    if (!chargeCircleEffect.isPlaying)
                    {
                        chargeCircleEffect.Play();
                    }
                }
            }
        }
        float bowChargingTime = 1f;
        WaitForSeconds bowChargingDelay;

        PlayerMovement playerMovement;

        Coroutine bowChargingRoutine;
        public ParticleSystem chargeCircleEffect;

        IEnumerator ChargingBowAttack()
        {
            IsBowCharging = true;
            yield return bowChargingDelay;
            MaxBowCharged = true;
        }

        IEnumerator PlayChargeCircleEffect()
        {
            yield return bowChargingDelay;
        }

        public void UnchargingBowAttack()
        {
            StopCoroutine(bowChargingRoutine);
            IsBowCharging = false;
            MaxBowCharged = false;
        }

        void Start()
        {
            playerMovement = GetComponent<PlayerMovement>();

            bowChargingDelay = new WaitForSeconds(bowChargingTime);
        }

        void Update()
        {
            if (chargeCircleEffect.isPlaying)
            {
                chargeCircleEffect.gameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
            }

            if (GameManager.instance.stopPlayerInput)
            {
                directionalInput = Vector2.zero;
                return;
            }
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
            if (Input.GetKey(KeyboardManager.instance.UpKey))
            {
                y = 1;
            }
            else if (Input.GetKey(KeyboardManager.instance.DownKey))
            {
                y = -1;
            }

            return new Vector2(x, y);
        }

        public void CheckInputKey()
        {
            CheckKey();
            CheckKeyDown();
            CheckKeyUp();
        }

        public void CheckKey()
        {
            if (Input.GetKey(KeyboardManager.instance.BowAttackKey))
            {
                if (!IsBowCharging)
                {
                    bowChargingRoutine = StartCoroutine(ChargingBowAttack());
                    StartCoroutine(PlayChargeCircleEffect());
                }
            }
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
            if (isKeyDownAttack  || isKeyDownJump || isKeyDownRoll)
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
                UnchargingBowAttack();
                chargeCircleEffect.Stop();
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