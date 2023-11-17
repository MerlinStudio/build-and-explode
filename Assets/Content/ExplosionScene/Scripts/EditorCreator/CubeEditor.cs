using UnityEngine;

public class CubeEditor : MonoBehaviour
{
    [SerializeField] private string m_id;

    public string Id => m_id;
    
    private void OnDrawGizmos()
    {
        transform.position = new Vector3(
            Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.y),
            Mathf.RoundToInt(transform.position.z));
    }
}
