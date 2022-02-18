using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RecipeCard : MonoBehaviour
{
    public Transform constructionBottom;
    public Transform buildingBottom;

    public SpriteRenderer thisRender;

    SpriteRenderer MakeResourceSprite(ResourceData data, ref bool hadAnyAny)
    {
        var render = Instantiate(MyPrefabs.current.GetShowCaseSprite(data.color == OreColor.Any));
        
        if(data.type == ResourceType.Bar)
        {
            render.sprite = MyPrefabs.current.block.render.sprite;
        }
        else
        {
            render.sprite = Database.current.spritesForResource.Get(data.type);
        }

        if (data.color == OreColor.Any)
        {
            hadAnyAny = true;
        }
        else
        {
            Database.SetColor(render, data.color);
        }

        return render;
    }

    SpriteRenderer MakeBuildingSprite(TutorialId id, bool blinking)
    {
        var render = Instantiate(MyPrefabs.current.GetShowCaseSprite(blinking));

        render.sprite = MyPrefabs.current.GetPrefab(id).GetComponent<SpriteRenderer>().sprite;

        if (!blinking)
        {
            Database.ClearColors(render);
        }

        return render;
    }

    public void Init(CombatRecipeData recipe)
    {
        bool blinkingBuilding = false;
        int i = 0;
        foreach (var item in recipe.items)
        {
            var instance = MakeResourceSprite(item, ref blinkingBuilding);
            instance.transform.position = constructionBottom.position + Construction.GetBlockOffset(i, item.type);
            instance.sortingOrder = thisRender.sortingOrder + 1 + i;
            instance.sortingLayerID = thisRender.sortingLayerID;
            instance.transform.SetParent(transform);
            i++;
        }

        var result = MakeBuildingSprite(recipe.result, blinkingBuilding);
        result.transform.position = buildingBottom.position;
        result.sortingLayerID = thisRender.sortingLayerID;
        result.sortingOrder = thisRender.sortingOrder + 1;
        result.transform.SetParent(transform);
    }
}
