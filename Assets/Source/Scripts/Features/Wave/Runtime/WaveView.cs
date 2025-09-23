using System.Collections;
using UnityEngine;

public class WaveView : MonoBehaviour
{
    [SerializeField] private Animator _waveStartAnimator;
    [SerializeField] private Animator _waveEndAnimator;

    public void WaveStartAnimation()
    {
        StartCoroutine(Animation(_waveStartAnimator));
    }

    private IEnumerator Animation(Animator animator)
    {
        animator.SetBool("IsOut", false);

        yield return new WaitForSeconds(3.0f);

        animator.SetBool("IsOut", true);
    }
}