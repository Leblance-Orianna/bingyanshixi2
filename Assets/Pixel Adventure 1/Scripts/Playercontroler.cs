using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Animator player;

    public float runSpeed;
    public float jumpSpeed;
    public Rigidbody2D playerRigidbody2D;
    public LayerMask mapLayer;
    public Collider2D playerCollider2D;

    private bool isGrounded = false;
    private bool isFirstJump;
    private float coyoteTime = 0.2f; // 角色离开地面后的延迟时间
    private float coyoteCounter = 0f; // 计时器
    private bool canDoubleJump = false; // 是否可以进行第二次跳跃
    private float groundWindow = 0.1f; // 地面时间窗口大小，单位为秒
    private float groundTimer = 0f; // 计时器，记录角色离开地面的时间
    public int maxJumpCount = 3; // 最大跳跃次数
    private int jumpCount = 0; // 已经跳跃的次数

    void Update()
    {
        Run();
        Jump();

        if (isGrounded)
        {
            canDoubleJump = true; // 当角色着地时，允许进行第二次跳跃
        }

        if (!isGrounded)
        {
            coyoteCounter += Time.deltaTime; // 开始计时
        }
        else
        {
            coyoteCounter = 0f; // 重置计时器
        }

        if (isGrounded)
        {
            canDoubleJump = true; // 当角色着地时，允许进行第二次跳跃
            groundTimer = 0f; // 重置计时器
        }
        else
        {
            groundTimer += Time.deltaTime; // 累加离开地面的时间
            if (groundTimer > groundWindow)
            {
                isGrounded = false; // 超出时间窗口后视为离开地面
            }
        }

        bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift); // 检测 Shift 键的按下状态

        if (isRunning)
        {
            runSpeed = 10f; // 设置疾跑时的移动速度
        }
        else
        {
            runSpeed = 5f; // 设置正常行走时的移动速度
        }
    }

    private void Run()
    {
        float runValue = Input.GetAxis("Horizontal");

        if (runValue != 0)
        {
            player.SetBool("Running", true);
            playerRigidbody2D.velocity = new Vector2(runValue * runSpeed, playerRigidbody2D.velocity.y);
            transform.localRotation = Quaternion.Euler(0, runValue < 0 ? 180 : 0, 0);
        }
        else
        {
            player.SetBool("Running", false);
        }
    }

    private void Jump()
    {
        float jumpValue = Input.GetAxis("Jump");

        if (isGrounded || coyoteCounter < coyoteTime) // 着地状态或离开地面不久内仍允许跳跃
        {
            if (isGrounded && jumpValue != 0) // 只有在着地且输入了起跳指令时才能起跳
            {
                player.SetBool("Jumping", true);
                playerRigidbody2D.velocity = new Vector2(0, jumpSpeed);
                jumpCount = 1; // 标记为第一次跳跃
            }
            else if (Input.GetButtonDown("Jump") && coyoteCounter < coyoteTime && jumpCount < maxJumpCount) // 在空中并且未达到最大跳跃次数时允许再次跳跃
            {
                player.SetBool("Jumping", true);
                playerRigidbody2D.velocity = new Vector2(0, jumpSpeed);
                jumpCount++; // 增加跳跃次数
            }
        }

        if (player.GetBool("Jumping"))
        {
            if (playerRigidbody2D.velocity.y < 0)
            {
                player.SetBool("Jumping", false);
                player.SetBool("Falling", true);
            }
        }
        if (playerCollider2D.IsTouchingLayers(mapLayer))
        {
            player.SetBool("Falling", false);
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
}


