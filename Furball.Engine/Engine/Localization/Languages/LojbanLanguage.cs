using System.Collections.Generic;

namespace Furball.Engine.Engine.Localization.Languages; 

public class LojbanLanguage : Language {
    public override ISO639_2Code  Iso6392Code() => ISO639_2Code.jbo;
    public override ISO639_1Code  Iso6391Code() => ISO639_1Code.None;
    public override ISO639_2Scope Scope()       => ISO639_2Scope.Individual;
    public override ISO639_2Type  Type()        => ISO639_2Type.Constructed;
    public override string        EnglishName() => "Lojban";
    public override string        NativeName()  => "la .lojban.";
    public override List<string>  OtherNames()  => new();
}