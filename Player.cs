using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Player : MonoBehaviour {

    [Header("Player")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float padding = 1f;
    [SerializeField] int health = 200;

    [Header("Projectile")]
    [SerializeField] GameObject laserPrefab;
    [SerializeField] float projectileSpeed = 11f;
    [SerializeField] float projectileFiringPeriod = 0.1f;

    [Header("Audio effects")]
    [SerializeField] AudioClip playerDeathSFX;
    [SerializeField] AudioClip playerShootingSFX;
    [SerializeField] [Range(0, 1)] float playerDeathVolume;
    [SerializeField] [Range(0, 1)] float playerLaserVolume;

    Coroutine firingCoroutine;

    float xMin;
    float xMax;
    float yMin;
    float yMax;



    // Start is called before the first frame update
    void Start()
    {
        SetWorldBoundaries();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Fire();
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
            playerDeath();
        }
    }

    private void playerDeath()
    {
        FindObjectOfType<LevelLoader>().LoadGameOver();
        Destroy(gameObject);
        AudioSource.PlayClipAtPoint(playerDeathSFX, Camera.main.transform.position, playerDeathVolume);
    }

    private void Fire()
    {
        if (Input.GetButtonDown("Fire1"))
        {
           firingCoroutine = StartCoroutine(FireContinuously());
        }


        if (Input.GetButtonUp("Fire1"))
        {
            StopCoroutine(firingCoroutine);
        }
    }

    private void Move()
    {
        var deltaX = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
        var deltaY = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;
        var newXPos = Mathf.Clamp(transform.position.x + deltaX, xMin, xMax);
        var newYPos = Mathf.Clamp(transform.position.y + deltaY, yMin, yMax);
        transform.position = new Vector2(newXPos, newYPos);
    }
    private void SetWorldBoundaries()
    {
        Camera gameCamera = Camera.main;

        // X axis borders

        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + padding;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - padding; ;

        // Y axis borders

        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + padding;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - padding;
    }

    IEnumerator FireContinuously()
    {
        while (true)
        {
            GameObject laser = Instantiate(laserPrefab, transform.position, Quaternion.identity) as GameObject;
            laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, projectileSpeed);

            AudioSource.PlayClipAtPoint(playerShootingSFX, Camera.main.transform.position, playerLaserVolume);

            yield return new WaitForSeconds(projectileFiringPeriod);
        }

    }

}
