using System.Collections.Generic;

namespace Furball.Engine.Engine.Localization.Languages; 

public class JapaneseLanguage : Language {
    public override ISO639_2Code  Iso6392Code()     => ISO639_2Code.jpn;
    public override ISO639_1Code  Iso6391Code()     => ISO639_1Code.ja;
    public override ISO639_2Scope Scope()           => ISO639_2Scope.Individual;
    public override ISO639_2Type  Type()            => ISO639_2Type.Living;
    public override string        EnglishName()     => "Japanese";
    public override string        NativeName()      => "日本語";
    public override List<string>  OtherNames()      => new();
    public override string        IetfLanguageTag() => "ja";
}