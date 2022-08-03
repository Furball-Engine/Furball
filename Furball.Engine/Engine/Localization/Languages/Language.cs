using System.Collections.Generic;

namespace Furball.Engine.Engine.Localization.Languages; 

public abstract class Language {
    public abstract ISO639_2Code Iso6392Code();
    public abstract ISO639_1Code Iso6391Code();
        
    public abstract ISO639_2Scope Scope();
    public abstract ISO639_2Type  Type();

    public abstract string       EnglishName();
    public abstract string       NativeName();
    public abstract List<string> OtherNames();
    public abstract string       IetfLanguageTag();
    
    public override string ToString() => $"{this.EnglishName()} ({this.NativeName()})";

    public static bool operator ==(Language a, Language b) {
        if (a is null && b is null)
            return true;
        if (a is null || b is null)
            return false;
            
        return a.Iso6392Code() == b.Iso6392Code();
    }
    public static bool operator !=(Language a, Language b) => !(a == b);

    private bool Equals(Language other) => this == other;
    public override bool Equals(object obj) {
        if (ReferenceEquals(null, obj))
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj.GetType() != this.GetType())
            return false;
        return this.Equals((Language)obj);
    }

    public override int GetHashCode() {
        unchecked {
            int hashcode = 7530656;
            hashcode = (hashcode * 3567833) ^ this.EnglishName().GetHashCode();
            hashcode = (hashcode * 3567833) ^ this.Iso6392Code().GetHashCode();
            hashcode = (hashcode * 3567833) ^ this.Iso6391Code().GetHashCode();
            return hashcode;
        }
    }
}