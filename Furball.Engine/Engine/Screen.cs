using System.Numerics;
using System.Threading;
using Furball.Engine.Engine.Graphics.Drawables.Managers;

namespace Furball.Engine.Engine; 

public class Screen {
    public DrawableManager Manager;
    public Screen() {
        this.Manager = new DrawableManager();

        this.Manager.OnScalingRelayoutNeeded += this.ManagerOnOnScalingRelayoutNeeded;
    }

    public void ManagerOnOnScalingRelayoutNeeded(object _, Vector2 e) {
        this.Relayout(e.X / e.Y * 720f, FurballGame.WindowHeight);
    }

    public virtual bool RequireLoadingScreen => false;
    /// <summary>
    /// Runs on another thread, intended to do any heavy tasks
    /// </summary>
    public virtual void BackgroundInitialize() {}
    
    public virtual void Initialize() {}

    public readonly object LoadingLock = new();

    private float _loadingProgress = 0f;
    public float LoadingProgress {
        get {
            lock (this.LoadingLock)
                return this._loadingProgress;
        }
        protected set {
            lock (this.LoadingLock)
                this._loadingProgress = value;
        }
    }

    public Thread BackgroundThread;

    private bool _loadingComplete = false;
    public bool LoadingComplete {
        get {
            lock (this.LoadingLock)
                return this._loadingComplete;
        }
        protected set {
            lock (this.LoadingLock)
                this._loadingComplete = value;
        }
    }

    private string _loadingStatus = "Loading...";
    public string LoadingStatus {
        get {
            lock (this.LoadingLock)
                return this._loadingStatus;
        }
        protected set {
            lock (this.LoadingLock)
                this._loadingStatus = value;
        }
    }

    public virtual void Relayout(float newWidth, float newHeight) {
            
    }

    public virtual void UpdateTextStrings() {
        
    }

    public virtual void Draw(double gameTime) {
        this.Manager?.Draw(gameTime, FurballGame.DrawableBatch);
    }

    public virtual void Update(double gameTime) {
        this.Manager?.Update(gameTime);
    }

    public virtual void Dispose() {
        this.Manager?.Dispose();
    }
}