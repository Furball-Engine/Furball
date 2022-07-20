using System.Collections.Generic;

namespace Furball.Engine.Engine.Localization.Languages; 

public class PolishLanguage : Language {
    public override ISO639_2Code  Iso6392Code() => ISO639_2Code.pol;
    public override ISO639_1Code  Iso6391Code() => ISO639_1Code.pl;
    public override ISO639_2Scope Scope()       => ISO639_2Scope.Individual;
    public override ISO639_2Type  Type()        => ISO639_2Type.Living;
    public override string        EnglishName() => "Polish";
    public override string        NativeName()  => "Język polski";
    public override List<string>  OtherNames()  => new();
}