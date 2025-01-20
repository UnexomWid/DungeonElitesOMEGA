using UnityEngine;

namespace OMEGA
{
    public static class Items
    {
        public enum ID
        {
            None = 0,
            BloodThirst = 1,
            Bomb = 2,
            Book = 3,
            Bracelet = 4,
            Heal = 5,
            IceBlade = 6,
            StatSword = 7,
            Wings = 8,
            FireBlade = 9,
            XpBottle = 10,
            LuckyBox = 11
        };

        public enum SaleType
        {
            SALE,
            NORMAL,
            MARKUP
        }

        public class SaleInfo
        {
            public SaleType type;
            public Color textColor;
            public float priceFactor;
            public float probability;
        }

        public static readonly SaleInfo[] saleInfoLookupAvaritia = new SaleInfo[]
        {
            new SaleInfo() { type = SaleType.NORMAL, textColor = Color.black, priceFactor = 1f, probability = 0.6f },
            new SaleInfo() { type = SaleType.SALE, textColor = Color.green, priceFactor = 0.5f, probability = 0.2f },
            new SaleInfo() { type = SaleType.MARKUP, textColor = Color.red, priceFactor = 2f, probability = 0.2f },
        };

        public static int GetPrice(ID id, int boughtAmount)
        {
            if (Data.trialMode == Data.TrialMode.AVARITIA)
            {
                // In Avaritia mode, the price increases when the player buys items from the same item menu
                float avaritiaComponent = (GetBasePrice(id) + Data.boughtItems * Data.AVARITIA_ITEM_PRICE_INCREASE) * Data.GetBadAvaritiaFactor();
                float extraComponent = Mathf.Pow(Data.AVARITIA_ITEM_PRICE_INCREASE_FACTOR, boughtAmount);

                return (int) Mathf.Floor(avaritiaComponent * extraComponent);
            }

            return GetBasePrice(id);
        }

        public static int GetBasePrice(ID id)
        {
            switch (id)
            {
                case ID.LuckyBox:
                {
                    return 200;
                }
                default:
                {
                    return 300;
                }
            }
        }

        public static SaleInfo GenerateSaleInfo()
        {
            if (Data.trialMode == Data.TrialMode.AVARITIA)
            {
                float max = 0f;
                foreach (var info in saleInfoLookupAvaritia)
                {
                    max += info.probability;
                }

                float val = UnityEngine.Random.Range(0f, max - float.Epsilon);

                float p = 0f;
                foreach (var info in saleInfoLookupAvaritia)
                {
                    p += info.probability;

                    if (val < p)
                    {
                        return info;
                    }
                }

                return saleInfoLookupAvaritia[saleInfoLookupAvaritia.Length - 1];
            }

            return saleInfoLookupAvaritia[0]; // Normal sale
        }
    }
}