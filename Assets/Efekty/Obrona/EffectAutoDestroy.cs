using UnityEngine;

public class EffectAutoDestroy : MonoBehaviour
{
    public float lifeTime = 1f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}