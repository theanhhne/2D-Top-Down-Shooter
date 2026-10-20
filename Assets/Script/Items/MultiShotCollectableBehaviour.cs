using UnityEngine;

public class MultiShotCollectableBehaviour : MonoBehaviour, ICollectableBehaviour
{
    [SerializeField]
    private int _extraBullets = 1;

    public void OnCollected(GameObject player)
    {
        PlayerShoot playerShoot = player.GetComponent<PlayerShoot>();

        if (playerShoot != null)
        {
            playerShoot.AddBullets(_extraBullets);
        }
    }
}
