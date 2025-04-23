using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float mouseSensitivity = 2f;
    private float rotationX = 0f;


    void Update()
    {
        // �̵� (WASD)
        float moveX = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        float moveZ = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        transform.Translate(moveX, 0, moveZ);

        // ���콺 ��Ŭ�� ���� ���ȸ� ȸ�� ����
        if (Input.GetMouseButton(1)) // ���콺 ������ ��ư
        {
            // ���콺 �¿� �� ���� ȸ��
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            transform.Rotate(0, mouseX, 0);

            // ���콺 ���� �� ī�޶� ȸ��
            rotationX -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            rotationX = Mathf.Clamp(rotationX, -90f, 90f);
            Camera.main.transform.localEulerAngles = new Vector3(rotationX, 0, 0);

        }

    }
}