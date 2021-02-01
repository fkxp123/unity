using System.Collections;
using UnityEngine;

namespace MomodoraCopy
{
    public class PlayerStatus : MonoBehaviour
    {
        public static PlayerStatus instance;

        [SerializeField]
        float maxHp = 100.0f;
        [SerializeField]
        float hp = 100.0f;
        public float Hp
        {
            get { return hp; }
            set
            {
                hp = Mathf.Clamp(value, 0, maxHp);
                if (hp <= 0)
                {
                    //animator.play(hurt), vibration, particle, invoke-setactive(false);
                    gameObject.SetActive(false);
                }
            }
        }



        //public int Hp = 100;
        public int CurrentHp;
        public float meleeAtk = 10.0f;
        //public float rangeAtk = 5;
        public int atk = 5;
        public float HitDistance = 3;
        public int alpha = 0;

        Player player;
        Animator animator;
        Rigidbody2D rigid;
        Enemy enemy;
        BoxCollider2D boxCollider;
        SpriteRenderer spriteRenderer;
        HealthBar healthBar;
        public bool isHit;
        public bool noHitMode;

        void Start()
        {
            player = GetComponent<Player>();
            animator = GetComponent<Animator>();
            rigid = GetComponent<Rigidbody2D>();
            boxCollider = GetComponent<BoxCollider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            healthBar = HealthBar.instance;
            enemy = Enemy.instance;
            //CurrentHp = Hp;
        }
        #region Singleton
        private void Awake()
        {
            if (instance == null)
            {
                DontDestroyOnLoad(this.gameObject);
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(this.gameObject);
            }
        }
        #endregion Sigleton

        public void Hit(int enemyAtk)
        {
            if (!noHitMode)
            {
                if (!isHit)
                {
                    StartCoroutine("HitCoroutine", enemyAtk);
                }
            }
        }
        IEnumerator HitCoroutine(int enemyAtk)
        {
            isHit = true;
            CurrentHp -= enemyAtk;
            healthBar.currentHp -= enemyAtk;
            healthBar.SetBlinkImg();
            Debug.Log("hp : " + CurrentHp);
            animator.SetTrigger("takeDamage");
            //player.stopAllInput = true;
            //player.stopMoving_X = true;
            boxCollider.isTrigger = true;
            //player.directionalInput = new Vector2(0, 0);
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
            rigid.velocity = new Vector2(0, rigid.velocity.y);
            //player.stopMoving_X = false;
            //player.stopAllInput = false;
        }
        IEnumerator HitGracePeriod()
        {
            Debug.Log("im hit");
            for (int i = 0; i < 10; i++)
            {
                spriteRenderer.material.color = new Color(1f, 1f, 1f, .5f);
                yield return new WaitForSeconds(0.1f);
                spriteRenderer.material.color = new Color(1f, 1f, 1f, 1f);
                yield return new WaitForSeconds(0.1f);
            }
            isHit = false;
            boxCollider.isTrigger = false;
            StopCoroutine("HitGracePeriod");
        }

        void OnTriggerEnter2D(Collider2D col)
        {
            //if (col.gameObject.CompareTag("enemy") && !player.isRoll)
            //{
            //    Hit(enemy.atk);
            //}
        }
        void OnTriggerStay2D(Collider2D col)
        {
            //if (col.gameObject.CompareTag("enemy") && !player.isRoll)
            //{
            //    Hit(enemy.atk);
            //}
        }

        // Update is called once per frame
        void Update()
        {
            if (isHit)
            {
                StartCoroutine("HitGracePeriod");
            }
        }
        public void TakeDamage(float damage, DamageType damageType, Quaternion damagedRotation)
        {
            //Hp -= damage;
            //fsm.currentState = BasicEnemyFsm.State.Hurt;
            //transform.rotation = Quaternion.Euler(transform.rotation.x, damagedRotation.y == 0 ? 180 : 0, transform.rotation.z);
            //enemyMovement.direction.x = damagedRotation.y == 0 ? 1 : -1;
            //hitEffectRenderer.flip = transform.rotation.y == 0 ? new Vector3(1, 0, 0) : new Vector3(0, 0, 0);
            //hitEffect.Play();
            //if (damageType == DamageType.Range)
            //{
            //    CancelInvoke();
            //    Invoke("ResetMove", 0.25f);
            //    Invoke("SetStateChase", knockBackTime);
            //}
            //else
            //{
            //    CancelInvoke();
            //    Invoke("ResetMove", knockBackTime);
            //    Invoke("SetStateChase", knockBackTime);
            //}
        }
        public void CrushedDeath()
        {
            Debug.Log("im dead");
            Hp = 0;
        }
    }

}