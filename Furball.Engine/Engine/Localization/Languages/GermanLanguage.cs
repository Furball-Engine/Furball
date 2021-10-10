using System.Collections.Generic;

namespace Furball.Engine.Engine.Localization.Languages {
    public class GermanLanguage : Language {
        public override ISO639_2Code  Iso6392Code() => ISO639_2Code.deu;
        public override ISO639_1Code  Iso6391Code() => ISO639_1Code.de;
        public override ISO639_2Scope Scope()       => ISO639_2Scope.Individual;
        public override ISO639_2Type  Type()        => ISO639_2Type.Living;
        public override string        EnglishName() => "German";
        public override string        NativeName()  => "Deutsch";
        public override List<string>  OtherNames()  => new();
    }
}
