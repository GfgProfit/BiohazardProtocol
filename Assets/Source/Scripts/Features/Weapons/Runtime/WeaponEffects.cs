using System.Collections;
using UnityEngine;

public sealed class WeaponEffects
{
    private readonly Transform _barrel;
    private readonly Animator _animator;
    private readonly AudioSource _audio;

    public WeaponEffects(Transform barrel, Animator animator, AudioSource audio)
    {
        _barrel = barrel; _animator = animator; _audio = audio;
    }

    public void PlayShoot(AnimationNames names, bool aimed) => _animator.Play(aimed ? names.AimShoot : names.Shoot);

    public void SetAimBool(string aimBool, bool inScope) => _animator.SetBool(aimBool, inScope);

    public void MoveStateBool(string name, bool value) => _animator.SetBool(name, value);

    public void PlayOne(AudioClip clip)
    {
        if (clip)
        {
            _audio.PlayOneShot(clip);
        }
    }

    public void PlayOneFrom(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0)
        {
            return;
        }

        PlayOne(clips[Random.Range(0, clips.Length)]);
    }

    public void Muzzle(bool enabled, GameObject[] prefabs, float scale, float destroyAfter)
    {
        if (!enabled || prefabs == null || prefabs.Length == 0)
        {
            return;
        }

        GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
        GameObject go = Object.Instantiate(prefab, _barrel.position, _barrel.rotation);

        go.transform.localScale = Vector3.one * scale;

        Object.Destroy(go, destroyAfter);
    }

    public TrailRenderer CreateTrail(TrailRenderer prefab, AnimationCurve widthCurve, float duration, float minVertexDistance, Gradient color, Material material)
    {
        TrailRenderer trailRenderer = Object.Instantiate(prefab, _barrel.position, Quaternion.identity);

        trailRenderer.widthCurve = widthCurve; trailRenderer.time = duration; trailRenderer.minVertexDistance = minVertexDistance;
        trailRenderer.colorGradient = color; trailRenderer.material = material;

        return trailRenderer;
    }

    public static IEnumerator AnimateTrail(TrailRenderer trailRenderer, Vector3 start, Vector3 hitPoint, float speed)
    {
        float distance = Vector3.Distance(start, hitPoint);
        float remaining = distance;

        while (remaining > 0f)
        {
            trailRenderer.transform.position = Vector3.Lerp(start, hitPoint, 1f - (remaining / distance));
            remaining -= speed * Time.deltaTime;

            yield return null;
        }

        trailRenderer.transform.position = hitPoint;

        Object.Destroy(trailRenderer.gameObject, trailRenderer.time);
    }
}