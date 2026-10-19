using UnityEngine;

public class InvincibilityCollectableBehaviour : MonoBehaviour, ICollectableBehaviour
{
    [SerializeField]
    private float _invincibilityDuration;

    [SerializeField]
    private Color _invincibilityColor;

    [SerializeField]
    private int _numberOfFlashes;

    public void OnCollected(GameObject player)
    {
        var invincibilityController = player.GetComponent<InvincibilityController>();
        invincibilityController.StartInvincibility(_invincibilityDuration, _invincibilityColor, _numberOfFlashes);
    }
}