using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;
using Supermarket;
using Supermarket.Products;

[DefaultExecutionOrder(-299)]
public static class AssetManager
{
    const string PRODUCTS_KEY = "products";
    const string LICENSES_KEY = "license";

    public static Dictionary<int, ProductInfo> ProductInfos { get; set; }
    public static Dictionary<int, License> Licenses { get; set; }


    public static async UniTask<bool> LoadLAllLicenses()
    {
        //Licenses
        AsyncOperationHandle<IList<License>> handle = Addressables.LoadAssetsAsync<License>(LICENSES_KEY, loadedProduct => { });

        await handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            IList<License> result = handle.Result;
            Licenses = new Dictionary<int, License>(result.Count);
            foreach (var license in result)
            {
                license.Init();
                Licenses.Add(license.LicenseId, license);
            }

            return true;
        }

        return false;
    }

    public static async UniTask<bool> LoadAllProducts()
    {
        //Product Infos
        AsyncOperationHandle<IList<ProductInfo>> handle = Addressables.LoadAssetsAsync<ProductInfo>(PRODUCTS_KEY, loadedProduct => { });

        await handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            IList<ProductInfo> result = handle.Result;
            ProductInfos = new Dictionary<int, ProductInfo>(result.Count);
            foreach (var productInfo in result)
            {
                ProductInfos.Add(productInfo.ProductId, productInfo);
            }

            return true;
        }

        return false;
    }

    public static ProductInfo GetProduct(int productId)
    {
        return ProductInfos[productId];
    }

    public static License GetLicense(int licenseId)
    {
        return Licenses[licenseId];
    }
}