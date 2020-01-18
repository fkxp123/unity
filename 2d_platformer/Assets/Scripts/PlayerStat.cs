using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    public static PlayerStat instance;
    public int Hp = 100;
    public int CurrentHp;
    public int atk = 5;
    public float HitDistance = 3;

    Player player;
    PlayerInput pi;
    Animator animator;
    Rigidbody2D rigid;
    Enemy enemy;
    BoxCollider2D boxCollider;
    //GameObject enemy;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        pi = PlayerInput.instance;
        player = Player.instance;
        //pi = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        //enemy = GameObject.FindGameObjectWithTag("enemy");
        boxCollider = GetComponent<BoxCollider2D>();
        enemy = Enemy.instance;
        CurrentHp = Hp;
    }

    public void Hit(int enemyAtk)
    {
        StartCoroutine("HitCoroutine", enemyAtk);
    }
    IEnumerator HitCoroutine(int enemyAtk)
    {
        CurrentHp -= enemyAtk;
        Debug.Log("hp : " + CurrentHp);
        animator.SetTrigger("takeDamage");
        player.stopAllInput = true;
        player.stopAllMove = true;
        boxCollider.isTrigger = true;
        if (CurrentHp <= 0)
        {
            Debug.Log("game over");
        }
        if (enemy.transform.position.x <= transform.position.x)
        {
            rigid.velocity = new Vector2(HitDistance, rigid.velocity.y);
        }
        else
        {
            rigid.velocity = new Vector2(-1 * HitDistance, rigid.velocity.y);
        }
        yield return new WaitForSeconds(0.6f);
        player.stopAllInput = false;
        player.stopAllMove = false;
        rigid.velocity = new Vector2(0, 0);

        //반짝반짝모션~ <-무적상태 hit -x bool값으로 hit제어
        yield return new WaitForSeconds(0.8f);
        boxCollider.isTrigger = false;

    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("enemy"))
        {
            Hit(enemy.atk);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
