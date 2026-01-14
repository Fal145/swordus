using UnityEngine;

public class MoveCam : MonoBehaviour
{
    [SerializeField] Transform camPos;

    public void Update()
    {
        transform.position = camPos.position;
    }
}
