using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CrazyMinnow.SALSA;
using FullSerializer;


public class DaimonManager : MonoBehaviour
{

    
    private DialogueService dDialogMgr;
    public SpeechInputService mySpeechInputMgr;
    private SpeechOutputService mySpeechOutputMgr;

	public GameObject myCharacter;
	private Animator myAnimator;
	
    private Salsa3D salsa3D;
    public RandomEyes3D randomEyes;
    public GameObject[] lookTargets;

    [SerializeField]
    private fsSerializer _serializer = new fsSerializer();

    private Dictionary<string, object> _context = null;
    private bool _waitingForResponse = true;

    public float wait = 0.0f;
    public bool check = false;
    public bool play = false;



    // Start is called before the first frame update
    void Start()
    {
       
        dDialogMgr = GetComponent<DialogueService>();
        mySpeechInputMgr = GetComponent<SpeechInputService>();
        mySpeechOutputMgr = GetComponent<SpeechOutputService>();
		myAnimator = myCharacter.GetComponent<Animator>();
    }


    IEnumerator Look(float preDelay, float duration, GameObject lookTarget)
    {
        yield return new WaitForSeconds(preDelay);

        Debug.Log("Look=" + "LEFT/RIGHT");
        randomEyes.SetLookTarget(lookTarget);

        yield return new WaitForSeconds(duration);

        randomEyes.SetLookTarget(null);
    }


    // Update is called once per frame
    void Update()
    {


        if (check)
        {
            wait -= Time.deltaTime; //reverse count
        }

        if ((wait < 0f) && (check))
        { 

            //check that clip is not playing		
            Debug.Log ("-------------------- Speech Output has finished playing, now reactivating SpeechInput.");
            check = false;

            //Now let's start listening again.....
            mySpeechInputMgr.Active = true;
            mySpeechInputMgr.StartRecording();

        }
    }


    // check for exercise name (from ExerciseController.cs)
    // and run the according animation (in myAnimator)
	public void Animate( string exercise ) {
		
		switch (exercise) {
			case "B12":
				myAnimator.Play("BreathingIdle");
				break;
            case "waving":
                myAnimator.Play("waving");
                break;
			default:
				break;
		}
		
	}
	
}
