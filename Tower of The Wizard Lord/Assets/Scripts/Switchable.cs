using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/**
 * Class contains behaviour for both switches and objects controlled by switches
 * This allows for some objects to be both, in case we want to implement chained interactions
 * 
 * Switches can be assigned multiple controllers. By default, all attached controllers must be activated to toggle a switch.
 * This behaviour can be changed by overriding the evaluateToggle method.
 * 
 * (Unity doesn't like serializing abstract classes or interfaces, so this combined class will have to do for now :/ )
 */
public class Switchable : MonoBehaviour
{
    private bool _ready;

    public virtual void Start()
    {
        _ready = false;
        toggles = new Dictionary<string, bool>();
        _switchState = false;
    }

    public virtual void switchableSetup()
    {
        foreach (Switchable controller in controllers)
        {
            toggles[controller.name] = controller._switchState;
            controller.OnFlip += toggleHandler;
        }
        //for (int i = 0; i < controllers.Length; i++)
        //{
        //    toggles[i] = controllers[i]._switchState;
            //handlers[i] = (state) =>
            //{
            //    toggles[i] = state;
            //    updateToggleState();
            //    Debug.Log($"Signal recieved from controller {i}");
            //};
            //controllers[i].OnFlip += toggleHandler;
    }

    public virtual void Update()
    {
        if (!_ready)
        {
            try
            {
                switchableSetup();  // necessary to avoid race condition when relying on other entities
                _ready = true;
                Debug.Log($"Setup finished: {this.name}");
            }
            catch (NullReferenceException e)
            {
                _ready = false;
                Debug.Log($"Setup delayed: {this.name}" +
                    $"\n{e.StackTrace}");
            }
        }
    }


    // fields and methods for handling behaviours of a switch
    public virtual event Action<bool, string> OnFlip;

    public bool _switchState { get; set; }

    public virtual void flip(bool state)
    {
        OnFlip?.Invoke(state, this.name);
    }


    // fields and methods for handling behaviours of an object controlled by a switch
    [SerializeField] Switchable[] controllers;
    private Dictionary<string, bool> toggles;

    private bool _state;

    public virtual void OnDestroy()
    {
        if (_ready)
        {
            for (int i = 0; i < controllers.Length; ++i)
            {
                controllers[i].OnFlip -= toggleHandler;
            }
        }
    }

    private void toggleHandler(bool state, string name)
    {
        toggles[name] = state;
        Debug.Log($"signal recieved from {name}");
        updateToggleState();
    }

    private void updateToggleState()
    {
        bool previous = _state;
        _state = evaluateToggle();

        if (_state != previous)
        {
            onStateChange(_state);
        }
    }

    /**
     * evaluate whether the rules for flipping the switch have been satisfied
     * can be overridden if different rules are preferred
     */
    public virtual bool evaluateToggle()
    {
        return toggles.Values.All(x => x);
    }

    /**
     * method to be called when the switch's state has been changed
     * this has no default implementation and must be overridden by derived classes
     */
    public virtual void onStateChange(bool state)
    {

    }
}