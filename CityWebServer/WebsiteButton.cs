using System;
using System.Diagnostics;
using CityWebServer.Helpers;
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
        private const String keyButtonPositionX = "browserButtonPositionX";
        private const String keyButtonPositionY = "browserButtonPositionY";

        private UIDragHandle _browserButtonDragHandle;
        private UIButton _browserButton;
        private UILabel _browserButtonLabel;
        private Vector2 _buttonPosition;
        private Boolean _useSavedPosition;

        public override void OnLevelLoaded(LoadMode mode)
        {
            if (mode != LoadMode.LoadGame && mode != LoadMode.NewGame)
            {
                base.OnLevelLoaded(mode);
                return;
            }

            // Get a reference to the game's UI.
            var uiView = UnityEngine.Object.FindObjectOfType<UIView>();

            var button = uiView.AddUIComponent(typeof(UIButton));
            _browserButton = button as UIButton;

            // The object should *never* be null.
            // We call this a "sanity check".
            if (_browserButton == null) { return; }

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
            _browserButton.tooltip = "Open Browser (Hold Shift to drag)";
            _browserButton.tooltipBox = uiView.defaultTooltipBox;

            // If the user has moved the button, load their saved position data.
            if (Configuration.HasSetting(keyButtonPositionX) && Configuration.HasSetting(keyButtonPositionY))
            {
                var buttonPositionX = Configuration.GetFloat(keyButtonPositionX);
                var buttonPositionY = Configuration.GetFloat(keyButtonPositionY);
                _buttonPosition = new Vector2(buttonPositionX, buttonPositionY);
                _useSavedPosition = true;
            }
            else
            {
                _useSavedPosition = false;
            }

            // Since we're on another thread, we can pretty safely spin until our object has been created.
            //while (_browserButton == null) { System.Threading.Thread.Sleep(100); }

            if (!_useSavedPosition)
            {
                // Get a reference to the game's UI.
                //var uiView = UnityEngine.Object.FindObjectOfType<UIView>();

                // The default position of the button is the middle of the screen.
                var buttonPositionX = (uiView.fixedWidth / 2f) + (_browserButton.width / 2f);
                var buttonPositionY = (uiView.fixedHeight / 2f) + (_browserButton.height / 2f);
                _buttonPosition = new Vector2(buttonPositionX, buttonPositionY);
            }
            _browserButton.absolutePosition = _buttonPosition;

            var labelObject = new GameObject();
            labelObject.transform.parent = uiView.transform;

            _browserButtonLabel = labelObject.AddComponent<UILabel>();
            _browserButtonLabel.textColor = new Color32(255, 255, 255, 255);
            _browserButtonLabel.transformPosition = new Vector3(1.15f, 0.90f);
            _browserButtonLabel.Hide();

            RegisterEvents();

            base.OnLevelLoaded(mode);
        }

        private void RegisterEvents()
        {
            // Accept button clicks.
            _browserButton.eventClick += OnBrowserButtonClick;
            _browserButton.eventMouseLeave += OnBrowserButtonMouseLeave;
        }

        private void OnBrowserButtonMouseLeave(UIComponent component, UIMouseEventParameter eventParam)
        {
            _buttonPosition = component.absolutePosition;

            Configuration.SetFloat(keyButtonPositionX, _buttonPosition.x);
            Configuration.SetFloat(keyButtonPositionY, _buttonPosition.y);
            Configuration.SaveSettings();
        }

        private void OnBrowserButtonClick(UIComponent component, UIMouseEventParameter args)
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                // If the left control key is pressed, open the folder where the configuration file is located.
                var filePath = Configuration.GetSettingsFilePath();
                var directory = System.IO.Path.GetDirectoryName(filePath);
                if (!String.IsNullOrEmpty(directory) && System.IO.Directory.Exists(directory))
                {
                    Process.Start(directory);
                }
            }
            else if (!Input.GetKey(KeyCode.LeftShift))
            {
                // Accept clicks only when shift isn't pressed.
                var endpoint = String.Format("{0}index.html", IntegratedWebServer.Endpoint);
                Process.Start(endpoint);
            }
        }
    }
}