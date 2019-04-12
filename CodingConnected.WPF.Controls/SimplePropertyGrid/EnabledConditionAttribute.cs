using System;

namespace CodingConnected.WPF.Controls
{
    public class EnabledConditionAttribute : Attribute
    {
        public EnabledConditionAttribute(string conditionPropertyName)
        {
            ConditionPropertyName = conditionPropertyName;
        }

        public string ConditionPropertyName { get; }
    }
}
