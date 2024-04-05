using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class switchTrigger : Switchable
{


    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        _switchState = true;
    }

    public override void switchableSetup()
    {
        base.switchableSetup();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        flip(false);
        transform.GetComponent<Collider2D>().enabled = false;
    }
}
