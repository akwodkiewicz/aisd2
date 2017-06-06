
namespace ASD
{

    public struct Vector2
    {
        public double X;
        public double Y;

        public static Vector2 Zero = new Vector2(0, 0);

        public Vector2(double X, double Y)
        {
            this.X = X;
            this.Y = Y;
        }

    }

    public class Provider
    {
        /// <summary>
        /// Maksymalna ilość dostarczanych/produkowanych surowców/części
        /// </summary>
        public int Capacity;
        /// <summary>
        /// Koszt wytworzenia/przetworzenia jednostki produktu.
        /// </summary>
        public double Cost;

        public Vector2 Position;

        public Provider(int capacity, double cost, Vector2 position = default(Vector2))
        {
            this.Capacity = capacity;
            this.Cost = cost;
            this.Position = position;
        }
    }

    public class Factory
    {
        /// <summary>
        /// Koszt wyprodukowania każdego produktu poniżej limitu
        /// </summary>
        public double LowerCost;
        /// <summary>
        /// Koszt wyprodukownaia każdego produktu powyżej limitu
        /// </summary>
        public double HigherCost;
        /// <summary>
        /// Limit produkcji tańszych urządzeń
        /// </summary>
        public int Limit;

        public Vector2 Position;

        public Factory(double lowerCost, double higherCost, int limit, Vector2 position = default(Vector2))
        {
            this.LowerCost = lowerCost;
            this.HigherCost = higherCost;
            this.Limit = limit;
            this.Position = position;
        }
    }
}
