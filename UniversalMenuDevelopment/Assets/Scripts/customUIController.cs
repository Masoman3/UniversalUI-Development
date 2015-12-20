using UnityEngine;
using System.Collections;
using UUI;
using UUI.Containers;
using UUI.Interactives;


public class customUIController : MonoBehaviour {
    UIController UserInterface;
    // Use this for initialization
    void Start () {
        UserInterface = new UIController(gameObject.GetComponent<Canvas>(), CreateMainMenuScene());
    }
	
	// Update is called once per frame
	void Update () {
	
	}


    UIScene CreateMainMenuScene()
    {
        UIScene Main = new UIScene("MainMenu", Color.white);

        Main.addUIContainer(new UIPanel("Menu", Color.red));
        //Main.GetSubContainer("Menu").addUIContainer(new UIPanel("Container", Color.blue));
        //Main.GetSubContainer("Menu").addUIContainer(CreateSubMenu());

        //UIElement[] MainMenu =
        //{
        //    new UniversalUI.Elements.UITriggers.UIButton("NewGame", "Create New Game", "sc-NewGame"),
        //    new UniversalUI.Elements.UITriggers.UIButton("LoadGame", "Load Saved Game", "sc-LoadGame"),
        //    new UniversalUI.Elements.UITriggers.UIButton("Options", "Open Options", "sc-Options")
        //};



        //Main.addUIContainer(CreateOptionsPopup());

        return Main;
    }

    //UISubmenu CreateSubMenu()
    //{
    //    UIPanel Audio = new UIPanel("Audio", Color.cyan);
    //    UIPanel Visual = new UIPanel("Visual", Color.blue);
    //    UIPanel GameOptions = new UIPanel("Game Options", Color.green);


    //    UISubmenu Options = new UISubmenu("Test1", new Color(0.0f, 0.0f, 0.0f, 0.8f), GameOptions);
    //    Options.addUIContainer(Visual);
    //    Options.addUIContainer(Audio);

    //    return Options;
    //}
}
