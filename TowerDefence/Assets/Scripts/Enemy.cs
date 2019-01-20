using UnityEngine;
using System.Collections.Generic;

public class Enemy : MonoBehaviour {
    
    [SerializeField] private Transform exitPoint;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float navigationUpdate;
	[SerializeField] private int enemyHealth;
	[SerializeField] private int rewardPoints;

	private int target = 0 ;
    private Transform enemy;
	private Collider2D enemyCollider;
	private Animator anim;
    private float navigationTime = 0;
	private bool isDead = false;

	public bool IsDead
	{
		get { return isDead; }
	}

	// Use this for initialization
	void Start () {
	    enemy = GetComponent<Transform> ();
		enemyCollider = GetComponent<Collider2D>();
		anim = GetComponent<Animator>();
        GameManager.Instance.RegisterEnemy(this);    
	}
	
	// Update is called once per frame
	void Update () {
		if ( waypoints!= null && !isDead)
        {
            navigationTime += Time.deltaTime;
            if (navigationTime > navigationUpdate)
            {
                if(target < waypoints.Length)
                    enemy.position = Vector2.MoveTowards(enemy.position , waypoints[target].position , navigationTime);
                else 
                    enemy.position = Vector2.MoveTowards(enemy.position , exitPoint.position , navigationTime);

                navigationTime = 0;
            }
        }
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Checkpoint")
        {
            target++;
        }
        else if (other.tag == "Finish")
        {
			GameManager.Instance.RoundEscape += 1;
			GameManager.Instance.TotalEscape += 1;
            GameManager.Instance.UnregisterEnemy(this);
			GameManager.Instance.isWaveOver();
        }
		else if (other.tag == "Projectile")
		{
			Projectile newP = other.gameObject.GetComponent<Projectile>();
			int hitpoint = newP.AttackStrength;
			enemyHit(hitpoint);
			Destroy(other.gameObject);
		}
    }

    public void enemyHit(int hitpoints)
	{
		if (enemyHealth - hitpoints > 0)
		{
			enemyHealth -= hitpoints;
			GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Hit);
			anim.Play("Hurt");
		}
		else
		{
			anim.SetTrigger("didDead");
			die();
		}
	}

	public void die()
	{
		isDead = true;
		enemyCollider.enabled = false;
		GameManager.Instance.RoundKilled += 1;
		GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Death);
		GameManager.Instance.addMoney(rewardPoints);
		GameManager.Instance.isWaveOver();
	}

}
