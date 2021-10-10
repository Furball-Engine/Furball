using System.Collections.Generic;

namespace Furball.Engine.Engine.Localization.Languages {
    public abstract class Language {
        public abstract ISO639_2Code Iso6392Code();
        public abstract ISO639_1Code Iso6391Code();
        
        public abstract ISO639_2Scope Scope();
        public abstract ISO639_2Type  Type();

        public abstract string       EnglishName();
        public abstract string       NativeName();
        public abstract List<string> OtherNames();

        public override string ToString() => $"{this.EnglishName()} ({this.NativeName()})";
    }
}
