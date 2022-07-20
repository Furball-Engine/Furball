using System;

namespace Furball.Engine.Engine.Localization.Exceptions; 

public class NoTranslationException : Exception {
    public override string Message => "There is no translation available for that key! Try adding a default translation.";
}