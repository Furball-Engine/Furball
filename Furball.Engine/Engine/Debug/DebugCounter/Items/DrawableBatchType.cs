namespace Furball.Engine.Engine.Debug.DebugCounter.Items {
    public class DrawableBatchType : DebugCounterItem {

        public override string GetAsString(double time) => $"rt: {FurballGame.DrawableBatch.RendererType.ToString()}";
    }
}
