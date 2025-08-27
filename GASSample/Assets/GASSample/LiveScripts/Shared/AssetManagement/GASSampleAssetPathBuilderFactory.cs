using CycloneGames.AssetManagement;

namespace GASSample.AssetManagement
{
    public class GASSampleAssetPathBuilderFactory : IAssetPathBuilderFactory
    {
        public IAssetPathBuilder Create(string type)
        {
            switch (type)
            {
                case "UI":
                    return new GASSampleUIAssetPathBuilder();
                default:
                    return null;
            }
        }
    }
    
    public class GASSampleUIAssetPathBuilder : IAssetPathBuilder
    {
        public string GetAssetPath(string key)
        {
            return $"Assets/GASSample/LiveContent/ScriptableObject/UI/Window/{key}.asset";
        }
    }
}