using System;
using System.Threading;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Vixie;


namespace Furball.Engine.Engine; 

public class Screen : GameComponent {
    protected DrawableManager Manager;
    public Screen() {
        this.Manager = new DrawableManager();
    }

    public virtual bool RequireLoadingScreen => false;
    /// <summary>
    /// Runs on another thread, intended to do any heavy tasks
    /// </summary>
    public virtual void BackgroundInitialize() {}

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

    public override void Draw(double gameTime) {
        this.Manager?.Draw(gameTime, FurballGame.DrawableBatch);

        base.Draw(gameTime);
    }

    public override void Update(double gameTime) {
        this.Manager?.Update(gameTime);

        base.Update(gameTime);
    }

    public override void Dispose() {
        this.Manager?.Dispose();
            
        base.Dispose();
    }
}