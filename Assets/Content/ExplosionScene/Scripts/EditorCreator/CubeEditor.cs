using UnityEngine;

public class CubeEditor : MonoBehaviour
{
    
    private void OnDrawGizmos()
    {
        transform.position = new Vector3(
            Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.y),
            Mathf.RoundToInt(transform.position.z));
    }
}
