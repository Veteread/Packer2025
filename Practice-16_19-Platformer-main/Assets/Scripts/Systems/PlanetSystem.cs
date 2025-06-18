using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class PlanetSystem : MonoBehaviour
{
    [System.Serializable]
    public class Planet
    {
        public string name;
        public Sprite icon;
        public Color themeColor;
     //   public BlockData[] availableBlocks;
        public float travelTime = 10f;
    }

    [SerializeField] private Planet[] planets;
    [SerializeField] private int currentPlanetIndex;

    public Planet CurrentPlanet => planets[currentPlanetIndex];

    public async UniTask TravelToPlanet(int planetIndex)
    {
        if (planetIndex < 0 || planetIndex >= planets.Length) return;

        // Показать экран загрузки/путешествия
        await TravelAnimation(planets[currentPlanetIndex], planets[planetIndex]);

        currentPlanetIndex = planetIndex;
        UpdateAvailableBlocks();
    }

    private async UniTask TravelAnimation(Planet from, Planet to)
    {
        // Реализация анимации перелета
        await UniTask.Delay(Mathf.RoundToInt(to.travelTime * 1000));
    }

    private void UpdateAvailableBlocks()
    {
        // Обновить доступные блоки в UI
    }
}