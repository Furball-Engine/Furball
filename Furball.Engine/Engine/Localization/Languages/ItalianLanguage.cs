using System.Collections.Generic;

namespace Furball.Engine.Engine.Localization.Languages; 

public class ItalianLanguage : Language {
    public override ISO639_2Code  Iso6392Code()     => ISO639_2Code.ita;
    public override ISO639_1Code  Iso6391Code()     => ISO639_1Code.it;
    public override ISO639_2Scope Scope()           => ISO639_2Scope.Individual;
    public override ISO639_2Type  Type()            => ISO639_2Type.Living;
    public override string        EnglishName()     => "Italian";
    public override string        NativeName()      => "italiano";
    public override List<string>  OtherNames()      => new();
    public override string        IetfLanguageTag() => "it";
}