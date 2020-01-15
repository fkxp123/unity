using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    public static PlayerStat instance;
    public int Hp;
    public int CurrentHp;
    public int atk;

    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        animator = GetComponent<Animator>();
        CurrentHp = Hp;
    }

    public void Hit(int enemyAtk)
    {
        CurrentHp -= enemyAtk;
        Debug.Log("hp : " + CurrentHp);
        animator.SetTrigger("takeDamage");
        if(CurrentHp <= 0)
        {
            Debug.Log("game over");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
