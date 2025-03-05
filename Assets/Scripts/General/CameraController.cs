using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private PlayerController thePlayer;
    public Transform playerItself;
    public Transform horizontalMove;
    private CinemachineVirtualCamera thisCamera;


    private void Awake()
    {
        thisCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        thePlayer = GetComponentInParent<ControllerManager>().thePlayer;

    }
    void Start()
    {
        thisCamera.Follow = playerItself;
    }

    // Update is called once per frame
    void Update()
    {
        //FollowPointUpdate();
    }
    private void FollowPointUpdate()
    {

        if (Mathf.Abs(thePlayer.thisRB.velocity.x) > .1f)
        {
            //Debug.Log("Ç°½ø");
            thisCamera.Follow = horizontalMove;

            thisCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset = new Vector3(thePlayer.faceDir, 0f, 0f);
        }
        else
        {
            thisCamera.Follow = playerItself;
            thisCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset = new Vector3(0f, 0f, 0f);

        }

    }
}
