/*
 * Copyright (c) 2019 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * Notwithstanding the foregoing, you may not use, copy, modify, merge, publish, 
 * distribute, sublicense, create a derivative work, and/or sell copies of the 
 * Software in any work that is designed, intended, or marketed for pedagogical or 
 * instructional purposes related to programming, coding, application development, 
 * or information technology.  Permission for such use, copying, modification,
 * merger, publication, distribution, sublicensing, creation of derivative works, 
 * or sale is expressly withheld.
 *    
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
namespace RayWenderlich.CommandPatternInUnity
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class SceneManager : MonoBehaviour
    {
        public const float CommandPauseTime = 0.5f;

        [Header("Set In Inspector")]
        [SerializeField]
        private Bot bot = null;
        [SerializeField]
        private UIManager uiManager = null;
        
        //1
        //This list will store all the commands (To be provide an easy way to access for Do/Undo/Redo operations)
        private List<BotCommand> botCommands = new List<BotCommand>();
        
        //Handles the command execution
        private Coroutine executeRoutine;

        //2 Continuosly monitoring if the player presses the Enter Key to start executing the commands or waiting for Bot Commands via the keyboard
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Return))
            {
                ExecuteCommands();
            }
            else
            {
                CheckForBotCommands();
            }
        }

        //3 uses the static method HnadleInput from the BotInputHandler to check if the user entered any inputs
        private void CheckForBotCommands()
        {
            var botCommand = BotInputHandler.HandleInput();
            //If a command exsits and the program is not executing any commands
            if (botCommand != null && executeRoutine == null)
            {
                //Add the new commands to the List of commands
                AddToCommands(botCommand);
            }
        }

        //4 Adding a new reference to the returned command instance to botCommands
        private void AddToCommands(BotCommand botCommand)
        {
            botCommands.Add(botCommand);
            //5 Read the command name from the botCommand object and display it via the uiManager to the Game UI Elements
            uiManager.InsertNewText(botCommand.ToString());
        }

        //6 Initiating a call to the coroutine function 
        private void ExecuteCommands()
        {
            if(executeRoutine != null)
            {
                return;
            }

            executeRoutine = StartCoroutine(ExecuteCommandsRoutine());
        }

        //7 The coroutine function
        private IEnumerator ExecuteCommandsRoutine()
        {
            Debug.Log("Executing...");

            uiManager.ResetScrollToTop();

            //8 loop over the commands inside the list 
            for(int i = 0, count = botCommands.Count; i < count; i++)
            {
                var command = botCommands[i];
                command.Execute(bot);

                //9 After a command gets executed removes it from the top of the display
                uiManager.RemoveFirstTextLine();
                yield return new WaitForSeconds(CommandPauseTime);
            }

            //10 After executing all commands the list gets cleared
            botCommands.Clear();

            //The bot resets its position to the last check point crossed on
            bot.ResetToLastCheckpoint();

            //To allow the user to register new commands
            executeRoutine = null;
        }
    }
}
