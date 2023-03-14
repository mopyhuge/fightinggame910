using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Max Values")]
    public int maxHp;
    public int maxJumps;
    public float curAtkTimer;
    public float slowTime;
    public float maxSpeed;
    public float maxChargeDmg;


    [Header("Cur Values")]
    public int curHP;
    public int curJumps;
    public int score;
    public float curMoveInput;
    public float lastHit;
    public float lastHitIce;
    public bool isSlowed;
    public float charge_dmg;
    public bool isCharging;
    public float chargeRate;

    [Header("Attacking")]
    public PlayerController curAttacker;
    public float attackDmg;
    public float attackSpeed;
    public float iceAtkSpeed;
    public float attackRate;
    public float lastAttackTime;
    public GameObject[] attackPrefabs;
    public int diecount;

    [Header("MODS")]
    public float moveSpeed;
    public float jumpForce;

    [Header("Audio Clips")]
    public AudioClip[] playerfx_list;

    [Header("Components")]
    [SerializeField]
    private Rigidbody2D rig;
    [SerializeField]
    private Animator anim;
    [SerializeField]
    private AudioSource audio;
    private Transform muzzle;
    private GameManager gameManager;
    private PlayerContainerUI playerUI;
    public GameObject deathEffectprefab;


    private void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        audio = GetComponent<AudioSource>();
        muzzle = GetComponentInChildren<muzzleScript>().GetComponent<Transform>();
        gameManager = GameObject.FindObjectOfType<GameManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        curJumps = maxJumps;
        curHP = maxHp;
        score = 0;
        diecount = 0;
        moveSpeed = maxSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < -10 || curHP <= 0)
        {
            die();
        }
        if(diecount >= 10)
        {
            die();
            diecount = 0;
        }
        if(curAttacker)
        {
            if(Time.time - lastHit > curAtkTimer)
            {
                curAttacker = null;
            }
        }
        if (isSlowed)
        {
            if (Time.time - lastHitIce > slowTime)
            {
                isSlowed = false;
                moveSpeed = maxSpeed;
            }
        }
        if (isCharging)
        {
            isSlowed = true;
            charge_dmg += chargeRate;
            if(charge_dmg > maxChargeDmg)
            {
                charge_dmg = maxChargeDmg;
            }
            playerUI.updateChargeBar(charge_dmg, maxChargeDmg);
        }
    }

    private void FixedUpdate()
    {
        move();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach(ContactPoint2D hit in collision.contacts)
        {
            if(hit.collider.CompareTag("Ground"))
            {
                if(hit.point.y < transform.position.y)
                {
                    curJumps = maxJumps;
                }
   
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }

    public void onMoveInput(InputAction.CallbackContext context)
    {
        print("MOPving");
        float x = context.ReadValue<float>();
        if (x > 0)
        {
            curMoveInput = 1;
        }
        else if (x < 0)
        {
            curMoveInput = -1;
        }
        else
        {
            curMoveInput = 0;
        }
    }


    private void move()
    {
        rig.velocity = new Vector2(curMoveInput * moveSpeed, rig.velocity.y);

        if(curMoveInput != 0)
        {
            transform.localScale = new Vector3(curMoveInput > 0 ? 1 : -1, 1, 1);
        }
    }

    private void jump()
    {

        audio.PlayOneShot(playerfx_list[0]);
        rig.velocity = new Vector2(rig.velocity.x, 0);
        rig.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    public void die()
    {
        Destroy(Instantiate(deathEffectprefab, transform.position, Quaternion.identity), 1f);
        audio.PlayOneShot(playerfx_list[1]);
        if(curAttacker!=null)
        {
            curAttacker.addScore();
        }
        else
        {
            score--;
            playerUI.updateScoreText(score);
            if (score < 0)
            {
                score = 0;
            } 
        }
        diecount++;
        respawn();

    }

    public void drop_out()
    {
        Destroy(playerUI.gameObject);
        Destroy(gameObject);
    }
    public void addScore()
    {
        score++;
        playerUI.updateScoreText(score);
    }

    public void takeDamage(int amount, PlayerController attacker)
    {
        curHP -= amount;
        curAttacker = attacker;
        lastHit = Time.time;
        if(isCharging)
        {
            charge_dmg /= 2;
        }
        playerUI.updateHealthBar(curHP, maxHp);
    }

    public void takeDamage(float amount,PlayerController attacker)
    {
        curHP -= (int)amount;
        curAttacker = attacker;
        lastHit = Time.time;
        if (isCharging)
        {
            charge_dmg /= 2;
        }
        playerUI.updateHealthBar(curHP, maxHp);
    }
    public void takeIceDamage(float amount, PlayerController attacker)
    {
        curHP -= (int)amount;
        curAttacker = attacker;
        lastHit = Time.time;
        isSlowed = true;
        lastHit = Time.time;
        moveSpeed /= 2;
        if (isCharging)
        {
            charge_dmg /= 2;
        }
        playerUI.updateHealthBar(curHP, maxHp);
    }

    private void respawn()
    {
        curHP = maxHp;
        curJumps = maxJumps;
        curAttacker = null;
        rig.velocity = Vector2.zero;
        transform.position = gameManager.spawn_points[Random.Range(0, gameManager.spawn_points.Length)].position;
        moveSpeed = maxSpeed;
    }

    public void onJumpInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if(curJumps > 0)
            {
                curJumps--;
                jump();
            }
            
        }
        
    }

    public void onBlockInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            print("Press block button");
        }

    }

    public void onStandardAtkInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && Time.time - lastAttackTime > attackRate)
        {
            lastAttackTime = Time.time;
            spawn_std_attack();
        }
        if (isCharging)
        {
            isCharging = false;
            charge_dmg = 0;
        }

    }

    public void spawn_std_attack()
    {
        GameObject fireBall = Instantiate(attackPrefabs[0], muzzle.position, Quaternion.identity);
        fireBall.GetComponent<projectileScript>().onSpawn(attackDmg, attackSpeed, this, transform.localScale.x);
    }

    

    public void onChargeAtkInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            isCharging = true;
            moveSpeed /= 2;
        }
        if (context.phase == InputActionPhase.Canceled)
        {
            isSlowed = false;
            isCharging = false;
            spawn_charge_attack();
            charge_dmg = 0;
            moveSpeed = maxSpeed;
        }

    }

    public void spawn_charge_attack()
    {
        GameObject chargeBall = Instantiate(attackPrefabs[2], muzzle.position, Quaternion.identity);
        chargeBall.GetComponent<projectileScript>().onSpawn(charge_dmg, iceAtkSpeed, this, transform.localScale.x);
    }

    public void onIceAtkInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && Time.time - lastAttackTime > attackRate*2)
        {
            lastAttackTime = Time.time;
            spawn_std_attack();
        }

    }

    public void spawn_ice_attack()
    {
        GameObject iceBall = Instantiate(attackPrefabs[1], muzzle.position, Quaternion.identity);
        iceBall.GetComponent<projectileScript>().onSpawn(attackDmg, iceAtkSpeed, this, transform.localScale.x);
    }

    public void onTaunt1Input(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            print("Press taunt 1 button");
        }

    }

    public void onTaunt2Input(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            print("Press taunt 2 button");
        }

    }

    public void onTaunt3Input(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            print("Press taunt 3 button");
        }

    }

    public void onTaunt4Input(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            print("Press taunt 4 button");
        }

    }

    public void onPauseInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            print("Press pause button");
        }

    }

    public void setUI(PlayerContainerUI playerUI)
    {
        this.playerUI = playerUI;
    }
}
