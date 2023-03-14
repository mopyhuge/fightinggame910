using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectileScript : MonoBehaviour
{
    // Start is called before the first frame update

    public AudioSource audio;
    public AudioClip[] clips;
    public float damage;
    public float lifeTime;
    public PlayerController owner;
    public float speed;
    public Rigidbody2D rig;

    private void Awake()
    {
        audio = GetComponent<AudioSource>();
        rig = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        audio.PlayOneShot(clips[1]);
        if(collision.CompareTag("Player")) 
        {
            collision.GetComponent<PlayerController>().takeDamage(damage, owner);

        }
        transform.position = new Vector3(transform.position.x, transform.position.y + 100, transform.position.z);
        Destroy(gameObject);
    }
    public void onSpawn(float damage, float speed, PlayerController owner, float dir)
    {
        setDamage(damage);
        setOwner(owner);
        setSpeed(speed);
        rig.velocity = new Vector2(dir * speed, 0);
    }
    public void setOwner(PlayerController owner)
    {
        this.owner = owner;
    }

    public void setDamage(float damage)
    {
        this.damage = damage;
    }
    public void setDamage(int damage)
    {
        this.damage = damage;
    }
    public void setSpeed(float speed)
    {
        this.speed = speed;
    }
}
