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
        RaycastHit hit1;
        RaycastHit hit2;

        bool isInDistance = vectorToPlayer.magnitude < visibleDistance;
        bool hitBoth = Physics.Raycast(startPosition + orthogonalVector * raycastOriginOffset, directionToPlayer, out hit1, vectorToPlayer.magnitude, viewBlockingLayers);
        hitBoth &= Physics.Raycast(startPosition - orthogonalVector * raycastOriginOffset, directionToPlayer, out hit2, vectorToPlayer.magnitude, viewBlockingLayers);
        Debug.DrawLine(startPosition + orthogonalVector * raycastOriginOffset, startPosition + orthogonalVector * raycastOriginOffset + directionToPlayer * vectorToPlayer.magnitude);
        Debug.DrawLine(startPosition - orthogonalVector * raycastOriginOffset, startPosition - orthogonalVector * raycastOriginOffset + directionToPlayer * vectorToPlayer.magnitude);
        Debug.DrawLine(startPosition, playerPos, Color.red);
        bool visible = isInDistance && !hitBoth;
        for(int i = 0; i < renderers.Length; ++i)
        {
            renderers[i].enabled = visible;
        }
    }
}
