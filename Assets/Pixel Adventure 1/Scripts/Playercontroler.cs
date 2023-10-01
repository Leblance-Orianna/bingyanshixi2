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
    private float coyoteTime = 0.2f; // ��ɫ�뿪�������ӳ�ʱ��
    private float coyoteCounter = 0f; // ��ʱ��
    private bool canDoubleJump = false; // �Ƿ���Խ��еڶ�����Ծ
    private float groundWindow = 0.1f; // ����ʱ�䴰�ڴ�С����λΪ��
    private float groundTimer = 0f; // ��ʱ������¼��ɫ�뿪�����ʱ��
    public int maxJumpCount = 3; // �����Ծ����
    private int jumpCount = 0; // �Ѿ���Ծ�Ĵ���

    void Update()
    {
        Run();
        Jump();

        if (isGrounded)
        {
            canDoubleJump = true; // ����ɫ�ŵ�ʱ��������еڶ�����Ծ
        }

        if (!isGrounded)
        {
            coyoteCounter += Time.deltaTime; // ��ʼ��ʱ
        }
        else
        {
            coyoteCounter = 0f; // ���ü�ʱ��
        }

        if (isGrounded)
        {
            canDoubleJump = true; // ����ɫ�ŵ�ʱ��������еڶ�����Ծ
            groundTimer = 0f; // ���ü�ʱ��
        }
        else
        {
            groundTimer += Time.deltaTime; // �ۼ��뿪�����ʱ��
            if (groundTimer > groundWindow)
            {
                isGrounded = false; // ����ʱ�䴰�ں���Ϊ�뿪����
            }
        }

        bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift); // ��� Shift ���İ���״̬

        if (isRunning)
        {
            runSpeed = 10f; // ���ü���ʱ���ƶ��ٶ�
        }
        else
        {
            runSpeed = 5f; // ������������ʱ���ƶ��ٶ�
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

        if (isGrounded || coyoteCounter < coyoteTime) // �ŵ�״̬���뿪���治������������Ծ
        {
            if (isGrounded && jumpValue != 0) // ֻ�����ŵ�������������ָ��ʱ��������
            {
                player.SetBool("Jumping", true);
                playerRigidbody2D.velocity = new Vector2(0, jumpSpeed);
                jumpCount = 1; // ���Ϊ��һ����Ծ
            }
            else if (Input.GetButtonDown("Jump") && coyoteCounter < coyoteTime && jumpCount < maxJumpCount) // �ڿ��в���δ�ﵽ�����Ծ����ʱ�����ٴ���Ծ
            {
                player.SetBool("Jumping", true);
                playerRigidbody2D.velocity = new Vector2(0, jumpSpeed);
                jumpCount++; // ������Ծ����
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


