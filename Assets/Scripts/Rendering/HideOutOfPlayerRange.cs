using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FadeInOut))]
public class HideOutOfPlayerRange : MonoBehaviour
{
    [SerializeField]
    private LayerMask viewBlockingLayers;
    [SerializeField]
    private float visibleDistance = 10.0f;
    [SerializeField]
    private string playerTag = "Player";
    [SerializeField]
    private float objectRadius = 0.5f;
    [SerializeField]
    private float raycastOriginOffset = 0.1f;
    [SerializeField]
    private float fadeDuration = 0.25f;

    private Renderer[] renderers;
    private FadeInOut fadeComponent;

    private Coroutine activeCoroutine;

    private void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
        fadeComponent = GetComponent<FadeInOut>();
    }

    private void Update()
    {
        if (activeCoroutine != null || renderers.Length == 0)
        {
            return;
        }

        Vector3 playerPos = GameObject.FindGameObjectWithTag(playerTag).transform.position;
        Vector3 vectorToPlayer = (playerPos - transform.position);
        Vector3 directionToPlayer = vectorToPlayer.normalized;
        Vector3 startPosition = transform.position + directionToPlayer * objectRadius;
        Vector3 orthogonalVector = new Vector3(directionToPlayer.z, 0, -directionToPlayer.x);
        Vector3 leftRayDirection = (playerPos - (startPosition + orthogonalVector * raycastOriginOffset)).normalized;
        Vector3 rightRayDirection = (playerPos - (startPosition - orthogonalVector * raycastOriginOffset)).normalized;
        RaycastHit hit1;
        RaycastHit hit2;

        bool isInDistance = vectorToPlayer.magnitude < visibleDistance;
        bool hitBoth = Physics.Raycast(startPosition + orthogonalVector * raycastOriginOffset, leftRayDirection, out hit1, vectorToPlayer.magnitude, viewBlockingLayers);
        hitBoth &= Physics.Raycast(startPosition - orthogonalVector * raycastOriginOffset, rightRayDirection, out hit2, vectorToPlayer.magnitude, viewBlockingLayers);
        if (hit1.collider == null) Debug.DrawLine(startPosition + orthogonalVector * raycastOriginOffset, startPosition + orthogonalVector * raycastOriginOffset + leftRayDirection * vectorToPlayer.magnitude);
        else Debug.DrawLine(startPosition + orthogonalVector * raycastOriginOffset, hit1.point, Color.cyan);
        if (hit2.collider == null) Debug.DrawLine(startPosition - orthogonalVector * raycastOriginOffset, startPosition - orthogonalVector * raycastOriginOffset + rightRayDirection * vectorToPlayer.magnitude);
        else Debug.DrawLine(startPosition - orthogonalVector * raycastOriginOffset, hit2.point, Color.cyan);
        bool visible = isInDistance && !hitBoth;

        if (visible && !renderers[0].enabled)
        {
            activeCoroutine = StartCoroutine(Show());
        }
        else if(!visible && renderers[0].enabled)
        {
            activeCoroutine = StartCoroutine(Hide());
        }
    }

    private IEnumerator Show()
    {
        for (int i = 0; i < renderers.Length; ++i)
        {
            renderers[i].enabled = true;
        }
        fadeComponent.FadeIn(fadeDuration);
        yield return new WaitForSeconds(fadeDuration);
        activeCoroutine = null;
    }

    private IEnumerator Hide()
    {
        fadeComponent.FadeOut(fadeDuration);
        yield return new WaitForSeconds(fadeDuration);
        for (int i = 0; i < renderers.Length; ++i)
        {
            renderers[i].enabled = false;
        }
        activeCoroutine = null;
    }
}
