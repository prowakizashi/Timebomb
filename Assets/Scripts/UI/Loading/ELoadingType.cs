using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class ELoadingType : IEquatable<ELoadingType>
{
    public string Name { get; private set; }

    private ELoadingType(string name)
    {
        Name = name;
    }

    public static bool operator ==(ELoadingType type1, ELoadingType type2)
    {
        return type1.Equals(type2);
    }

    public static bool operator !=(ELoadingType type1, ELoadingType type2)
    {
        return type1.Name != type2.Name;
    }

    public bool Equals(ELoadingType type2)
    {
        if (ReferenceEquals(type2, null))
        {
            return false;
        }
        if (ReferenceEquals(this, type2))
        {
            return true;
        }

        return Name.Equals(type2.Name);
    }

    public override bool Equals(object o)
    {
        return base.Equals(o);
    }

    public override int GetHashCode()
    {  
        return base.GetHashCode();  
    }  
}