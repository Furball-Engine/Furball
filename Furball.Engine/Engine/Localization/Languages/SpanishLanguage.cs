using System.Collections.Generic;

namespace Furball.Engine.Engine.Localization.Languages; 

public class SpanishLanguage : Language {
    public override ISO639_2Code  Iso6392Code() => ISO639_2Code.spa;
    public override ISO639_1Code  Iso6391Code() => ISO639_1Code.es;
    public override ISO639_2Scope Scope()       => ISO639_2Scope.Individual;
    public override ISO639_2Type  Type()        => ISO639_2Type.Living;
    public override string        EnglishName() => "Spanish";
    public override string        NativeName()  => "español";
    public override List<string>  OtherNames()  => new();
}