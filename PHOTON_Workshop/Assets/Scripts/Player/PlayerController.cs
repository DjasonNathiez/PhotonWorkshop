using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    private Vector2 input;
    
    #region UNITY

    private void Update()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (input.magnitude >= 0.5f)
        {
            transform.position += new Vector3(input.x, 0, input.y) * (moveSpeed * Time.deltaTime);
        }
    }

    #endregion

    #region RPC



    #endregion
}
