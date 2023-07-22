using UnityEngine;


public class enablerSMFocus : MonoBehaviour
{
    public IAStateMachineManager ISM;
    public float distanceFocus = 1f;
    public float maxDistnaceLoosFocus = 3f;
    public EnnemieAnimator EA;
    private Transform Player;
    // Start is called before the first frame update
    void Start()
    {
        Player = GameManager.instance.player;
    }

    // Update is called once per frame
    void Update()
    {
       

        if ( Vector3.Distance(Player.transform.position,this.transform.position) < distanceFocus  
             && !ISM.getStateValue(AvailaibleState.fight) )
        {
            EA.SetWalk(true);
            ISM.enableState(AvailaibleState.focus);
        } 
        
        if (Vector3.Distance(Player.transform.position, this.transform.position) > maxDistnaceLoosFocus || ISM.getStateValue(AvailaibleState.hit))
        {
            EA.SetWalk(true);
            if (ISM.getStateValue(AvailaibleState.hit))
            {
                EA.SetWalk(false);
            }
            ISM.disableState(AvailaibleState.focus);
        }
    }
}
