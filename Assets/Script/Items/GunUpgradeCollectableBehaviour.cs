using UnityEngine;

public class GunUpgradeCollectableBehaviour : MonoBehaviour, ICollectableBehaviour
{
    [SerializeField]
    private int _maxBulletIncrease = 2;

    public void OnCollected(GameObject player)
    {
        PlayerShoot playerShoot = player.GetComponent<PlayerShoot>();

        if (playerShoot != null)
        {
            playerShoot.IncreaseMaxBullets(_maxBulletIncrease);
        }
    }
}
