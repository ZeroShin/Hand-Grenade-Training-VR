/************************************************************************************
Copyright : Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.

Licensed under the Oculus Utilities SDK License Version 1.31 (the "License"); you may not use
the Utilities SDK except in compliance with the License, which is provided at the time of installation
or download, or which otherwise accompanies this software in either electronic or hard copy form.

You may obtain a copy of the License at
https://developer.oculus.com/licenses/utilities-1.31

Unless required by applicable law or agreed to in writing, the Utilities SDK distributed
under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
ANY KIND, either express or implied. See the License for the specific language governing
permissions and limitations under the License.
************************************************************************************/

using System;
using UnityEngine;

/// <summary>
/// An object that can be grabbed and thrown by OVRGrabber.
/// </summary>
public class OVRGrabbable : MonoBehaviour
{
    //폭팔 지연시간..
    public float fire_delay = 2.0f;
    //폭팔 오브젝트
    public GameObject explosionEffect;

    //수류탄 핀 제거 사운드
    public AudioClip PullPin, DropPin;          //HJ
    AudioSource audio;                          //HJ

    //bool 타입으로 폭팔제어
    bool hasExploded = false;
    //폭탄 던지는 속도
    public float speed = 3.0f;

    [SerializeField]
    protected bool m_allowOffhandGrab = true;
    [SerializeField]
    protected bool m_snapPosition = false;
    [SerializeField]
    protected bool m_snapOrientation = false;
    [SerializeField]
    protected Transform m_snapOffset;
    [SerializeField]
    protected Collider[] m_grabPoints = null;

    protected bool m_grabbedKinematic = false;
    protected Collider m_grabbedCollider = null;
    protected OVRGrabber m_grabbedBy = null;
    //양손을 따로 사용하기 위해서 선언해주었다.
    protected OVRGrabber1 m_grabbedBy1 = null;

    //안전핀과 안전 클립을 따로 하기 위해서 사용
    public GameObject Clip;
    public GameObject Pin;

    private GameObject pinObj;                  //HJ
    private GameObject clipObj;                 //HJ

    //핀과 클립이 있는지를 파악해주기 위해서 사용함. 조건문 사용을 위해서 default를 false로 사용.
    //public bool pinCheck = true;
    //public bool clipCheck = false;
    //작동이 안되용..ㅠㅠ

    /// <summary>
    /// If true, the object can currently be grabbed.
    /// </summary>
    public bool allowOffhandGrab
    {
        get { return m_allowOffhandGrab; }
    }

    /// <summary>
    /// If true, the object is currently grabbed.
    /// </summary>
    public bool isGrabbed
    {
        get { return m_grabbedBy != null; }
    }

    public bool isGrabbed1
    {
        get { return m_grabbedBy1 != null; }
    }

    /// <summary>
    /// If true, the object's position will snap to match snapOffset when grabbed.
    /// </summary>
    public bool snapPosition
    {
        get { return m_snapPosition; }
    }

    /// <summary>
    /// If true, the object's orientation will snap to match snapOffset when grabbed.
    /// </summary>
    public bool snapOrientation
    {
        get { return m_snapOrientation; }
    }

    /// <summary>
    /// An offset relative to the OVRGrabber where this object can snap when grabbed.
    /// </summary>
    public Transform snapOffset
    {
        get { return m_snapOffset; }
    }

    /// <summary>
    /// Returns the OVRGrabber currently grabbing this object.
    /// </summary>
    public OVRGrabber grabbedBy
    {
        get { return m_grabbedBy; }
    }
    public OVRGrabber1 grabbedBy1
    {
        get { return m_grabbedBy1; }
    }

    /// <summary>
    /// The transform at which this object was grabbed.
    /// </summary>
    public Transform grabbedTransform
    {
        get { return m_grabbedCollider.transform; }
    }

    /// <summary>
    /// The Rigidbody of the collider that was used to grab this object.
    /// </summary>
    public Rigidbody grabbedRigidbody
    {
        get { return m_grabbedCollider.attachedRigidbody; }
    }

    /// <summary>
    /// The contact point(s) where the object was grabbed.
    /// </summary>
    public Collider[] grabPoints
    {
        get { return m_grabPoints; }
    }

    /// <summary>
    /// Notifies the object that it has been grabbed.
    /// </summary>
    virtual public void GrabBegin(OVRGrabber hand, Collider grabPoint)
    {
        m_grabbedBy = hand;
        m_grabbedCollider = grabPoint;
        gameObject.GetComponent<Rigidbody>().isKinematic = true;

        audio.PlayOneShot(PullPin, 0.7f);       //HJ
        pinObj.AddComponent<Outline>();         //HJ
        clipObj.AddComponent<Outline>();        //HJ

    }

    virtual public void GrabBegin1(OVRGrabber1 hand, Collider grabPoint)
    {
        //핀태그와 클립태그로 인해서 서로 다른 상황을 연출하도록 한다.
        if (Pin.tag == grabPoint.tag)
        {
            m_grabbedBy1 = hand;
            m_grabbedCollider = grabPoint;
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
            OVRInput.SetControllerVibration(0.8f, 0.5f, OVRInput.Controller.LTouch);
            // 핀과 클립이 제거됨을 확인한다.
            //pinCheck = true;
        }
        if (Clip.tag == grabPoint.tag)
        {
            m_grabbedBy1 = hand;
            m_grabbedCollider = grabPoint;
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
            Clip.transform.SetParent(null);
            OVRInput.SetControllerVibration(0.8f, 0.5f, OVRInput.Controller.RTouch);
            // 핀과 클립이 제거됨을 확인한다.
            //clipCheck = true;
        }
    }

    /// <summary>
    /// Notifies the object that it has been released.
    /// </summary>
    virtual public void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.isKinematic = m_grabbedKinematic;
        rb.velocity = linearVelocity * speed;
        rb.angularVelocity = angularVelocity;
        m_grabbedBy = null;
        m_grabbedCollider = null;
        // 차일드에 핀과 클립이 없을 경우에만 터지도록 유도한다.(실험중)
        Invoke("Explode", fire_delay);
        //if (pinCheck)
        //{
        //    // delay 이후 Explode를 실행한다.
        //    //Invoke("Explode", fire_delay);
        //}   
    }

    void Awake()
    {
        audio = GetComponent<AudioSource>();                //HJ

        if (m_grabPoints.Length == 0)
        {
            // Get the collider from the grabbable
            Collider collider = this.GetComponent<Collider>();
            if (collider == null)
            {
                throw new ArgumentException("Grabbables cannot have zero grab points and no collider -- please add a grab point or collider.");
            }

            // Create a default grab point
            m_grabPoints = new Collider[1] { collider };
        }
    }

    protected virtual void Start()
    {
        //m_grabbedKinematic = GetComponent<Rigidbody>().isKinematic;
        //테스트용 폭팔코드
        //Invoke("Explode", fire_delay);
        Clip = GameObject.FindWithTag("Clip");
        Pin = GameObject.FindWithTag("Pin");

        //audio = GetComponent<AudioSource>();                //HJ

        pinObj = GameObject.Find("Grenade_Safety_Pin");     //HJ
        clipObj = GameObject.Find("Grenade_Safetyclip");    //HJ

    }

    void OnDestroy()
    {
        if (m_grabbedBy != null)
        {
            // Notify the hand to release destroyed grabbables
            m_grabbedBy.ForceRelease(this);
        }
    }

    void Explode()
    {
        Instantiate(explosionEffect, transform.position, transform.rotation);
        // 폭탄이 터진다면 진동울리게 함. 주파수, 진폭 ( 0~1사이값을 받는다), 진동컨트롤러(양손동시 선언 안됨)
        OVRInput.SetControllerVibration(1, 1, OVRInput.Controller.RTouch);
        OVRInput.SetControllerVibration(1, 1, OVRInput.Controller.LTouch);

        Destroy(gameObject);
        hasExploded = true;
    }

    void OnCollisionEnter(Collision col)   //HJ
    {
        if (col.gameObject.tag == "FLOOR")
        {
            Debug.Log(audio);
            audio.PlayOneShot(DropPin, 0.7f);

        }

    }


}
