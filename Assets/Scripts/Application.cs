using System;
using UnityEngine;

public class Application : MonoBehaviour
{
    // Reference to the root instances of the MVC.
    public Model model;
    public View view;
    public Controller controller;

    // Init things here
    void Start() {
        // init the model
        this.model.init();
        this.controller.init(model, view);

    }

}
