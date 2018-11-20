using System;

namespace CodingConnected.WPF.Controls
{
    public class BrowsableConditionAttribute : Attribute
    {
        public BrowsableConditionAttribute(string conditionPropertyName)
        {
            ConditionPropertyName = conditionPropertyName;
        }

        public string ConditionPropertyName { get; }
    }
}
