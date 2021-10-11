using System;
using System.IO;

namespace Furball.Engine.Engine.Configuration {
    public abstract class Config {
        public abstract string Filename();

        public abstract void Save();

        public abstract void Load();

        protected FileStream GetFileReadStream() {
            if (!File.Exists(this.Filename()))
                this.Save();

            return File.OpenRead(this.Filename());
        }
    }

    public class InvalidConfigException : Exception {
        public override string Message => "The configuration file is invalid!";
    }
}
