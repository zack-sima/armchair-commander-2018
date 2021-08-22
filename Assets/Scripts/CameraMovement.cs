using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {
    //change cam to other one
    public bool aboveHeight;
    public float transformationHeight; 
    public Camera otherCamera; 

    Vector3 lastMousePos;
    Vector3 currentMousePos;
    Vector3 movePos;
    bool stoppedDrag;
    Controller controllerScript;

    //limits
    float heightMaxLimit = 60f;
    float heightMinLimit = 4f;

    public bool isMobile;

    //count fps
    float fps = 0;
    int fpsIndex = 0;

    private void Start() {
        controllerScript = GameObject.Find("Controller").GetComponent<Controller>();
    }
    void Update() {
        otherCamera.transform.position = transform.position; 
        if (aboveHeight) { 
            if (transform.position.y < transformationHeight) {
                otherCamera.name = "Main Camera";
                name = "Main Camera 2";
                otherCamera.GetComponent<CameraMovement>().enabled = true;
                otherCamera.GetComponent<Camera>().enabled = true;
                GetComponent<Camera>().enabled = false;
                GetComponent<CameraMovement>().enabled = false;
            }
        } else if (transform.position.y >= transformationHeight) {
            otherCamera.name = "Main Camera";
            name = "Main Camera 2";
            otherCamera.GetComponent<CameraMovement>().enabled = true;
            otherCamera.GetComponent<Camera>().enabled = true;
            GetComponent<Camera>().enabled = false;
            GetComponent<CameraMovement>().enabled = false;
        }

        fpsIndex++; 
        fps += Time.deltaTime;
        if (fpsIndex == 70) {
            fpsIndex = 0;
            //print(1 / (fps / 70f));
            fps = 0; 
        }

        lastMousePos = currentMousePos;
        currentMousePos = Input.mousePosition;
        if (Input.GetMouseButtonUp(0)) {
            stoppedDrag = true;
        }
        float speed = (1 + (transform.position.y - 4) / 10f) / 6;
        if (!controllerScript.mouseOnUI) {
            if (Input.touchCount < 2 && Input.GetMouseButton(0)) {
                    if (stoppedDrag) {
                        lastMousePos = currentMousePos;
                        stoppedDrag = false;
                    }
                    //control camera snapping
                    //if (Vector3.Distance(lastMousePos, currentMousePos) < 260f)
                    movePos = -(new Vector3(lastMousePos.y, 0, -lastMousePos.x) - new Vector3(currentMousePos.y, 0, -currentMousePos.x)) / 40;
            } else {
                stoppedDrag = true;
            }
            if (controllerScript.roundPassDelay <= 0)
                transform.position += new Vector3(movePos.z * speed, movePos.y, -movePos.x * speed);
            movePos *= 0.8f;
        }
        if (controllerScript.roundPassDelay <= 0) {
            //phone/tablet drag movement
            if (Input.touchCount == 2) {
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

                Vector3 hit1Point = Vector3.zero;

                RaycastHit hit1;
                Ray ray1 = Camera.main.ScreenPointToRay(new Vector3(touchOne.position.x, touchOne.position.y, 0));
                if (Physics.Raycast(ray1, out hit1, 1000f)) {
                    hit1Point = hit1.point;
                }

                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(new Vector3(touchZero.position.x, touchZero.position.y, 0));
                if (Physics.Raycast(ray, out hit, 1000f)) {
                    if (!(transform.position.y >= heightMaxLimit && deltaMagnitudeDiff > 0 || transform.position.y <= heightMinLimit && deltaMagnitudeDiff <= 0)) {
                        Quaternion originalRotation = transform.rotation;
                        transform.LookAt((hit.point + hit1Point) / 2);
                        transform.Translate(new Vector3(0, 0, -deltaMagnitudeDiff / 20), Space.Self);
                        transform.rotation = originalRotation;
                    }
                }
            }

            if (!isMobile) {
                if (Input.mouseScrollDelta.y < 0f && transform.position.y < heightMaxLimit) {
                    if (Input.mouseScrollDelta.y < -16f)
                        transform.Translate(Vector3.forward * Time.deltaTime * -16f * 4);
                    else
                        transform.Translate(Vector3.forward * Time.deltaTime * Input.mouseScrollDelta.y * 4);
                }
                if (Input.mouseScrollDelta.y > 0f && transform.position.y > heightMinLimit) {
                    if (Input.mouseScrollDelta.y > 16f)
                        transform.Translate(Vector3.forward * Time.deltaTime * 16f * 4);
                    else
                        transform.Translate(Vector3.back * Time.deltaTime * -Input.mouseScrollDelta.y * 4);
                }
                //keyboard movement
                Quaternion originalRot = transform.rotation;
                transform.rotation = Quaternion.identity;

                float mult = 1.5f;
                if (Input.GetKey(KeyCode.LeftShift)) {
                    mult = 3.5f;
                }
                if (Input.GetKey(KeyCode.W)) {
                    transform.Translate(Vector3.forward * Time.deltaTime * 3 * mult);
                }
                if (Input.GetKey(KeyCode.S)) {
                    transform.Translate(Vector3.back * Time.deltaTime * 3 * mult);
                }
                if (Input.GetKey(KeyCode.A)) {
                    transform.Translate(Vector3.left * Time.deltaTime * 3 * mult);
                }
                if (Input.GetKey(KeyCode.D)) {
                    transform.Translate(Vector3.right * Time.deltaTime * 3 * mult);
                }
                if (Input.GetKey(KeyCode.DownArrow) && transform.position.y < heightMaxLimit) {
                    transform.Translate(Vector3.up * Time.deltaTime * 3 * mult);
                }
                if (Input.GetKey(KeyCode.UpArrow) && transform.position.y > heightMinLimit) {
                    transform.Translate(Vector3.down * Time.deltaTime * 3 * mult);
                }
                transform.rotation = originalRot;
            }
            if (transform.position.y < heightMinLimit)
                transform.position = new Vector3(transform.position.x, heightMinLimit, transform.position.z);
        }
    }
}
