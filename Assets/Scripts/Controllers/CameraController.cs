using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    private Transform target;
    public float smoothing;
    public static CameraController Instance { get; protected set; }
    public Vector2 minPosition;
    public Vector2 maxPosition;
    public LayerMask IndicatorLayer;
    public GameObject Boss,IndicatorObject;
    public bool IndicatorOn = false;

    void Start()
    {
        if (Instance != null)
        {
            Debug.Log("Err there are 2 instances of gameControllers");
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        target = Player.Instance.transform;
    }
    private void Update()
    {
        if(IndicatorOn == true)
        {
            Indicator();
        }
    }
    void Indicator()
    {
        Boss = GameObject.FindObjectOfType<FirstBoss>().gameObject;
        Vector3 dir =  Boss.transform.position - transform.position;
        dir.Normalize();
        RaycastHit2D hit = Physics2D.Raycast(transform.position,dir,9, IndicatorLayer);
        if (hit.collider != null&&hit.collider.tag != "Blocks")
        {

            if (Boss.GetComponent<SpriteRenderer>().isVisible == false)
            {
                IndicatorObject.SetActive(true);
                IndicatorObject.transform.position = hit.point;
            }
            else
            {
                IndicatorObject.SetActive(false);
            }
        }
    }
    void LateUpdate()
    {
        //5.5 5 | 83.5 54
        if(transform.position != target.position)
        {
            Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);

            targetPosition.x = Mathf.Clamp(targetPosition.x, minPosition.x, maxPosition.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minPosition.y, maxPosition.y);

            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing);

        }
    }
}
