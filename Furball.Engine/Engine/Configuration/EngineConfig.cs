using System.IO;
using Furball.Engine.Engine.Localization;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace Furball.Engine.Engine.Configuration {
    [JsonObject(MemberSerialization.OptIn)]
    public class EngineConfig : Config {
        public override string Filename() => "furball.conf";

        [JsonProperty]
        public ISO639_2Code Language = ISO639_2Code.eng;
        [JsonProperty]
        public Rectangle CurrentResolution = new(0, 0, FurballGame.DEFAULT_WINDOW_WIDTH, FurballGame.DEFAULT_WINDOW_HEIGHT);

        public override void Save() {
            FileStream   stream = File.Create(this.Filename());
            StreamWriter writer = new(stream);

            string json = JsonConvert.SerializeObject(this);

            writer.Write(json);
            writer.Close();
            stream.Close();
        }

        public override void Load() {
            FileStream   stream = this.GetFileReadStream();
            StreamReader reader = new(stream);

            string json = reader.ReadToEnd();

            stream.Close();

            EngineConfig config = JsonConvert.DeserializeObject<EngineConfig>(json);

            if (config is null) throw new InvalidConfigException();

            this.Language          = config.Language;
            this.CurrentResolution = config.CurrentResolution;

        }
    }
}
