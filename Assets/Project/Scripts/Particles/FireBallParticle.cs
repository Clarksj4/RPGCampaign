using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallParticle : MonoBehaviour
{
    public ParticleSystem Ball;
    public ParticleSystem Tail;
    public ParticleSystem Explosion;
    public AudioSource Shot;
    public AudioSource ExplosionAudio;

    public float DestinationProximity = 0.1f;

    public void MoveToDetonate(Transform target, float speed, Action moveComplete, Action explosionComplete)
    {
        StartCoroutine(DoMoveToDetonate(target, speed, moveComplete, explosionComplete));
    }

    public void Move(Transform target, float speed, Action destinationReached)
    {
        StartCoroutine(DoMove(target, speed, destinationReached));
    }

    public void Detonate(bool destroy, Action explosionComplete)
    {
        DoDetonate(destroy, explosionComplete);
    }

    IEnumerator DoMoveToDetonate(Transform target, float speed, Action moveComplete, Action explosionComplete)
    {
        yield return StartCoroutine(DoMove(target, speed, moveComplete));
        yield return StartCoroutine(DoDetonate(true, explosionComplete));
    }

    IEnumerator DoMove(Transform target, float speed, Action destinationReached)
    {
        // Play particle systems and sound
        Ball.Play();
        Tail.Play();
        Shot.Play();

        // Move towards target transform over time (at speed)
        while (Vector3.Distance(transform.position, target.position) > DestinationProximity)
        {
            transform.LookAt(target);
            Vector3 moveStep = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            transform.position = moveStep;

            yield return null;
        }

        // Stop particle systems
        Ball.Stop();
        Tail.Stop();
        Shot.Stop();

        // Call the destinationReached delegate if it was supplied
        if (destinationReached != null)
            destinationReached();
    }

    IEnumerator DoDetonate(bool destroy, Action explosionComplete)
    {
        // Play particle systems and sound
        Explosion.Play();
        ExplosionAudio.Play();

        // Wait for explosion particle to finish
        yield return new WaitWhile(() => Explosion.isPlaying);

        // Stop particle systems
        Explosion.Stop();
        ExplosionAudio.Stop();

        // Call the explosionComplete delegate if it was supplied
        if (explosionComplete != null)
            explosionComplete();

        // Destroy the object is asked to
        if (destroy)
            Destroy(gameObject);
    }
}
