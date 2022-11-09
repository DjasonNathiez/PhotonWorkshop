using Photon.Pun;
using UnityEngine;

public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable, IDamageable
{
    private PlayerNetworkSetup playerNS;
    private PhotonView photonView;
    public int currentHealth;

    private void Awake()
    {
        playerNS = GetComponent<PlayerNetworkSetup>();
        photonView = GetComponent<PhotonView>();
    }

    //Cette classe sert à envoyer et recevoir les informations du stream
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //IsWritting si on est le propriétaire => envoie les informations.
        if (stream.IsWriting)
        {
            stream.SendNext(currentHealth);
            stream.SendNext(playerNS.healthInfoText.text = currentHealth.ToString());
        }
        else
        {
            //dans ce cas là on est en IsReading et on recoit les informations. Si on est pas propriétaire du View.
            currentHealth = (int) stream.ReceiveNext();
            playerNS.healthInfoText.text = (string) stream.ReceiveNext();
        }
    }

    public void TakeDamage(int amount)
    {
        if (!photonView.IsMine) return; //Cette ligne permet d'empêcher que l'information ne soit envoyer à tous les autres clients.
        
        currentHealth -= amount;
        
        Debug.LogError(gameObject.name + " health is " + currentHealth);
        
        if (currentHealth <= 0)
        {
            Debug.LogError(gameObject.name + " health is " + currentHealth);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        IDamageable damageableEntity = other.gameObject.GetComponent<IDamageable>();
        
        if (damageableEntity != null)
        {
            damageableEntity.TakeDamage(1);
            Debug.LogError(other.gameObject.name + " take damage. It's new health is " + other.gameObject.GetComponent<PlayerManager>().currentHealth);
        }
    }
}
