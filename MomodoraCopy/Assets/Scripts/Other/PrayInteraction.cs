using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MomodoraCopy
{
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class PrayInteraction : MonoBehaviour
    {
        LayerMask playerMask;

        public GameObject interactPoint;

        bool isNear;
        bool isPraying;

        Rigidbody2D rigid;

        int selectedCount;
        int SelectedCount
        {
            get { return selectedCount; }
            set
            {
                if (value > maxSelectedCount)
                {
                    value = 0;
                }
                else if (value < 0) 
                {
                    value = maxSelectedCount;
                }

                GameManager.instance.MoveChoiceButton(value, selectedCount);

                selectedCount = value;
            }
        }
        int maxSelectedCount = 2;

        CheckPoint checkPoint;

        void Start()
        {
            playerMask = 1 << 8;

            rigid = transform.GetComponent<Rigidbody2D>();
            rigid.bodyType = RigidbodyType2D.Kinematic;

            checkPoint = transform.parent.gameObject.GetComponent<CheckPoint>();
        }

        void Update()
        {
            if (isPraying)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    SelectedCount -= 1;
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    SelectedCount += 1;
                }
                else if (Input.GetKeyDown(KeyCode.A))
                {
                    switch (SelectedCount)
                    {
                        case 0:
                            GameManager.instance.Save(checkPoint);
                            GameManager.instance.ClosePraySelectBox();
                            GameManager.instance.playerFsm.stateMachine.SetState(GameManager.instance.playerFsm.idle);
                            GameManager.instance.StopRotateChoiceButton();
                            SelectedCount = 0;
                            isPraying = false;
                            break;
                        case 1:
                            break;
                        case 2:
                            GameManager.instance.ClosePraySelectBox();
                            GameManager.instance.playerFsm.stateMachine.SetState(GameManager.instance.playerFsm.idle);
                            GameManager.instance.StopRotateChoiceButton();
                            SelectedCount = 0;
                            isPraying = false;
                            break;
                        default:
                            break;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    GameManager.instance.ClosePraySelectBox();
                    GameManager.instance.playerFsm.stateMachine.SetState(GameManager.instance.playerFsm.idle);
                    GameManager.instance.StopRotateChoiceButton();
                    SelectedCount = 0;
                    isPraying = false;
                }
                
                return;
            }
            if (isNear)
            {
                if (Input.GetKeyDown(KeyboardManager.instance.UpKey) &&
                    GameManager.instance.playerMovement.isGround)
                {
                    isPraying = true;
                    GameManager.instance.OpenPraySelectBox();
                    GameManager.instance.playerFsm.stateMachine.SetState(GameManager.instance.playerFsm.pray);
                    GameManager.instance.StartRotateChoiceButton();
                }
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                if (isPraying)
                {
                    return;
                }
                GameManager.instance.OpenPrayBox(interactPoint.transform.position);
            }
        }

        void OnTriggerStay2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                isNear = true;

                if (isPraying)
                {
                    GameManager.instance.ClosePrayBox();
                    return;
                }

            }
        }
        void OnTriggerExit2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                isNear = false;
                GameManager.instance.ClosePrayBox();
            }
        }
    }
}