using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class CharacterView : MonoBehaviour
{
    // ref for the fps view : https://gist.github.com/KarlRamstedt/407d50725c7b6abeaf43aee802fdd88e
    
    [Header("Compenents")] 
    public Camera cameraPlayer;
    public Transform playerBody;

    [Header("Attributes")] 
    public float mouseSensitivy = 35f;
    public float maxRotation = 90f;
    
    
    // Some private values

    private Quaternion baseQuaternion; // this quaternion is for store the first rotation of cam
    private Vector2 rotation;
    
    const string xAxis = "Mouse X"; // axis const
    const string yAxis = "Mouse Y";

    
    
    void Start()
    {
        // lock the mouse
        rotation = new Vector2(0,0);
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        GameManager.instance.e_PlayerHurt.AddListener( functionHit);

    }

    private bool PlayerHit = false;
    
    void Update()
    {
        fpsView();
    }

    private Quaternion xQuat;
    private Quaternion yQuat;


    private float offsetHit;
    public void functionHit()
    {
        offsetHit = Random.Range(-90f,90f);
    }
    
    void fpsView()
    {
        // this is for prevent mouse shift in begin of play mode
        if (Time.time < 0.5) return;

        rotation.x += Input.GetAxisRaw(xAxis) *  mouseSensitivy * Time.deltaTime ;
        rotation.y += Input.GetAxisRaw(yAxis) *  -mouseSensitivy * Time.deltaTime;
        rotation.y =  Mathf.Clamp(rotation.y, -maxRotation, maxRotation);


        offsetHit = Mathf.Lerp(offsetHit, 0.0f, Time.deltaTime*5.0f);

        xQuat = Quaternion.Euler(0,rotation.x, offsetHit);
        yQuat = Quaternion.Euler(rotation.y,0.0f, 0.0f);
        
        
        cameraPlayer.transform.localRotation = xQuat*yQuat;

    }
    
}
