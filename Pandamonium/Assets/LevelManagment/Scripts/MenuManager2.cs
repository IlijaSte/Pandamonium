using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace LevelManagment
{
    public class MenuManager2 : MonoBehaviour
    {
        [SerializeField]
        private MainMenu mainMenuPrefab;
        [SerializeField]
        private SettingsMenu settingsMenuPrefab;
        [SerializeField]
        private CreditsMenu creditsScreenPrefab;
        [SerializeField]
        private PausedMenu pauseMenuPrefab;
        [SerializeField]
        private GameMenu gameMenuPrefab;
        [SerializeField]
        private WinScreen winScreenPrefab;

        [SerializeField]
        private Transform _menuParent;

        private Stack<Menu> _menuStack = new Stack<Menu>(); // initialize as an empty stack of menus

        private static MenuManager2 _instance;
        public static MenuManager2 Instance { get { return _instance; } }

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
                InitializeMenus();
                DontDestroyOnLoad(gameObject);
            }

            
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        private void InitializeMenus()
        {
            if (_menuParent == null)
            {
                GameObject menuParentObject = new GameObject("Menus");
                _menuParent = menuParentObject.transform;
            }

            DontDestroyOnLoad(_menuParent.gameObject);

            // Array of all menu prefabs
            //Menu[] menuPrefabs = { mainMenuPrefab, settingsMenuPrefab, creditsScreenPrefab, pauseMenuPrefab, gameMenuPrefab, winScreenPrefab };

            System.Type myType = this.GetType();
            Debug.Log(myType.ToString());

            BindingFlags myFlags = BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic;
            FieldInfo[] fieldsInfos = myType.GetFields(myFlags);
            //foreach (FieldInfo field in fieldsInfos)
            //{
            //    Debug.Log("field is:  " + field);
            //    //Debug.Log(field.GetValue(this) as Menu);
            //    //Debug.Log("GetValue() returns:  " +field.GetValue(this));
            //}

            foreach (FieldInfo field in fieldsInfos)
            {
                Menu prefab = field.GetValue(this) as Menu;
                
                Debug.Log(prefab);

                if (prefab != null)
                {
                    Menu menuInstance = Instantiate(prefab, _menuParent);
                    // turn off (set inactive) any object that is not main menu
                    if (prefab != mainMenuPrefab)
                    {
                        menuInstance.gameObject.SetActive(false);
                    }
                    else
                    {
                        OpenMenu(menuInstance);
                    }
                }
            }
        }

        public void OpenMenu(Menu menuInstance)
        {
            if (menuInstance == null)
            {
                Debug.LogWarning("MENUMANAGER OpenMenu() ERROR: invalide menu");
                return;
            }

            if (_menuStack.Count > 0)
            {
                foreach(Menu menu in _menuStack)
                {
                    menu.gameObject.SetActive(false);
                }
            }

            menuInstance.gameObject.SetActive(true);
            _menuStack.Push(menuInstance);
        }

        public void CloseMenu()
        {
            if (_menuStack.Count == 0)
            {
                Debug.LogWarning("MENUMANAGER CloseMenu() ERROR: No Menus in stack. (empty stack)");
                return;
            }

            Menu topMenu = _menuStack.Pop();
            topMenu.gameObject.SetActive(false);

            if (_menuStack.Count >0)
            {
                Menu nextMenu = _menuStack.Peek();
                nextMenu.gameObject.SetActive(true);
            }
        }
    }
}