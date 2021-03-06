﻿namespace Gravity.Server.ProcessingNodes.Transform.UrlRewriteRules.Interfaces.Conditions
{
    internal interface INumberMatch: ICondition
    {
        INumberMatch Initialize(
            IValueGetter valueGetter,
            CompareOperation compareOperation,
            int match,
            bool inverted = false,
            int defaultValue = 0);
    }
}
