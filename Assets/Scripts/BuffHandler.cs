using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffHandler : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField]
    private BuffType buffType;
    public bool isCollected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !isCollected)
        {
            switch (buffType)
            {
                case BuffType.LESS_GRAVITY:
                    playerController.isLowGravityBuffActive = true;
                    break;
                case BuffType.MORE_GRAVITY:
                    playerController.isLowGravityBuffActive = true;
                    break;
                case BuffType.SPEED_UP:
                    playerController.isLowGravityBuffActive = true;
                    break;
                case BuffType.SPEED_DOWN:
                    playerController.isLowGravityBuffActive = true;
                    break;
                case BuffType.EXTRA_SHIELD:
                    playerController.isLowGravityBuffActive = true;
                    break;

            }
            isCollected = true;
        }
    }
    public enum BuffType
    {
        LESS_GRAVITY,
        MORE_GRAVITY,
        SPEED_UP,
        SPEED_DOWN,
        EXTRA_SHIELD
    }
}
