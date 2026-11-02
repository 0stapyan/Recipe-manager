using System;

namespace RecipeManager.main.Achievements
{
    public sealed class StockOneHundredKg : IAchievement, IPersistableAchievement
    {
        public string Name => "Bulk Buyer";
        public string Description => "Stock 100 kg of items.";
        public AchievementType TargetEvent => AchievementType.Stock;

        public const decimal TargetKg = 100m;
        public decimal TotalKgStocked { get; private set; } = 0m;

        public bool IsUnlocked => TotalKgStocked >= TargetKg;
        public string Status => IsUnlocked ? "UNLOCKED" : $"{TotalKgStocked:N2}/{TargetKg} kg";

        public string Id => "StockOneHundredKg";
        public AchievementState ExportState() => new AchievementState
        {
            Id = Id,
            IsUnlocked = IsUnlocked,
            DecimalProgress = TotalKgStocked
        };
        public void ImportState(AchievementState s)
        {
            TotalKgStocked = s?.DecimalProgress ?? 0m;
        }

        public bool HandleEvent(object data)
        {
            if (IsUnlocked) return false;
            decimal qty = 0m;
            string unit = "kg";

            if (data != null)
            {
                var t = data.GetType();
                var pq = t.GetProperty("quantity");
                var pu = t.GetProperty("unit");

                if (pq != null && pq.GetValue(data) is IConvertible c1)
                    qty = Convert.ToDecimal(c1);
                if (pu != null && pu.GetValue(data) is string u && !string.IsNullOrWhiteSpace(u))
                    unit = u.Trim();
            }
            decimal inKg = unit.ToLowerInvariant() switch
            {
                "kg" => qty,
                "g"  => qty / 1000m,
                "l"  => qty,
                "ml" => qty / 1000m,
                _    => qty
            };

            TotalKgStocked += inKg;
            return IsUnlocked;
        }
    }
}
