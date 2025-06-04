using System;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class ActionNameAttribute : Attribute
{
    public string ActionName { get; }
    public ActionNameAttribute(string actionName)
    {
        ActionName = actionName;
    }
}
