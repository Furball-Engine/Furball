using System.Collections.Generic;

namespace Furball.Engine.Engine.Localization.Languages; 

public class EsperantoLanguage : Language {
    public override ISO639_2Code  Iso6392Code()     => ISO639_2Code.epo;
    public override ISO639_1Code  Iso6391Code()     => ISO639_1Code.eo;
    public override ISO639_2Scope Scope()           => ISO639_2Scope.Individual;
    public override ISO639_2Type  Type()            => ISO639_2Type.Constructed;
    public override string        EnglishName()     => "Esperanto";
    public override string        NativeName()      => "Esperanto";
    public override List<string>  OtherNames()      => new();
    public override string        IetfLanguageTag() => "eo";
}