namespace BankSystem.Common.Utils
{
    using System.Collections.Generic;

    public static class ActionArgumentsUtil
    {
        public static object GetModel(IDictionary<string, object> actionArguments)
        {
            object model = null;

            foreach (var obj in actionArguments.Values)
            {
                var type = obj.GetType();
                if (type.IsClass || type.IsInterface)
                {
                    model = obj;
                }

                // We've successfully parsed the model and it's not necessary to loop through anymore 
                if (model != null)
                {
                    break;
                }
            }

            return model;
        }
    }
}
