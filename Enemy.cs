using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;

public class Enemy : MonoBehaviour
{

    [SerializeField] float health = 100;
    [Header("Enemy Fire Frequency")]
    [SerializeField] float shotCounter;
    [SerializeField] float minTimeBetweenShots = 0.3f;
    [SerializeField] float maxTimeBetweenShots = 2f;
    [Header("The laser object")]
    [SerializeField] GameObject enemyLaser;
    [SerializeField] float enemyLaserSpeed = 8f;
    [Header("Enemy Explosion")]
    [SerializeField] GameObject enemyExplosion;
    [SerializeField] float explosionDuration;
    [Header("Audio effects")]
    [SerializeField] AudioClip enemyDeathSFX;
    [SerializeField] AudioClip enemyShootingSFX;
    [SerializeField] [Range(0,1)] float deathVolume;
    [SerializeField] [Range(0, 1)] float laserVolume;
    // Start is called before the first frame update
    void Start()
    {
        shotCounter = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
    }

    // Update is called once per frame
    void Update()
    {
        CountAndShoot();

    }

    private void CountAndShoot()
    {
        shotCounter -= Time.deltaTime;
        if (shotCounter <= 0f)
        {
            Fire();
            shotCounter = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);

        }
    }

    private void Fire()
    {
        GameObject laser = Instantiate(
            enemyLaser,
            transform.position,
            Quaternion.identity) as GameObject;
        laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -enemyLaserSpeed);
        AudioSource.PlayClipAtPoint(enemyShootingSFX, Camera.main.transform.position, laserVolume);

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
        if(!damageDealer)
        {
            return;
        }
        ProcessHit(damageDealer);
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();
        damageDealer.Hit();
        if (health <= 0)
        {
            Explosion();
        }
    }

    private void Explosion()
    {
        Destroy(gameObject);
        GameObject explosion = Instantiate(enemyExplosion, transform.position, transform.rotation);
        Destroy(explosion, explosionDuration);
        AudioSource.PlayClipAtPoint(enemyDeathSFX, Camera.main.transform.position, deathVolume);
    }
}
