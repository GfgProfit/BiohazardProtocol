using UnityEngine;

public class RandomAnimChoice : MonoBehaviour
{
    public Animator animator;

    void Start()
    {
        // ������ ��� Walk
        animator.SetInteger("WalkIndex", Random.Range(0, 3));
        // ������ ��� Sprint
        animator.SetInteger("SprintIndex", Random.Range(0, 3));
    }
}
