using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelManagment
{
    public class GameMenu : Menu<GameMenu>
    {
		public void OnPausePressed()
		{
            Time.timeScale = 0;
            //if (MenuManager.Instance != null && PausedMenu.Instance != null)
            //         {
            //             MenuManager.Instance.OpenMenu(PausedMenu.Instance);
            //         }
            PausedMenu.Open();
		}
    }
}
