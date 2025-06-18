using UnityEngine;

public class EngineEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private float emissionRate = 20f;

    private void Start()
    {
        particles = gameObject.AddComponent<ParticleSystem>();
        var main = particles.main;
        main.startSpeed = 2f;
        main.startSize = 0.1f;
        main.startColor = new Color(0.8f, 0.9f, 1f, 0.7f);

        var emission = particles.emission;
        emission.rateOverTime = emissionRate;

        var shape = particles.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 15f;
    }

    public void SetActive(bool active)
    {
        if (active && !particles.isPlaying)
            particles.Play();
        else if (!active && particles.isPlaying)
            particles.Stop();
    }
}