using System;
using System.Diagnostics;
using ColossalFramework.UI;
using ICities;
using UnityEngine;

namespace CityWebServer
{
    /// <summary>
    /// Adds a button to the UI for quick access to the website.
    /// </summary>
    /// <remarks>
    /// Most/All of this code was sourced from here: 
    /// https://github.com/AlexanderDzhoganov/Skylines-FPSCamera/blob/master/FPSCamera/Mod.cs
    /// </remarks>
    public class WebsiteButton : LoadingExtensionBase
    {
        private UIDragHandle _browserButtonDragHandle;
        private UIButton _browserButton;
        private UILabel _browserButtonLabel;

        public override void OnLevelLoaded(LoadMode mode)
        {
            // Get a reference to the game's UI.
            var uiView = UnityEngine.Object.FindObjectOfType<UIView>();

            var button = uiView.AddUIComponent(typeof(UIButton));
            _browserButton = button as UIButton;

            // The object should *never* be null.
            // We call this a "sanity check".
            if (_browserButton == null) { return; }
            
            // Give it a name so we can find it later (if we need to)
            _browserButton.name = "browserButton";

            // Create a drag handler and attach it to our button.
            _browserButtonDragHandle = button.AddUIComponent<UIDragHandle>();
            _browserButtonDragHandle.target = _browserButton;
            
            _browserButton.width = 36;
            _browserButton.height = 36;
            _browserButton.pressedBgSprite = "OptionBasePressed";
            _browserButton.normalBgSprite = "OptionBase";
            _browserButton.hoveredBgSprite = "OptionBaseHovered";
            _browserButton.disabledBgSprite = "OptionBaseDisabled";
            _browserButton.normalFgSprite = "ToolbarIconZoomOutGlobe";
            _browserButton.foregroundSpriteMode = UIForegroundSpriteMode.Scale;
            _browserButton.scaleFactor = 1.0f;
            _browserButton.tooltip = "Open Browser";
            _browserButton.tooltipBox = uiView.defaultTooltipBox;

            // Set button position.
            var x = 66;
            var y = 10;
            _browserButton.relativePosition = new Vector2(x, y);
            
            var labelObject = new GameObject();
            labelObject.transform.parent = uiView.transform;

            _browserButtonLabel = labelObject.AddComponent<UILabel>();
            _browserButtonLabel.textColor = new Color32(255, 255, 255, 255);
            _browserButtonLabel.transformPosition = new Vector3(1.15f, 0.90f);
            _browserButtonLabel.Hide();

            RegisterEvents();
        }

        private void RegisterEvents()
        {
            // Accept button clicks.
            _browserButton.eventClick += OnBrowserButtonClick;
        }

        private void OnBrowserButtonClick(UIComponent component, UIMouseEventParameter args)
        {
            // Accept clicks only when shift isn't pressed.
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                // If this wasn't a move operation, then run the click event.
                Process.Start("http://localhost:8080/index.html");    
            }
        }
    }
}
