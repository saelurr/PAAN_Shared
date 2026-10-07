// using UnityEngine;

// public class SpoonTip : MonoBehaviour
// {
//     public LayerMask leafLayer;
//     public float rayLength = 0.02f;

//     void FixedUpdate()
//     {
//         RaycastHit hit;
//         if (Physics.Raycast(transform.position, -transform.up, out hit, rayLength, leafLayer))
//         {
//             Debug.Log("Ray hit: " + hit.collider.gameObject.name + " UV: " + hit.textureCoord);
//             LeafPainter painter = hit.collider.GetComponent<LeafPainter>();
//             if (painter != null)
//                 Debug.Log("Found painter, stamping");
//             else
//                 Debug.Log("No LeafPainter found on: " + hit.collider.gameObject.name);
//         }
//         else
//         {
//             Debug.Log("Ray missed");
//         }
//     }
// }