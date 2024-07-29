using Hieki.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Supermarket.Products
{
    [Serializable]
    public class ArrangementGrid
    {
        enum Alignment
        {
            Start,
            Spacing,
            Center,
        }

        enum StartCorner
        {
            ForwardLeft,
            ForwardRight,
            BackLeft,
            BackRight,
        }

        enum StartAxis
        {
            Horizontal,
            Vertical,
        }

        /*        enum TweenType
                {
                    Flash,
                    Linear,
                    Sine
                }*/


        public Transform transform;
        public Vector3 offset;

        public float sizeX;
        public float sizeZ;

        [SerializeField] private Alignment alignment = Alignment.Center;
        public float spacing;

        [SerializeField] private StartCorner startCorner;

        [SerializeField] private StartAxis startAxis;

        [SerializeField] private LeanTweenType tween = LeanTweenType.easeInSine;
        [SerializeField] private float tweenTime = .25f;

        public int Count => /*products.Count;*/ count;
        [SerializeField] private int count;
        public Stack<ProductOnSale> products = new Stack<ProductOnSale>();

        public bool Push(ProductOnSale product, bool setParent = true)
        {
            if (products.TryPeek(out var p) && p.ProductInfo != product.ProductInfo)
            {
                Debug.LogWarning($"{p.GetType()} and {product.GetType()} are not the same type", transform);
                return false;
            }
            int count = Count;

            Vector2 size = product.GetWorldSize();

            int maxX = (int)(sizeX / (size.x + spacing));
            int maxZ = (int)(sizeZ / (size.y + spacing));
            int x, z;

            switch (startAxis)
            {
                case StartAxis.Horizontal:
                    z = count / maxX;
                    x = count % maxX;

                    if (z + 1 > maxZ)
                        return false;

                    break;
                case StartAxis.Vertical:
                    z = count % maxZ;
                    x = count / maxZ;

                    if (x + 1 > maxX)
                        return false;

                    break;
                default:
                    maxX = maxZ = x = z = 0;
                    break;
            }

            //Debug.Log(x + " " + z);


            Vector3 worldSpacing;
            float pHalfX = size.x / 2;
            float pHalfZ = size.y / 2;
            switch (alignment)
            {
                case Alignment.Spacing:
                    worldSpacing = new Vector3(pHalfX + spacing, 0, pHalfZ + spacing);
                    break;
                case Alignment.Center:
                    float spaceX = sizeX - size.x * maxX;
                    float spaceZ = sizeZ - size.y * maxZ;
                    int spaceCountX = maxX + 1;
                    int spaceCountZ = maxZ + 1;
                    worldSpacing = new Vector3(spaceX / spaceCountX * (x + 1), 0, spaceZ / spaceCountZ * (z + 1)) + new Vector3(pHalfX, 0, pHalfZ);
                    break;
                case Alignment.Start:
                    worldSpacing = new Vector3(pHalfX, 0, pHalfZ);
                    break;
                default:
                    worldSpacing = Vector3.zero;
                    break;
            }

            float halfSizeX = sizeX / 2;
            float halfSizeZ = sizeZ / 2;

            Vector3 startCorner = this.startCorner switch
            {
                StartCorner.ForwardLeft => new Vector3(-halfSizeX, 0, halfSizeZ),
                StartCorner.ForwardRight => new Vector3(halfSizeX, 0, halfSizeZ),
                StartCorner.BackLeft => new Vector3(-halfSizeX, 0, -halfSizeZ),
                StartCorner.BackRight => new Vector3(halfSizeX, 0, -halfSizeZ),
                _ => Vector3.zero
            };

            Vector3 relativePos = new Vector3(x * (size.x /*+ spacing*/), 0, z * (size.y /*+ spacing*/));

            Vector3 worldPos = relativePos + worldSpacing;

            switch (this.startCorner)
            {
                case StartCorner.BackRight:
                    worldPos.x *= -1;
                    break;
                case StartCorner.BackLeft:
                    break;
                case StartCorner.ForwardRight:
                    worldPos.x *= -1;
                    worldPos.z *= -1;
                    break;
                case StartCorner.ForwardLeft:
                    worldPos.z *= -1;
                    break;
            }

            Vector3 origin = Position.Offset(transform, offset);
            worldPos += startCorner;
            worldPos = worldPos.RotateVector(transform.eulerAngles.y) + origin;

            product.transform.LeanMove(worldPos, tweenTime).setEase(tween).setOnComplete(() =>
            {
                //product.transform.position = worldPos;
                if (setParent)
                    product.transform.parent = transform;
                //product.transform.localRotation = Quaternion.identity;
                products.Push(product);
            });

            this.count++;

            return true;
        }

        public ProductOnSale Pop()
        {
            if (products.TryPop(out var product))
            {
                count--;
                return product;
            }

            return null;
        }

        public ProductOnSale Peek()
        {
            if (products.TryPeek(out var product))
            {
                return product;
            }

            return null;
        }

        public Vector3 WorldPos() => Position.Offset(transform, offset);

#if UNITY_EDITOR
        [Header("Display Settings"), Space]
        public Color displayColor = Color.white;
#endif
        public void OnDrawGizmo()
        {
#if UNITY_EDITOR
            if (transform == null)
                return;

            Vector3 origin = WorldPos();
            Gizmos.color = displayColor;
            //Gizmos.DrawWireCube(origin, 2 * spacing * Vector3.one);
            SceneDrawer.DrawWireCubeGizmo(origin, 2 * (alignment == Alignment.Spacing ? spacing : transform.lossyScale.magnitude / 15) * Vector3.one, transform.rotation, displayColor);

            Vector3 bottomLeft = origin + new Vector3(-sizeX / 2, 0, -sizeZ / 2);
            Vector3 topLeft = origin + new Vector3(-sizeX / 2, 0, sizeZ / 2);
            Vector3 bottomRight = origin + new Vector3(sizeX / 2, 0, -sizeZ / 2);
            Vector3 topRight = origin + new Vector3(sizeX / 2, 0, sizeZ / 2);

            //Gizmos.DrawLine(bottomLeft, topLeft);
            //Gizmos.DrawLine(topLeft, topRight);
            //Gizmos.DrawLine(topRight, bottomRight);
            //Gizmos.DrawLine(bottomRight, bottomLeft);

            SceneDrawer.DrawWireCubeHandles(origin, new Vector3(sizeX, 0, sizeZ), transform.rotation, displayColor, 2.5f);
#endif
        }
    }

}
