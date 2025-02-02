#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CustomButtonReplacer : MonoBehaviour
{
    [MenuItem("Tools/Replace Selected Buttons with Darkening Buttons")]
    private static void ReplaceSelectedButtons()
    {
        foreach (GameObject selectedObject in Selection.gameObjects)
        {
            Button button = selectedObject.GetComponent<Button>();
            if (button != null)
                ReplaceButtonWithCustom(button);
        }

        Debug.Log("Replaced selected buttons with CustomUIButton");
    }

    private static void ReplaceButtonWithCustom(Button button)
    {
        GameObject buttonObj = button.gameObject;

        Button.ButtonClickedEvent clickedEvent = button.onClick;
        ColorBlock colorBlock = button.colors;
        Sprite originalSprite = button.GetComponent<Image>().sprite;

        DestroyImmediate(button);

        DarkeningButton customButton = buttonObj.AddComponent<DarkeningButton>();
        buttonObj.GetComponent<Image>().sprite = originalSprite;

        Button newButton = customButton.GetComponent<Button>();
        newButton.onClick = clickedEvent;
        newButton.colors = colorBlock;
    }
}
#endif
