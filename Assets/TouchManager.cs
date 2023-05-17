using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TouchManager : MonoBehaviour
{
    Rigidbody2D rb;

    IEnumerator movementCoroutine;
    IEnumerator decelerationCoroutine;

    public InputAction touchPositionAction;
    public InputAction touchPressAction;

    public float speed = 200;
    public float decelerationRate = 1.0f; // Tasa de desaceleración

    private void Awake()
    {
        touchPositionAction.Enable();
        touchPressAction.Enable();

        rb = GetComponent<Rigidbody2D>();
        movementCoroutine = MovementCoroutine();
    }

    private void OnEnable()
    {
        touchPressAction.performed += TouchPressed;
        touchPressAction.canceled += TouchReleased;
    }

    private void OnDisable()
    {
        touchPressAction.performed -= TouchPressed;
        touchPressAction.canceled -= TouchReleased;
    }

    private void TouchPressed(InputAction.CallbackContext context)
    {
        StartCoroutine(movementCoroutine);
        decelerationCoroutine = DecelerationCoroutine();
    }

    private void TouchReleased(InputAction.CallbackContext context)
    {
        StopCoroutine(movementCoroutine);
        StartCoroutine(decelerationCoroutine);
        decelerationCoroutine = null; // Reiniciar la corrutina de deceleración
    }

    private IEnumerator MovementCoroutine()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();

            Vector2 touchPosition = touchPositionAction.ReadValue<Vector2>();
            float screenWidth = Screen.width;

            if (touchPosition.x < screenWidth / 2)
            {
                // Mover hacia la izquierda
                // Debug.Log("pulsando izquierda");
                rb.velocity = new Vector2(-speed * Time.fixedDeltaTime, rb.velocity.y);
            }
            else
            {
                // Mover hacia la derecha
                // Debug.Log("pulsando derecha");
                rb.velocity = new Vector2(speed * Time.fixedDeltaTime, rb.velocity.y);
            }
        }
    }

    private IEnumerator DecelerationCoroutine()
    {
        while (rb.velocity.x != 0f)
        {
            yield return new WaitForFixedUpdate();

            Debug.Log("decelerando...");
            rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0f, decelerationRate * Time.fixedDeltaTime), rb.velocity.y);

            if (Mathf.Abs(rb.velocity.x) < 0.01f)
            {
                rb.velocity = new Vector2(0f, rb.velocity.y); // Establecer la velocidad en cero
                yield break; // Detener completamente la corrutina
            }
        }
    }


}
