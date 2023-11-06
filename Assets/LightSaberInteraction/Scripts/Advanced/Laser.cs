using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


namespace LightSaber.advanced
{
    [RequireComponent(typeof(XRGrabInteractable))]
    public class Laser : MonoBehaviour
    {
        #region Variables
        [SerializeField] private GameObject laserRoot;
        [SerializeField] private AudioClip laserOnSfx;
        [SerializeField] private AudioClip laserOffSfx;
        [SerializeField] private AudioClip laserHumSfx;
        [SerializeField] private AudioClip laserSwingSfx;
        [SerializeField] private float resizeSpeed = 1f;

        [Tooltip("angular velocity threshold to play the swing Sfx")]
        [SerializeField] private float angSpeedThresholdForSfx = 2f;

        private bool laserOn;
        private XRGrabInteractable grabInteractable;
        private AudioSource audioSource;
        private Vector3 fullLaserSize;
        private XRDeviceData controllerData=null;
        #endregion 

        private void Awake()
        {
            grabInteractable = GetComponent<XRGrabInteractable>();

        }
        private void OnEnable()
        {
            grabInteractable.activated.AddListener((arg) => ActivateLaser());
            grabInteractable.deactivated.AddListener((arg) => DeActivateLaser());

            grabInteractable.selectEntered.AddListener(GetXRControllerData);
            grabInteractable.selectExited.AddListener((arg) => { if (laserOn) DeActivateLaser(); });
        }
        private void OnDisable()
        {
            grabInteractable.activated.RemoveListener((arg) => ActivateLaser());
            grabInteractable.deactivated.RemoveListener((arg) => DeActivateLaser());

            grabInteractable.selectEntered.RemoveListener(GetXRControllerData);
            grabInteractable.selectExited.RemoveListener((arg) => { if (laserOn) DeActivateLaser(); });
        }

        private void GetXRControllerData(SelectEnterEventArgs arg0)
        {
            Transform interactorParent = arg0.interactorObject.transform.parent;
            controllerData = interactorParent.GetComponent<XRDeviceData>();
        }

        void Start()
        {
            InitializeLaser();
        }

        private void Update()
        {
            AdjustLaserScale();

            HandleContinuousLaserSound();
        }

        private bool IsLaserSwinging()
        {
            if (controllerData == null) return false;

            float laserAngularSpeed = controllerData.AngularVelocity.magnitude;

            //HACK: the code snippet commented below accomplishes the same as the return expression at the bottom, but it's more difficult to read.
            /*if (laserOn && laserAngularSpeed  > angVelocityThresholdForSfx)
                return true;
            else return false; */

            return laserOn && laserAngularSpeed > angSpeedThresholdForSfx;
        }
       
        private void InitializeLaser()
        {
            //Disable laserRoot gameobject
            laserRoot.SetActive(false);

            //Add audio Source component and make it a 3D sound
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1;

            //Cache maximum laser size and shrink laser
            fullLaserSize = laserRoot.transform.localScale;
            laserRoot.transform.localScale = new Vector3(fullLaserSize.x, 0f, fullLaserSize.z);
        }

        public void ActivateLaser()
        {
            laserOn = true;

            laserRoot.SetActive(true);

            PlaySoundAbruptly(laserOnSfx);
            audioSource.PlayOneShot(laserHumSfx);
            
        }

        public void DeActivateLaser()
        {
            laserOn = false;
            PlaySoundAbruptly(laserOffSfx);
        }

        private void PlaySoundAbruptly(AudioClip sound)
        {
                audioSource.Stop();
                audioSource.PlayOneShot(sound);
        }

        private void AdjustLaserScale()
        {
            //TODO: We will later move this function to a Coroutine for optimization.
            if (laserOn && laserRoot.transform.localScale.y < fullLaserSize.y)
            {
                //expand laser
                laserRoot.transform.localScale += new Vector3(0f, Time.deltaTime * resizeSpeed, 0f);
            }
            else if (!laserOn && laserRoot.transform.localScale.y > 0)
            {
                //shrink laser
                laserRoot.transform.localScale -= new Vector3(0f, Time.deltaTime * resizeSpeed, 0f);
            }
            else if (!laserOn)
            {
                //Deactivate the laser when fully shrunk
                laserRoot.SetActive(false);
            }
            else return;
        }

        void HandleContinuousLaserSound()
        {
            if (laserOn)
            {
                if (IsLaserSwinging())
                {
                    audioSource.PlayOneShot(laserSwingSfx);
                }
                else if (!audioSource.isPlaying)
                {
                    PlaySoundAbruptly(laserHumSfx);
                }
            }
        }

    }
}
