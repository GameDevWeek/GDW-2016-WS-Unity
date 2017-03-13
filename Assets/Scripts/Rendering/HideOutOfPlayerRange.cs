using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private Renderer[] renderers;

    private void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
    }

    private void Update()
    {
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
        for(int i = 0; i < renderers.Length; ++i)
        {
            renderers[i].enabled = visible;
        }
    }
}
