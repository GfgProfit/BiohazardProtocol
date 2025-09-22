using UnityEngine;

public class RandomAnimChoice : MonoBehaviour
{
    public Animator animator;

    void Start()
    {
        // Рандом для Walk
        animator.SetInteger("WalkIndex", Random.Range(0, 3));
        // Рандом для Sprint
        animator.SetInteger("SprintIndex", Random.Range(0, 3));
    }
}
