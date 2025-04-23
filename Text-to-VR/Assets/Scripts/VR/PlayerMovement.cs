using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float mouseSensitivity = 2f;
    private float rotationX = 0f;


    void Update()
    {
        // 이동 (WASD)
        float moveX = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        float moveZ = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        transform.Translate(moveX, 0, moveZ);

        // 마우스 우클릭 눌린 동안만 회전 가능
        if (Input.GetMouseButton(1)) // 마우스 오른쪽 버튼
        {
            // 마우스 좌우 → 몸통 회전
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            transform.Rotate(0, mouseX, 0);

            // 마우스 상하 → 카메라 회전
            rotationX -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            rotationX = Mathf.Clamp(rotationX, -90f, 90f);
            Camera.main.transform.localEulerAngles = new Vector3(rotationX, 0, 0);

        }

    }
}