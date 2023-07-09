using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{

    [SerializeField] private Animator animator; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

 
    public void setIsMoving(bool isMoving)
    {
        animator.SetBool("isMoving", isMoving);
    }

    public void setIsSprinting(bool isSprinting)
    {
        animator.SetBool("isSprinting", isSprinting);
    }

    public void setIsLedgeHanging(bool isLedgeHanging)
    {
        animator.SetBool("isLedgeHanging", isLedgeHanging);
    }

    public void setIsGround(bool isGrounded)
    {
        animator.SetBool("isGrounded", isGrounded);
    }

    public void startJump()
    {
        StartCoroutine(setJumpTrigger());
    }

    private IEnumerator setJumpTrigger()
    {
        animator.SetBool("didJump", true);

        yield return new WaitForSeconds(.5f);

        animator.SetBool("didJump", false);
    }
}
