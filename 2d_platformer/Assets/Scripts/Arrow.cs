using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float ArrowSpeed;
    public float DestroyTime;
    Player player;
    public float setDirection;

    public float Distance;
    public LayerMask isLayer;
    Enemy enemy;
    PlayerStat playerStat;

    // Start is called before the first frame update
    void Start()
    {
        player = Player.instance;
        enemy = Enemy.instance;
        playerStat = PlayerStat.instance;
        Invoke("DestroyArrow", DestroyTime);
        setDirection = player.transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D ray = Physics2D.Raycast(transform.position, setDirection * transform.right, Distance, isLayer);
        if(ray.collider != null)
        {
            if(ray.collider.tag == "enemy")
            {
                enemy.HitbyPlayer(playerStat.atk);
            }
            DestroyArrow();
        }
        if (setDirection == 1)
        {
            transform.Translate(Vector2.right * ArrowSpeed * Time.deltaTime);
        }
        else
        {
            transform.localScale = new Vector2(-1, 1);
            transform.Translate(-1 * Vector2.right * ArrowSpeed * Time.deltaTime);
        }
    }
    void DestroyArrow()
    {
        Destroy(gameObject);
    }
}
