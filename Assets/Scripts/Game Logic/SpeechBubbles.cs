using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechBubbles : MonoBehaviour
{

    
    [SerializeField] private GameObject m_mainCamera;
    [SerializeField] private GameObject m_dialogTarget;
    [SerializeField] private GameObject m_player;
    [SerializeField] private string m_initalTextGerman;
    [SerializeField] private string m_initalTextEnglish;
    [SerializeField] private bool english_German;
    [SerializeField] private Speaker m_speaker;
   

    public bool m_isActiv = true;
    private ElephantMovement elephantMovement;
    private ElephantControl elephantControl;
    private CameraController cameraController;
    private bool focusOnTarget=false;
   
	void Start ()
	{

        elephantMovement = m_player.GetComponent<ElephantMovement>();
        elephantControl = m_player.GetComponent<ElephantControl>();
        cameraController = m_mainCamera.GetComponent<CameraController>();
    }
	
	// Update is called once per frame
	void Update () {

        if (focusOnTarget)
        {
            if (!GameObject.FindObjectOfType<DialogSystem>().IsDialogInProgress())
            {
                cameraController.EnableNormalCam();
                elephantMovement.allowToMove = true;

                focusOnTarget = false;
                m_isActiv = false;
            }
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (m_isActiv)
        {
            if (other.tag == "Player")
            {
                activateDialog();
            }
        }
    }

    
      public  void activateDialog()
    {
       
                string initalText;
                if (english_German)
                {
                    initalText = m_initalTextEnglish;
                }
                else
                {
                    initalText = m_initalTextGerman;
                }

                elephantMovement.allowToMove = false;
                cameraController.EnableSpectatorCam(m_dialogTarget.transform);
                GameObject.FindObjectOfType<DialogSystem>().StartDialog(initalText, m_speaker);
                focusOnTarget = true;
            
        }
    }



