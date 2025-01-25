using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>, DeathAnimation, TakeDamage
{
    [SerializeField] private float startMoveSpeed = 4f;

    [SerializeField] private SpriteRenderer spriteRenderer;
    private bool isInvincible = false;

    private Transform spawnPosition;

    [SerializeField] private Sprite[] playerSprites;
    [SerializeField] private int spriteIndex = 0;

    private PlayerControls playerControls;
    private Vector2 movement;
    private Rigidbody2D rb;
    private float moveSpeed;

    private bool damageAnim = false;

    [SerializeField] public int health = 3;

    [SerializeField] private Weapon playerWeapon;
    protected override void Awake()
    {
        base.Awake();

        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        spawnPosition = SpawnPoint.Instance.GetPosition();

        moveSpeed = startMoveSpeed;

    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void Update()
    {
        PlayerInput();
    }

    private void LateUpdate()
    {
        LimitPosition();
    }

    private void FixedUpdate()
    {
        if (!damageAnim) Move();
    }

    private void PlayerInput()
    {
        movement = playerControls.Movement.Move.ReadValue<Vector2>();
        playerControls.Combat.Shoot.performed += _ => playerWeapon.playerTrigger = true;
        playerControls.Combat.Shoot.canceled += _ => playerWeapon.playerTrigger = false;
    }

    private void Move()
    {
        rb.MovePosition(rb.position + movement * (moveSpeed * Time.fixedDeltaTime));
    }

    private void LimitPosition()
    {
        Bounds bounds = StageManager.Instance.playerArena.bounds;
        Vector2 maxPos = new Vector3(Math.Max(bounds.min.x, Math.Min(rb.position.x, bounds.max.x)), Math.Max(bounds.min.y, Math.Min(rb.position.y, bounds.max.y)));
        rb.position = maxPos;
    }

    public void TakeDamage(int damage)
    {
        if (!isInvincible)
        {
            health -= damage;
            StartCoroutine(DeathAnimation());
            if (health <= 0)
            {
                StageManager.Instance.Lose();
                // LOSE
            }
        }
    }

    public IEnumerator DeathAnimation()
    {
        isInvincible = true;
        damageAnim = true;
        do
        {
            spriteIndex = (spriteIndex + 1) % playerSprites.Length;
            if (spriteIndex != 0)
            {
                spriteRenderer.sprite = playerSprites[spriteIndex];
                this.transform.localScale = Vector3.one;
            }
            else
            {
                spriteRenderer.sprite = null;
                this.transform.localScale = Vector3.one / 2;
            }
            yield return new WaitForSeconds(0.3f);
        } while (spriteIndex > 0);
        damageAnim = false;
        BulletsManager.Instance.PopAllBullets();
        StartCoroutine(InvincibleRespawn());
    }

    public IEnumerator InvincibleRespawn()
    {
        if (spawnPosition != null) transform.position = spawnPosition.position;
        int i = 0;
        do
        {
            spriteRenderer.sprite = null;
            yield return new WaitForSeconds(0.2f);
            spriteRenderer.sprite = playerSprites[0];
            yield return new WaitForSeconds(0.2f);
            i++;
        } while (i < 3);
        isInvincible = false;
    }

}
