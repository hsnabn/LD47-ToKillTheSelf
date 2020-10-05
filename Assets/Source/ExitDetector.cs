using UnityEngine;

public class ExitDetector : MonoBehaviour
{
    public GameDirector director;

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.TryGetComponent<PlayerController>(out _))
        {
            director.EnterNextRoom();
            Destroy(this.gameObject);
        }
    }
}
