using System.Collections.Generic;

namespace Furball.Engine.Engine.Localization.Languages; 

public class EnglishLanguage : Language {
    public override ISO639_2Code  Iso6392Code()     => ISO639_2Code.eng;
    public override ISO639_1Code  Iso6391Code()     => ISO639_1Code.en;
    public override ISO639_2Scope Scope()           => ISO639_2Scope.Individual;
    public override ISO639_2Type  Type()            => ISO639_2Type.Living;
    public override string        EnglishName()     => "English";
    public override string        NativeName()      => "English";
    public override List<string>  OtherNames()      => new();
    public override string        IetfLanguageTag() => "en";
}