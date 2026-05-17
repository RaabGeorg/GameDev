using UnityEngine;
using UnityEngine.InputSystem;

public class Sign : MonoBehaviour
{
    [SerializeField] private GameObject dialogBox;
    private bool canInteract;
    private InputAction interactAction;
    
    private void Start() {
    //By default, the interact action has to be held down for 0.5 seconds before being activated!
    //To make things simple in this lecture, we are using the Attack Action for now (Left Mouse)
    //For your own project: Change the input actions accordingly!
        this.interactAction = InputSystem.actions.FindAction( "Attack" );
        this.interactAction.performed += ToggleDialogBox;
        this.dialogBox.SetActive( false );
        this.canInteract = false ;
    }
    private void ToggleDialogBox( InputAction . CallbackContext cxt) {
        if ( this.canInteract) {
            this.dialogBox.SetActive(! this.dialogBox.activeInHierarchy);
        }
    }
    
    private void OnTriggerEnter( Collider other) {
        if (!other.CompareTag( "Player" )) return ;
        this.canInteract = true ;
    }
    private void OnTriggerExit( Collider other) {
        if (!other.CompareTag( "Player" )) return ;
        this.canInteract = false ;
        this.dialogBox.SetActive( false );
    }
}
