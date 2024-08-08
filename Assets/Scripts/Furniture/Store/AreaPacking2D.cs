using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AreaPacking2D
{
    public float sizeX;
    public float sizeY;

    public AreaSplitOption SplitOption;

    List<Area> availableAreas;

    struct Area
    {
        public float X;
        public float Y;

        public Vector2 origin;
    }

    public enum AreaSplitOption
    {
        Horizontal,
        Vertical
    }

    public struct Item
    {
        public float sizeX;
        public float sizeY;

        public float spacing;
    }

    public AreaPacking2D() { }

    public AreaPacking2D(float sizeX, float sizeY, AreaSplitOption splitOption)
    {
        this.sizeX = sizeX;
        this.sizeY = sizeY;
        SplitOption = splitOption;
    }

    public void StartArea()
    {
        availableAreas = new List<Area>(10)
        {
            new Area()
            {
                X = sizeX,
                Y = sizeY,
                origin = Vector2.zero,
            }
        };
    }

    public bool Pack(Item item, out Vector2 packedPosition)
    {
        for (int i = 0; i < availableAreas.Count; i++)
        {
            Area area = availableAreas[i];
            if (CanPack(area, item))
            {
                Area splitedArea = SplitArea(area, item, i);
                packedPosition = splitedArea.origin;

                return true;
            }
        }
        packedPosition = Vector2.zero;
        return false;
    }

    private bool CanPack(Area area, Item item)
    {
        if (area.X >= item.sizeX + item.spacing && area.Y >= item.sizeY + item.spacing)
        {
            return true;
        }

        return false;
    }

    private Area SplitArea(Area area, Item item, int index)
    {
        Area newArea_01 = new Area();
        Area newArea_02 = new Area();

        float itemSpaceX = item.sizeX + item.spacing;
        float itemSpaceY = item.sizeY + item.spacing;

        int exceedX = (itemSpaceX > area.X / 2 ? -1 : 1);
        int exceedY = (itemSpaceY > area.Y / 2 ? -1 : 1);

        switch (SplitOption)
        {
            case AreaSplitOption.Horizontal:
                newArea_01.X = area.X;
                newArea_01.Y = area.Y - itemSpaceY;
                newArea_01.origin = new Vector2(area.origin.x, area.origin.y + (area.Y / 2 - itemSpaceY) * exceedY + newArea_01.Y / 2 * exceedY);

                newArea_02.X = area.X - itemSpaceX;
                newArea_02.Y = area.Y - itemSpaceY;
                newArea_02.origin = new Vector2(area.origin.x + (area.X / 2 - itemSpaceX) * exceedX + newArea_02.X / 2 * exceedX, area.origin.y + (area.Y / 2 - itemSpaceY) * exceedY + newArea_02.Y / 2 * exceedY);

                break;
            case AreaSplitOption.Vertical:
                break;
            default:
                break;
        }

        availableAreas.RemoveAt(index);
        availableAreas.Add(newArea_01);
        availableAreas.Add(newArea_02);

        Area splitedArea = new Area()
        {
            X = itemSpaceX,
            Y = itemSpaceY,
        };

        splitedArea.origin = new Vector2(area.origin.x + splitedArea.X / 2 * exceedX, area.origin.y + splitedArea.Y / 2 * exceedY);

        return splitedArea;
    }

    public void OnDrawGizmo()
    {
#if UNITY_EDITOR
        return;
#endif
    }
}