using System.Collections.Generic;

namespace Furball.Engine.Engine.Localization.Languages; 

public class ArabicLanguage : Language {
    public override ISO639_2Code  Iso6392Code()     => ISO639_2Code.ara;
    public override ISO639_1Code  Iso6391Code()     => ISO639_1Code.ar;
    public override ISO639_2Scope Scope()           => ISO639_2Scope.Macrolanguage;
    public override ISO639_2Type  Type()            => ISO639_2Type.Living;
    public override string        EnglishName()     => "Arabic";
    public override string        NativeName()      => "العَرَبِيَّة";
    public override List<string>  OtherNames()      => new();
    public override string        IetfLanguageTag() => "ar";
}