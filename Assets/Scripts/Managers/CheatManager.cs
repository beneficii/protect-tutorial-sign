using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class CheatManager : MonoBehaviour
{
    public InputData playerInput;
    public TextMeshPro txt;

    string sequence = "";

    Toggles cheats = new Toggles();

    System.Action action = null;
    float activeUntil = 0f;

    bool recording = false;
    Vector2Int prevDirection = Vector2Int.zero;

    private void OnEnable()
    {
        GameEvents.PreRestart += ApplyCodes;
        ResourceId.OnInit += OnResourceCreated;
    }

    private void OnDisable()
    {
        GameEvents.PreRestart -= ApplyCodes;
        ResourceId.OnInit -= OnResourceCreated;
    }

    public void ApplyCodes()
    {
        if(Time.time < activeUntil)
        {
            action?.Invoke();
        }
    }

    void OnResourceCreated(ResourceId resource)
    {
        if(cheats.swapOreForBricks)
        {
            if(resource.data.type == ResourceType.Ore)
            {
                resource.data.type = ResourceType.Bar;
            }
        }
    }

    private void Update()
    {
        if (GameEvents.IsPaused) return;

        if(playerInput.useButton.State == KeyState.Down)
        {
            sequence = "";
            recording = true;
        }

        if(recording)
        {
            var direction = playerInput.Direction;

            if(prevDirection == Vector2Int.zero && direction != prevDirection)
            {
                if (direction == Vector2Int.left) sequence += "a";
                if (direction == Vector2Int.right) sequence += "d";
                if (direction == Vector2Int.up) sequence += "w";
                if (direction == Vector2Int.down) sequence += "s";
            }

            prevDirection = direction;
        }


        if (playerInput.useButton.State == KeyState.Up)
        {
            recording = false;
            switch (sequence)
            {
                case "wwaadd":
                    action = Database.current.asset.LoadAll;
                    txt.SetText("Cheat activated: Database load");
                    activeUntil = Time.time + 5f;
                    break;
                case "wasd":
                    UIInputSystem.is8Way = !UIInputSystem.is8Way;
                    txt.SetText("Direction controls changed");
                    activeUntil = Time.time + 2f;
                    break;
                case "sssss":
                    cheats.swapOreForBricks = !cheats.swapOreForBricks;
                    txt.SetText("Cheat activated: Ore is Bricks");
                    activeUntil = Time.time + 2f;
                    break;
                case "wswsws":
                    txt.SetText("Cheat activated: Speed");
                    Time.timeScale = Time.timeScale = 2f; 
                    break;
            }
        }

        if(activeUntil > 0 && Time.time > activeUntil)
        {
            activeUntil = 0;
            txt.SetText("");
        } 
    }

    class Toggles
    {
        public bool swapOreForBricks;
    }
}
