using UnityEngine;
using System.Collections;

/// <summary>
/// Author: Erik Jungnickel - http://backyard-dev.de
/// This class registers user inputs and creates events when certain keys are pressed.
/// Other classes can subscribe to those events.
/// </summary>
public class InputController : MonoBehaviour {
    public event DirChangedEvent dirChanged;
    public delegate void DirChangedEvent(Vector3 dir);

    public event GameStateChanged stateChanged;
    public delegate void GameStateChanged(bool paused);

    bool gamePaused;

	// Use this for initialization
	void Start () {
        gamePaused = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (!gamePaused) //don't fire direction changed events if game is paused
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                dirChanged(Vector3.left);
            }
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                dirChanged(Vector3.right);
            }
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                dirChanged(Vector3.up);
            }
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                dirChanged(Vector3.down);
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            gamePaused = !gamePaused;
            stateChanged(gamePaused);
        }
	}
}
