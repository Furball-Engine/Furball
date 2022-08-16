using System.Numerics;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Vixie.Backends.Shared;
using SixLabors.Fonts;

namespace Furball.Game.Screens.Tests; 

public class SmartTextTest : TestScreen {
    public override void Initialize() {
        base.Initialize();

        this.Manager.Add(
        new SmartTextDrawable(
        new Vector2(5),
        @"من البيان والتبيين الى البتاع والتبتيع
أحمد فؤاد نجم، البلاغة، والرأسمال الثقافي
عيش البورجوازية عيش قلق، تتفجّر فيه الفتن والفورات، تطغى عليه العيون الحاسدة والعوارض الفاسدة، تتصارع طوائفه ليس صراع الثيران: بل صراع الأرانب والأسود (وهي بدورها أصناف وطبقات: الغضنفر، اللبوة، الشبل، السبع، وأخيراً وليس آخراً الفصل السوري..). إلا أنه عيش قد تطورت فيه آليات الكبت، الى جانب أجهزة الحكم والصمت، علّه يواظب على رتابته الرزينة، وكبائره المشينة الحزينة. لم نجئ الى هنا اليوم من أجل مقاومة هذه الجرائم، لم نأتي كي نتصدّى لها في القرى كما في العواصم. كما أننا ليس قصدنا أن ننصّب الحاج أحمد قائدنا العظيم، بطلنا الحكيم، في مواجهتنا المظالم المعروفة والمصاعب المعهودة، المتفق عليها.. إنما وجدنا في بعض ما كتبه، ليس باباً بل أكرة باب، وليس مفتاحاً بل بطاقة مالية توافي بالمهمة، وقول يا مسهّل!
- يا مسهّل!",
        SystemFonts.Get("Arial"),
        20,
        Vector2.One,
        Color.White
        ) {
            ScreenOriginType = OriginType.TopRight,
            OriginType       = OriginType.TopRight
        }
        );
    }
}
