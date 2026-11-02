namespace RecipeManager
{
    public interface ILimitsService
    {
        PlanLimits GetLimits(PlanType planType);
    }
}